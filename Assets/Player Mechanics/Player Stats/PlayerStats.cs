using UnityEngine;
using Unity.Netcode;

public class PlayerStats : NetworkBehaviour
{
    public StaminaSystem StaminaSystem { get; private set; }
    public HealthSystem HealthSystem { get; private set; }
    public SanitySystem SanitySystem { get; private set; }

    private void Awake()
    {
        StaminaSystem = GetComponent<StaminaSystem>();
        HealthSystem = GetComponent<HealthSystem>();
        SanitySystem = GetComponent<SanitySystem>();
    }
}

