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
    public float dropForce = 10f; // Fuerza con la que se lanza el arma

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

        if (Input.GetMouseButton(0))
        {
            activeWeapon.TryShoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            activeWeapon.StartReload();
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

            // Centro de la cámara (donde está la mira)
            Vector3 cameraCenter = playerCamera.transform.position;
            Vector3 cameraForward = playerCamera.transform.forward;

            // Spawn adelante del centro (para que no esté dentro de la cámara)
            Vector3 throwPosition = cameraCenter + cameraForward * 0.7f;

            // Velocidad de lanzamiento hacia donde miras
            Vector3 throwVelocity = cameraForward * dropForce;

            // Lanzar
            GameObject thrownWeapon = weaponInventory.ThrowWeapon(activeSlot, throwPosition, throwVelocity);
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
