using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class LitmusPaper_Refactor : BaseEnemyRefactor
{
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

    private SpriteRenderer spriteRenderer;
    private bool exploded = false;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (changingSprites != null && changingSprites.Count > 0)
            spriteRenderer.sprite = changingSprites[0];

        EnemyManager.Instance.RegisterEnemy(this);
    }
    public void CheckHealthState()
    {

        float hpPercent = (float)statManager.GetStat(EStatType.Health).currentValue / maxHealth;

        if (hpPercent <= 25f && !exploded)
        {
            exploded = true;
            EnterAcidicBurst();
            return;
        }

        if (hpPercent <= 0.25f)
        {
            // Stage 3 → Acidic (Red)
            IncreaseSpeedOnce();
            SetSprite(2);
        }
        else if (hpPercent <= 0.75f)
        {
            // Stage 2 → Neutral (Green)
            IncreaseSpeedOnce();
            SetSprite(1);
        }
        else if (hpPercent <= 1f)
        {
            // Stage 1 → Alkaline shifting
            SetSprite(0);
        }
    }

    private void SetSprite(int index)
    {
        if (changingSprites == null || index >= changingSprites.Count) return;
        if (spriteRenderer.sprite == changingSprites[index]) return; // avoid spam

        spriteRenderer.sprite = changingSprites[index];
    }
    private void EnterAcidicBurst()
    {

        ApplyBlastDamage();
        Die();
    }
    protected virtual void ApplyBlastDamage()
    {
        // Ensure we operate on the 2D plane (Z = 0)
        Vector3 blastPos = new Vector3(transform.position.x, transform.position.y, 0f);

        // Overlap search
        Collider2D[] hits = Physics2D.OverlapCircleAll(blastPos, radius, damageArea);

        // Band thresholds
        float innerRadius = radius * 0.33f; // 0–33%
        float midRadius = radius * 0.66f;   // 33–66%

        foreach (Collider2D hit in hits)
        {
            // Try to find HealthSystem on the collider or its parents
            HealthSystem health = hit.GetComponent<HealthSystem>();
            if (health == null)
                health = hit.GetComponentInParent<HealthSystem>();

            if (health != null)
            {
                float distance = Vector2.Distance(new Vector2(blastPos.x, blastPos.y), hit.transform.position);
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

                // Call the existing HealthSystem method
                health.Damage(DamageItems.GetModifiedDamage(damageToDeal));
            }
        }
    }

    protected override void UpdateStatsHandled()
    {
        base.UpdateStatsHandled();
        CheckHealthState();
    }

    private void IncreaseSpeedOnce()
    {
       statManager.ModifyStat(EStatType.MoveSpeed, moveSpeedIncreaser);
    }
    private void OnDrawGizmos()
    {
        // Draw outer -> mid -> inner with colors similar to your old script
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius * 0.66f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius * 0.33f);
    }
}