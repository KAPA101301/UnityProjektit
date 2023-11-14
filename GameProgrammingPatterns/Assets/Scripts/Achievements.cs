using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievements : MonoBehaviour
{

    private const int nAchievements = 3;
    public enum Achievement_ID
    {
        Coin_Collector,
        Terminator,
        SomeOtherFancyAchievement
    }

    // Are the achievements unlocked
    private bool[] bUnlockedAchievements = new bool[nAchievements];


    private int nCoins = 0;
    private int nEnemies = 0;

    private void Start()
    {
        // Add our method to listen to Coin-class event:
        RotateCoin.OnCoinCollected += CoinWasCollected;
        Enemy.OnEnemyDestroyed += EnemyWasDestroyed;
    }

    void CoinWasCollected()
    {
        nCoins++;
        if (nCoins == 5)
        {
            int index = (int)Achievement_ID.Coin_Collector;
            if (!bUnlockedAchievements[index])
            {
                bUnlockedAchievements[index] = true;
                Debug.Log("You've unlocked: COIN COLLECTOR!!!");
            }
        }
    }
    void EnemyWasDestroyed()
    {
        nEnemies++;
        if (nEnemies == 10)
        {
            int index = (int)Achievement_ID.Terminator;
            if (!bUnlockedAchievements[index])
            {
                bUnlockedAchievements[index] = true;
                Debug.Log("You've unlocked: TERMINATOR!!!");
            }
        }
    }
}
