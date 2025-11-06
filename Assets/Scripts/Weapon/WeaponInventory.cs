using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    [Header("Weapon Slots")]
    private Weapon[] weaponSlots = new Weapon[2];
    private int activeSlotIndex = 0;
    
    [Header("Weapon Holder")]
    public Transform weaponHolder;
    
    void Start()
    {
        // VALIDACIÓN: Asegurar que weaponHolder está asignado
        if (weaponHolder == null)
        {
            Debug.LogError("❌ WeaponHolder NO está asignado en WeaponInventory! Búscalo automáticamente...");
            
            // Intentar encontrarlo automáticamente
            Transform mainCamera = Camera.main?.transform;
            if (mainCamera != null)
            {
                weaponHolder = mainCamera.Find("WeaponHolder");
                
                if (weaponHolder == null)
                {
                    Debug.LogWarning("⚠️ WeaponHolder no encontrado. Creándolo automáticamente...");
                    GameObject holderObj = new GameObject("WeaponHolder");
                    holderObj.transform.SetParent(mainCamera);
                    holderObj.transform.localPosition = new Vector3(0.5f, -0.3f, 0.5f);
                    holderObj.transform.localRotation = Quaternion.identity;
                    weaponHolder = holderObj.transform;
                    Debug.Log("✅ WeaponHolder creado automáticamente");
                }
                else
                {
                    Debug.Log("✅ WeaponHolder encontrado automáticamente");
                }
            }
            else
            {
                Debug.LogError("❌ No se encontró MainCamera. Asigna manualmente weaponHolder.");
            }
        }
        else
        {
            Debug.Log("✅ WeaponHolder asignado correctamente: " + weaponHolder.name);
        }
    }
    
    public Weapon GetActiveWeapon()
    {
        return weaponSlots[activeSlotIndex];
    }
    
    public Weapon GetWeaponInSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= weaponSlots.Length)
            return null;
        
        return weaponSlots[slotIndex];
    }
    
    public int GetActiveSlotIndex()
    {
        return activeSlotIndex;
    }
    
    public bool HasFreeSlot(out int freeSlotIndex)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] == null)
            {
                freeSlotIndex = i;
                return true;
            }
        }
        
        freeSlotIndex = -1;
        return false;
    }
    
    public void EquipWeapon(GameObject weaponPrefab, WeaponInstance weaponInstance, int slotIndex)
    {
        // VALIDACIÓN CRÍTICA
        if (weaponHolder == null)
        {
            Debug.LogError("❌ CRITICAL: weaponHolder es NULL en EquipWeapon()");
            return;
        }
        
        if (weaponPrefab == null)
        {
            Debug.LogError("❌ weaponPrefab es NULL en EquipWeapon()");
            return;
        }
        
        if (weaponInstance == null || weaponInstance.weaponData == null)
        {
            Debug.LogError("❌ weaponInstance o weaponData es NULL");
            return;
        }
        
        // Si hay arma en ese slot, eliminarla
        if (weaponSlots[slotIndex] != null)
        {
            Destroy(weaponSlots[slotIndex].gameObject);
        }
        
        // Instanciar nueva arma como hijo del weaponHolder
        GameObject newWeaponObj = Instantiate(weaponPrefab, weaponHolder);
        Debug.Log("✅ Arma instanciada: " + newWeaponObj.name + " en slot " + slotIndex);
        
        // Posicionar según WeaponData
        newWeaponObj.transform.localPosition = weaponInstance.weaponData.spawnPosition;
        newWeaponObj.transform.localEulerAngles = weaponInstance.weaponData.spawnRotation;
        
        Weapon weaponComponent = newWeaponObj.GetComponent<Weapon>();
        if (weaponComponent == null)
        {
            weaponComponent = newWeaponObj.AddComponent<Weapon>();
        }
        
        weaponComponent.Initialize(weaponInstance);
        
        // Desactivar física al equipar
        Rigidbody rb = newWeaponObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        
        Collider col = newWeaponObj.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
        
        // Remover WeaponPickup y Outline si existen (ya están equipados)
        WeaponPickup pickup = newWeaponObj.GetComponent<WeaponPickup>();
        if (pickup != null)
        {
            Destroy(pickup);
        }
        
        Outline outline = newWeaponObj.GetComponent<Outline>();
        if (outline != null)
        {
            Destroy(outline);
        }
        
        weaponSlots[slotIndex] = weaponComponent;
        
        // Si este es el slot activo, activar visualmente
        newWeaponObj.SetActive(slotIndex == activeSlotIndex);
        
        Debug.Log("✅ Arma equipada exitosamente en slot " + slotIndex);
    }
    
    public void SwitchToSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= weaponSlots.Length)
            return;
        
        if (weaponSlots[slotIndex] == null)
            return;
        
        if (activeSlotIndex == slotIndex)
            return;
        
        // Desactivar arma actual
        if (weaponSlots[activeSlotIndex] != null)
        {
            weaponSlots[activeSlotIndex].gameObject.SetActive(false);
        }
        
        // Activar nueva arma
        activeSlotIndex = slotIndex;
        weaponSlots[activeSlotIndex].gameObject.SetActive(true);
        
        Debug.Log("Cambiado a slot: " + slotIndex);
    }
    
    public GameObject DropWeapon(int slotIndex)
    {
        if (weaponSlots[slotIndex] == null)
            return null;
        
        GameObject weaponObj = weaponSlots[slotIndex].gameObject;
        
        // Desparentar del weaponHolder
        weaponObj.transform.SetParent(null);
        
        // Activar física
        Rigidbody rb = weaponObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        
        Collider col = weaponObj.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
        
        // Añadir componente WeaponPickup si no lo tiene
        if (weaponObj.GetComponent<WeaponPickup>() == null)
        {
            WeaponPickup pickup = weaponObj.AddComponent<WeaponPickup>();
            pickup.weaponData = weaponSlots[slotIndex].weaponData;
            pickup.savedInstance = weaponSlots[slotIndex].GetWeaponInstance();
        }
        
        weaponSlots[slotIndex] = null;
        
        Debug.Log("Arma dropeada del slot: " + slotIndex);
        
        return weaponObj;
    }
}
