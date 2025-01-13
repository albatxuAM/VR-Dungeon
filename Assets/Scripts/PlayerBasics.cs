using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBasics : MonoBehaviour, IDamageable
{

    [SerializeField] public int maxHealth;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameObject hurtTunneling;
    [SerializeField] private AudioClip hurt;
    [SerializeField] private AudioSource source;
    private int currentHealth;


    //public bool CanPickup() => currentHealth < maxHealth;

    public bool CanPickup()
    {
        Debug.Log("currentHealth: " + currentHealth + " maxHealth: " + maxHealth);
        return currentHealth < maxHealth;
    }


    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    Debug.Log(currentHealth);
        //}
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    TakeDamage(1);
        //}
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        source.PlayOneShot(hurt);
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

    public void Heal(int healAmount)
    {
        int healthBefore = currentHealth;
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Te has curado. Vida anterior: " + healthBefore + " Vida restante: " + currentHealth);
        healthBar.SetHealth(currentHealth);
        // call OnHeal action
        //float trueHealAmount = currentHealth - healthBefore;
        //if (trueHealAmount > 0f)
        //{
        //    //OnHealed?.Invoke(trueHealAmount);
        //}
    }
}
