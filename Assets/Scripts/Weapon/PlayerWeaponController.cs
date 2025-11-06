using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private Transform weaponSpawnPoint;
    [SerializeField] private WeaponData startingWeaponData;

    private Weapon[] weaponSlots = new Weapon[2];
    private int activeSlotIndex = 0;
    private bool canSwitch = true;

    private void Start()
    {
    }

    private void Update()
    {
        Weapon currentWeapon = GetActiveWeapon();
        if (currentWeapon == null)
            return;

        if (Input.GetMouseButton(0))
        {
            currentWeapon.Fire();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentWeapon.Reload();
        }

        if (Input.GetKeyDown(KeyCode.E) && canSwitch)
        {
            SwitchWeapon();
        }
    }

    public void EquipWeapon(WeaponData weaponData)
    {
        if (weaponData.modelPrefab == null)
        {
            Debug.LogError("ModelPrefab no asignado en " + weaponData.name);
            return;
        }

        // Buscar slot vacío o encontrar si el arma ya existe
        int targetSlot = -1;

        // Verificar si esta arma ya está en algún slot
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] != null &&
                weaponSlots[i].GetWeaponData().weaponID == weaponData.weaponID)
            {
                // El arma ya existe, solo cambiar a ella
                activeSlotIndex = i;
                weaponSlots[i].gameObject.SetActive(true);

                // Desactivar la otra arma
                int otherSlot = 1 - i;
                if (weaponSlots[otherSlot] != null)
                {
                    weaponSlots[otherSlot].gameObject.SetActive(false);
                }

                Debug.Log($"Arma existente: {weaponData.weaponName}");
                return;
            }
        }

        // Si no existe, buscar slot vacío
        if (weaponSlots[0] == null)
        {
            targetSlot = 0;
        }
        else if (weaponSlots[1] == null)
        {
            targetSlot = 1;
        }
        else
        {
            // Ambos slots ocupados, guardar en el inactivo
            int inactiveSlot = 1 - activeSlotIndex;
            Destroy(weaponSlots[inactiveSlot].gameObject);
            targetSlot = inactiveSlot;
        }

        // Crear nueva arma
        GameObject weaponInstance = Instantiate(
            weaponData.modelPrefab,
            weaponSpawnPoint
        );

        weaponInstance.transform.localPosition = weaponData.spawnPosition;
        weaponInstance.transform.localRotation = Quaternion.Euler(weaponData.spawnRotation);

        Weapon newWeapon = weaponInstance.GetComponent<Weapon>();
        if (newWeapon == null)
            newWeapon = weaponInstance.AddComponent<Weapon>();

        newWeapon.weaponData = weaponData;
        newWeapon.Initialize(); // Solo inicializar si es NUEVA

        weaponSlots[targetSlot] = newWeapon;

        // Si es el primer arma, equiparla
        if (activeSlotIndex == targetSlot || (weaponSlots[activeSlotIndex] == null))
        {
            activeSlotIndex = targetSlot;
            weaponInstance.SetActive(true);
        }
        else
        {
            // Guardar en el otro slot, desactivada
            weaponInstance.SetActive(false);
        }

        Debug.Log($"Arma equipada en slot {targetSlot}: {weaponData.weaponName}");
    }

    public void SwitchWeapon()
    {
        canSwitch = false;

        int otherSlot = 1 - activeSlotIndex;

        if (weaponSlots[otherSlot] == null)
        {
            Debug.LogWarning("No hay arma en el otro slot");
            canSwitch = true;
            return;
        }

        if (weaponSlots[activeSlotIndex] != null)
        {
            weaponSlots[activeSlotIndex].gameObject.SetActive(false);
            Debug.Log($"Desactivado: {weaponSlots[activeSlotIndex].GetWeaponName()}");
        }

        activeSlotIndex = otherSlot;

        weaponSlots[activeSlotIndex].gameObject.SetActive(true);
        Debug.Log($"Arma activa: {weaponSlots[activeSlotIndex].GetWeaponName()}");

        Invoke(nameof(AllowSwitch), 0.5f);
    }

    private void AllowSwitch()
    {
        canSwitch = true;
    }

    public Weapon GetActiveWeapon()
    {
        if (activeSlotIndex >= 0 && activeSlotIndex < weaponSlots.Length)
        {
            if (weaponSlots[activeSlotIndex] != null && weaponSlots[activeSlotIndex].gameObject.activeSelf)
                return weaponSlots[activeSlotIndex];
        }

        return null;
    }

    public Weapon GetWeaponInSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < weaponSlots.Length)
            return weaponSlots[slotIndex];

        return null;
    }
    public int GetActiveSlotIndex()
    {
        return activeSlotIndex;
    }
}
