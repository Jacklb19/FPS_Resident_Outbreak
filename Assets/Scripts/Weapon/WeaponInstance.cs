using UnityEngine;

[System.Serializable]
public class WeaponInstance
{
    public WeaponData weaponData;
    public int currentMagazineAmmo;
    
    // ═══ CAMBIADO: Ya no almacenamos munición aquí ═══
    // La munición de reserva ahora está en AmmoInventory
    
    public WeaponInstance(WeaponData data)
    {
        weaponData = data;
        currentMagazineAmmo = data.magazineSize;
    }
    
    public bool CanShoot()
    {
        return currentMagazineAmmo > 0;
    }
    
    // ═══ CAMBIADO: Ahora requiere AmmoInventory ═══
    public bool CanReload(AmmoInventory ammoInventory)
    {
        if (ammoInventory == null) return false;
        
        int reserveAmmo = ammoInventory.GetAmmo(weaponData.weaponID);
        return currentMagazineAmmo < weaponData.magazineSize && reserveAmmo > 0;
    }
    
    // ═══ CAMBIADO: Ahora consume del inventario centralizado ═══
    public void Reload(AmmoInventory ammoInventory)
    {
        if (ammoInventory == null) return;
        
        int ammoNeeded = weaponData.magazineSize - currentMagazineAmmo;
        int ammoConsumed = ammoInventory.ConsumeAmmo(weaponData, ammoNeeded);
        
        currentMagazineAmmo += ammoConsumed;
    }
    
    public void ConsumeAmmo()
    {
        currentMagazineAmmo = Mathf.Max(0, currentMagazineAmmo - 1);
    }
    
    // ═══ NUEVO: Obtener munición de reserva desde el inventario ═══
    public int GetReserveAmmo(AmmoInventory ammoInventory)
    {
        if (ammoInventory == null) return 0;
        return ammoInventory.GetAmmo(weaponData.weaponID);
    }
}
