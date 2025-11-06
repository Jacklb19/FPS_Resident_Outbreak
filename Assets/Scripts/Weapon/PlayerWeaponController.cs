using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public WeaponInventory weaponInventory;
    
    [Header("Raycast Settings")]
    public float interactDistance = 3f;
    public LayerMask pickupLayer;
    
    private WeaponPickup currentTargetPickup;
    private WeaponPickup previousTargetPickup;
    
    void Update()
    {
        HandleWeaponSwitching();
        HandleWeaponActions();
        HandlePickupDetection();
    }
    
    void HandleWeaponSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponInventory.SwitchToSlot(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weaponInventory.SwitchToSlot(1);
        }
    }
    
    void HandleWeaponActions()
    {
        Weapon activeWeapon = weaponInventory.GetActiveWeapon();
        if (activeWeapon == null) return;
        
        if (Input.GetMouseButton(0))
        {
            activeWeapon.TryShoot();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            activeWeapon.StartReload();
        }
    }
    
    void HandlePickupDetection()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, pickupLayer))
        {
            WeaponPickup pickup = hit.collider.GetComponent<WeaponPickup>();
            
            if (pickup != null)
            {
                currentTargetPickup = pickup;
                
                if (previousTargetPickup != null && previousTargetPickup != currentTargetPickup)
                {
                    previousTargetPickup.ShowOutline(false);
                }
                
                currentTargetPickup.ShowOutline(true);
                previousTargetPickup = currentTargetPickup;
                
                if (Input.GetKeyDown(KeyCode.F))
                {
                    PickupWeapon(pickup, hit.point);
                }
                
                return;
            }
        }
        
        if (previousTargetPickup != null)
        {
            previousTargetPickup.ShowOutline(false);
            previousTargetPickup = null;
        }
        
        currentTargetPickup = null;
    }
    
    void PickupWeapon(WeaponPickup pickup, Vector3 pickupPosition)
    {
        WeaponInstance weaponInstance = pickup.GetWeaponInstance();
        
        if (weaponInventory.HasFreeSlot(out int freeSlot))
        {
            // Hay slot libre, equipar directamente
            weaponInventory.EquipWeapon(pickup.weaponData.modelPrefab, weaponInstance, freeSlot);
            Destroy(pickup.gameObject);
        }
        else
        {
            // No hay slots libres, intercambiar con el arma activa
            int activeSlot = weaponInventory.GetActiveSlotIndex();
            
            // ═══ CAMBIO: Pasar la posición donde estaba el pickup ═══
            GameObject droppedWeapon = weaponInventory.DropWeapon(activeSlot, pickupPosition);
            
            if (droppedWeapon != null)
            {
                // Ya se creó en pickupPosition, solo ajustar rotación
                droppedWeapon.transform.rotation = Quaternion.identity;
            }
            
            weaponInventory.EquipWeapon(pickup.weaponData.modelPrefab, weaponInstance, activeSlot);
            Destroy(pickup.gameObject);
        }
        
        previousTargetPickup = null;
        currentTargetPickup = null;
    }
    
    public Weapon GetActiveWeapon() => weaponInventory.GetActiveWeapon();
    public Weapon GetWeaponInSlot(int slot) => weaponInventory.GetWeaponInSlot(slot);
    public int GetActiveSlotIndex() => weaponInventory.GetActiveSlotIndex();
}
