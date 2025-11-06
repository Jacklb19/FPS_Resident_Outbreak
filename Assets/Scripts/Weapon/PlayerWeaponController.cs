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
    public float dropDistance = 2f; // Distancia frente al jugador donde se tira el arma
    public float dropForce = 5f; // Fuerza con la que se lanza el arma

    private WeaponPickup currentTargetPickup;
    private WeaponPickup previousTargetPickup;

    void Update()
    {
        HandleWeaponSwitching();
        HandleWeaponActions();
        HandlePickupDetection();
        HandleWeaponDrop(); // ← NUEVO
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

    // ═══ NUEVO: Sistema de tirar armas ═══
    void HandleWeaponDrop()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            int activeSlot = weaponInventory.GetActiveSlotIndex();

            if (activeSlot == -1 || weaponInventory.GetActiveWeapon() == null)
            {
                Debug.Log("[PlayerWeaponController] No hay arma equipada para tirar");
                return;
            }

            Weapon activeWeapon = weaponInventory.GetActiveWeapon();
            if (activeWeapon.IsReloading())
            {
                Debug.Log("[PlayerWeaponController] No puedes tirar el arma mientras recargas");
                return;
            }

            // Dirección exacta de la cámara
            Vector3 cameraForward = playerCamera.transform.forward;

            // Posición inicial: frente a la cámara
            Vector3 dropPosition = playerCamera.transform.position + cameraForward * 1f;

            // Tirar el arma
            GameObject droppedWeapon = weaponInventory.DropWeapon(activeSlot, dropPosition);

            if (droppedWeapon != null)
            {
                Rigidbody rb = droppedWeapon.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Tirar hacia donde apunta la cámara
                    rb.AddForce(cameraForward * dropForce, ForceMode.VelocityChange);

                    // Rotación visual
                    rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.VelocityChange);
                }

                Debug.Log($"[PlayerWeaponController] Arma tirada hacia {cameraForward}");
            }
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
