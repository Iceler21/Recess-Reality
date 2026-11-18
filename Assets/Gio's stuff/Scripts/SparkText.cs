using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ParticleSystem))]
public class SparkText2D : MonoBehaviour
{
    public Texture2D textTexture; // Must have Read/Write enabled
    public float scatterRadius = 8f;
    public float assembleSpeed = 5f;
    public float textScale = 0.02f;

    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    private List<Vector3> targetPositions = new List<Vector3>();
    private bool isDispersing = false;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        if (textTexture == null)
        {
            Debug.LogError("SparkText2D: Please assign a Text Texture in the Inspector!");
            return;
        }

        if (targetPositions.Count == 0)
        {
            ExtractPixels();
        }

        if (targetPositions.Count > 0)
        {
            Assemble();
        }
        else
        {
            Debug.LogWarning("SparkText2D: No bright/opaque pixels found in Text Texture!");
        }
    }

    void ExtractPixels()
    {
        targetPositions.Clear();
        for (int x = 0; x < textTexture.width; x += 2)
        {
            for (int y = 0; y < textTexture.height; y += 2)
            {
                Color pixel = textTexture.GetPixel(x, y);
                // Works with either transparent background OR black background
                if (pixel.a > 0.5f && pixel.grayscale > 0.5f)
                {
                    float posX = (x - textTexture.width / 2f) * textScale;
                    float posY = (y - textTexture.height / 2f) * textScale;
                    targetPositions.Add(new Vector3(posX, posY, 0f));
                }
            }
        }
        Debug.Log("SparkText2D: Loaded " + targetPositions.Count + " spark positions.");
    }

    public void Assemble()
    {
        isDispersing = false;
        ps.Clear();

        var main = ps.main;
        main.useUnscaledTime = true;
        main.maxParticles = targetPositions.Count;
        ps.Emit(targetPositions.Count);

        particles = new ParticleSystem.Particle[targetPositions.Count];
        int count = ps.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            particles[i].position = (Vector3)Random.insideUnitCircle * scatterRadius;
            particles[i].velocity = Vector3.zero;
        }
        ps.SetParticles(particles, count);
    }

    public void Disperse(float burstSpeed = 12f)
    {
        if (particles == null || particles.Length == 0) return;

        isDispersing = true;
        int count = ps.GetParticles(particles);
        for (int i = 0; i < count; i++)
        {
            Vector2 outwardDir = particles[i].position.normalized;
            Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
            particles[i].velocity = (outwardDir + randomOffset) * burstSpeed;
            particles[i].remainingLifetime = 0.4f;
        }
        ps.SetParticles(particles, count);
    }

    void Update()
    {
        // Safely exit if dispersing or if particles are not ready
        if (isDispersing || particles == null || particles.Length == 0) return;

        int count = ps.GetParticles(particles);
        for (int i = 0; i < count; i++)
        {
            particles[i].position = Vector3.Lerp(
                particles[i].position,
                targetPositions[i],
                Time.unscaledDeltaTime * assembleSpeed
            );
        }
        ps.SetParticles(particles, count);
    }
}
