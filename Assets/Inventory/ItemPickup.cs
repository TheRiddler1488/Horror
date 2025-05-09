using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData data;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out InventorySystem inventory))
        {
            inventory.Pickup(data);
            Destroy(gameObject);
        }
    }
}
