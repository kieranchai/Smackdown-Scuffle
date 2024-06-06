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
        if (player.EquippedWeapon.MediumImpactSFX != null)
            player.AS.PlayOneShot(player.EquippedWeapon.MediumImpactSFX);
        if (player.EquippedWeapon.MediumHitEffect != null)
            Instantiate(player.EquippedWeapon.MediumHitEffect, collision.ClosestPoint(transform.position), Quaternion.identity);
        EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
        if (collision.gameObject.GetComponent<EnemyController>())
        {
            enemy.TakeDamage(damage, strength);
        }
    }
}
