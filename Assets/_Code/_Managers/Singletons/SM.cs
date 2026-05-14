
using UnityEngine;
using System;
public class SM : Singleton<SM>
{
    public XpManager XPManager;

    private void Awake()
    {
        XPManager = GetComponentInChildren<XpManager>();
    }

    
}
