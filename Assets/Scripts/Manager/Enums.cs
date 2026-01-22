using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EStatType
{
    MoveSpeed,
    Health,
    HealthRegen,
    AttackCooldown,
    ActiveDuration,
    ProjectileCount,
    ProjectileSpeed,
    Damage,
    AOESize,
    FireRate
}

public enum EWeaponName
{
    AcidRainClouds,
    Bacteria,
    Laser,
    LithiumIon,
    Magnet,
    Prism,
    SolarSystem,
    TuningFork,
}

public enum EPriority
{
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5
}

public enum ECardType
{
    AddsWeapon,
    AffectsPlayer,
    AffectsEnemy,
    AffectsWeaponLevel,
    AffectsSpecificWeaponStat,
    AffectsAllWeaponsStat,
}