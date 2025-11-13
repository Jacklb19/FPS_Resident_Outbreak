using UnityEngine;
using System;

public class GeneratorSwitch : MonoBehaviour
{
    [Header("Estado")]
    public bool isActivated = false;

    [Header("Spawn al activar")]
    public WaveConfig.EnemyEntry spawnOnActivate;

    [Header("Feedback Visual/Audio")]
    public AudioClip activationSound;
    public GameObject activationEffect;
    public Material activatedMaterial;
    public Color activatedEmissionColor = Color.green;

    private Renderer rend;
    private Material originalMaterial;

    public event Action<GeneratorSwitch> onActivated;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalMaterial = rend.material;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActivated) return;
        if (!other.CompareTag("Player")) return;

        Activate();
    }

    public void Activate()
    {
        if (isActivated) return;

        isActivated = true;
        onActivated?.Invoke(this);

        // Audio
        if (activationSound != null)
        {
            AudioSource.PlayClipAtPoint(activationSound, transform.position);
        }

        // Efecto de partículas
        if (activationEffect != null)
        {
            Instantiate(activationEffect, transform.position, Quaternion.identity);
        }

        // Cambiar material/emisión
        if (rend != null)
        {
            if (activatedMaterial != null)
            {
                rend.material = activatedMaterial;
            }
            else
            {
                // Si no hay material custom, cambiar emisión
                rend.material.EnableKeyword("_EMISSION");
                rend.material.SetColor("_EmissionColor", activatedEmissionColor * 2f);
            }
        }

        Debug.Log($"[Generator] {gameObject.name} activado");
    }
}
