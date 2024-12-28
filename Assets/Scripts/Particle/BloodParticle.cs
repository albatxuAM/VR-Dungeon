using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticle : MonoBehaviour
{
    // variable para cargar el prefab de las particulas de la sangre, su transfomr y un boolean que revisa que se haiga asignado el prefab

[SerializeField] private ParticleSystem particleFX;
[SerializeField] private Transform particleOrigin;
private bool _hasParticleSystem;

private void Awake()
{
    //revisa que no este vacio el apartado del prefab
    if( particleFX !=null)
        {
            _hasParticleSystem = true;
        }
}
public void PlayDamageFx()
        {
            //hace un return  y despues ejecuta el sistema de particulas
            if(!_hasParticleSystem) return;
            particleFX.transform.SetPositionAndRotation(particleOrigin.position,particleOrigin.rotation);
            particleFX.Play();

        }
    }
