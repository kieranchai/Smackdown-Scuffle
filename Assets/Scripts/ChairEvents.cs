using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairEvents : MonoBehaviour
{
    [HideInInspector]
    public PlayerController player;
    [HideInInspector]
    public AttackStrength strength;
    [HideInInspector]
    public int damage;
    public Collider _col;
    public bool hasHit = false;

    public void MoveForward()
    {
        StartCoroutine(ForcedFwdMovement());
    }

    public System.Collections.IEnumerator ForcedFwdMovement()
    {
        float moveTime = 0f;
        while (moveTime < 0.2f)
        {
            player.CC.Move(player.transform.forward * 5f * Time.deltaTime);
            moveTime += Time.deltaTime;
            yield return null;
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (!hasHit)
        {
            hasHit = true;
            if (player.EquippedWeapon.LightImpactSFX != null)
                player.AS.PlayOneShot(player.EquippedWeapon.LightImpactSFX);
            if (player.EquippedWeapon.LightHitEffect != null)
                Instantiate(player.EquippedWeapon.LightHitEffect, collision.ClosestPoint(transform.position), Quaternion.identity);
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (collision.gameObject.GetComponent<EnemyController>())
            {
                Instantiate(player.light_comicVFX, collision.ClosestPoint(transform.position) + new Vector3(0, 0, 0.2f), Quaternion.identity);
                enemy.TakeDamage(damage, strength);
            }
        }
    }
}
