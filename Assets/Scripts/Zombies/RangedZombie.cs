using System.Collections;
using UnityEngine;

public class RangedZombie : Zombie
{
    [Header("Ranged Settings")]
    public Transform firePoint;
    public float launchAngleDeg = 45f;

    protected override IEnumerator PerformAttack()
    {
        isAttacking = true;

        navAgent.isStopped = true;
        navAgent.velocity = Vector3.zero;
        if (animator != null) animator.SetBool("isWalking", false);

        if (animator != null) animator.SetTrigger("ATTACK");
        if (config.attackSounds != null && config.attackSounds.Length > 0 && audioSource != null)
        {
            var attackSound = config.attackSounds[Random.Range(0, config.attackSounds.Length)];
            audioSource.PlayOneShot(attackSound);
        }

        // Momento del lanzamiento
        yield return new WaitForSeconds(0.4f);

        if (target != null && !isDead && config.projectilePrefab != null && firePoint != null)
        {
            Vector3 start = firePoint.position;
            Vector3 targetPos = target.position + Vector3.up * config.aimHeightOffset;

            GameObject proj = Instantiate(config.projectilePrefab, start, Quaternion.identity);
            Rigidbody rb = proj.GetComponent<Rigidbody>();

            float timeToHit = 0.5f; // valor por defecto por si acaso

            if (rb != null)
            {
                rb.useGravity = true;

                Vector3 toTarget = targetPos - start;
                Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);
                float distanceXZ = toTargetXZ.magnitude;

                if (distanceXZ > 0.01f)
                {
                    Vector3 dirXZ = toTargetXZ.normalized;
                    float angleRad = launchAngleDeg * Mathf.Deg2Rad;
                    float v = config.projectileSpeed;

                    // Velocidad inicial
                    Vector3 velocity =
                        dirXZ * v * Mathf.Cos(angleRad) +
                        Vector3.up * v * Mathf.Sin(angleRad);

                    rb.velocity = velocity;

                    // Tiempo aproximado hasta el objetivo en XZ
                    float horizontalSpeed = v * Mathf.Cos(angleRad);
                    timeToHit = distanceXZ / horizontalSpeed;
                }
            }

            // Corrutina que hace el daño cuando "llega" la piedra
            StartCoroutine(ApplyRangedDamageAfterDelay(timeToHit));
        }

        lastAttackTime = Time.time;
        yield return new WaitForSeconds(0.6f);

        isAttacking = false;
        navAgent.isStopped = false;
    }

    private IEnumerator ApplyRangedDamageAfterDelay(float delay)
    {
        // Esperar hasta que el proyectil debería llegar
        yield return new WaitForSeconds(delay);

        if (target == null || isDead) yield break;

        // Solo dañamos si sigue relativamente cerca (como antes)
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= config.attackRange)
        {
            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(config.rangedDamage);
            }
        }
    }
}
