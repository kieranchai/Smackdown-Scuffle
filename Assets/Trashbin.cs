using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trashbin : MonoBehaviour
{
    private Collider _col;
    private Rigidbody _rb;
    public PlayerController player;
    private bool hasCollided = false;
    private float lifeTime = 0f;
    private bool hasDamagedEnemy = false;

    private void Update()
    {
        if (hasCollided)
        {
            lifeTime += Time.deltaTime;
            if (lifeTime >= 5f)
            {
                DestroyTrashBin();
            }
        }
    }

    public void DestroyTrashBin()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Level"))
        {
            hasCollided = true;
            if (player.EquippedWeapon.HeavyImpactSFX != null)
                player.AS.PlayOneShot(player.EquippedWeapon.HeavyImpactSFX);
            if (player.EquippedWeapon.HeavyHitEffect != null)
                Instantiate(player.EquippedWeapon.HeavyHitEffect, collision.contacts[0].point, Quaternion.identity);

            if (collision.gameObject.CompareTag("Enemy"))
            {
                if (hasDamagedEnemy) return;
                hasDamagedEnemy = true;
                EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
                enemy.TakeDamage(player.EquippedWeapon.HeavyAttackDamage, AttackStrength.Heavy);
            }
        }
    }
}
