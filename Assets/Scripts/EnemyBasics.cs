using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class EnemyBasics : MonoBehaviour, IDamageable
{

    //Vida
    [SerializeField]private int health;
    private int currentHealth;

    //NavAgent
    [SerializeField]private NavMeshAgent agent;
    [SerializeField]public Transform player;
    [SerializeField]public LayerMask whatIsGround, whatIsWall, whatIsPlayer;

    //Patrullaje
    private Vector3 walkPoint;
    private bool walkPointSet;
    [SerializeField]private float walkPointRange;

    //Estados
    [SerializeField]private float sightRange;
    public bool playerInSightRange;

    //Sistema de ataque
    [SerializeField]private int damage;
    [SerializeField]private float atackCd;
    private bool invulnerable = false;

    private Rigidbody rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = health;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // Mira si el jugador esta dentro del rango de vista
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (!playerInSightRange) Patroling();
        if (playerInSightRange) ChasePlayer();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Enemigo golpeado. Vida restante: " + currentHealth);

        if (currentHealth <= 0) 
        {
            rb.useGravity = true;
            
            
            Invoke(nameof(DestroyEnemy), 3f);
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
        Debug.Log("Enemigo derrotado");
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        //Calcula un punto alatorio dentro del rango
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        Vector3 directionToWalkPoint = walkPoint - transform.position;

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround) && !Physics.Raycast(transform.position, directionToWalkPoint.normalized, directionToWalkPoint.magnitude, whatIsWall)) walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void OnDrawGizmosSelected()
    {
        // Rango de visión
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // Rango de patrullaje
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, walkPointRange);

        // Punto al que el enemigo se moverá, solo si el punto ya está establecido
        if (walkPointSet)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(walkPoint, 0.1f);
        }

        Debug.DrawLine(transform.position, walkPoint, Color.green, 2f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica si el objeto que colisiona implementa la interfaz IDamageable
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        // comprueba que el player no es invulnerable
        if (damageable != null && collision.gameObject.tag != "Enemy")
        {
            if (!invulnerable)
            {
                // Llama a la corrutina HitEnemy
                StartCoroutine(HitEnemy(damageable));
                //Debug.Log("has sido atacado");
            }
        }
    }

    // Corrutina que llama al metodo Damage() del objeto que haya golpeado la espada y crea un tempodicador para que tenga un tiempo de espera de ataque
    IEnumerator HitEnemy(IDamageable damageable)
    {
        damageable.TakeDamage(damage);

        invulnerable = true;

        Transform playerSave = player;

        player = this.transform;

        yield return new WaitForSeconds(atackCd);

        invulnerable = false;

        player = playerSave;
    }
}
