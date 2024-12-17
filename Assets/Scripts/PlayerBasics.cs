using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBasics : MonoBehaviour, IDamageable
{

    [SerializeField] public int maxHealth;
    [SerializeField] private HealthBar healthBar;
    private int currentHealth;

    public bool CanPickup() => currentHealth < maxHealth;

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
        Debug.Log("Te has hecho daño. Vida restante: " + currentHealth);

        if (currentHealth <= 0) Invoke(nameof(Muerte), 0f);
    }

    private void Muerte()
    {
        Debug.Log("Te has muerto");
        SceneManager.LoadScene(2);
    }

    public void Heal(int healAmount)
    {
        int healthBefore = currentHealth;
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // call OnHeal action
        float trueHealAmount = currentHealth - healthBefore;
        if (trueHealAmount > 0f)
        {
            //OnHealed?.Invoke(trueHealAmount);
        }
    }
}
