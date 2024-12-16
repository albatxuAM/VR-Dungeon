using UnityEngine;

public class KeyAreaTrigger : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que entr� tiene un InventoryManager
        InventoryManager inventoryManager = other.GetComponent<InventoryManager>();

        if (inventoryManager != null)
        {
            // Verifica si el jugador tiene la llave en su inventario
            if (inventoryManager.DoesItemExist("Key"))
            {
                Debug.Log("La llave est� en el inventario. Se puede acceder.");
                inventoryManager.RemoveFromInventory("Key");
                // Realiza las acciones que desees cuando el jugador tiene la llave
                ChangeLevel();  // Ejemplo de acci�n
            }
            else
            {
                Debug.Log("No tienes la llave.");
            }
        }
    }

    private void ChangeLevel()
    {
        // L�gica para abrir la puerta o activar el �rea
        Debug.Log("Cambio de nivel.");
        // Aqu� puedes agregar animaciones, efectos, etc.

        //SceneManager.LoadScene("MainMenu");
    }
}
