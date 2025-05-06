using Unity.Netcode;
using UnityEngine;

public class StaminaSystem : NetworkBehaviour
{
    public float MaxStamina = 100f;
    public float DrainRate = 20f; // Стамина в секунду при беге
    public float RegenRate = 15f; // Восстановление в секунду

    public NetworkVariable<float> CurrentStamina = new(100f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private bool isRunning = false;

    public void SetRunning(bool state)
    {
        if (!IsOwner) return;
        isRunning = state;
    }

    void Update()
    {
        if (!IsServer) return;

        if (isRunning)
        {
            CurrentStamina.Value -= DrainRate * Time.deltaTime;
        }
        else
        {
            CurrentStamina.Value += RegenRate * Time.deltaTime;
        }

        CurrentStamina.Value = Mathf.Clamp(CurrentStamina.Value, 0f, MaxStamina);
    }

    public void RecoverStamina(float amount)
    {
        if (!IsServer) return;
        CurrentStamina.Value = Mathf.Min(CurrentStamina.Value + amount, MaxStamina);
    }

    public bool CanRun()
    {
        return CurrentStamina.Value > 0.1f;
    }
}
