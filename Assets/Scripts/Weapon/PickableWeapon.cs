using UnityEngine;

public class PickableWeapon : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private float pickupRange = 70f; // Rango de visión
    [SerializeField] private Color outlineColor = Color.yellow;
    [SerializeField] private float outlineWidth = 4f;
    
    private Outline outline;
    private bool isInView = false;

    private void Start()
    {
        // Obtener o agregar Outline
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }

        // Configurar Outline
        if (outline != null)
        {
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = outlineColor;
            outline.OutlineWidth = outlineWidth;
            outline.enabled = false;
        }
    }

    private void Update()
    {
        CheckIfInCrosshair();

        if (isInView && Input.GetKeyDown(KeyCode.F))
        {
            PickUp();
        }
    }

    private void CheckIfInCrosshair()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
            return;

        // Raycast desde el CENTRO DE LA PANTALLA (donde está la mira)
        Ray ray = mainCam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        // Verificar si el raycast golpea ESTE objeto (el arma)
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            // Si el raycast golpea ESTE objeto
            if (hit.collider.gameObject == gameObject)
            {
                if (!isInView)
                {
                    isInView = true;
                    EnableOutline();
                    Debug.Log($"[{weaponData.weaponName}] ¡En la mira! Presiona F para recoger");
                }
            }
            else
            {
                // El raycast golpeó algo más
                if (isInView)
                {
                    isInView = false;
                    DisableOutline();
                    Debug.Log($"[{weaponData.weaponName}] Ya no está en la mira");
                }
            }
        }
        else
        {
            // El raycast no golpeó nada
            if (isInView)
            {
                isInView = false;
                DisableOutline();
            }
        }

        // DEBUG: Dibujar el raycast en la escena
        Debug.DrawRay(ray.origin, ray.direction * pickupRange, isInView ? Color.green : Color.red);
    }

    private void EnableOutline()
    {
        if (outline != null)
            outline.enabled = true;
    }

    private void DisableOutline()
    {
        if (outline != null)
            outline.enabled = false;
    }

    public void PickUp()
    {
        PlayerWeaponController playerController = FindObjectOfType<PlayerWeaponController>();
        
        if (playerController != null)
        {
            playerController.EquipWeapon(weaponData);
            Debug.Log($"¡Recogiste: {weaponData.weaponName}!");
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("PlayerWeaponController no encontrado");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2f); // Mostrar el arma en la escena
    }
}
