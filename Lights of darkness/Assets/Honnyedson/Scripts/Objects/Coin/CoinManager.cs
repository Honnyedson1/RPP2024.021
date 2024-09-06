using System;
using UnityEngine;
using System.Collections.Generic;
public class CoinManager : MonoBehaviour
{
    public int CoinCaunt = 0;

    
    public void AddCoin(int i)
    {
        CoinCaunt += i;
    }

    private void OnEnable()
    {
        CoinObs.coin += AddCoin;
    }

    private void OnDisable()
    {
        CoinObs.coin -= AddCoin;
    }
}