using UnityEngine;

public class HealthPickup : Pickup
{
    [Header("Parameters")]
    [Tooltip("Amount of health to heal on pickup")]
    public int HealAmount;

    [Tooltip("GameObject ref to delete when destroy")]
    public GameObject parentObj;

    protected override void OnPicked(InventoryManager player)
    {
        PlayerBasics playerHealth = player.GetComponent<PlayerBasics>();
        if (playerHealth && playerHealth.CanPickup())
        {
            playerHealth.Heal(HealAmount);
            PlayPickupFeedback();
            //Destroy(gameObject);
            if (parentObj)
                Destroy(parentObj);
            else
                Destroy(gameObject);
        }
    }
}
