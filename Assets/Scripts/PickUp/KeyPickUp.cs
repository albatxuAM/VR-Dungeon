using UnityEngine;

public class KeyPickup : Pickup
{
    [Tooltip("GameObject ref to delete when destroy")]
    public GameObject parentObj;

    protected override void OnPicked(InventoryManager byPlayer)
    {
        PlayPickupFeedback();

        if (parentObj)
            Destroy(parentObj);
        else
            Destroy(gameObject);

        byPlayer.AddToInventory("key");
    }
}
