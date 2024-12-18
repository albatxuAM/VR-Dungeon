using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerBasics : MonoBehaviour, IDamageable
{

    [SerializeField] public int maxHealth;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameObject hurtTunneling;
    private int currentHealth;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log(currentHealth);
        }
        if (Input.GetKeyDown(KeyCode.Space))
		{
			TakeDamage(1);
		}
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        StartCoroutine(HitAnim());
        Debug.Log("Te has hecho daño. Vida restante: " + currentHealth);

        if (currentHealth <= 0) Invoke(nameof(Muerte), 0f);
    }

    IEnumerator HitAnim()
    {
        hurtTunneling.SetActive(true);
        yield return new WaitForSeconds(.5f);
        hurtTunneling.SetActive(false);
    }

    private void Muerte()
    {
        Debug.Log("Te has muerto");
        SceneManager.LoadScene(2);
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
