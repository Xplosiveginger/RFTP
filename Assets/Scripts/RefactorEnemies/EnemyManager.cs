using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [SerializeField] private Transform player;

    private readonly List<BaseEnemyRefactor> activeEnemies = new List<BaseEnemyRefactor>();
    private readonly List<ENP_Enemy> enpEnemies = new List<ENP_Enemy>(); 
    private readonly List<MutatedRats_Refactor> mutatedRats = new List<MutatedRats_Refactor>();
    private readonly List<Skeleton_refactor> skeletons = new List<Skeleton_refactor>();
    private readonly List<LitmusPaper_Refactor> litmusPaper = new List<LitmusPaper_Refactor>();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (player == null) return;

        Vector3 playerPos = player.position;

        HandleEnemyMovement(playerPos);
        HandleENPCombination();
        HandleMutatedRatAcid(playerPos, Time.deltaTime);
        HandleSkeletonAttacks(playerPos, Time.deltaTime);
    }

    // =======================================================
    // MOVEMENT
    // =======================================================
    private void HandleEnemyMovement(Vector3 playerPos)
    {
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            activeEnemies[i].UpdateMovement(playerPos);
        }
    }

    // =======================================================
    // ENP COMBINATION
    // =======================================================
    private void HandleENPCombination()
    {
        if (enpEnemies.Count < 3) return;

        for (int i = 0; i < enpEnemies.Count; i++)
        {
            ENP_Enemy e1 = enpEnemies[i];

            for (int j = i + 1; j < enpEnemies.Count; j++)
            {
                ENP_Enemy e2 = enpEnemies[j];
                if (Vector3.Distance(e1.transform.position, e2.transform.position) > Mathf.Max(e1.combineRadius, e2.combineRadius))
                    continue;

                for (int k = j + 1; k < enpEnemies.Count; k++)
                {
                    ENP_Enemy e3 = enpEnemies[k];
                    if (Vector3.Distance(e1.transform.position, e3.transform.position) > Mathf.Max(e1.combineRadius, e3.combineRadius))
                        continue;

                    bool electron = false, proton = false, neutron = false;
                    ENP_Enemy[] group = { e1, e2, e3 };

                    foreach (var item in group)
                    {
                        switch (item.Type)
                        {
                            case ENPType.Electron: electron = true; break;
                            case ENPType.Proton: proton = true; break;
                            case ENPType.Neutron: neutron = true; break;
                        }
                    }

                    if (electron && proton && neutron)
                    {
                        Vector3 spawnPos = (e1.transform.position + e2.transform.position + e3.transform.position) / 3f;
                        Debug.Log("🔵 ENP Combination → Atom Spawn!");

                        foreach (var item in group)
                            DespawnEnemy(item);

                        EnemySpawner.Instance.SpawnAtom(spawnPos);
                        return; // Only one combination per frame
                    }
                }
            }
        }
    }

    // =======================================================
    // MUTATED RAT ACID SPAWNING
    // =======================================================
    private void HandleMutatedRatAcid(Vector3 playerPos, float deltaTime)
    {
        for (int i = 0; i < mutatedRats.Count; i++)
        {
            mutatedRats[i].CheckAcidSpawn(playerPos, deltaTime);
        }
    }

    // =======================================================
    // SKELETON ATTACK
    // =======================================================
    private void HandleSkeletonAttacks(Vector3 playerPos, float deltaTime)
    {
        for (int i = 0; i < skeletons.Count; i++)
        {
            skeletons[i].CheckAttack(playerPos, deltaTime);
        }
    }

    // =======================================================
    // SPAWNING / DESPAWNING
    // =======================================================
    public void RegisterEnemy(BaseEnemyRefactor enemy)
    {
        if (!activeEnemies.Contains(enemy))
            activeEnemies.Add(enemy);

        if (enemy is ENP_Enemy enp && !enpEnemies.Contains(enp))
            enpEnemies.Add(enp);

        if (enemy is MutatedRats_Refactor rat && !mutatedRats.Contains(rat))
            mutatedRats.Add(rat);

        if (enemy is Skeleton_refactor skel && !skeletons.Contains(skel))
            skeletons.Add(skel);
        
        if(enemy is LitmusPaper_Refactor litmus && !litmusPaper.Contains(litmus))
            litmusPaper.Add(litmus);    
    }

    public void DespawnEnemy(BaseEnemyRefactor enemy)
    {
        if (enemy == null) return;

        activeEnemies.Remove(enemy);

        if (enemy is ENP_Enemy enp)
            enpEnemies.Remove(enp);

        if (enemy is MutatedRats_Refactor rat)
            mutatedRats.Remove(rat);

        if (enemy is Skeleton_refactor skel)
            skeletons.Remove(skel);

        if(enemy is LitmusPaper_Refactor litmus)
            litmusPaper.Remove(litmus);

        enemy.ResetOnDeath();
        enemy.gameObject.SetActive(false);
    }

    public void SetPlayer(Transform p) => player = p;
}
