using UnityEngine;

public class ExitTriggerZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[ExitTrigger] Detectado: {other.name} | Tag: {other.tag}");
        
        if (!other.CompareTag("Player")) return;

        var flow = FindObjectOfType<Level1_HospitalFlow>();
        if (flow != null)
        {
            Debug.Log("[ExitTrigger] Notificando a LevelFlow");
            flow.OnPlayerReachedExit();
        }
        else
        {
            Debug.LogWarning("[ExitTrigger] No se encontró Level1_HospitalFlow en escena");
        }
    }
}
