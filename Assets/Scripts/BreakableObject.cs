using UnityEngine;


public class BreakableObject : MonoBehaviour
{
    public GameObject brokenVersionPrefab;
    [Range(0f, 1f)]
    public float breakProbability = 0.25f;

    public GameObject breakParticles;
    public float breakHeight = 2f;
    private bool isBeingHeld = false;
    private bool breakedByFalling = false;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //if (!isBeingHeld)
        //{
        //    CheckIfFallen();
        //}
    }

    public void OnReleased()
    {
        isBeingHeld = false;

        CheckIfFallen();
    }

    public void OnGrabbed()
    {
        isBeingHeld = true;
    }

    private void CheckIfFallen()
    {
        if (transform.position.y <= breakHeight)
        {
            //BreakObject();
            breakedByFalling = true;
        }
    }

    private void BreakObject()
    {
        // Generamos un número aleatorio entre 0 y 1
        float randomValue = Random.Range(0f, 1f);

        // Comprobamos si el número aleatorio es menor o igual que la probabilidad de generar
        if (randomValue <= breakProbability)
        {
            Instantiate(brokenVersionPrefab, transform.position, Quaternion.identity);
        }
        Instantiate(breakParticles, transform.position, transform.rotation);
        Destroy(gameObject); // Destruir el objeto original
    }

    private void SpawnInside()
    {
        // Generamos un número aleatorio entre 0 y 1
        float randomValue = Random.Range(0f, 1f);

        // Comprobamos si el número aleatorio es menor o igual que la probabilidad de generar
        if (randomValue <= breakProbability)
        {
            Instantiate(brokenVersionPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verificar si el objeto que colisiona implementa la interfaz IDamaging
        IDamaging damagingObject = collision.gameObject.GetComponent<IDamaging>();
        if (damagingObject != null)
        {
            // Llamamos al m�todo Damage() cuando un objeto da�ino golpea

        }

        if (breakedByFalling)
        {
            BreakObject();
        }
    }
}
