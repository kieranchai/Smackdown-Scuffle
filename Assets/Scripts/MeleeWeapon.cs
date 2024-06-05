using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Weapon", menuName = "Weapons/Melee Weapon")]
public class MeleeWeapon : Weapon
{
    [Header("Stamina Properties")]
    public float StaminaRegenRate = 1f;
    public float MaxStamina = 100;
    public float CurrentStamina = 100;
    public float LightStaminaCost = 10f;
    public float MediumStaminaCost = 30f;
    public float HeavyStaminaCost = 50f;

    private void OnEnable()
    {
        CurrentStamina = MaxStamina;
    }

    public override void PerformLightAttack(PlayerController player)
    {
        player.GetComponent<AudioSource>().PlayOneShot(LightAttackSFX);
        player.PlayAnimation(LightAttackAnim);
        player.InitialiseChairScript(AttackStrength.Light, LightAttackDamage);
        player.StartCoroutine(player.TargetImpact(AttackStrength.Light, LightAttackDelay));
        player.StartCoroutine(player.ResetAttackAfterDelay(LightAttackCooldown));
    }

    public override void PerformMediumAttack(PlayerController player)
    {
        player.GetComponent<AudioSource>().PlayOneShot(MediumAttackSFX);
        player.PlayAnimation(MediumAttackAnim);
        player.InitialiseChairScript(AttackStrength.Medium, MediumAttackDamage);
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
        switch (strength)
        {
            case AttackStrength.Light:
                if (CurrentStamina < LightStaminaCost) return false;
                CurrentStamina -= LightStaminaCost;
                break;
            case AttackStrength.Medium:
                if (CurrentStamina < MediumStaminaCost) return false;
                CurrentStamina -= MediumStaminaCost;
                break;
            case AttackStrength.Heavy:
                if (CurrentStamina < HeavyStaminaCost) return false;
                CurrentStamina -= HeavyStaminaCost;
                break;
        }
        return true;
    }

    public void RegenerateStamina()
    {
        if (CurrentStamina < MaxStamina)
            CurrentStamina += StaminaRegenRate * Time.deltaTime;
    }
}