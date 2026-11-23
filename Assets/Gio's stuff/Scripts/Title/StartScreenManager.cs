using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{
    [Header("Fade Overlay")]
    public GameObject fadeOverlayObject;
    private CanvasGroup fadeOverlay;

    [Header("Fade Timings")]
    public float fadeInDelay = 0.5f;
    public float fadeInDuration = 1.5f;
    public float fadeOutDelay = 2.0f;
    public float fadeOutDuration = 1.0f;

    [Header("UI Elements to Reveal")]
    public GameObject titleObject;
    public GameObject startButtonObject;
    public GameObject creditsButtonObject;
    public GameObject quitButtonObject;

    [Header("Reveal Timings (Seconds from Start)")]
    public float titleDelay = 1.0f;
    public float startButtonDelay = 1.5f;
    public float creditsDelay = 1.8f;
    public float quitDelay = 2.1f;

    [Header("Scene Settings")]
    public string nextSceneName = "GameScene";

    private void Awake()
    {
        if (fadeOverlayObject != null)
        {
            fadeOverlay = fadeOverlayObject.GetComponent<CanvasGroup>();
            if (fadeOverlay == null)
            {
                fadeOverlay = fadeOverlayObject.AddComponent<CanvasGroup>();
            }
        }
    }

    private void Start()
    {
        if (titleObject) titleObject.SetActive(false);
        if (startButtonObject) startButtonObject.SetActive(false);
        if (creditsButtonObject) creditsButtonObject.SetActive(false);
        if (quitButtonObject) quitButtonObject.SetActive(false);

        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 1f;
            fadeOverlay.blocksRaycasts = true;
            StartCoroutine(FadeInRoutine());
        }

        StartCoroutine(RevealAfterDelay(titleObject, titleDelay));
        StartCoroutine(RevealAfterDelay(startButtonObject, startButtonDelay));
        StartCoroutine(RevealAfterDelay(creditsButtonObject, creditsDelay));
        StartCoroutine(RevealAfterDelay(quitButtonObject, quitDelay));
    }

    private IEnumerator FadeInRoutine()
    {
        // Wait before revealing the start screen
        yield return new WaitForSeconds(fadeInDelay);

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            fadeOverlay.alpha = 1f - Mathf.Clamp01(elapsed / fadeInDuration);
            yield return null;
        }

        fadeOverlay.alpha = 0f;
        fadeOverlay.blocksRaycasts = false;
    }

    public void OnStartButtonClicked()
    {
        StartCoroutine(FadeOutAndLoad());
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private IEnumerator FadeOutAndLoad()
    {
        if (fadeOverlay != null)
        {
            // Block extra clicks immediately while waiting
            fadeOverlay.blocksRaycasts = true;

            // Wait a couple seconds before starting the fade to black
            yield return new WaitForSeconds(fadeOutDelay);

            float elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                fadeOverlay.alpha = Mathf.Clamp01(elapsed / fadeOutDuration);
                yield return null;
            }

            fadeOverlay.alpha = 1f;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator RevealAfterDelay(GameObject target, float delay)
    {
        if (target == null) yield break;

        yield return new WaitForSeconds(delay);
        target.SetActive(true);
    }
}
