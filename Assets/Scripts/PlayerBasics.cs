using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBasics : MonoBehaviour, IDamageable
{

    [SerializeField] public int maxHealth;
    private int currentHealth;

    public bool CanPickup() => currentHealth < maxHealth;

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
