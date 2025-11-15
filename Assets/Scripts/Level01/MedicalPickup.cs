using UnityEngine;
using System;

public class MedicalPickup : MonoBehaviour
{
    [Header("Visual")]
    public AudioClip collectSound;
    public GameObject collectEffect;

    private Outline[] outlines;
    private bool collected = false;

    public event Action onCollected;

    void Awake()
    {
        // Configurar Outline
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

    public bool TryPickup()
    {
        if (collected) return false;

        collected = true;
        
        // ✅ Disparar evento para Level1_HospitalFlow
        onCollected?.Invoke();

        // Efectos visuales/audio
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        Debug.Log($"[MedicalPickup] Paquete recolectado: {gameObject.name}");
        Destroy(gameObject);
        return true;
    }
}
