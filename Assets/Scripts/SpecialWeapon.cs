using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Special Weapon", menuName = "Weapons/Special Weapon")]
public class SpecialWeapon : Weapon
{
    public float LightSpecialCDTimer = 0f;
    public float MediumSpecialCDTimer = 0f;
    public float HeavySpecialCDTimer = 0f;

    private void OnEnable()
    {
        LightSpecialCDTimer = 0f;
        MediumSpecialCDTimer = 0f;
        HeavySpecialCDTimer = 0f;
    }

    public override void PerformLightAttack(PlayerController player)
    {
        player.PlayAnimation(LightAttackAnim);
        player.StartCoroutine(player.CastSpecial(AttackStrength.Light, LightAttackDelay));
    }

    public override void PerformMediumAttack(PlayerController player)
    {
        player.PlayAnimation(MediumAttackAnim);
        player.StartCoroutine(player.CastSpecial(AttackStrength.Medium, MediumAttackDelay));
    }

    public override void PerformHeavyAttack(PlayerController player)
    {
        player.PlayAnimation(HeavyAttackAnim);
        player.StartCoroutine(player.CastSpecial(AttackStrength.Heavy, HeavyAttackDelay));
    }

    public override bool ExpendResource(AttackStrength strength)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateCooldowns()
    {
        if (LightSpecialCDTimer < LightAttackCooldown)
            LightSpecialCDTimer += Time.deltaTime;
        if (MediumSpecialCDTimer < MediumAttackCooldown)
            MediumSpecialCDTimer += Time.deltaTime;
        if (HeavySpecialCDTimer < HeavyAttackCooldown)
            HeavySpecialCDTimer += Time.deltaTime;
    }

    public void InitSpecials()
    {
        LightSpecialCDTimer = LightAttackCooldown;
        MediumSpecialCDTimer = MediumAttackCooldown;
        HeavySpecialCDTimer = HeavyAttackCooldown;
    }
}