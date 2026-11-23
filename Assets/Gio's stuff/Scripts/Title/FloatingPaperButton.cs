using UnityEngine;

public class FloatingPaperButton : MonoBehaviour
{
    [Header("Fly-In Settings")]
    public float startOffsetY = -1200f;
    public float flyInDuration = 1.2f;

    [Header("Gentle Floating Settings")]
    public float floatDistanceY = 8f;
    public float floatDistanceX = 4f;
    public float floatSpeed = 1.5f;
    public float tiltAngle = 2.5f;

    [HideInInspector]
    public bool isPaused = false;

    private RectTransform buttonRect;
    private Vector2 targetPosition;
    private Vector2 startPosition;
    private float randomPhase;

    private float flyTimer = 0f;
    private bool hasLanded = false;
    private bool initialized = false;

    private void Awake()
    {
        buttonRect = GetComponent<RectTransform>();
        targetPosition = buttonRect.anchoredPosition;
        startPosition = new Vector2(targetPosition.x, targetPosition.y + startOffsetY);
        randomPhase = Random.Range(0f, Mathf.PI * 2f);
        initialized = true;
    }

    private void OnEnable()
    {
        if (!initialized || isPaused) return;

        // Reset to bottom of screen ready to glide up in Update()
        flyTimer = 0f;
        hasLanded = false;
        buttonRect.anchoredPosition = startPosition;
        transform.localRotation = Quaternion.identity;
    }

    private void Update()
    {
        if (isPaused) return;

        // Phase 1: Fly up from bottom of screen
        if (!hasLanded)
        {
            float duration = Mathf.Max(0.2f, flyInDuration);
            flyTimer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(flyTimer / duration);
            float easeOut = 1f - Mathf.Pow(1f - t, 3f);

            buttonRect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, easeOut);

            float flutter = Mathf.Sin(t * Mathf.PI * 3f) * (tiltAngle * 2.5f) * (1f - t);
            transform.localRotation = Quaternion.Euler(0f, 0f, flutter);

            if (t >= 1f)
            {
                buttonRect.anchoredPosition = targetPosition;
                hasLanded = true;
            }
            return;
        }

        // Phase 2: Gentle floating in place
        float time = (Time.unscaledTime * floatSpeed) + randomPhase;

        float offsetY = Mathf.Sin(time) * floatDistanceY;
        float offsetX = Mathf.Cos(time * 0.7f) * floatDistanceX;
        float angle = Mathf.Sin(time * 0.85f) * tiltAngle;

        buttonRect.anchoredPosition = targetPosition + new Vector2(offsetX, offsetY);
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
