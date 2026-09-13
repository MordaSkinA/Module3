using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private readonly Dictionary<ItemData, int> items = new Dictionary<ItemData, int>();

    public void AddItem(ItemData item, int amount = 1)
    {
        if (items.ContainsKey(item))
        {
            items[item] += amount;
        }
        else
        {
            items[item] = amount;
        }
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (!items.ContainsKey(item) || items[item] < amount)
        {
            return false;
        }

        items[item] -= amount;

        if (items[item] <= 0)
        {
            items.Remove(item);
        }

        return true;
    }

    public bool HasItem(ItemData item, int amount = 1)
    {
        return items.ContainsKey(item) && items[item] >= amount;
    }
}