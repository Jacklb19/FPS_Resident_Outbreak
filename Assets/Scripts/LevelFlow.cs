using UnityEngine;

public abstract class LevelFlow : MonoBehaviour
{
    [Header("Referencias")]
    public SpawnManager spawnManager;
    public GameObject exitDoor;
    public Transform exitTrigger;

    protected int objectivesCompleted = 0;
    protected int objectivesRequired = 0;
    protected bool exitUnlocked = false;

    protected virtual void Start()
    {
        if (spawnManager == null) spawnManager = FindObjectOfType<SpawnManager>();
        Setup();
    }

    protected abstract void Setup();

    protected void UnlockExit()
    {
        if (exitUnlocked) return;
        exitUnlocked = true;

        if (exitDoor != null)
        {
            exitDoor.SetActive(true);
            Debug.Log("[LevelFlow] Salida desbloqueada y visible");
        }

        Debug.Log("[LevelFlow] Objetivo completado - Dirígete a la salida");
    }

    protected void CheckExitTrigger(Collider other)
    {
        if (!exitUnlocked) return;
        if (!other.CompareTag("Player")) return;

        Debug.Log("[LevelFlow] Jugador alcanzó la salida");
        OnLevelComplete();
    }

    protected virtual void OnLevelComplete()
    {
        // Sobrescribir en cada nivel si necesitas lógica especial
        GameManager.instance?.LoadNextLevel();
    }
}
