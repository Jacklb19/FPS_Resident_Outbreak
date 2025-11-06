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
                
                // Si es un nuevo target, desactivar el anterior
                if (previousTargetPickup != null && previousTargetPickup != currentTargetPickup)
                {
                    previousTargetPickup.ShowOutline(false);
                }
                
                // Activar outline del target actual
                currentTargetPickup.ShowOutline(true);
                previousTargetPickup = currentTargetPickup;
                
                // Recoger con F
                if (Input.GetKeyDown(KeyCode.F))
                {
                    PickupWeapon(pickup, hit.point);
                }
                
                return;
            }
        }
        
        // Si no hay target, desactivar outline anterior
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
            weaponInventory.EquipWeapon(pickup.weaponData.modelPrefab, weaponInstance, freeSlot);
            Destroy(pickup.gameObject);
        }
        else
        {
            int activeSlot = weaponInventory.GetActiveSlotIndex();
            GameObject droppedWeapon = weaponInventory.DropWeapon(activeSlot);
            
            if (droppedWeapon != null)
            {
                droppedWeapon.transform.position = pickupPosition;
                droppedWeapon.transform.rotation = Quaternion.identity;
            }
            
            weaponInventory.EquipWeapon(pickup.weaponData.modelPrefab, weaponInstance, activeSlot);
            Destroy(pickup.gameObject);
        }
        
        // Resetear referencias de outline
        previousTargetPickup = null;
        currentTargetPickup = null;
    }
    
    public Weapon GetActiveWeapon() => weaponInventory.GetActiveWeapon();
    public Weapon GetWeaponInSlot(int slot) => weaponInventory.GetWeaponInSlot(slot);
    public int GetActiveSlotIndex() => weaponInventory.GetActiveSlotIndex();
}
