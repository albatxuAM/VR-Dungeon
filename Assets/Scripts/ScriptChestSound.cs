using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptChestSound : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip audioClip;
    // Start is called before the first frame update
    
    public void OpenChestSound()
    {
        audioSource.PlayOneShot(audioClip);
    }
}
