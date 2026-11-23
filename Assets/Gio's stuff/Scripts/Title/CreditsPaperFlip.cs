using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class CreditsPaperFlip : MonoBehaviour
{
    [Header("Credits Text")]
    [TextArea(4, 12)]
    public string creditsBodyText = "Game Design & Programming\nYour Name\n\nArt & UI\nYour Name";
    public float creditsFontSize = 14f;

    [Header("Full Screen Size & Timing")]
    public Vector2 expandedPosition = Vector2.zero;
    public Vector3 expandedScale = new Vector3(4.5f, 4.5f, 1f);
    public float flipDuration = 0.65f;

    private RectTransform rectTransform;
    private FloatingPaperButton floatingScript;
    private Button uiButton;
    private TMP_Text buttonText;

    private string originalButtonText;
    private float originalFontSize;
    private Vector3 originalTextScale;
    private TextWrappingModes originalWrappingMode;

    private Vector2 savedPosition;
    private Vector3 savedScale;
    private Quaternion savedRotation;

    private bool isExpanded = false;
    private bool isFlipping = false;
    private bool flippingToOpen = false;
    private float flipTimer = 0f;
    private bool textSwapped = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        floatingScript = GetComponent<FloatingPaperButton>();
        uiButton = GetComponent<Button>();
        buttonText = GetComponentInChildren<TMP_Text>(true);

        if (buttonText != null)
        {
            originalButtonText = buttonText.text;
            originalFontSize = buttonText.fontSize;
            originalTextScale = buttonText.rectTransform.localScale;
            originalWrappingMode = buttonText.textWrappingMode;
        }

        if (uiButton != null)
        {
            uiButton.onClick.RemoveListener(ToggleCredits);
            uiButton.onClick.AddListener(ToggleCredits);
        }
    }

    public void ToggleCredits()
    {
        if (isFlipping) return;

        isFlipping = true;
        flipTimer = 0f;
        textSwapped = false;

        if (!isExpanded)
        {
            flippingToOpen = true;
            savedPosition = rectTransform.anchoredPosition;
            savedScale = rectTransform.localScale;
            savedRotation = rectTransform.localRotation;

            if (floatingScript != null) floatingScript.isPaused = true;
            transform.SetAsLastSibling();
        }
        else
        {
            flippingToOpen = false;
        }
    }

    private void Update()
    {
        if (isExpanded && !isFlipping)
        {
            bool clicked = false;

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                clicked = true;
            else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
                clicked = true;
            else if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                clicked = true;

            if (clicked)
            {
                ToggleCredits();
                return;
            }
        }

        if (!isFlipping) return;

        float duration = Mathf.Max(0.15f, flipDuration);
        flipTimer += Time.unscaledDeltaTime;
        float t = Mathf.Clamp01(flipTimer / duration);
        float ease = t * t * (3f - 2f * t);

        Vector2 startPos = flippingToOpen ? savedPosition : expandedPosition;
        Vector2 endPos = flippingToOpen ? expandedPosition : savedPosition;

        Vector3 startScale = flippingToOpen ? savedScale : expandedScale;
        Vector3 endScale = flippingToOpen ? expandedScale : savedScale;

        float startY = flippingToOpen ? 0f : 180f;
        float endY = flippingToOpen ? 180f : 0f;

        float startZ = flippingToOpen ? savedRotation.eulerAngles.z : 0f;
        float endZ = flippingToOpen ? 0f : savedRotation.eulerAngles.z;

        rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, ease);
        rectTransform.localScale = Vector3.Lerp(startScale, endScale, ease);

        float currentY = Mathf.Lerp(startY, endY, ease);
        float currentZ = Mathf.LerpAngle(startZ, endZ, ease);
        float pitchX = Mathf.Sin(ease * Mathf.PI) * 12f;

        rectTransform.localRotation = Quaternion.Euler(pitchX, currentY, currentZ);

        // Swap text, font size, wrapping, and un-mirror at the 90-degree midpoint
        if (!textSwapped && ease >= 0.5f && buttonText != null)
        {
            if (flippingToOpen)
            {
                buttonText.text = creditsBodyText;
                buttonText.fontSize = creditsFontSize;
                buttonText.textWrappingMode = TextWrappingModes.Normal;
                buttonText.rectTransform.localScale = new Vector3(-Mathf.Abs(originalTextScale.x), originalTextScale.y, originalTextScale.z);
            }
            else
            {
                buttonText.text = originalButtonText;
                buttonText.fontSize = originalFontSize;
                buttonText.textWrappingMode = originalWrappingMode;
                buttonText.rectTransform.localScale = originalTextScale;
            }
            textSwapped = true;
        }

        if (t >= 1f)
        {
            rectTransform.anchoredPosition = endPos;
            rectTransform.localScale = endScale;
            rectTransform.localRotation = Quaternion.Euler(0f, endY, endZ);

            isExpanded = flippingToOpen;
            isFlipping = false;

            if (!isExpanded && floatingScript != null)
            {
                floatingScript.isPaused = false;
            }
        }
    }
}
