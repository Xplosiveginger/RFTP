using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : BaseEnemy
{
    public GameObject Player { get; private set; }
    HealthSystem playerHealth;
    [Space]
    [Header("Setup")]
    public float chargeSpeed = 5f;
    public float circleAngularSpeed = 5f;
    public Rigidbody2D rb;
    public Vector2 trapRectangleSize = new Vector2(5, 3);
    public float trapCircleRadius = 5f;


    public EnemyAI ai;
    public Transform obstacleCheckTransform;
    public Vector2 obstacleCheckSize;
    public LayerMask obstacleLayerMask;
    public ParticleSystem toxicTrail;

    StateMachine statemachine;
    [SerializeField, DisplayOnly] private bool playerInRange = false;
    [DisplayOnly] public bool charging = false;
    [DisplayOnly] public bool obstacleDetected = false;

    public EntityState currState;

    public MB_FollowPlayerState followPlayerState { get; private set; }
    public ChargedAttack_State chargedAttackState { get; private set; }
    public MB_TrapPlayerState trapPlayerState { get; private set; }
    public MB_pentTrapPlayerState pentTrapPlayerState { get; private set; }
    public MB_circTrapPlayerState circTrapPlayerState { get; private set; }

    private void Awake()
    {
        statemachine = new StateMachine();
        ai = GetComponent<EnemyAI>();
        rb = GetComponent<Rigidbody2D>();
        Player = GameObject.FindGameObjectWithTag("Player");

        followPlayerState = new MB_FollowPlayerState(this, statemachine, null, "followPlayer");
        chargedAttackState = new ChargedAttack_State(this, statemachine, null, "chargedAttack");
        trapPlayerState = new MB_TrapPlayerState(this, statemachine, null, "trapPlayer");
        pentTrapPlayerState = new MB_pentTrapPlayerState(this, statemachine, null, "trapPlayer");
        circTrapPlayerState = new MB_circTrapPlayerState(this, statemachine, null, "trapPlayer");
    }

    private void Start()
    {
        statemachine.Initilize(followPlayerState);
    }

    private void Update()
    {
        obstacleDetected = Physics2D.OverlapBox(obstacleCheckTransform.position, obstacleCheckSize, 0,obstacleLayerMask);

        statemachine.UpdateActiveState();

        if (Input.GetKeyDown(KeyCode.E))
        {
            charging = true;
        }
        else
        {
            charging = false;
        }

        currState = statemachine.CurrentState;

        DebugCurrState();
    }

    protected override void DoDamageToPlayerOnCollision(Collision2D player)
    {
        this.Player = player.gameObject;
        playerHealth = this.Player.GetComponent<HealthSystem>();

        playerHealth.Damage((int)damageOnContact);
    }

    private void DebugCurrState()
    {
        if (currState == followPlayerState)
            Debug.Log("Follow Player State");
        else if (currState == chargedAttackState)
            Debug.Log("Charged Attack State");
        else if (currState == trapPlayerState)
            Debug.Log("Trap Player State");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(obstacleCheckTransform.position, obstacleCheckSize);
    }
}
