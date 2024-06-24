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

    public AudioClip woosh1SFX;
    public AudioClip woosh2SFX;
    public AudioClip dashSFX;
    public AudioClip impact1SFX;
    public AudioClip impact2SFX;
    public AudioClip equipSFX;

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerController>();
    }

    public void PlayEquip()
    {
        player.AS.PlayOneShot(equipSFX);
    }

    public void PlayWoosh()
    {
        if (Random.Range(0, 2) == 0) player.AS.PlayOneShot(woosh1SFX);
        else player.AS.PlayOneShot(woosh2SFX);
    }

    public void MoveForward()
    {
        StartCoroutine(ForcedFwdMovement());
        player.AS.PlayOneShot(dashSFX);
    }

    public System.Collections.IEnumerator ForcedFwdMovement()
    {
        float moveTime = 0f;
        while (moveTime < 0.2f)
        {
            player.CC.Move(player.transform.forward * 8f * Time.deltaTime);
            moveTime += Time.deltaTime;
            yield return null;
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (!hasHit)
        {
            hasHit = true;
            if (Random.Range(0, 2) == 0) player.AS.PlayOneShot(impact1SFX);
            else player.AS.PlayOneShot(impact2SFX);

            if (player.EquippedWeapon.LightHitEffect != null)
            {
                player.CameraShake(AttackStrength.Light);
                Instantiate(player.EquippedWeapon.LightHitEffect, collision.ClosestPoint(transform.position), Quaternion.identity);
            }
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (collision.gameObject.GetComponent<EnemyController>())
            {
                Instantiate(player.light_comicVFX, collision.ClosestPoint(transform.position) + new Vector3(0, 0, 0.2f), Quaternion.identity);
                enemy.TakeDamage(damage, strength);
            }
        }
    }
}
