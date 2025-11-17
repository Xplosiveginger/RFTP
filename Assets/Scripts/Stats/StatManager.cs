using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public HealthSystem health;

    public List<Stat> statList;

    private void Start()
    {
        foreach(Stat stat in statList)
        {
            stat.Init();
        }
    }
}
