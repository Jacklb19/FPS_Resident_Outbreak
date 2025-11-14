using System.Collections;
using UnityEngine;

public class RangedZombie : Zombie
{
    [Header("Ranged Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float aimHeightOffset = 1.2f; // para apuntar al pecho/cabeza

    protected override IEnumerator PerformAttack()
    {
        isAttacking = true;
        navAgent.isStopped = true;
        navAgent.velocity = Vector3.zero;

        // Animación de lanzar
        animator.SetTrigger("ATTACK"); // usa tu animación de lanzar

        // Esperar hasta el momento del lanzamiento (ajusta este tiempo)
        yield return new WaitForSeconds(0.4f);

        if (target != null && projectilePrefab != null && firePoint != null)
        {
            // Mirar hacia el jugador
            Vector3 dirLook = (target.position - transform.position).normalized;
            Quaternion lookRot = Quaternion.LookRotation(new Vector3(dirLook.x, 0, dirLook.z));
            transform.rotation = lookRot;

            // Dirección del disparo (apuntando un poco más alto)
            Vector3 targetPos = target.position + Vector3.up * aimHeightOffset;
            Vector3 dir = (targetPos - firePoint.position).normalized;

            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(dir));

            // Pasar daño al proyectil (opcional)
            ZombieProjectile zp = proj.GetComponent<ZombieProjectile>();
            if (zp != null)
            {
                zp.damage = attackDamage;
            }

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = dir * projectileSpeed;
            }
        }

        lastAttackTime = Time.time;

        // Espera a que termine la animación aproximadamente
        yield return new WaitForSeconds(0.6f);

        isAttacking = false;
        navAgent.isStopped = false;
    }
}
