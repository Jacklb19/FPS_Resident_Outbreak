using UnityEngine;

[System.Serializable]
public class WeaponInstance
{
    public WeaponData weaponData;
    public int currentMagazineAmmo;
    public int totalReserveAmmo;
    
    public WeaponInstance(WeaponData data, int startingAmmo = -1)
    {
        weaponData = data;
        currentMagazineAmmo = data.magazineSize;
        
        // Si no se especifica, usa 3 cargadores de reserva
        totalReserveAmmo = startingAmmo >= 0 ? startingAmmo : data.magazineSize * 3;
    }
    
    public bool CanShoot()
    {
        return currentMagazineAmmo > 0;
    }
    
    public bool CanReload()
    {
        return currentMagazineAmmo < weaponData.magazineSize && totalReserveAmmo > 0;
    }
    
    public void Reload()
    {
        int ammoNeeded = weaponData.magazineSize - currentMagazineAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, totalReserveAmmo);
        
        currentMagazineAmmo += ammoToReload;
        totalReserveAmmo -= ammoToReload;
    }
    
    public void ConsumeAmmo()
    {
        currentMagazineAmmo = Mathf.Max(0, currentMagazineAmmo - 1);
    }
}
