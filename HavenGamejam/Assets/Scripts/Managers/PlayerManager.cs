using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    PlayerController player;


    public float MaxStamina = 100f;

    public float CurrentStamina = 100f;

    public bool CanTeleport = true;

    public bool CanMove = true;
    public bool CanAttack = true;
    Coroutine StopMovementCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<PlayerController>();
        CurrentStamina = MaxStamina; // Initialize current stamina to max at the start
        StopMovementCoroutine = null;
    }

    // Update is called once per frame
    void Update()
    {
        // Clamp stamina to max value
        if (CurrentStamina > MaxStamina)
        {
            CurrentStamina = MaxStamina;
        }
        /*
        if (dialogueManager.DialogueIsPlaying)
        {
            CanMove = false;
            CanAttack = false;
        }
        else
        {
            CanMove = true;
            CanAttack = true;
        }
        */
        
    }

    public void StopMovement(float delay, float duration)
    {
        if (StopMovementCoroutine != null)
        {
            StopCoroutine(StopMovementCoroutine);
        }
        StopMovementCoroutine = StartCoroutine(StopMovementRoutine(delay, duration));
    }

    private IEnumerator StopMovementRoutine(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        CanMove = false;
        yield return new WaitForSeconds(duration);
        CanMove = true;
    }
}
