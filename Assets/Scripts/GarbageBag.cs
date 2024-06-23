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

    private void Awake()
    {
    }

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
            if (player.EquippedWeapon.LightHitEffect != null)
                Instantiate(player.EquippedWeapon.LightHitEffect, collision.contacts[0].point, Quaternion.identity);

            if (collision.gameObject.CompareTag("Enemy"))
            {
                if (hasDamagedEnemy) return;
                hasDamagedEnemy = true;
                EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
                GameObject vfx = Instantiate(player.ranged_light_comicVFX, collision.contacts[0].point + new Vector3(0, 0, 0.2f), Quaternion.identity);

                float distance = Vector3.Distance(vfx.transform.position, player.transform.position);
                float scaleFactor = 1.0f + distance * 0.08f;
                vfx.transform.GetChild(1).transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
                for (int i = 0; i < vfx.transform.GetChild(1).childCount; i++)
                {
                    Transform childTransform = vfx.transform.GetChild(1).GetChild(i);
                    childTransform.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
                }

                enemy.TakeDamage(player.EquippedWeapon.LightAttackDamage, AttackStrength.Light);
            }
        }
    }
}
