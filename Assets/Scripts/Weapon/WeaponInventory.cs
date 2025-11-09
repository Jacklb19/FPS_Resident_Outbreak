using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    [Header("Weapon Slots")]
    public Transform weaponSlot1;
    public Transform weaponSlot2;

    [Header("Drop Settings")]
    public LayerMask groundLayer;
    
    // ═══ NUEVO: Referencia al inventario de munición ═══
    [Header("Ammo System")]
    public AmmoInventory ammoInventory;

    private Weapon[] weapons = new Weapon[2];
    private int activeSlotIndex = -1;
    
    void Awake()
    {
        // Asegurar que existe AmmoInventory
        if (ammoInventory == null)
        {
            ammoInventory = GetComponent<AmmoInventory>();
            
            if (ammoInventory == null)
            {
                ammoInventory = gameObject.AddComponent<AmmoInventory>();
            }
        }
    }

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
        
        // ═══ CAMBIADO: Pasar AmmoInventory ═══
        weapon.Initialize(weaponInstance, ammoInventory);
        
        weapons[slotIndex] = weapon;

        weaponObj.SetActive(false);

        if (activeSlotIndex == slotIndex)
        {
            int temp = activeSlotIndex;
            activeSlotIndex = -1;
            SwitchToSlot(temp);
        }
        else if (activeSlotIndex == -1)
        {
            SwitchToSlot(slotIndex);
        }
    }

    public void SwitchToSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > 1) return;

        if (activeSlotIndex == slotIndex)
        {
            if (weapons[slotIndex] != null && !weapons[slotIndex].gameObject.activeSelf)
            {
                weapons[slotIndex].gameObject.SetActive(true);
                weapons[slotIndex].OnEquip();
            }
            return;
        }

        if (activeSlotIndex != -1 && weapons[activeSlotIndex] != null)
        {
            weapons[activeSlotIndex].OnUnequip();
            weapons[activeSlotIndex].gameObject.SetActive(false);
        }

        activeSlotIndex = slotIndex;

        if (weapons[activeSlotIndex] != null)
        {
            weapons[activeSlotIndex].gameObject.SetActive(true);
            weapons[activeSlotIndex].OnEquip();
        }
    }

    public GameObject DropWeapon(int slotIndex, Vector3 dropPosition)
    {
        if (slotIndex < 0 || slotIndex > 1) return null;
        if (weapons[slotIndex] == null) return null;

        Weapon weapon = weapons[slotIndex];
        WeaponInstance weaponInstance = weapon.GetWeaponInstance();
        WeaponData weaponData = weaponInstance.weaponData;

        weapon.OnUnequip();

        Vector3 finalPosition = CalculateGroundPosition(dropPosition, weaponData);

        GameObject droppedWeapon = Instantiate(weaponData.modelPrefab, finalPosition, Quaternion.identity);

        Rigidbody rb = droppedWeapon.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Collider col = droppedWeapon.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        WeaponPickup pickup = droppedWeapon.GetComponent<WeaponPickup>();
        if (pickup == null)
        {
            pickup = droppedWeapon.AddComponent<WeaponPickup>();
        }

        Weapon weaponScript = droppedWeapon.GetComponent<Weapon>();
        if (weaponScript != null)
        {
            weaponScript.enabled = false;
        }

        pickup.weaponData = weaponData;
        pickup.Initialize(weaponData, weaponInstance);

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

    public GameObject ThrowWeapon(int slotIndex, Vector3 throwPosition, Vector3 throwVelocity)
    {
        if (slotIndex < 0 || slotIndex > 1) return null;
        if (weapons[slotIndex] == null) return null;

        Weapon weapon = weapons[slotIndex];
        WeaponInstance weaponInstance = weapon.GetWeaponInstance();
        WeaponData weaponData = weaponInstance.weaponData;

        weapon.OnUnequip();

        Vector3 adjustedThrowPosition = AdjustThrowPosition(throwPosition, weaponData);

        GameObject thrownWeapon = Instantiate(weaponData.modelPrefab, adjustedThrowPosition, Quaternion.identity);

        Rigidbody rb = thrownWeapon.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.velocity = throwVelocity;
            rb.angularVelocity = Vector3.zero;

            rb.AddTorque(Random.insideUnitSphere * 3f, ForceMode.VelocityChange);
        }

        Collider col = thrownWeapon.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        WeaponPickup pickup = thrownWeapon.GetComponent<WeaponPickup>();
        if (pickup == null)
        {
            pickup = thrownWeapon.AddComponent<WeaponPickup>();
        }

        Weapon weaponScript = thrownWeapon.GetComponent<Weapon>();
        if (weaponScript != null)
        {
            weaponScript.enabled = false;
        }

        pickup.weaponData = weaponData;
        pickup.Initialize(weaponData, weaponInstance);

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

        return thrownWeapon;
    }

    private Vector3 AdjustThrowPosition(Vector3 basePosition, WeaponData weaponData)
    {
        GameObject tempWeapon = Instantiate(weaponData.modelPrefab, Vector3.zero, Quaternion.identity);
        tempWeapon.name = "TempWeapon_ThrowAdjust";

        Collider weaponCollider = tempWeapon.GetComponent<Collider>();
        Vector3 offset = Vector3.zero;

        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
            Bounds bounds = weaponCollider.bounds;
            Vector3 pivotToCenter = bounds.center - tempWeapon.transform.position;
            offset = -pivotToCenter;
        }

        Destroy(tempWeapon);
        return basePosition + offset;
    }

    private Vector3 CalculateGroundPosition(Vector3 dropPosition, WeaponData weaponData)
    {
        float weaponHeight = GetWeaponHeight(weaponData);
        Vector3 rayStart = dropPosition + Vector3.up * 3f;
        RaycastHit groundHit;
        LayerMask layerToUse = groundLayer.value != 0 ? groundLayer : ~0;

        if (Physics.Raycast(rayStart, Vector3.down, out groundHit, 10f, layerToUse))
        {
            return groundHit.point + Vector3.up * weaponHeight;
        }

        if (Physics.SphereCast(rayStart, 0.5f, Vector3.down, out groundHit, 10f, layerToUse))
        {
            return groundHit.point + Vector3.up * weaponHeight;
        }

        if (Physics.Raycast(dropPosition, Vector3.down, out groundHit, 5f, layerToUse))
        {
            return groundHit.point + Vector3.up * weaponHeight;
        }

        Collider[] colliders = Physics.OverlapSphere(dropPosition, 3f, layerToUse);
        float lowestPoint = float.MaxValue;
        bool foundGround = false;

        foreach (Collider col in colliders)
        {
            if (col.isTrigger) continue;
            float topPoint = col.bounds.max.y;
            if (topPoint < dropPosition.y && topPoint < lowestPoint)
            {
                lowestPoint = topPoint;
                foundGround = true;
            }
        }

        if (foundGround)
        {
            return new Vector3(dropPosition.x, lowestPoint + weaponHeight, dropPosition.z);
        }

        return dropPosition + Vector3.up * weaponHeight;
    }

    private float GetWeaponHeight(WeaponData weaponData)
    {
        GameObject tempWeapon = Instantiate(weaponData.modelPrefab, Vector3.zero, Quaternion.identity);
        tempWeapon.name = "TempWeapon_Measuring";

        float height = 0.2f;

        Collider weaponCollider = tempWeapon.GetComponent<Collider>();
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
            Bounds bounds = weaponCollider.bounds;
            float pivotToBottom = Mathf.Abs(tempWeapon.transform.position.y - bounds.min.y);

            if (pivotToBottom > 0.01f)
            {
                height = pivotToBottom;
            }
            else
            {
                height = bounds.extents.y;
            }
        }

        Destroy(tempWeapon);
        return height;
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

    public bool HasWeaponEquipped()
    {
        return activeSlotIndex != -1 && weapons[activeSlotIndex] != null;
    }
    
    // ═══ NUEVO: Getter para AmmoInventory ═══
    public AmmoInventory GetAmmoInventory()
    {
        return ammoInventory;
    }
}
