using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Economy/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public int price;
    public float weight;
    public Sprite icon;
}