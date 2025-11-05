using UnityEngine;
using UnityEngine.UI;

public class DynamicCrosshair : MonoBehaviour {
    [SerializeField] private Image crosshairImage;
    [SerializeField] private float minSize = 30f;
    [SerializeField] private float maxSize = 80f;
    [SerializeField] private float spreadIncreaseAmount = 30f;
    [SerializeField] private float spreadDecreaseSpeed = 5f;
    
    private float currentSpread = 0f;

    void Start() {
        // Si no asignaste la imagen desde el Inspector, la obtén automáticamente
        if (crosshairImage == null) {
            crosshairImage = GetComponent<Image>();
        }
        currentSpread = 0f;
        UpdateCrosshairSize();
    }

    void Update() {
        // Reducir el spread gradualmente cuando no disparas
        currentSpread = Mathf.Lerp(currentSpread, 0f, Time.deltaTime * spreadDecreaseSpeed);
        
        UpdateCrosshairSize();
    }

    // Llamar esta función cada vez que dispares
    public void ExpandCrosshair() {
        currentSpread = Mathf.Min(currentSpread + spreadIncreaseAmount, maxSize);
    }

    void UpdateCrosshairSize() {
        // Calcula el tamaño basado en el spread actual
        float newSize = Mathf.Lerp(minSize, maxSize, currentSpread / maxSize);
        crosshairImage.rectTransform.sizeDelta = new Vector2(newSize, newSize);
    }
}
