using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTimer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DeleteParticle());
    }

    IEnumerator DeleteParticle()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);   
    }
}
