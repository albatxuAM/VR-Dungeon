using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTouch : MonoBehaviour
{
    // Cuando un objeto con un RigidBody colisione con este, sera destruido
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.gameObject);
        Debug.Log("objeto destruido");
    }
}
