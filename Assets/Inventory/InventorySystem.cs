using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class InventorySystem : NetworkBehaviour
{
    public Transform itemHoldPosition;
    public int maxSlots = 4;

    public List<ItemData> inventory = new();
    private GameObject heldItem;
    private int selectedSlot = 0;

    [SerializeField] private InventoryUIManager uiManager;

    void Start()
    {
        if (IsOwner)
            uiManager.Setup(this);
    }

    void Update()
    {
        if (!IsOwner) return;

        for (int i = 0; i < maxSlots; i++)
        {
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i)))
                SelectSlot(i);
        }

        if (Input.GetMouseButtonDown(0))
            UseSelectedItem();
    }

    public void Pickup(ItemData item)
    {
        if (inventory.Count >= maxSlots) return;
        inventory.Add(item);
        uiManager.RefreshUI(inventory, selectedSlot);
        if (inventory.Count == 1)
            SelectSlot(0);
    }

    private void SelectSlot(int index)
    {
        if (index >= inventory.Count) return;
        selectedSlot = index;
        SpawnHeldItem(inventory[index]);
        uiManager.RefreshUI(inventory, selectedSlot);
    }

    private void SpawnHeldItem(ItemData data)
    {
        if (heldItem) Destroy(heldItem);
        heldItem = Instantiate(data.prefab, itemHoldPosition);
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;
    }

    private void UseSelectedItem()
    {
        if (heldItem == null) return;

        UsableItem usable = heldItem.GetComponent<UsableItem>();
        if (usable != null)
            usable.Use();
    }
}
