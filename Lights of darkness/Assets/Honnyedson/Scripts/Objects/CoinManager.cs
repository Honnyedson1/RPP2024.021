using System;
using UnityEngine;
using System.Collections.Generic;

// Observável (Subject)
public class CoinManager : MonoBehaviour
{
    private List<IObserver<Coin>> observers = new List<IObserver<Coin>>();

    public void Attach(IObserver<Coin> observer)
    {
        observers.Add(observer);
    }

    public void Detach(IObserver<Coin> observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers(Coin coin)
    {
        foreach (var observer in observers)
        {
            observer.OnNext(coin);
        }
    }

    public void CollectCoin(Coin coin)
    {
        // Lógica para coletar a moeda
        Destroy(coin.gameObject);
        NotifyObservers(coin);
    }
}