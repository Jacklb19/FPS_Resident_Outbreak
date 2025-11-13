using UnityEngine;

public class Level1_HospitalFlow : LevelFlow
{
    [Header("Hospital - Paquetes Médicos")]
    public int packagesRequired = 4;
    private int packagesCollected = 0;

    protected override void Setup()
    {
        objectivesRequired = packagesRequired;

        var pickups = FindObjectsOfType<MedicalPickup>();
        foreach (var pickup in pickups)
        {
            pickup.onCollected += OnPackageCollected;
        }

        Debug.Log($"[Hospital] {pickups.Length} paquetes médicos en escena. Objetivo: {packagesRequired}");
    }

    private void OnPackageCollected()
    {
        packagesCollected++;
        objectivesCompleted = packagesCollected;

        Debug.Log($"[Hospital] Paquete recolectado: {packagesCollected}/{packagesRequired}");

        if (packagesCollected >= packagesRequired)
        {
            UnlockExit();
        }
    }

    // ✅ NUEVO: método público llamado desde ExitTriggerZone
    public void OnPlayerReachedExit()
    {
        if (!exitUnlocked)
        {
            Debug.LogWarning("[Hospital] Salida aún bloqueada");
            return;
        }

        Debug.Log("[Hospital] Jugador alcanzó la salida → cargando siguiente nivel");
        GameManager.instance?.LoadNextLevel();
    }
    protected new void UnlockExit()
    {
        if (exitUnlocked) return;
        exitUnlocked = true;

        if (exitDoor != null)
        {
            exitDoor.SetActive(true);

            // ✅ Abrir puerta con script
            DoorOpener doorOpener = exitDoor.GetComponent<DoorOpener>();
            if (doorOpener != null)
            {
                doorOpener.Open();
            }

            // Activar Outline
            Outline[] outlines = exitDoor.GetComponentsInChildren<Outline>(true);
            foreach (var outline in outlines)
            {
                outline.enabled = true;
            }

            Debug.Log("[Hospital] Salida desbloqueada y outline activado");
        }

        // Bonus por completar misión
        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(500);
        }

        Debug.Log("[Hospital] Objetivo completado - Dirígete a la salida");
    }


    private void OnTriggerEnter(Collider other)
    {
        CheckExitTrigger(other);
    }

    private void OnDestroy()
    {
        var pickups = FindObjectsOfType<MedicalPickup>();
        foreach (var pickup in pickups)
        {
            pickup.onCollected -= OnPackageCollected;
        }
    }
}
