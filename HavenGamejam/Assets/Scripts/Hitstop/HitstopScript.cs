using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImpactFeedbackManager : MonoBehaviour
{
    public static ImpactFeedbackManager Instance { get; private set; }

    [Header("Hitstop Defaults")]
    [SerializeField] private float defaultHitstopDuration = 0.08f;

    [Header("Parry Flash Settings")]
    [SerializeField] private Color parryFlashColor = new Color(1f, 1f, 0.9f, 0.9f); // Crisp off-white/yellow
    [SerializeField] private float defaultFlashDuration = 0.12f;

    private Image _flashImage;
    private Coroutine _hitstopCoroutine;
    private Coroutine _flashCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SetupFlashCanvas();
    }

    /// <summary>
    /// Automatically builds a full-screen UI overlay for the parry flash so no manual UI setup is required.
    /// </summary>
    private void SetupFlashCanvas()
    {
        GameObject canvasObj = new GameObject("ParryFlashCanvas");
        canvasObj.transform.SetParent(transform);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // Ensure it renders on top of all other UI

        GameObject imageObj = new GameObject("FlashOverlay");
        imageObj.transform.SetParent(canvasObj.transform, false);

        _flashImage = imageObj.AddComponent<Image>();
        _flashImage.color = Color.clear;
        _flashImage.raycastTarget = false; // Prevent blocking mouse/touch input

        RectTransform rect = _flashImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
    }

    /// <summary>
    /// Triggers a hitstop freeze frame.
    /// </summary>
    /// <param name="duration">Freeze duration in real-time seconds. Uses default if <= 0.</param>
    public void TriggerHitstop(float duration = -1f)
    {
        float targetDuration = duration > 0f ? duration : defaultHitstopDuration;

        if (_hitstopCoroutine != null)
        {
            StopCoroutine(_hitstopCoroutine);
        }
        _hitstopCoroutine = StartCoroutine(HitstopRoutine(targetDuration));
    }

    /// <summary>
    /// Triggers an ULTRAKILL-style screen flash.
    /// </summary>
    /// <param name="duration">Fade-out time in real-time seconds. Uses default if <= 0.</param>
    /// <param name="customColor">Optional color override for the flash.</param>
    public void TriggerParryFlash(float duration = -1f, Color? customColor = null)
    {
        float targetDuration = duration > 0f ? duration : defaultFlashDuration;
        Color targetColor = customColor ?? parryFlashColor;

        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
        }
        _flashCoroutine = StartCoroutine(FlashRoutine(targetDuration, targetColor));
    }

    /// <summary>
    /// Convenience method to trigger both the freeze frame and the parry flash simultaneously.
    /// </summary>
    public void TriggerParryFeedback(float hitstopDuration = 0.1f, float flashDuration = 0.15f)
    {
        TriggerHitstop(hitstopDuration);
        TriggerParryFlash(flashDuration);
    }

    private IEnumerator HitstopRoutine(float duration)
    {
        Time.timeScale = 0f;
        
        // WaitForSecondsRealtime ignores Time.timeScale = 0
        yield return new WaitForSecondsRealtime(duration);
        
        Time.timeScale = 1f;
        _hitstopCoroutine = null;
    }

    private IEnumerator FlashRoutine(float duration, Color startColor)
    {
        _flashImage.color = startColor;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // unscaledDeltaTime ensures the flash fades smoothly even while Time.timeScale is 0
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / duration);
            _flashImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        _flashImage.color = Color.clear;
        _flashCoroutine = null;
    }

    private void OnDestroy()
    {
        // Safety reset in case the object is destroyed mid-hitstop
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
    }
}