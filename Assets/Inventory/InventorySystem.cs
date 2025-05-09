using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class InventorySystem : NetworkBehaviour
{
    [SerializeField] public Transform itemHoldPoint;
    [SerializeField] public List<ItemSlot> slots;
    [SerializeField] public int maxSlots = 4;
    [SerializeField] public int selectedSlot = 0;

    private GameObject heldItemInstance;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            SetSelectedSlot(0);
    }

    public void SetSelectedSlot(int index)
    {
        if (index < 0 || index >= maxSlots) return;
        selectedSlot = index;
        UpdateUI();
    }

    public void Pickup(ItemData data)
    {
        if (selectedSlot < 0 || selectedSlot >= slots.Count) return;

        GameObject itemPrefab = data.prefab;
        GameObject instance = Instantiate(itemPrefab);
        instance.GetComponent<NetworkObject>()?.Spawn();
        slots[selectedSlot].AssignItem(data, instance);
    }

    [ServerRpc]
    public void PickupItemServerRpc(NetworkObjectReference itemRef, string prefabName)
    {
        if (!itemRef.TryGet(out NetworkObject item)) return;

        item.Despawn();
        SpawnItemInHandClientRpc(prefabName);
    }

    [ClientRpc]
    private void SpawnItemInHandClientRpc(string prefabName)
    {
        if (!IsOwner) return;

        GameObject prefab = Resources.Load<GameObject>("Items/" + prefabName);
        if (prefab == null)
        {
            Debug.LogWarning("Не найден префаб: " + prefabName);
            return;
        }

        heldItemInstance = Instantiate(prefab, itemHoldPoint.position, itemHoldPoint.rotation, itemHoldPoint);
        heldItemInstance.GetComponent<NetworkObject>()?.Spawn();
    }

    [ServerRpc]
    public void DropItemServerRpc()
    {
        if (heldItemInstance == null) return;

        NetworkObject netObj = heldItemInstance.GetComponent<NetworkObject>();
        if (netObj != null && netObj.IsSpawned)
        {
            heldItemInstance.transform.SetParent(null);
            netObj.Spawn();
        }
        heldItemInstance = null;
        ClearSelectedSlotClientRpc();
    }

    [ClientRpc]
    private void ClearSelectedSlotClientRpc()
    {
        if (selectedSlot >= 0 && selectedSlot < slots.Count)
        {
            slots[selectedSlot].ClearSlot();
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].SetSelected(i == selectedSlot);
        }
    }
}
