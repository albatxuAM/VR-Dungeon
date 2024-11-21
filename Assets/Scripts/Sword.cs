using UnityEngine;

public class Sword : MonoBehaviour, IDamaging
{
    public void Damage()
    {
        // Lógica para cuando la espada golpea algo
        Debug.Log("¡La espada ha golpeado!");
    }
}
