using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image icon;
    public GameObject selectorFrame;

    public ItemData itemData { get; private set; }
    public GameObject itemObject { get; private set; }

    public void AssignItem(ItemData data, GameObject instance)
    {
        itemData = data;
        itemObject = instance;
        icon.sprite = data.icon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        itemData = null;
        itemObject = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void SetSelected(bool selected)
    {
        if (selectorFrame != null)
            selectorFrame.SetActive(selected);
    }

    public void SetItem(ItemData data, GameObject obj)
    {
        AssignItem(data, obj);
    }

    public void SetHighlight(bool active)
    {
        if (selectorFrame != null)
            selectorFrame.SetActive(active);
    }

    public bool HasItem() => itemObject != null;
}