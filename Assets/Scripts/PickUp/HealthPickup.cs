using UnityEngine;

public class HealthPickup : Pickup
{
    [Header("Parameters")]
    [Tooltip("Amount of health to heal on pickup")]
    public int HealAmount;

    [Tooltip("GameObject ref to delete when destroy")]
    public GameObject parentObj;

    [Tooltip("Destroy obj when destroyed")]
    public bool destroyable = true;

    protected override void OnPicked(InventoryManager player)
    {
        PlayerBasics playerHealth = player.GetComponent<PlayerBasics>();
        if (playerHealth && playerHealth.CanPickup())
        {
            playerHealth.Heal(HealAmount);
            PlayPickupFeedback();

            // Referencia al objeto objetivo (parentObj o el propio gameObject)
            GameObject target = parentObj ? parentObj : gameObject;

            if (destroyable)
                Destroy(target);
            else
                target.SetActive(false);
        }
    }
}
