using UnityEngine;

public class Trader : MonoBehaviour
{
    public ItemData itemForSale;

    public bool TryBuy(Wallet buyerWallet, Inventory buyerInventory)
    {
        if (!buyerWallet.TrySpend(itemForSale.price))
        {
            return false;
        }

        buyerInventory.AddItem(itemForSale);
        return true;
    }
}