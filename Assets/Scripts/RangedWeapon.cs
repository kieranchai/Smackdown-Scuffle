using UnityEngine;

[CreateAssetMenu(fileName = "New Ranged Weapon", menuName = "Weapons/Ranged Weapon")]
public class RangedWeapon : Weapon
{
    [Header("Ammo Properties")]
    public int ReloadAmount = 50;
    public int MaxAmmo = 50;
    public int CurrentAmmo = 50;
    public int LightAmmoCost = 1;
    public int MediumAmmoCost = 5;
    public int HeavyAmmoCost = 10;
    public AnimationClip ReloadAnim;
    public AudioClip ReloadSFX;
    public bool CurrentlyReloading;
    public float ReloadDuration = 1;

    private void OnEnable()
    {
        CurrentAmmo = MaxAmmo;
        CurrentlyReloading = false;
    }

    public override void OnInitialize()
    {
        FindObjectOfType<PlayerController>().Controls.Reload.performed += ctx => Reload();
        base.OnInitialize();
    }

    public override void PerformLightAttack(PlayerController player)
    {
        if(!CurrentlyReloading)
        {
            player.GetComponent<AudioSource>().PlayOneShot(LightAttackSFX);
            player.PlayAnimation(LightAttackAnim);
            player.StartCoroutine(player.TargetImpact(AttackStrength.Light, LightAttackDelay));
            player.StartCoroutine(player.ResetAttackAfterDelay(LightAttackCooldown));
        }
    }

    public override void PerformMediumAttack(PlayerController player)
    {
        if(!CurrentlyReloading)
        {
            player.GetComponent<AudioSource>().PlayOneShot(MediumAttackSFX);
            player.PlayAnimation(MediumAttackAnim);
            player.StartCoroutine(player.TargetImpact(AttackStrength.Medium, MediumAttackDelay));
            player.StartCoroutine(player.ResetAttackAfterDelay(MediumAttackCooldown));
        }
    }

    public override void PerformHeavyAttack(PlayerController player)
    {
        if(!CurrentlyReloading)
        {
            player.PlayAnimation(HeavyAttackAnim);
            player.GetComponent<AudioSource>().PlayOneShot(HeavyAttackSFX);
            player.StartCoroutine(player.TargetImpact(AttackStrength.Heavy, HeavyAttackDelay));
            player.StartCoroutine(player.ResetAttackAfterDelay(HeavyAttackCooldown));
        }
    }

    public override bool ExpendResource(AttackStrength strength)
    {
        switch (strength)
        {
            case AttackStrength.Light:
                if (CurrentAmmo < LightAmmoCost) return false;
                CurrentAmmo -= LightAmmoCost;
                break;
            case AttackStrength.Medium:
                if (CurrentAmmo < MediumAmmoCost) return false;
                CurrentAmmo -= MediumAmmoCost;
                break;
            case AttackStrength.Heavy:
                if (CurrentAmmo < HeavyAmmoCost) return false;
                CurrentAmmo -= HeavyAmmoCost;
                break;
        }
        return true;
    }

    public void Reload()
    {
        var player = FindObjectOfType<PlayerController>();
        CurrentlyReloading = true;
        player.PlayAnimation(ReloadAnim);
        player.GetComponent<AudioSource>().PlayOneShot(ReloadSFX);
        player.StartCoroutine(player.Reload(ReloadDuration, this));
    }
}