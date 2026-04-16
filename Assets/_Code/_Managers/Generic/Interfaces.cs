using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IHealth: IDamageable
{
  
}

public interface IDamageable
{
    public void TakeDamage();
}
