public class KeyPickup : Pickup
{
    protected override void OnPicked(InventoryManager byPlayer)
    {
        PlayPickupFeedback();
        Destroy(gameObject);
        //byPlayer.AddToInventory("key");
    }
}
