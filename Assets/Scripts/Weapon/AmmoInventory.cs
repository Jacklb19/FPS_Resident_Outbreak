using UnityEngine;
using System.Collections.Generic;

public class AmmoInventory : MonoBehaviour
{
    private Dictionary<string, int> ammoStorage = new Dictionary<string, int>();

    public void InitializeAmmo(WeaponData weaponData)
    {
        if (weaponData == null) return;

        if (ammoStorage.ContainsKey(weaponData.weaponID))
            return;

        ammoStorage[weaponData.weaponID] = weaponData.startingReserveAmmo;
    }

    public int GetAmmo(string weaponID)
    {
        if (ammoStorage.ContainsKey(weaponID))
            return ammoStorage[weaponID];

        return 0;
    }

    public int GetAmmo(WeaponData weaponData)
    {
        if (weaponData == null) return 0;
        return GetAmmo(weaponData.weaponID);
    }

    public void SetAmmo(WeaponData weaponData, int amount)
    {
        if (weaponData == null) return;
        ammoStorage[weaponData.weaponID] = Mathf.Max(0, amount);
    }

    public int ConsumeAmmo(WeaponData weaponData, int amount)
    {
        if (weaponData == null) return 0;

        int currentAmmo = GetAmmo(weaponData.weaponID);
        int ammoToConsume = Mathf.Min(amount, currentAmmo);

        ammoStorage[weaponData.weaponID] = currentAmmo - ammoToConsume;

        return ammoToConsume;
    }

    public int AddAmmo(WeaponData weaponData, int amount)
    {
        if (weaponData == null || amount <= 0) return 0;

        if (!ammoStorage.ContainsKey(weaponData.weaponID))
        {
            InitializeAmmo(weaponData);
        }

        int currentAmmo = GetAmmo(weaponData.weaponID);
        int maxAmmo = weaponData.maxReserveAmmo;

        int ammoToAdd = Mathf.Min(amount, maxAmmo - currentAmmo);

        if (ammoToAdd > 0)
        {
            ammoStorage[weaponData.weaponID] = currentAmmo + ammoToAdd;
        }

        return ammoToAdd;
    }

    public bool CanAddAmmo(WeaponData weaponData)
    {
        if (weaponData == null) return false;

        int currentAmmo = GetAmmo(weaponData.weaponID);
        return currentAmmo < weaponData.maxReserveAmmo;
    }

    public int GetAmmoNeeded(WeaponData weaponData)
    {
        if (weaponData == null) return 0;

        int currentAmmo = GetAmmo(weaponData.weaponID);
        return weaponData.maxReserveAmmo - currentAmmo;
    }

    public void DebugPrintInventory()
    {
        Debug.Log("=== AMMO INVENTORY ===");
        foreach (var entry in ammoStorage)
        {
            Debug.Log($"{entry.Key}: {entry.Value} balas");
        }
    }
}
