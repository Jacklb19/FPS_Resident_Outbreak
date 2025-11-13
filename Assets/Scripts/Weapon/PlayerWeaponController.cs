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

    private AmmoPickup currentTargetAmmo;
    private AmmoPickup previousTargetAmmo;

    private MedicalPickup currentTargetMedical;
    private MedicalPickup previousTargetMedical;

    private GeneratorSwitch currentTargetGenerator;
    private GeneratorSwitch previousTargetGenerator;

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

        if (Input.GetMouseButtonDown(1))
        {
            activeWeapon.EnterAds();
        }
        else if (Input.GetMouseButtonUp(1))
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
            // ✅ NUEVO: Detectar GeneratorSwitch
            GeneratorSwitch generatorSwitch = hit.collider.GetComponent<GeneratorSwitch>();
            if (generatorSwitch != null && !generatorSwitch.isActivated)
            {
                currentTargetGenerator = generatorSwitch;

                if (previousTargetGenerator != null && previousTargetGenerator != currentTargetGenerator)
                    previousTargetGenerator.ShowOutline(false);
                if (previousTargetMedical != null)
                {
                    previousTargetMedical.ShowOutline(false);
                    previousTargetMedical = null;
                }
                if (previousTargetAmmo != null)
                {
                    previousTargetAmmo.ShowOutline(false);
                    previousTargetAmmo = null;
                }
                if (previousTargetPickup != null)
                {
                    previousTargetPickup.ShowOutline(false);
                    previousTargetPickup = null;
                }

                currentTargetGenerator.ShowOutline(true);
                previousTargetGenerator = currentTargetGenerator;
                currentTargetMedical = null;
                currentTargetAmmo = null;
                currentTargetPickup = null;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    ActivateGenerator(generatorSwitch);
                }

                return;
            }

            MedicalPickup medicalPickup = hit.collider.GetComponent<MedicalPickup>();
            if (medicalPickup != null)
            {
                currentTargetMedical = medicalPickup;

                if (previousTargetMedical != null && previousTargetMedical != currentTargetMedical)
                    previousTargetMedical.ShowOutline(false);
                if (previousTargetGenerator != null)
                {
                    previousTargetGenerator.ShowOutline(false);
                    previousTargetGenerator = null;
                }
                if (previousTargetAmmo != null)
                {
                    previousTargetAmmo.ShowOutline(false);
                    previousTargetAmmo = null;
                }
                if (previousTargetPickup != null)
                {
                    previousTargetPickup.ShowOutline(false);
                    previousTargetPickup = null;
                }

                currentTargetMedical.ShowOutline(true);
                previousTargetMedical = currentTargetMedical;
                currentTargetAmmo = null;
                currentTargetPickup = null;
                currentTargetGenerator = null;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    PickupMedical(medicalPickup);
                }

                return;
            }

            AmmoPickup ammoPickup = hit.collider.GetComponent<AmmoPickup>();

            if (ammoPickup != null && ammoPickup.IsAvailable())
            {
                currentTargetAmmo = ammoPickup;

                if (previousTargetAmmo != null && previousTargetAmmo != currentTargetAmmo)
                {
                    previousTargetAmmo.ShowOutline(false);
                }

                if (previousTargetPickup != null)
                {
                    previousTargetPickup.ShowOutline(false);
                    previousTargetPickup = null;
                }
                if (previousTargetGenerator != null)
                {
                    previousTargetGenerator.ShowOutline(false);
                    previousTargetGenerator = null;
                }

                currentTargetAmmo.ShowOutline(true);
                previousTargetAmmo = currentTargetAmmo;
                currentTargetPickup = null;
                currentTargetGenerator = null;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    PickupAmmo(ammoPickup);
                }

                return;
            }

            WeaponPickup weaponPickup = hit.collider.GetComponent<WeaponPickup>();

            if (weaponPickup != null)
            {
                currentTargetPickup = weaponPickup;

                if (previousTargetPickup != null && previousTargetPickup != currentTargetPickup)
                {
                    previousTargetPickup.ShowOutline(false);
                }

                if (previousTargetAmmo != null)
                {
                    previousTargetAmmo.ShowOutline(false);
                    previousTargetAmmo = null;
                }
                if (previousTargetGenerator != null)
                {
                    previousTargetGenerator.ShowOutline(false);
                    previousTargetGenerator = null;
                }

                currentTargetPickup.ShowOutline(true);
                previousTargetPickup = currentTargetPickup;
                currentTargetAmmo = null;
                currentTargetGenerator = null;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    PickupWeapon(weaponPickup);
                }

                return;
            }
        }

        if (previousTargetPickup != null)
        {
            previousTargetPickup.ShowOutline(false);
            previousTargetPickup = null;
        }
        if (previousTargetMedical != null)
        {
            previousTargetMedical.ShowOutline(false);
            previousTargetMedical = null;
        }
        if (previousTargetAmmo != null)
        {
            previousTargetAmmo.ShowOutline(false);
            previousTargetAmmo = null;
        }
        if (previousTargetGenerator != null)
        {
            previousTargetGenerator.ShowOutline(false);
            previousTargetGenerator = null;
        }

        currentTargetPickup = null;
        currentTargetAmmo = null;
        currentTargetMedical = null;
        currentTargetGenerator = null;
    }


    // ═══ CAMBIADO: Usa AmmoInventory directamente ═══
    void PickupAmmo(AmmoPickup ammoPickup)
    {
        AmmoInventory ammoInventory = weaponInventory.GetAmmoInventory();

        if (ammoPickup.TryPickup(ammoInventory, out int ammoAdded))
        {
            Debug.Log($"¡Recogiste {ammoAdded} balas de {ammoPickup.GetTargetWeaponData().weaponName}!");

            previousTargetAmmo = null;
            currentTargetAmmo = null;
        }
    }
    void PickupMedical(MedicalPickup pickup)
    {
        // Solo recoger para progreso de misión, sin curar
        if (pickup.TryPickup())
        {
            Debug.Log("[Player] Paquete médico recolectado para misión");
            previousTargetMedical = null;
            currentTargetMedical = null;
        }
    }

    void ActivateGenerator(GeneratorSwitch generator)
    {
        if (generator.TryActivate())
        {
            Debug.Log($"[Player] Generador activado: {generator.gameObject.name}");
            previousTargetGenerator = null;
            currentTargetGenerator = null;
        }
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
