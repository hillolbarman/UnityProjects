using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static int score { get; private set; }
    float lastEnemyKilledTime;
    int curentStreakCount;
    float streakExpiryTime = 1;

    void Start()
    {
        Enemy.OnDeathStatic += OnEnemyKilled;
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
        score = 0;
    }

    void Update()
    {
        
    }

    void OnEnemyKilled()
    {
        if (Time.time < lastEnemyKilledTime + streakExpiryTime)
        {
            curentStreakCount++;
        }
        else
        {
            curentStreakCount = 0;
        }
        
        lastEnemyKilledTime = Time.time;
        score += 4+(int)Mathf.Pow(2,curentStreakCount);
    }

    void OnPlayerDeath()
    {
        Enemy.OnDeathStatic -= OnEnemyKilled;
    }
}
