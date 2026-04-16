using UnityEngine;
using UnityEngine.AI;

public enum ENPType
{
    Electron,
    Proton,
    Neutron
}

public class ENP_Enemy : BaseEnemyRefactor
{
    [Header("ENP Type")]
    public ENPType Type;

    [Header("Combination Settings")]
    public float combineRadius = 3f;
    public Color gizmoColor = new Color(0, 1, 1, 0.25f);
    public bool showGizmo = true;

    // ENP enemies no longer need Awake registration; handled via EnemyManager/Spawner

    private void OnDrawGizmosSelected()
    {
        if (!showGizmo) return;
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, combineRadius);
    }
}
