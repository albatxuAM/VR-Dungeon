using Unity.VisualScripting;
using UnityEngine;

public class Sword : MonoBehaviour, IDamaging
{

    [SerializeField]private int damage;
    private Rigidbody rb;
[SerializeField]    private float speedThreshold = 100f;
    // private float cooldownTime = 0.5f;
    private float currentSpeed;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        currentSpeed = rb.velocity.magnitude;
    }

    public void Damage()
    {
        // L�gica para cuando la espada golpea algo
        Debug.Log("�La espada ha golpeado!");
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
        {
            if (currentSpeed > speedThreshold)
            {
                damageable.Damage(damage);
            }
        }
    }
}
