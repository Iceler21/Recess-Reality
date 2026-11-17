using System.Collections.Generic;
using UnityEngine;

public class Spark2Text : MonoBehaviour
{
    [Header("References")]
    public Texture2D textPNG;
    public ParticleSystem sparkleSystem;

    [Header("Layout & Density")]
    [Range(1, 10)] public int pixelStep = 3;
    public float worldScale = 0.02f;
    [Range(0.1f, 0.9f)] public float alphaThreshold = 0.5f;

    [Header("Assembly & Swirl")]
    public float scatterRadius = 4f;
    public float assembleDuration = 2.5f;
    [Tooltip("How much the particles swirl in arcs while flying in.")]
    public float swirlStrength = 1.5f;
    public float swirlFrequency = 1.2f;

    [Header("Magical Jitter & Twinkle")]
    [Tooltip("How much particles gently float/jitter once formed.")]
    public float idleJitterAmount = 0.04f;
    public float idleJitterSpeed = 2.5f;
    public float twinkleSpeed = 6f;
    public Color baseColor = new Color(1f, 0.9f, 0.5f, 1f); // Warm gold
    public Color accentColor = new Color(0.6f, 0.95f, 1f, 1f); // Magical cyan

    private ParticleSystem.Particle[] particles;
    private Vector3[] startPositions;
    private Vector3[] targetPositions;
    private float[] randomSeeds;
    private float[] assembleDelays;
    private Color[] particleColors;
    private float timer;

    void Start()
    {
        List<Vector3> validPoints = new List<Vector3>();
        float halfWidth = textPNG.width * 0.5f;
        float halfHeight = textPNG.height * 0.5f;

        for (int x = 0; x < textPNG.width; x += pixelStep)
        {
            for (int y = 0; y < textPNG.height; y += pixelStep)
            {
                if (textPNG.GetPixel(x, y).a >= alphaThreshold)
                {
                    Vector3 localPos = new Vector3(
                        (x - halfWidth) * worldScale,
                        (y - halfHeight) * worldScale,
                        0f
                    );
                    validPoints.Add(transform.TransformPoint(localPos));
                }
            }
        }

        int count = validPoints.Count;
        particles = new ParticleSystem.Particle[count];
        startPositions = new Vector3[count];
        targetPositions = validPoints.ToArray();
        randomSeeds = new float[count];
        assembleDelays = new float[count];
        particleColors = new Color[count];

        sparkleSystem.Emit(count);
        sparkleSystem.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            randomSeeds[i] = Random.Range(0f, 100f);
            // Stagger arrival slightly (up to 35% of total duration) for a wave effect
            assembleDelays[i] = Random.Range(0f, assembleDuration * 0.35f);

            startPositions[i] = targetPositions[i] + Random.insideUnitSphere * scatterRadius;
            particles[i].position = startPositions[i];
            particles[i].remainingLifetime = 999f;

            // Blend between warm gold and cyan for magical variety
            particleColors[i] = Color.Lerp(baseColor, accentColor, Random.value * 0.4f);
        }

        sparkleSystem.SetParticles(particles, count);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float time = Time.time;
        int count = sparkleSystem.GetParticles(particles);
        float baseSize = sparkleSystem.main.startSize.constant;

        for (int i = 0; i < count; i++)
        {
            float seed = randomSeeds[i];

            // 1. Calculate individual staggered progress (0 to 1)
            float activeTime = Mathf.Max(0f, timer - assembleDelays[i]);
            float individualDuration = assembleDuration - assembleDelays[i];
            float rawProgress = Mathf.Clamp01(activeTime / Mathf.Max(0.01f, individualDuration));
            float t = Mathf.SmoothStep(0f, 1f, rawProgress);

            // 2. Base Lerp from scattered point to letter pixel
            Vector3 currentPos = Vector3.Lerp(startPositions[i], targetPositions[i], t);

            // 3. Organic 3D Perlin noise offset (strong swirl mid-flight, subtle hover when landed)
            float nx = Mathf.PerlinNoise(seed, time * swirlFrequency) - 0.5f;
            float ny = Mathf.PerlinNoise(seed + 10f, time * swirlFrequency) - 0.5f;
            float nz = Mathf.PerlinNoise(seed + 20f, time * swirlFrequency) - 0.5f;
            Vector3 noiseDir = new Vector3(nx, ny, nz) * 2f;

            // Arc envelope peaks at t = 0.5 and drops to 0 at t = 1
            float flightSwirl = Mathf.Sin(t * Mathf.PI) * swirlStrength;

            // Gentle continuous idle shimmer once settled
            float jx = Mathf.PerlinNoise(seed + 30f, time * idleJitterSpeed) - 0.5f;
            float jy = Mathf.PerlinNoise(seed + 40f, time * idleJitterSpeed) - 0.5f;
            Vector3 idleJitter = new Vector3(jx, jy, 0f) * (idleJitterAmount * t);

            particles[i].position = currentPos + (noiseDir * flightSwirl) + idleJitter;

            // 4. Magical Twinkle (modulate size & alpha with overlapping sine waves)
            float twinkle = 0.5f + 0.5f * Mathf.Sin(time * twinkleSpeed + seed * 10f);
            particles[i].startSize = baseSize * Mathf.Lerp(0.4f, 1.35f, twinkle);

            Color c = particleColors[i];
            // Fade in smoothly as they spawn, then twinkle
            float spawnFade = Mathf.Clamp01(timer * 3f);
            c.a = Mathf.Lerp(0.35f, 1f, twinkle) * spawnFade;
            particles[i].startColor = c;
        }

        sparkleSystem.SetParticles(particles, count);
    }
}
