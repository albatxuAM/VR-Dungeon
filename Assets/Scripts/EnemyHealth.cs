using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{

    [SerializeField]public int health;
    private int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = health;
    }

    public void Damage(int damage)
    {
        if (currentHealth <= 0)
        {
            Debug.Log("Enemigo derrotado");
            Destroy(this.gameObject);
        } else {
            Debug.Log("Enemigo golpeado");
            currentHealth -= damage;
        }
    }

}
