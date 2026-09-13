using UnityEngine;

public class Wallet : MonoBehaviour
{
    public int coins = 0;

    public void AddCoins(int amount)
    {
        coins += amount;
    }

    public bool TrySpend(int amount)
    {
        if (coins < amount)
        {
            return false;
        }

        coins -= amount;
        return true;
    }
}