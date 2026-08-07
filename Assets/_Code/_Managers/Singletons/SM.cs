
using UnityEngine;
using System;
public class SM : Singleton<SM>
{
    public PlayerController2D Player;
    public XpManager XPManager;
    public GameStatManager GameStatManager;

    private void Awake()
    {
        XPManager = GetComponentInChildren<XpManager>();
        GameStatManager = GetComponentInChildren<GameStatManager>();
    }

    
}
