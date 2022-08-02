using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int health;
    public int damage;

    public int maxHealth = 10;

    private void Start()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        health = maxHealth;
    }
}
