using System;
using TMPro;
using UnityEngine;

public class RunReportManager : MonoBehaviour
{
    public static RunReportManager Instance;
    public GameStat_SO gameStat;
    public RunReport_UI runReportUI;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
}