using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy_Damage_Contact))]
public class LitmusPaper_Refactor : BaseEnemyRefactor
{
    [Header("Litmus Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Litmus States Sprites")]
    public List<Sprite> changingSprites;

    [Header("Blast Damage Settings")]
    public float radius = 5f;
    public int minDamage = 10;
    public int midDamage = 20;
    public int maxDamage = 40;

    [Header("MoveSpeed")]
    public float moveSpeedIncreaser = 0.5f;
    public LayerMask damageArea;

    [Header("DamageInterval per Second")]
    public float AlkalineStateDamageInterval = 1f;
    public float NeutralStateDamageInterval = 1f;
    public float AcidicStateDamageInterval = 1f;

    private Enemy_Damage_Contact enemyDamageContact;

    private LitmusPhase currentPhase;

    private enum LitmusPhase
    {
        Purple,
        Green,
        Red
    }

    protected override void Awake()
    {
        base.Awake();

        enemyDamageContact = GetComponent<Enemy_Damage_Contact>();

        EnemyManager.Instance.RegisterEnemy(this);

        // Start in Purple phase
        SetPhase(LitmusPhase.Purple);
    }

    private void Update()
    {
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (animator == null)
            return;

        bool isAttacking = enemyDamageContact != null &&
                           enemyDamageContact.IsPlayerInContact;

        // 0 = walking / idle
        // 1 = attacking
        float animationParameter = isAttacking ? 1f : 0f;

        // Make sure only the current phase is active.
        animator.SetBool("Purple", currentPhase == LitmusPhase.Purple);
        animator.SetBool("Green", currentPhase == LitmusPhase.Green);
        animator.SetBool("Red", currentPhase == LitmusPhase.Red);

        // Set the parameter for the current phase.
        animator.SetFloat(
            "PurplePara",
            currentPhase == LitmusPhase.Purple ? animationParameter : 0f
        );

        animator.SetFloat(
            "GreenPara",
            currentPhase == LitmusPhase.Green ? animationParameter : 0f
        );

        animator.SetFloat(
            "RedPara",
            currentPhase == LitmusPhase.Red ? animationParameter : 0f
        );
    }

    public void CheckHealthState()
    {
        float hpPercent = health / maxHealth;

        if (hpPercent <= 0.5f)
        {
            // Red phase
            SetPhase(LitmusPhase.Red);

            IncreaseSpeedOnce();

            enemyDamageContact.ModifyDamageInterval(
                AcidicStateDamageInterval
            );
        }
        else if (hpPercent <= 0.75f)
        {
            // Green phase
            SetPhase(LitmusPhase.Green);

            IncreaseSpeedOnce();

            enemyDamageContact.ModifyDamageInterval(
                NeutralStateDamageInterval
            );
        }
        else
        {
            // Purple phase
            SetPhase(LitmusPhase.Purple);

            enemyDamageContact.ModifyDamageInterval(
                AlkalineStateDamageInterval
            );
        }
    }

    private void SetPhase(LitmusPhase phase)
    {
        // Don't repeatedly change the sprite/phase every health update.
        if (currentPhase == phase)
            return;

        currentPhase = phase;

        switch (currentPhase)
        {
            case LitmusPhase.Purple:
                SetSprite(0);
                break;

            case LitmusPhase.Green:
                SetSprite(1);
                break;

            case LitmusPhase.Red:
                SetSprite(2);
                break;
        }
    }

    private void SetSprite(int index)
    {
        if (spriteRenderer == null)
            return;

        if (changingSprites == null ||
            index < 0 ||
            index >= changingSprites.Count)
            return;

        if (spriteRenderer.sprite == changingSprites[index])
            return;

        spriteRenderer.sprite = changingSprites[index];
    }

    protected override void UpdateStatsHandled()
    {
        base.UpdateStatsHandled();
    }

    public override void UpdateHealth()
    {
        base.UpdateHealth();
        CheckHealthState();
    }

    private void IncreaseSpeedOnce()
    {
        statManager.ModifyStat(
            EStatType.MoveSpeed,
            moveSpeedIncreaser
        );
    }

    protected virtual void ApplyBlastDamage()
    {
        Vector3 blastPos = new Vector3(
            transform.position.x,
            transform.position.y,
            0f
        );

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            blastPos,
            radius,
            damageArea
        );

        float innerRadius = radius * 0.33f;
        float midRadius = radius * 0.66f;

        foreach (Collider2D hit in hits)
        {
            HealthSystem health = hit.GetComponent<HealthSystem>();

            if (health == null)
                health = hit.GetComponentInParent<HealthSystem>();

            if (health != null)
            {
                float distance = Vector2.Distance(
                    new Vector2(blastPos.x, blastPos.y),
                    hit.transform.position
                );

                int damageToDeal;

                if (distance <= innerRadius)
                {
                    damageToDeal = maxDamage;
                }
                else if (distance <= midRadius)
                {
                    damageToDeal = midDamage;
                }
                else
                {
                    damageToDeal = minDamage;
                }

                health.Damage(
                    DamageItems.GetModifiedDamage(damageToDeal)
                );
            }
        }
    }

    private void EnterAcidicBurst()
    {
        ApplyBlastDamage();
        Die();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            radius
        );

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            radius * 0.66f
        );

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(
            transform.position,
            radius * 0.33f
        );
    }
}