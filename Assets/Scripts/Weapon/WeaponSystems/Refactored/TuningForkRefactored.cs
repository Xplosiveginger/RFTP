using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuningForkRefactored : WeaponBase
{
    protected override void Awake()
    {
        base.Awake();

        UpdateStatsHandled();
    }
}
