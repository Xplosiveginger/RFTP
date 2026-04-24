
using UnityEngine;
using System;
public class SM : Singleton<SM>
{

    [HideInInspector] public XpManager XPManager;

    private void Awake()
    {
        XPManager = GetComponentInChildren<XpManager>();
    }

}
