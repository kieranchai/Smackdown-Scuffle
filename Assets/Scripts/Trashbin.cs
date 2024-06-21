using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trashbin : MonoBehaviour
{
    private Collider _col;
    private Rigidbody _rb;
    [SerializeField]
    private Material flickerMat;

    [HideInInspector]
    public PlayerController player;

    private float lifeTime = 0f;
    private bool hasCollided = false;
    private bool hasDamagedEnemy = false;
    private bool hasSetFlicker = false;

    private void Start()
    {
        _col = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
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
                    gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = flickerMat;
                    gameObject.transform.GetChild(1).GetComponent<MeshRenderer>().material = flickerMat;
                }
                else
                {
                    gameObject.GetComponent<MeshRenderer>().material.color = new Color(255f / 255f, 255f / 255f, 255f / 255f, Mathf.PingPong(Time.time, 1));
                    gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = new Color(255f / 255f, 255f / 255f, 255f / 255f, Mathf.PingPong(Time.time, 1));
                    gameObject.transform.GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(255f / 255f, 255f / 255f, 255f / 255f, Mathf.PingPong(Time.time, 1));
                }
            }
            if (lifeTime >= 5f)
            {
                if (gameObject.GetComponent<MeshRenderer>().material.color.a <= 0.3f) DestroyTrashBin();
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
                GameObject vfx = Instantiate(player.ranged_heavy_comicVFX, collision.contacts[0].point + new Vector3(0, 0, 0.2f), Quaternion.identity);

                float distance = Vector3.Distance(vfx.transform.position, player.transform.position);
                float scaleFactor = 1.0f + distance * 0.15f;
                vfx.transform.GetChild(1).transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
                for (int i = 0; i < vfx.transform.GetChild(1).childCount; i++)
                {
                    Transform childTransform = vfx.transform.GetChild(1).GetChild(i);
                    childTransform.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
                }

                enemy.TakeDamage(player.EquippedWeapon.HeavyAttackDamage, AttackStrength.Heavy);
            }
        }
    }
}
