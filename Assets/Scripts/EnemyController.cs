using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("Health Parameters")]
    public int MaxHealth = 10;
    public int CurrentHealth = 10;

    [Header("Damaged Animations")]
    public AnimationClip LightDamageAnim;
    public AnimationClip MediumDamageAnim;
    public AnimationClip HeavyDamageAnim;
    public AnimationClip DeathAnim;
    public AnimationClip SlammedAnim;

    [Header("Combat Animations")]
    public AnimationClip LightAttackAnim;
    public AnimationClip MediumAttackAnim;
    public AnimationClip HeavyAttackAnim;

    [Header("Combat Parameter")]
    public int LightAttackDamage = 1;
    public int MediumAttackDamage = 2;
    public int HeavyAttackDamage = 3;

    [Header("Sound Effects")]
    public AudioClip LightDamageSFX;
    public AudioClip MediumDamageSFX;
    public AudioClip HeavyDamageSFX;
    public AudioClip DeathSFX;

    [Header("Object References")]
    public Animator EnemyAnimator;
    public GameObject EnemyPrefab;
    public GameObject SpawnParticles;
    public Rigidbody EnemyRigidBody;

    public EnemyHP enemyHPBar;

    private void OnEnable()
    {
        Instantiate(SpawnParticles, transform.position, Quaternion.identity);
        CurrentHealth = MaxHealth;
        GetComponent<CapsuleCollider>().enabled = true;
        enemyHPBar.ResetBar();
    }

    public void TakeDamage(int damageAmount, AttackStrength strength)
    {
        if (CurrentHealth > 0)
        {
            GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.95f, 1.15f);
            Debug.Log("Enemy hit for " + damageAmount);
            switch (strength)
            {
                case AttackStrength.Light:
                    EnemyAnimator.Play(LightDamageAnim.name, -1, 0);
                    GetComponent<AudioSource>().PlayOneShot(LightDamageSFX);
                    break;

                case AttackStrength.Medium:
                    EnemyAnimator.Play(MediumDamageAnim.name, -1, 0);
                    GetComponent<AudioSource>().PlayOneShot(MediumDamageSFX);
                    break;

                case AttackStrength.Heavy:
                    EnemyAnimator.Play(HeavyDamageAnim.name, -1, 0);
                    GetComponent<AudioSource>().PlayOneShot(HeavyDamageSFX);
                    break;
            }

            CurrentHealth -= damageAmount;

            if (CurrentHealth <= 0)
                Die();

            enemyHPBar.UpdateHealthBar();
            enemyHPBar.ShowName();
            enemyHPBar.ShowHealth();
        }
    }

    public IEnumerator ChokeDeath(int damageAmount)
    {
        CurrentHealth -= damageAmount;
        enemyHPBar.UpdateHealthBar();
        enemyHPBar.ShowName();
        enemyHPBar.ShowHealth();
        yield return new WaitForSeconds(0.4f);
        if (CurrentHealth <= 0)
        {
            GetComponent<AudioSource>().Stop();
            EnemyAnimator.Play(DeathAnim.name, -1, 0);
            GetComponent<AudioSource>().PlayOneShot(DeathSFX);
            Invoke("Respawn", 3f);
        }
        yield return null;
    }

    public void GetSlammed(int damage)
    {
        if (CurrentHealth > 0)
        {
            GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.95f, 1.15f);
            EnemyAnimator.Play(SlammedAnim.name, -1, 0);
            GetComponent<AudioSource>().PlayOneShot(MediumDamageSFX);

            CurrentHealth -= damage;

            if (CurrentHealth <= 0) Die();

            enemyHPBar.UpdateHealthBar();
            enemyHPBar.ShowName();
            enemyHPBar.ShowHealth();
        }
    }

    public void Knockback(Vector3 direction)
    {
        EnemyRigidBody.AddForce(direction * 30f, ForceMode.Impulse);
    }

    public void DealDamage(AttackStrength strength)
    {
        if (CurrentHealth > 0)
        {
            GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.95f, 1.15f);
            switch (strength)
            {
                case AttackStrength.Light:
                    EnemyAnimator.Play(LightAttackAnim.name, -1, 0);
                    GetComponent<AudioSource>().PlayOneShot(LightDamageSFX);
                    break;

                case AttackStrength.Medium:
                    EnemyAnimator.Play(MediumAttackAnim.name, -1, 0);
                    GetComponent<AudioSource>().PlayOneShot(MediumDamageSFX);
                    break;

                case AttackStrength.Heavy:
                    EnemyAnimator.Play(HeavyAttackAnim.name, -1, 0);
                    GetComponent<AudioSource>().PlayOneShot(HeavyDamageSFX);
                    break;
            }
        }
    }

    public void Die()
    {
        GetComponent<AudioSource>().Stop();
        EnemyRigidBody.isKinematic = true;
        GetComponent<CapsuleCollider>().enabled = false;
        EnemyAnimator.Play(DeathAnim.name, -1, 0);
        GetComponent<AudioSource>().PlayOneShot(DeathSFX);
        Invoke("Respawn", 3f);
    }

    public void Respawn()
    {
        GameObject enemy = Instantiate(EnemyPrefab, transform.position, transform.rotation);
        enemy.GetComponent<Rigidbody>().isKinematic = false;

        Destroy(this.gameObject);
    }

}
