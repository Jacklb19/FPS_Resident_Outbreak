using System.Collections;
using UnityEngine;

public class RangedZombie : Zombie
{
    [Header("Ataque a distancia")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float aimHeightOffset = 1.2f;

    protected override IEnumerator PerformAttack()
    {
        isAttacking = true;

        // Nos aseguramos de que no se mueva durante el ataque
        navAgent.isStopped = true;
        navAgent.velocity = Vector3.zero;
        animator.SetBool("isWalking", false);

        // Lanzar animación
        animator.SetTrigger("ATTACK");

        // Espera hasta el momento del lanzamiento (ajusta según tu anim)
        yield return new WaitForSeconds(0.4f);

        if (target != null)
        {
            // Asegurarnos de mirar al jugador justo al lanzar
            Vector3 lookDir = (target.position - transform.position).normalized;
            Quaternion lookRot = Quaternion.LookRotation(new Vector3(lookDir.x, 0, lookDir.z));
            transform.rotation = lookRot;

            // ---- PROYECTIL VISUAL ----
            if (projectilePrefab != null && firePoint != null)
            {
                Vector3 targetPos = target.position + Vector3.up * aimHeightOffset;
                Vector3 dir = (targetPos - firePoint.position).normalized;

                GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(dir));

                Rigidbody rb = proj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = dir * projectileSpeed;
                }
            }

            // ---- DAÑO REAL SOLO SI SIGUE EN RANGO ----
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget <= attackRange)
            {
                PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                }
            }
        }

        lastAttackTime = Time.time;

        // Espera a que termine la anim de ataque
        yield return new WaitForSeconds(0.6f);

        isAttacking = false;

        // Si estás fuera de rango, en el Update volverá a perseguirte
        navAgent.isStopped = false;
    }
}
