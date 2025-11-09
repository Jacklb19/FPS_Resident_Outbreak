using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sistema centralizado de munición. Almacena munición por weaponID.
/// </summary>
public class AmmoInventory : MonoBehaviour
{
    // Diccionario: weaponID -> cantidad de munición
    private Dictionary<string, int> ammoStorage = new Dictionary<string, int>();
    
    /// <summary>
    /// Inicializa la munición para un arma cuando se equipa por primera vez.
    /// </summary>
    public void InitializeAmmo(WeaponData weaponData)
    {
        if (weaponData == null) return;
        
        // Si ya existe munición para esta arma, no hacer nada
        if (ammoStorage.ContainsKey(weaponData.weaponID))
            return;
        
        // Inicializar con la munición inicial configurada
        ammoStorage[weaponData.weaponID] = weaponData.startingReserveAmmo;
        
        Debug.Log($"Munición inicializada para {weaponData.weaponName}: {weaponData.startingReserveAmmo}");
    }
    
    /// <summary>
    /// Obtiene la munición actual para un arma específica.
    /// </summary>
    public int GetAmmo(string weaponID)
    {
        if (ammoStorage.ContainsKey(weaponID))
            return ammoStorage[weaponID];
        
        return 0;
    }
    
    /// <summary>
    /// Establece la munición para un arma específica.
    /// </summary>
    public void SetAmmo(string weaponID, int amount)
    {
        ammoStorage[weaponID] = Mathf.Max(0, amount);
    }
    
    /// <summary>
    /// Consume munición (al recargar). Retorna la cantidad realmente consumida.
    /// </summary>
    public int ConsumeAmmo(WeaponData weaponData, int amount)
    {
        if (weaponData == null) return 0;
        
        int currentAmmo = GetAmmo(weaponData.weaponID);
        int ammoToConsume = Mathf.Min(amount, currentAmmo);
        
        ammoStorage[weaponData.weaponID] = currentAmmo - ammoToConsume;
        
        return ammoToConsume;
    }
    
    /// <summary>
    /// Añade munición para un arma. Respeta el máximo. Retorna la cantidad realmente añadida.
    /// </summary>
    public int AddAmmo(WeaponData weaponData, int amount)
    {
        if (weaponData == null || amount <= 0) return 0;
        
        // Asegurar que existe la entrada
        if (!ammoStorage.ContainsKey(weaponData.weaponID))
        {
            InitializeAmmo(weaponData);
        }
        
        int currentAmmo = GetAmmo(weaponData.weaponID);
        int maxAmmo = weaponData.maxReserveAmmo;
        
        // Calcular cuánto se puede añadir realmente
        int ammoToAdd = Mathf.Min(amount, maxAmmo - currentAmmo);
        
        if (ammoToAdd > 0)
        {
            ammoStorage[weaponData.weaponID] = currentAmmo + ammoToAdd;
        }
        
        return ammoToAdd;
    }
    
    /// <summary>
    /// Verifica si se puede añadir munición (no está al máximo).
    /// </summary>
    public bool CanAddAmmo(WeaponData weaponData)
    {
        if (weaponData == null) return false;
        
        int currentAmmo = GetAmmo(weaponData.weaponID);
        return currentAmmo < weaponData.maxReserveAmmo;
    }
    
    /// <summary>
    /// Retorna cuánta munición falta para llegar al máximo.
    /// </summary>
    public int GetAmmoNeeded(WeaponData weaponData)
    {
        if (weaponData == null) return 0;
        
        int currentAmmo = GetAmmo(weaponData.weaponID);
        return weaponData.maxReserveAmmo - currentAmmo;
    }
    
    /// <summary>
    /// DEBUG: Imprime todo el inventario de munición.
    /// </summary>
    public void DebugPrintInventory()
    {
        Debug.Log("=== AMMO INVENTORY ===");
        foreach (var entry in ammoStorage)
        {
            Debug.Log($"{entry.Key}: {entry.Value} balas");
        }
    }
}
