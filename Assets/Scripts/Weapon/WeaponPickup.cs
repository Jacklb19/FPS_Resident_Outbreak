using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public WeaponData weaponData;
    private WeaponInstance weaponInstance;
    
    private Outline[] outlines;
    private Rigidbody rb;
    
    void Awake()
    {
        // Configurar Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Configuración del Rigidbody
        rb.mass = 1f;
        rb.drag = 0.5f;
        rb.angularDrag = 0.5f;
        rb.useGravity = true;
        rb.isKinematic = false;
        
        // Configurar Outline
        outlines = GetComponentsInChildren<Outline>(true);
        ShowOutline(false);
        
        // Desactivar Animator si existe
        Animator animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }
        
        // Desactivar script Weapon si existe
        Weapon weaponScript = GetComponent<Weapon>();
        if (weaponScript != null)
        {
            weaponScript.enabled = false;
        }
    }
    
    void Start()
    {
        if (weaponInstance == null && weaponData != null)
        {
            weaponInstance = new WeaponInstance(weaponData);
        }
    }
    
    public void Initialize(WeaponData data, WeaponInstance instance)
    {
        weaponData = data;
        weaponInstance = instance;
        
        // Asegurar que el Rigidbody esté activo
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }
    
    public void ShowOutline(bool show)
    {
        if (outlines == null || outlines.Length == 0)
            return;
        
        foreach (Outline outline in outlines)
        {
            if (outline != null)
            {
                outline.enabled = show;
            }
        }
    }
    
    public WeaponInstance GetWeaponInstance()
    {
        if (weaponInstance == null && weaponData != null)
        {
            weaponInstance = new WeaponInstance(weaponData);
        }
        
        return weaponInstance;
    }
}
