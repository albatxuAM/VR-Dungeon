using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasics : MonoBehaviour, IDamageable
{

    [SerializeField]public int maxHealth;
    private int currentHealth;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log(currentHealth);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("te has hecho daño, vida restante" + currentHealth);
    }
}
