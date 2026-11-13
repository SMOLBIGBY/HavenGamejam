using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public float MaxStamina = 100f;

    public float CurrentStamina = 100f;

    public bool CanTeleport = true;

    public bool CanMove = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentStamina = MaxStamina; // Initialize current stamina to max at the start
        
    }

    // Update is called once per frame
    void Update()
    {
        // Clamp stamina to max value
        if (CurrentStamina > MaxStamina)
        {
            CurrentStamina = MaxStamina;
        }
        
    }
}
