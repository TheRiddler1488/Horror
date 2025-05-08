using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image highlight;

    public void SetItem(Sprite itemIcon)
    {
        icon.sprite = itemIcon;
        icon.enabled = itemIcon != null;
    }

    public void SetHighlight(bool isActive)
    {
        highlight.enabled = isActive;
    }
}
