using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    [Header("Weapon Slots")]
    public Transform weaponSlot1;
    public Transform weaponSlot2;
    
    private Weapon[] weapons = new Weapon[2];
    private int activeSlotIndex = -1;
    
    public void EquipWeapon(GameObject weaponPrefab, WeaponInstance weaponInstance, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > 1) return;
        
        Transform targetSlot = slotIndex == 0 ? weaponSlot1 : weaponSlot2;
        
        // Destruir arma anterior si existe
        if (weapons[slotIndex] != null)
        {
            Destroy(weapons[slotIndex].gameObject);
        }
        
        // Instanciar nueva arma
        GameObject weaponObj = Instantiate(weaponPrefab, targetSlot);
        weaponObj.transform.localPosition = weaponInstance.weaponData.spawnPosition;
        weaponObj.transform.localRotation = Quaternion.Euler(weaponInstance.weaponData.spawnRotation);
        
        Weapon weapon = weaponObj.GetComponent<Weapon>();
        if (weapon == null)
        {
            weapon = weaponObj.AddComponent<Weapon>();
        }
        
        weapon.Initialize(weaponInstance);
        weapons[slotIndex] = weapon;
        
        // Desactivar inicialmente
        weaponObj.SetActive(false);
        
        // Si no hay arma activa, activar esta automáticamente
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
        
        // Desequipar arma actual
        if (activeSlotIndex != -1 && weapons[activeSlotIndex] != null)
        {
            weapons[activeSlotIndex].OnUnequip(); // ← Solo esto es nuevo
            weapons[activeSlotIndex].gameObject.SetActive(false);
        }
        
        // Equipar nueva arma
        activeSlotIndex = slotIndex;
        weapons[activeSlotIndex].gameObject.SetActive(true);
        weapons[activeSlotIndex].OnEquip(); // ← Solo esto es nuevo
    }
    
    // ═══ MÉTODO ORIGINAL - Crear pickup en la posición donde se recogió ═══
    public GameObject DropWeapon(int slotIndex, Vector3 dropPosition)
    {
        if (slotIndex < 0 || slotIndex > 1) return null;
        if (weapons[slotIndex] == null) return null;
        
        Weapon weapon = weapons[slotIndex];
        WeaponInstance weaponInstance = weapon.GetWeaponInstance();
        WeaponData weaponData = weaponInstance.weaponData;
        
        weapon.OnUnequip(); // ← Solo esto es nuevo
        
        // ═══ TU SISTEMA ORIGINAL ═══
        // Crear pickup usando el modelPrefab del WeaponData
        GameObject droppedWeapon = Instantiate(weaponData.modelPrefab, dropPosition, Quaternion.identity);
        
        // Asegurarse que tenga WeaponPickup
        WeaponPickup pickup = droppedWeapon.GetComponent<WeaponPickup>();
        if (pickup == null)
        {
            pickup = droppedWeapon.AddComponent<WeaponPickup>();
        }
        
        // Inicializar el pickup con los datos actuales
        pickup.weaponData = weaponData;
        pickup.Initialize(weaponData, weaponInstance);
        
        // Destruir arma del inventario
        Destroy(weapon.gameObject);
        weapons[slotIndex] = null;
        
        // Si era el arma activa, resetear
        if (activeSlotIndex == slotIndex)
        {
            activeSlotIndex = -1;
            
            // Activar el otro slot si existe
            int otherSlot = slotIndex == 0 ? 1 : 0;
            if (weapons[otherSlot] != null)
            {
                SwitchToSlot(otherSlot);
            }
        }
        
        return droppedWeapon;
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
