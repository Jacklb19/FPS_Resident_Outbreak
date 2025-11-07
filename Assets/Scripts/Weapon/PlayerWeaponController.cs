using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public WeaponInventory weaponInventory;

    [Header("Raycast Settings")]
    public float interactDistance = 3f;
    public LayerMask pickupLayer;

    [Header("Drop Settings")]
    public float dropForce = 10f;

    private WeaponPickup currentTargetPickup;
    private WeaponPickup previousTargetPickup;

    void Update()
    {
        HandleWeaponSwitching();
        HandleWeaponActions();
        HandlePickupDetection();
        HandleWeaponDrop();
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

        // Disparo con botón izquierdo
        if (Input.GetMouseButton(0))
        {
            activeWeapon.TryShoot();
        }

        // Recarga con R
        if (Input.GetKeyDown(KeyCode.R))
        {
            activeWeapon.StartReload();
        }

        // ═══ ADS CON BOTÓN DERECHO DEL MOUSE ═══
        if (Input.GetMouseButtonDown(1)) // Presionar botón derecho
        {
            activeWeapon.EnterAds();
        }
        else if (Input.GetMouseButtonUp(1)) // Soltar botón derecho
        {
            activeWeapon.ExitAds();
        }
    }



    void HandleWeaponDrop()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            int activeSlot = weaponInventory.GetActiveSlotIndex();

            if (activeSlot == -1 || weaponInventory.GetActiveWeapon() == null)
            {
                return;
            }

            Weapon activeWeapon = weaponInventory.GetActiveWeapon();
            if (activeWeapon.IsReloading())
            {
                return;
            }

            Vector3 cameraCenter = playerCamera.transform.position;
            Vector3 cameraForward = playerCamera.transform.forward;
            Vector3 throwPosition = cameraCenter + cameraForward * 0.7f;
            Vector3 throwVelocity = cameraForward * dropForce;

            weaponInventory.ThrowWeapon(activeSlot, throwPosition, throwVelocity);
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
                    PickupWeapon(pickup);
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

    void PickupWeapon(WeaponPickup pickup)
    {
        WeaponInstance weaponInstance = pickup.GetWeaponInstance();
        Vector3 originalPosition = pickup.transform.position;

        if (weaponInventory.HasFreeSlot(out int freeSlot))
        {
            weaponInventory.EquipWeapon(pickup.weaponData.modelPrefab, weaponInstance, freeSlot);
            Destroy(pickup.gameObject);
        }
        else
        {
            int activeSlot = weaponInventory.GetActiveSlotIndex();

            GameObject droppedWeapon = weaponInventory.DropWeapon(activeSlot, originalPosition);

            if (droppedWeapon != null)
            {
                droppedWeapon.transform.rotation = pickup.transform.rotation;
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
