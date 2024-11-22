using UnityEngine;


public class BreakableObject : MonoBehaviour
{
    public GameObject brokenVersionPrefab;
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
        Instantiate(brokenVersionPrefab, transform.position, transform.rotation);
        Instantiate(breakParticles, transform.position, transform.rotation);
        Destroy(gameObject); // Destruir el objeto original
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verificar si el objeto que colisiona implementa la interfaz IDamaging
        IDamaging damagingObject = collision.gameObject.GetComponent<IDamaging>();
        if (damagingObject != null)
        {
            // Llamamos al método Damage() cuando un objeto dañino golpea

        }

        if (breakedByFalling)
        {
            BreakObject();
        }
    }
}
