using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemLevel
{
    public GameObject itemPrefab;
    public Sprite cardSprite;            // ? Card image for this level
    [TextArea] public string levelUpInfo; // ? New: Info text for this level
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;

    [Header("Per Level Settings")]
    public List<ItemLevel> levels;
}
