using UnityEngine;

public class UsableItem : Item
{
    public virtual void Use()
    {
        Debug.Log($"Used {data.itemName}");
    }
}