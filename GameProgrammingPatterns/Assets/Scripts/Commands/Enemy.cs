using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject enemyPrefab;
    

    public static event Action OnEnemyDestroyed;

    private void OnDisable()
    {
        OnEnemyDestroyed?.Invoke();
    }
    private void OnTriggerEnter(Collider other)
    {
        Destroy(enemyPrefab);
    }
}
