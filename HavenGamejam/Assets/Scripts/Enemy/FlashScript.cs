using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlash : MonoBehaviour, ICanBeFlashed
{
    [SerializeField] private float flashDuration = 0.3f;

    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propBlock;
    private Coroutine flashCoroutine;

    private static readonly int FlashAmountID = Shader.PropertyToID("_FlashAmount");

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        propBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        // Optional: For testing, trigger flash with the F key
        if (Input.GetKeyDown(KeyCode.K))
        {
            TriggerFlash();
            Debug.Log("Flash triggered on " + gameObject.name);
        }
    }

    public void TriggerFlash()
    {
        // Stop any running flash so rapid hits reset the timer cleanly
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        SetFlashAmount(1f);
        yield return new WaitForSeconds(flashDuration);
        SetFlashAmount(0f);
        flashCoroutine = null;
    }

    private void SetFlashAmount(float amount)
    {
        // Temporary test: turns sprite white instead of using shader
        spriteRenderer.color = amount > 0.5f ? Color.grey : Color.white;
    }

    public void Flash()
    {
        TriggerFlash();
        Debug.Log("Flash triggered on " + gameObject.name);

    }
    
}
