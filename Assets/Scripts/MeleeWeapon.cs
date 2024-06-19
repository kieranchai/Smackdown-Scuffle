using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Weapon", menuName = "Weapons/Melee Weapon")]
public class MeleeWeapon : Weapon
{
    [Header("Stamina Properties")]
    public float LightStaminaCost = 10f;
    public float MediumStaminaCost = 30f;
    public float HeavyStaminaCost = 50f;

    public override void PerformLightAttack(PlayerController player)
    {
        player.GetComponent<AudioSource>().PlayOneShot(LightAttackSFX);
        if (Type == WeaponType.Melee)
        {
            if (Random.Range(0, 2) == 0) player.PlayAnimation(LightAttackAnim);
            else player.PlayAnimation(LightAttack2Anim);
        } else
        {
            player.PlayAnimation(LightAttackAnim);
        }
        player.InitialiseChairScript(AttackStrength.Light, LightAttackDamage);
        player.StartCoroutine(player.ResetAttackAfterDelay(LightAttackCooldown));
    }

    public override void PerformMediumAttack(PlayerController player)
    {
        player.GetComponent<AudioSource>().PlayOneShot(MediumAttackSFX);
        player.PlayAnimation(MediumAttackAnim);
        player.InitialiseChairScript(AttackStrength.Medium, MediumAttackDamage);
        player.StartCoroutine(player.TargetImpact(AttackStrength.Medium, MediumAttackDelay));
        player.StartCoroutine(player.ResetAttackAfterDelay(MediumAttackCooldown));
    }

    public override void PerformHeavyAttack(PlayerController player)
    {
        player.PlayAnimation(HeavyAttackAnim);
        player.GetComponent<AudioSource>().PlayOneShot(HeavyAttackSFX);
        player.StartCoroutine(player.TargetImpact(AttackStrength.Heavy, HeavyAttackDelay));
        player.StartCoroutine(player.ResetAttackAfterDelay(HeavyAttackCooldown));
    }

    public override bool ExpendResource(AttackStrength strength)
    {
        throw new System.NotImplementedException();
    }
}