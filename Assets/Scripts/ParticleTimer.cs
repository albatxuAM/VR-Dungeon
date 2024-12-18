using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTimer : MonoBehaviour
{

    void OnEnable()
    {
        StartCoroutine(DeleteParticle());
    }

    IEnumerator DeleteParticle()
    {
        yield return new WaitForSeconds(1f);
        SimplePool.Despawn(gameObject);
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
