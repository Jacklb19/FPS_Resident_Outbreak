using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public WeaponData weaponData;
    public WeaponInstance savedInstance;
    
    [Header("Pickup Settings")]
    public float pickupDistance = 3f;
    
    [Header("Outline Settings")]
    public Color outlineColor = Color.white;
    public float outlineWidth = 5f;
    
    private Outline outline;
    
    void Start()
    {
        // Asegurar que tiene Rigidbody y Collider
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 1f;
        }
        
        if (GetComponent<Collider>() == null)
        {
            BoxCollider col = gameObject.AddComponent<BoxCollider>();
            col.size = Vector3.one * 0.5f;
        }
        
        // Asegurar capa "Pickable"
        int pickableLayer = LayerMask.NameToLayer("Pickable");
        if (pickableLayer != -1)
        {
            gameObject.layer = pickableLayer;
        }
        else
        {
            Debug.LogWarning("Layer 'Pickable' no existe. Créalo en Project Settings > Tags and Layers");
        }
        
        // Si no tiene instancia guardada, crear una nueva
        if (savedInstance == null && weaponData != null)
        {
            savedInstance = new WeaponInstance(weaponData);
        }
        
        // Configurar Outline
        SetupOutline();
    }
    
    void SetupOutline()
    {
        // Buscar o añadir componente Outline
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }
        
        // Configurar estilo
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = outlineColor;
        outline.OutlineWidth = outlineWidth;
        
        // Desactivar por defecto
        outline.enabled = false;
    }
    
    public void ShowOutline(bool show)
    {
        if (outline != null)
        {
            outline.enabled = show;
        }
    }
    
    public WeaponInstance GetWeaponInstance()
    {
        return savedInstance;
    }
}
