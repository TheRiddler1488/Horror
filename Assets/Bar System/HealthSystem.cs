using Unity.Netcode;
using UnityEngine;

public class HealthSystem : NetworkBehaviour
{
    public float MaxHealth = 100f;

    public NetworkVariable<float> CurrentHealth = new(100f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public void TakeDamage(float amount)
    {
        if (!IsServer) return;

        CurrentHealth.Value = Mathf.Max(CurrentHealth.Value - amount, 0f);

        if (CurrentHealth.Value <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        if (!IsServer) return;

        CurrentHealth.Value = Mathf.Min(CurrentHealth.Value + amount, MaxHealth);
    }

    private void Die()
    {
        Debug.Log($"Игрок {OwnerClientId} погиб.");
        
    }
}
