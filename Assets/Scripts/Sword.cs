using UnityEngine;

public class Sword : MonoBehaviour, IDamaging
{
    public void Damage()
    {
        // L�gica para cuando la espada golpea algo
        Debug.Log("�La espada ha golpeado!");
    }
}
