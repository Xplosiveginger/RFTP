using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatListUI : MonoBehaviour
{
    public GameObject player;
    public List<Stat> statList;
    public GameObject statContainer;

    public List<GameObject> statContainerList;

    private void OnEnable()
    {
        ClearUIList();
        GetStatList();
        PopulateUIList();
    }

    private void GetStatList()
    {
        statList = player.GetComponent<StatManager>().GetAllStats();
    }

    private void PopulateUIList()
    {
        GetComponent<VerticalLayoutGroup>().enabled = true;

        foreach (Stat stat in statList)
        {
            GameObject go = Instantiate(statContainer);
            go.transform.SetParent(transform);

            go.transform.GetChild(0).GetComponent<TMP_Text>().text = stat.statName.ToString();
            go.transform.GetChild(1).GetComponent<TMP_Text>().text = stat.currentMultiplier.ToString();
            statContainerList.Add(go);
        }

        Invoke("DisableVerticalLayoutGroup", 0.1f);
    }

    private void ClearUIList()
    {
        foreach(GameObject go in statContainerList)
        {
            Destroy(go);
        }
        statContainerList.Clear();
    }

    private void DisableVerticalLayoutGroup()
    {
        GetComponent<VerticalLayoutGroup>().enabled = false;
    }
}