using UnityEngine;

public class KeyAreaTrigger : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que entró tiene un InventoryManager
        InventoryManager inventoryManager = other.GetComponent<InventoryManager>();

        if (inventoryManager != null)
        {
            // Verifica si el jugador tiene la llave en su inventario
            if (inventoryManager.DoesItemExist("Key"))
            {
                Debug.Log("La llave está en el inventario. Se puede acceder.");
                inventoryManager.RemoveFromInventory("Key");
                // Realiza las acciones que desees cuando el jugador tiene la llave
                ChangeLevel();  // Ejemplo de acción
            }
            else
            {
                Debug.Log("No tienes la llave.");
            }
        }
    }

    private void ChangeLevel()
    {
        // Lógica para abrir la puerta o activar el área
        Debug.Log("Cambio de nivel.");
        // Aquí puedes agregar animaciones, efectos, etc.

        //SceneManager.LoadScene("MainMenu");
    }
}
