using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    [Header("Weapon Slots")]
    public Transform weaponSlot1;
    public Transform weaponSlot2;
    
    [Header("Drop Settings")]
    public LayerMask groundLayer; // ← Asignar en Inspector
    
    private Weapon[] weapons = new Weapon[2];
    private int activeSlotIndex = -1;
    
    public void EquipWeapon(GameObject weaponPrefab, WeaponInstance weaponInstance, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > 1) return;
        
        Transform targetSlot = slotIndex == 0 ? weaponSlot1 : weaponSlot2;
        
        if (weapons[slotIndex] != null)
        {
            Destroy(weapons[slotIndex].gameObject);
        }
        
        GameObject weaponObj = Instantiate(weaponPrefab, targetSlot);
        weaponObj.transform.localPosition = weaponInstance.weaponData.spawnPosition;
        weaponObj.transform.localRotation = Quaternion.Euler(weaponInstance.weaponData.spawnRotation);
        
        Rigidbody rb = weaponObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        
        Collider col = weaponObj.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
        
        Weapon weapon = weaponObj.GetComponent<Weapon>();
        if (weapon == null)
        {
            weapon = weaponObj.AddComponent<Weapon>();
        }
        
        weapon.enabled = true;
        weapon.Initialize(weaponInstance);
        weapons[slotIndex] = weapon;
        
        weaponObj.SetActive(false);
        
        if (activeSlotIndex == -1)
        {
            SwitchToSlot(slotIndex);
        }
    }
    
    public void SwitchToSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > 1) return;
        if (weapons[slotIndex] == null) return;
        if (activeSlotIndex == slotIndex) return;
        
        if (activeSlotIndex != -1 && weapons[activeSlotIndex] != null)
        {
            weapons[activeSlotIndex].OnUnequip();
            weapons[activeSlotIndex].gameObject.SetActive(false);
        }
        
        activeSlotIndex = slotIndex;
        weapons[activeSlotIndex].gameObject.SetActive(true);
        weapons[activeSlotIndex].OnEquip();
    }
    
    public GameObject DropWeapon(int slotIndex, Vector3 dropPosition)
    {
        if (slotIndex < 0 || slotIndex > 1) return null;
        if (weapons[slotIndex] == null) return null;
        
        Weapon weapon = weapons[slotIndex];
        WeaponInstance weaponInstance = weapon.GetWeaponInstance();
        WeaponData weaponData = weaponInstance.weaponData;
        
        weapon.OnUnequip();
        
        // Calcular posición sobre el suelo
        Vector3 finalPosition = CalculateGroundPosition(dropPosition, weaponData);
        
        // Crear el arma en la posición correcta
        GameObject droppedWeapon = Instantiate(weaponData.modelPrefab, finalPosition, Quaternion.identity);
        
        // Activar física
        Rigidbody rb = droppedWeapon.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        // Activar collider
        Collider col = droppedWeapon.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
        
        // Configurar WeaponPickup
        WeaponPickup pickup = droppedWeapon.GetComponent<WeaponPickup>();
        if (pickup == null)
        {
            pickup = droppedWeapon.AddComponent<WeaponPickup>();
        }
        
        // Desactivar script Weapon
        Weapon weaponScript = droppedWeapon.GetComponent<Weapon>();
        if (weaponScript != null)
        {
            weaponScript.enabled = false;
        }
        
        pickup.weaponData = weaponData;
        pickup.Initialize(weaponData, weaponInstance);
        
        // Limpiar inventario
        Destroy(weapon.gameObject);
        weapons[slotIndex] = null;
        
        if (activeSlotIndex == slotIndex)
        {
            activeSlotIndex = -1;
            
            int otherSlot = slotIndex == 0 ? 1 : 0;
            if (weapons[otherSlot] != null)
            {
                SwitchToSlot(otherSlot);
            }
        }
        
        return droppedWeapon;
    }
    
    private Vector3 CalculateGroundPosition(Vector3 dropPosition, WeaponData weaponData)
    {
        // Raycast hacia abajo para encontrar el suelo
        Vector3 rayStart = dropPosition + Vector3.up * 2f;
        RaycastHit groundHit;
        
        // ═══ ERROR 1 CORREGIDO: groundLayer en vez de pickupLayer ═══
        bool hitGround = Physics.Raycast(rayStart, Vector3.down, out groundHit, 5f, groundLayer);
        
        if (!hitGround)
        {
            Debug.LogWarning($"[WeaponInventory] No se encontró suelo debajo de {dropPosition}");
            return dropPosition + Vector3.up * 0.2f;
        }
        
        // Crear arma temporalmente para medir su collider
        GameObject tempWeapon = Instantiate(weaponData.modelPrefab, Vector3.zero, Quaternion.identity);
        tempWeapon.name = "TempWeapon_Measuring";
        
        Collider weaponCollider = tempWeapon.GetComponent<Collider>();
        float offset = 0.1f;
        
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
            tempWeapon.transform.position = Vector3.zero;
            
            Bounds bounds = weaponCollider.bounds;
            float pivotToBottom = tempWeapon.transform.position.y - bounds.min.y;
            
            offset = pivotToBottom;
            
            if (offset <= 0.01f)
            {
                offset = bounds.extents.y;
            }
            
            Debug.Log($"[WeaponInventory] {weaponData.weaponName} - Offset calculado: {offset:F3}m");
        }
        else
        {
            Debug.LogWarning($"[WeaponInventory] {weaponData.weaponName} no tiene Collider");
        }
        
        Destroy(tempWeapon);
        
        Vector3 finalPosition = groundHit.point + Vector3.up * offset;
        
        return finalPosition;
    }
    
    public bool HasFreeSlot(out int freeSlot)
    {
        if (weapons[0] == null)
        {
            freeSlot = 0;
            return true;
        }
        if (weapons[1] == null)
        {
            freeSlot = 1;
            return true;
        }
        
        freeSlot = -1;
        return false;
    }
    
    public Weapon GetActiveWeapon()
    {
        if (activeSlotIndex == -1) return null;
        return weapons[activeSlotIndex];
    }
    
    public Weapon GetWeaponInSlot(int slot)
    {
        if (slot < 0 || slot > 1) return null;
        return weapons[slot];
    }
    
    public int GetActiveSlotIndex() => activeSlotIndex;
}
