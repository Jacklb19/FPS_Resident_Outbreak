using UnityEngine;

public class ExitTriggerZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[ExitTrigger] Detectado: {other.name} | Tag: {other.tag}");
        
        if (!other.CompareTag("Player")) return;

        // ✅ Buscar cualquier LevelFlow activo en la escena
        var levelFlow = FindObjectOfType<LevelFlow>();
        
        if (levelFlow != null)
        {
            Debug.Log($"[ExitTrigger] Notificando a {levelFlow.GetType().Name}");
            
            // Intentar llamar OnPlayerReachedExit si existe
            if (levelFlow is Level1_HospitalFlow hospitalFlow)
            {
                hospitalFlow.OnPlayerReachedExit();
            }
            else if (levelFlow is Level2_StreetFlow streetFlow)
            {
                streetFlow.OnPlayerReachedExit();
            }
            else if (levelFlow is Level3_GraveyardFlow graveyardFlow)
            {
                graveyardFlow.OnPlayerReachedExit();
            }
            else
            {
                // Fallback genérico
                Debug.Log("[ExitTrigger] Llamando GameManager.LoadNextLevel directamente");
                GameManager.instance?.LoadNextLevel();
            }
        }
        else
        {
            Debug.LogWarning("[ExitTrigger] No se encontró LevelFlow en escena");
            // Fallback: cargar siguiente nivel directamente
            GameManager.instance?.LoadNextLevel();
        }
    }
}
