using UnityEngine;
using System.Collections;

public class DoorOpener : MonoBehaviour
{
    [Header("Config")]
    public float openAngle = -90f;
    public float openSpeed = 2f;
    public AudioClip openSound;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isOpening = false;
    private AudioSource audioSource;

    void Awake()
    {
        closedRotation = transform.localRotation;
        openRotation = Quaternion.Euler(0, openAngle, 0);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void Open()
    {
        if (isOpening) return;
        StartCoroutine(OpenDoor());
    }

    private IEnumerator OpenDoor()
    {
        isOpening = true;

        if (openSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(openSound);
        }

        float elapsed = 0f;
        float duration = 1f / openSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localRotation = Quaternion.Lerp(closedRotation, openRotation, t);
            yield return null;
        }

        transform.localRotation = openRotation;
        Debug.Log("[Door] Puerta abierta");
    }
}
