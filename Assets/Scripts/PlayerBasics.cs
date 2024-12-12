using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        Debug.Log("Te has hecho daño. Vida restante: " + currentHealth);

        if (currentHealth <= 0) Invoke(nameof(Muerte), 0f);
    }

    private void Muerte()
    {
        Debug.Log("Te has muerto");
        SceneManager.LoadScene(2);
    }
}
