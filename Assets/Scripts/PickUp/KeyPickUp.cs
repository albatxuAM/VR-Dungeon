using UnityEngine;

public class KeyPickup : Pickup
{
    [Tooltip("GameObject ref to delete when destroy")]
    public GameObject parentObj;

    protected override void OnPicked(InventoryManager byPlayer)
    {
        PlayPickupFeedback();

        GameObject target = parentObj ? parentObj : gameObject;
        Destroy(target);

        byPlayer.AddToInventory("key");
    }
}
