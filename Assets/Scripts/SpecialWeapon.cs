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
        player.GetComponent<AudioSource>().PlayOneShot(LightAttackSFX);
        player.PlayAnimation(LightAttackAnim);
        player.StartCoroutine(player.CastSpecial(AttackStrength.Light, LightAttackDelay));
    }

    public override void PerformMediumAttack(PlayerController player)
    {
        player.GetComponent<AudioSource>().PlayOneShot(MediumAttackSFX);
        player.PlayAnimation(MediumAttackAnim);
        player.StartCoroutine(player.CastSpecial(AttackStrength.Medium, MediumAttackDelay));
    }

    public override void PerformHeavyAttack(PlayerController player)
    {
        player.PlayAnimation(HeavyAttackAnim);
        player.GetComponent<AudioSource>().PlayOneShot(HeavyAttackSFX);
        player.StartCoroutine(player.CastSpecial(AttackStrength.Heavy, HeavyAttackDelay));
    }

    public override bool ExpendResource(AttackStrength strength)
    {
        switch (strength)
        {
            case AttackStrength.Light:
                if (LightSpecialCDTimer < LightAttackCooldown) return false;
                LightSpecialCDTimer = 0;
                break;
            case AttackStrength.Medium:
                if (MediumSpecialCDTimer < MediumAttackCooldown) return false;
                MediumSpecialCDTimer = 0;
                break;
            case AttackStrength.Heavy:
                if (HeavySpecialCDTimer < HeavyAttackCooldown) return false;
                HeavySpecialCDTimer = 0;
                break;
        }
        return true;
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