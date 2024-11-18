using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class tp : MonoBehaviour
{

    [SerializeField] GameObject player;
    [SerializeField] GameObject portal;

    void Start()
    {
        player = GetComponent<GameObject>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.transform.position = portal.transform.position;
        }
    }
}
