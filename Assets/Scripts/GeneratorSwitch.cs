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

    private Outline[] outlines;
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

        outlines = GetComponentsInChildren<Outline>(true);
        ShowOutline(false);
    }

    public void ShowOutline(bool show)
    {
        if (outlines == null || outlines.Length == 0) return;

        foreach (Outline outline in outlines)
        {
            if (outline != null)
            {
                outline.enabled = show;
            }
        }
    }

    public bool TryActivate()
    {
        if (isActivated) return false;

        isActivated = true;
        onActivated?.Invoke(this);

        if (activationSound != null)
        {
            AudioSource.PlayClipAtPoint(activationSound, transform.position);
        }

        if (activationEffect != null)
        {
            Instantiate(activationEffect, transform.position, Quaternion.identity);
        }

        if (rend != null)
        {
            if (activatedMaterial != null)
            {
                rend.material = activatedMaterial;
            }
            else
            {
                rend.material.EnableKeyword("_EMISSION");
                rend.material.SetColor("_EmissionColor", activatedEmissionColor * 2f);
            }
        }

        ShowOutline(false);

        Debug.Log($"[Generator] {gameObject.name} activado");
        return true;
    }
}