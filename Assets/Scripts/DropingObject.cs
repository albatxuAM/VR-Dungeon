using UnityEngine;


public class DropingObject : MonoBehaviour
{
    public GameObject dropPrefab;
    [Range(0f, 1f)]
    public float dropProbability = 0.25f;

    private bool drop = false;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        drop = true;
    }
    public void SpawnInside()
    {
        if (drop)
        {
            drop = false;

            // Generamos un número aleatorio entre 0 y 1
            float randomValue = Random.Range(0f, 1f);

            // Comprobamos si el número aleatorio es menor o igual que la probabilidad de generar
            if (randomValue <= dropProbability)
            {
                Vector3 pos = transform.position;
                pos.y += 0.5f;
                Instantiate(dropPrefab, pos, Quaternion.identity);
            }
        }
    }
}
