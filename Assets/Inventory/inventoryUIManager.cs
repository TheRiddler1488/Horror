using UnityEngine;
using System.Collections.Generic;

public class InventoryUIManager : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotsParent;

    private List<ItemSlot> slots = new();
    private InventorySystem inventorySystem;

    public void Setup(InventorySystem inv)
    {
        inventorySystem = inv;

        for (int i = 0; i < inv.maxSlots; i++)
        {
            GameObject obj = Instantiate(slotPrefab, slotsParent);
            ItemSlot slot = obj.GetComponent<ItemSlot>();
            slots.Add(slot);
        }
    }

    public void RefreshUI(List<ItemData> items, int selectedIndex)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < items.Count)
                slots[i].SetItem(items[i].icon);
            else
                slots[i].SetItem(null);

            slots[i].SetHighlight(i == selectedIndex);
        }
    }
}
