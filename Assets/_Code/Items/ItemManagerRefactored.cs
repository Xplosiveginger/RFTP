using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManagerRefactored : MonoBehaviour
{
    public List<Item> activeItems;

    public void Initialize()
    {
        activeItems = new List<Item>();
    }

    public void AddItem(Item itemData)
    {
        activeItems.Add(itemData);
    }

    //private void 
}
