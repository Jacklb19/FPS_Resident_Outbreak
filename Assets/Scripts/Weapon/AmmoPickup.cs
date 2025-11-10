using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Ammo Configuration")]
    [Tooltip("El arma para la cual es esta munición")]
    public WeaponData targetWeaponData;
    
    [Tooltip("Cantidad de munición que otorga esta caja")]
    public int ammoAmount = 30;
    
    [Header("Audio")]
    [Tooltip("Sonido al recoger la munición (opcional, se usa el del arma si no se especifica)")]
    public AudioClip pickupSound;
    
    [Header("Visual Settings")]
    [Tooltip("Se destruye al recogerla? Si no, solo se desactiva temporalmente")]
    public bool destroyOnPickup = true;
    
    [Tooltip("Tiempo de respawn si destroyOnPickup es false")]
    public float respawnTime = 30f;
    
    private Outline[] outlines;
    private bool isAvailable = true;
    private MeshRenderer[] meshRenderers;
    private Collider boxCollider;
    
    void Awake()
    {
        outlines = GetComponentsInChildren<Outline>(true);
        ShowOutline(false);
        
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        boxCollider = GetComponent<Collider>();
        
        if (targetWeaponData == null)
        {
            Debug.LogError($"AmmoPickup en {gameObject.name} no tiene WeaponData asignado!", this);
        }
        
        if (ammoAmount <= 0)
        {
            Debug.LogWarning($"AmmoPickup en {gameObject.name} tiene ammoAmount <= 0", this);
        }
    }
    
    public void ShowOutline(bool show)
    {
        if (outlines == null || outlines.Length == 0 || !isAvailable)
            return;
        
        foreach (Outline outline in outlines)
        {
            if (outline != null)
            {
                outline.enabled = show;
            }
        }
    }
    
    // ═══ CAMBIADO: Ahora solo necesita AmmoInventory ═══
    public bool TryPickup(AmmoInventory ammoInventory, out int ammoAdded)
    {
        ammoAdded = 0;
        
        if (!isAvailable || targetWeaponData == null || ammoInventory == null)
            return false;
        
        // ═══ NUEVA LÓGICA: Siempre puedes recoger si no estás al máximo ═══
        if (!ammoInventory.CanAddAmmo(targetWeaponData))
        {
            Debug.Log($"Munición de {targetWeaponData.weaponName} ya está al máximo.");
            return false;
        }
        
        // Añadir munición al inventario centralizado
        ammoAdded = ammoInventory.AddAmmo(targetWeaponData, ammoAmount);
        
        // Reproducir sonido
        AudioClip soundToPlay = pickupSound != null ? pickupSound : targetWeaponData.ammoPickupSound;
        if (soundToPlay != null)
        {
            AudioSource.PlayClipAtPoint(soundToPlay, transform.position);
        }
        
        ShowOutline(false);
        
        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(RespawnCoroutine());
        }
        
        return true;
    }
    
    private System.Collections.IEnumerator RespawnCoroutine()
    {
        isAvailable = false;
        
        foreach (MeshRenderer mr in meshRenderers)
        {
            if (mr != null) mr.enabled = false;
        }
        
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }
        
        yield return new WaitForSeconds(respawnTime);
        
        isAvailable = true;
        
        foreach (MeshRenderer mr in meshRenderers)
        {
            if (mr != null) mr.enabled = true;
        }
        
        if (boxCollider != null)
        {
            boxCollider.enabled = true;
        }
    }
    
    public bool IsAvailable()
    {
        return isAvailable;
    }
    
    public WeaponData GetTargetWeaponData()
    {
        return targetWeaponData;
    }
}
