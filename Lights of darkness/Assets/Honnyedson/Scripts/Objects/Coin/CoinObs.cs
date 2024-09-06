using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoinObs
{

    public static event Action<int> coin;

    public static void OnCoin(int obj)
    {
        coin?.Invoke(obj);
    }
}
