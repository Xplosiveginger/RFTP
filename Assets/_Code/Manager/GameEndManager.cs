using System.Collections;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class GameEndManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject endPanel;

    [Header("Game End Settings")]
    [SerializeField] private float gameEndTime = 10f;

    [Header("Game References")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerController2D playerController;
    [SerializeField] private GameObject playerWeapons;
    [SerializeField] private Transform endGamePlayerPos;
    [Header("Camera References")]
    [SerializeField] private GameObject oldCamera;
    [SerializeField] private GameObject oldCinemachine;
    [SerializeField] private GameObject endGameCamera;

    [Header("Runtime")]
    [SerializeField] private float gameTime = 0f;
    [Header("Boss Entrance")]
    [SerializeField] private RectTransform boss;
    [SerializeField] private Vector2 bossStartPos;
    [SerializeField] private Vector2 bossEndPos;
    [SerializeField] private float bossMoveDelay = 1f;
    [SerializeField] private float bossMoveDuration = 2f;
    
    [Header("End Game Transition")]
    [SerializeField] private GameObject transitionObject;
    [SerializeField] private float transitionTime = 2f;
    

    [Header("End Game Tilemaps")]
    [SerializeField] private GameObject gameTile;
    [SerializeField] private GameObject endTile;

    [Header("End Game Dialogue")]
    [SerializeField] private GameObject dialogueObject;
    [SerializeField] private float dialogueDelay = 1f;
    private bool gameEnded = false;
    
    private void Start()
    {
        if (enemySpawner != null)
        {
            enemySpawner.OnGameTimeUpdated += UpdateGameTime;
        }
        else
        {
            Debug.LogError("[GameEndManager] Enemy Spawner reference is missing!");
        }

        if (dialogueObject != null)
        {
            GameEndDialogueManager dialogueManager =
                dialogueObject.GetComponent<GameEndDialogueManager>();

            if (dialogueManager != null)
            {
                dialogueManager.OnDialogueComplete += OnDialogueComplete;
            }
            else
            {
                Debug.LogError("[GameEndManager] GameEndDialogueManager not found on dialogue object!");
            }
        }
    }

    private void OnDestroy()
    {
        if (enemySpawner != null)
        {
            enemySpawner.OnGameTimeUpdated -= UpdateGameTime;
        }

        if (dialogueObject != null)
        {
            GameEndDialogueManager dialogueManager =
                dialogueObject.GetComponent<GameEndDialogueManager>();

            if (dialogueManager != null)
            {
                dialogueManager.OnDialogueComplete -= OnDialogueComplete;
            }
        }
    }
    private void OnDialogueComplete()
    {
        Debug.Log("[GameEndManager] Dialogue complete!");

        // Hide the normal game tilemap
        if (gameTile != null)
        {
            gameTile.SetActive(false);
        }

        // Show the end-game tilemap
        if (endTile != null)
        {
            endTile.SetActive(true);
        }

        // Hide dialogue
        if (dialogueObject != null)
        {
            dialogueObject.SetActive(false);
        }
    }
    private void UpdateGameTime(float time)
    {
        gameTime = time;

        if (!gameEnded && gameTime >= gameEndTime)
        {
            EndGame();
        }
    }

    private void PlayBossEntrance()
    {
        if (boss == null)
        {
            Debug.LogWarning("[GameEndManager] Boss reference is missing!");
            return;
        }

        // Put boss at starting position
        boss.anchoredPosition = bossStartPos;

        // Move boss from start to end
        boss.DOAnchorPos(
                bossEndPos,
                bossMoveDuration
            )
            .SetDelay(bossMoveDelay)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // Then wait before starting dialogue
                StartCoroutine(EnableDialogueAfterDelay());
            });
    }

    private IEnumerator EnableDialogueAfterDelay()
    {
        yield return new WaitForSeconds(dialogueDelay);

        if (dialogueObject != null)
        {
            dialogueObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[GameEndManager] Dialogue object reference is missing!");
        }
    }
    private void PlayEndGameTransition()
    {
        if (transitionObject == null)
        {
            Debug.LogWarning("[GameEndManager] Transition object reference is missing!");
            return;
        }

        // Show the transition video
        transitionObject.SetActive(true);

        // Hide it again after the transition time
        StartCoroutine(HideTransitionAfterDelay());
    }

    private IEnumerator HideTransitionAfterDelay()
    {
        yield return new WaitForSeconds(transitionTime);

        if (transitionObject != null)
        {
            transitionObject.SetActive(false);
        }
    }
    private void EndGame()
    {
        gameEnded = true;

        Debug.Log("[GameEndManager] GAME ENDED!");

        // --------------------------------------------------
        // 1. Disable EnemySpawner script
        // --------------------------------------------------
        if (enemySpawner != null)
        {
            enemySpawner.enabled = false;
        }
        // Disable player movement
        if (playerController != null)
        {
            playerController.gameEnded = true;
        }
        // --------------------------------------------------
        // 2. Hide all existing enemies
        // --------------------------------------------------
        if (enemySpawner != null)
        {
            Transform spawnerTransform = enemySpawner.transform;

            foreach (Transform child in spawnerTransform)
            {
                child.gameObject.SetActive(false);
            }
        }

        // --------------------------------------------------
        // 3. Teleport player
        // --------------------------------------------------
        if (player != null && endGamePlayerPos != null)
        {
            player.transform.position = endGamePlayerPos.position;
        }

        // Start boss entrance
        PlayBossEntrance();
        // Play end-game transition
        PlayEndGameTransition();
        // --------------------------------------------------
        // 4. Disable player weapons
        // --------------------------------------------------
        if (playerWeapons != null)
        {
            playerWeapons.SetActive(false);
        }

        // --------------------------------------------------
        // 5. Hide old camera
        // --------------------------------------------------
        if (oldCamera != null)
        {
            oldCamera.SetActive(false);
        }

        // --------------------------------------------------
        // 6. Hide old Cinemachine
        // --------------------------------------------------
        if (oldCinemachine != null)
        {
            oldCinemachine.SetActive(false);
        }

        // --------------------------------------------------
        // 7. Show end-game camera
        // --------------------------------------------------
        if (endGameCamera != null)
        {
            endGameCamera.SetActive(true);
        }

        // --------------------------------------------------
        // 8. Show end panel
        // --------------------------------------------------
        if (endPanel != null)
        {
            endPanel.SetActive(true);
        }
    }
}