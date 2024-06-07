using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageBag : MonoBehaviour
{
    private Collider _col;
    private Rigidbody _rb;

    [SerializeField]
    private Material flickerMat;

    [HideInInspector]
    public PlayerController player;

    private bool hasCollided = false;
    private float lifeTime = 0f;
    private bool hasDamagedEnemy = false;
    private bool hasSetFlicker = false;

    private void Update()
    {
        if (hasCollided)
        {
            lifeTime += Time.deltaTime;
            if (lifeTime >= 3f)
            {
                if (!hasSetFlicker)
                {
                    hasSetFlicker = true;
                    gameObject.GetComponent<MeshRenderer>().material = flickerMat;
                }
                else gameObject.GetComponent<MeshRenderer>().material.color = new Color(51f / 255f, 51f / 255f, 51f / 255f, Mathf.PingPong(Time.time, 1));
            }
            if (lifeTime >= 5f)
            {
                if (gameObject.GetComponent<MeshRenderer>().material.color.a <= 0.3f) DestroyGarbageBag();
            }
        }
    }

    public void DestroyGarbageBag()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Level"))
        {
            hasCollided = true;
            if (player.EquippedWeapon.LightImpactSFX != null)
                player.AS.PlayOneShot(player.EquippedWeapon.LightImpactSFX);
            if (player.EquippedWeapon.LightHitEffect != null)
                Instantiate(player.EquippedWeapon.LightHitEffect, collision.contacts[0].point, Quaternion.identity);

            if (collision.gameObject.CompareTag("Enemy"))
            {
                if (hasDamagedEnemy) return;
                hasDamagedEnemy = true;
                EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
                enemy.TakeDamage(player.EquippedWeapon.LightAttackDamage, AttackStrength.Light);
            }
        }
    }
}
