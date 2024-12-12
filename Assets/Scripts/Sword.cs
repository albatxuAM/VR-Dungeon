using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Sword : MonoBehaviour, IDamaging
{

    [SerializeField]private int damage;
    [SerializeField]private float soundSpeedThreshold;
    [SerializeField]private float speedThreshold;
    [SerializeField] private float atackCd;
    [SerializeField] private AudioSource audioSource;
    private Rigidbody rb;
    private float currentSpeed;
    private bool invulnerable = false;
    private float lastSoundTime = 0f;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Guarda constantemente la velocidad de la espada
        currentSpeed = rb.velocity.magnitude;

        if (currentSpeed > soundSpeedThreshold && Time.time >= lastSoundTime + atackCd)
        {
            PlaySwordSound();
        }
    }

    private void PlaySwordSound()
    {
        audioSource.Play();
        lastSoundTime = Time.time;
    }

    public void Damage()
    {
        // L�gica para cuando la espada golpea algo
        Debug.Log("�La espada ha golpeado!");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica si el objeto que colisiona implementa la interfaz IDamageable
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        // comprueba que el enemigo no es invulnerable y que el jugador este atacando lo suficientemente rapido
        if (damageable != null)
        {
            if (currentSpeed > speedThreshold)
            {
                if (!invulnerable)
                {
                    // Llama a la corrutina HitEnemy
                    StartCoroutine(HitEnemy(damageable));
                }
            }
        }
    }

    // Corrutina que llama al metodo Damage() del objeto que haya golpeado la espada y crea un tempodicador para que tenga un tiempo de espera de ataque
    IEnumerator HitEnemy(IDamageable damageable)
    {
        damageable.TakeDamage(damage);

        invulnerable = true;

        yield return new WaitForSeconds(atackCd);

        invulnerable = false;
    }
}
