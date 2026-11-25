using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class WarpSceneTransition : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag your Global Volume object from the Hierarchy here.")]
    [SerializeField] private Volume postProcessVolume;

    [Header("Scene Settings")]
    [SerializeField] private string nextSceneName = "GameScene";

    [Header("Default Post-Processing Setup")]
    [Range(-1f, 1f)]
    [SerializeField] private float defaultIntensity = 0f;

    [Range(0.01f, 5f)]
    [SerializeField] private float defaultScale = 1f;

    [Header("Target Warp Setup")]
    [Range(0.1f, 10f)]
    [SerializeField] private float transitionDuration = 1.5f;

    [Range(-1f, 1f)]
    [SerializeField] private float targetIntensity = -1f;

    [Range(0.01f, 5f)]
    [SerializeField] private float targetScale = 0.01f;

    private LensDistortion lensDistortion;
    private bool isTransitioning = false;

    private void Start()
    {
        if (postProcessVolume != null && postProcessVolume.profile.TryGet(out lensDistortion))
        {
            // Activate the overrides and apply your default setup immediately
            lensDistortion.intensity.overrideState = true;
            lensDistortion.scale.overrideState = true;

            lensDistortion.intensity.value = defaultIntensity;
            lensDistortion.scale.value = defaultScale;
        }
        else
        {
            Debug.LogWarning("WarpSceneTransition: Make sure a Volume with a Lens Distortion override is assigned.");
        }
    }

    // Hook this up to your Start Button's OnClick() event
    public void StartGameWithWarp()
    {
        if (isTransitioning) return;
        StartCoroutine(WarpAndLoadRoutine());
    }

    private IEnumerator WarpAndLoadRoutine()
    {
        isTransitioning = true;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            if (lensDistortion != null)
            {
                lensDistortion.intensity.value = Mathf.Lerp(defaultIntensity, targetIntensity, smoothT);
                lensDistortion.scale.value = Mathf.Lerp(defaultScale, targetScale, smoothT);
            }

            yield return null;
        }

        if (lensDistortion != null)
        {
            lensDistortion.intensity.value = targetIntensity;
            lensDistortion.scale.value = targetScale;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
