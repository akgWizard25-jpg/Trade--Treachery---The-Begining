using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoinSystem 
{
    public static string ConvertCoinToString(long amount)
    {
        double result = amount;

        if (amount >= 1_000_000_000) // Billion
        {
            result = amount / 1_000_000_000.0;
            return result.ToString("0.##") + "B"; // Keeps 2 decimal places if needed
        }
        else if (amount >= 1_000_000) // Million
        {
            result = amount / 1_000_000.0;
            return result.ToString("0.##") + "M";
        }
        else if (amount >= 1_000) // Thousand
        {
            result = amount / 1_000.0;
            return result.ToString("0.##") + "K";
        }

        return amount.ToString(); // Less than 1K, return as is
    }

}
