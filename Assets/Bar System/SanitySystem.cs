using Unity.Netcode;
using UnityEngine;

public class SanitySystem : NetworkBehaviour
{
    public float MaxSanity = 100f;
    public float DrainRateInDarkness = 5f;

    public NetworkVariable<float> CurrentSanity = new(100f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private bool isInDarkness = false;

    public void SetInDarkness(bool value)
    {
        if (!IsOwner) return;
        isInDarkness = value;
    }

    void Update()
    {
        if (!IsServer) return;

        if (isInDarkness)
        {
            CurrentSanity.Value -= DrainRateInDarkness * Time.deltaTime;
            CurrentSanity.Value = Mathf.Max(0f, CurrentSanity.Value);
        }
    }

    public void RecoverSanity(float amount)
    {
        if (!IsServer) return;

        CurrentSanity.Value = Mathf.Min(CurrentSanity.Value + amount, MaxSanity);
    }
}
