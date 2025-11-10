using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HospitalObjective : MonoBehaviour
{
    [SerializeField] private int requiredSupplies = 4;
    [SerializeField] private int pointsPerSupply = 50; // PDF
    private int collected = 0;

    public void OnSupplyCollected()
    {
        collected++;
        GameManager.instance.AddScore(pointsPerSupply);
        if (collected >= requiredSupplies)
            GameManager.instance.LoadNextLevel();
    }
}

public class SupplyPickup : MonoBehaviour
{
    [SerializeField] private HospitalObjective objective;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            objective?.OnSupplyCollected();
            Destroy(gameObject);
        }
    }
}
