using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Tooltip("Player's maximum inventory slots")]
    public int MaxInventorySlots = 10;

    // Inventory system
    private Dictionary<string, int> inventory = new Dictionary<string, int>();

    public bool AddToInventory(string itemName, int quantity = 1)
    {
        // Convertir el nombre del ítem a minúsculas
        itemName = itemName.ToLower();

        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName] += quantity;
        }
        else
        {
            if (inventory.Count >= MaxInventorySlots)
            {
                Debug.LogWarning("Inventory is full!");
                return false;
            }

            inventory[itemName] = quantity;
        }

        Debug.Log($"Picked up {itemName}. Total: {inventory[itemName]}");
        return true;
    }

    public void RemoveFromInventory(string itemName, int quantity = 1)
    {
        // Convertir el nombre del ítem a minúsculas
        itemName = itemName.ToLower();

        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName] -= quantity;

            if (inventory[itemName] <= 0)
            {
                inventory.Remove(itemName);
            }

            Debug.Log($"Removed {itemName}. Remaining: {(inventory.ContainsKey(itemName) ? inventory[itemName] : 0)}");
        }
        else
        {
            Debug.LogWarning($"Item {itemName} not found in inventory.");
        }
    }

    public Dictionary<string, int> GetInventory()
    {
        return new Dictionary<string, int>(inventory);
    }

    public bool DoesItemExist(string itemName)
    {
        // Convertir el nombre del ítem a minúsculas
        itemName = itemName.ToLower();

        return inventory.ContainsKey(itemName);
    }
}
