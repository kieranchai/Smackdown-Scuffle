using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Ranged Weapon", menuName = "Weapons/Ranged Weapon")]
public class RangedWeapon : Weapon
{
    [Header("Ammo Properties")]
    public int ReloadAmount = 50;
    public int MaxAmmo = 50;
    public int CurrentAmmo = 50;
    public int LightAmmoCost = 1;
    public int MediumAmmoCost = 5;
    public AnimationClip ReloadAnim;
    public AudioClip ReloadSFX;
    public bool CurrentlyReloading;
    public float ReloadDuration = 3f;

    private Coroutine reloadCoroutine;

    private void OnEnable()
    {
        CurrentAmmo = MaxAmmo;
        CurrentlyReloading = false;
    }

    public override void OnInitialize()
    {
        FindObjectOfType<PlayerController>().Controls.Reload.performed += ctx => Reload();
        FindObjectOfType<PlayerController>().RemoveHUDAmmoCircles();
        FindObjectOfType<PlayerController>().UpdateHUDAmmoCircles(CurrentAmmo, MaxAmmo);
        base.OnInitialize();
    }

    public override void PerformLightAttack(PlayerController player)
    {
        StopReload();
        player.GetComponent<AudioSource>().PlayOneShot(LightAttackSFX);
        player.PlayAnimation(LightAttackAnim);
        player.InitialiseGarbageScript();
        player.StartCoroutine(player.ResetAttackAfterDelay(LightAttackCooldown));
    }

    public override void PerformMediumAttack(PlayerController player)
    {
        StopReload();
        player.GetComponent<AudioSource>().PlayOneShot(MediumAttackSFX);
        player.PlayAnimation(MediumAttackAnim);
        player.InitialiseGarbageScript();
        player.StartCoroutine(player.ResetAttackAfterDelay(MediumAttackCooldown));
    }

    public override void PerformHeavyAttack(PlayerController player)
    {
        StopReload();
        player.StartCoroutine(HeavyAttack(player));
    }

    IEnumerator HeavyAttack(PlayerController player)
    {
        player.PlayAnimation(HeavyAttackAnim);
        player.GetComponent<AudioSource>().PlayOneShot(HeavyAttackSFX);
        player.InitialiseGarbageScript();
        yield return new WaitForSeconds(2f);
        CurrentlyReloading = true;
        player.StartCoroutine(player.Reload(1.25f, this, MaxAmmo));
        player.StartCoroutine(player.ResetAttackAfterDelay(1.25f));
    }

    public override bool ExpendResource(AttackStrength strength)
    {
        switch (strength)
        {
            case AttackStrength.Light:
                if (CurrentAmmo < LightAmmoCost) return false;
                CurrentAmmo -= LightAmmoCost;
                FindObjectOfType<PlayerController>().RemoveHUDAmmoCircles();
                FindObjectOfType<PlayerController>().UpdateHUDAmmoCircles(CurrentAmmo, MaxAmmo);
                break;
            case AttackStrength.Medium:
                if (CurrentAmmo < MediumAmmoCost) return false;
                CurrentAmmo -= MediumAmmoCost;
                FindObjectOfType<PlayerController>().RemoveHUDAmmoCircles();
                FindObjectOfType<PlayerController>().UpdateHUDAmmoCircles(CurrentAmmo, MaxAmmo);
                break;
            case AttackStrength.Heavy:
                if (CurrentAmmo < LightAmmoCost) return false;
                CurrentAmmo -= CurrentAmmo;
                FindObjectOfType<PlayerController>().RemoveHUDAmmoCircles();
                FindObjectOfType<PlayerController>().UpdateHUDAmmoCircles(CurrentAmmo, MaxAmmo);
                break;
        }
        return true;
    }

    public void Reload()
    {
        if (CurrentAmmo == MaxAmmo) return;

        var player = FindObjectOfType<PlayerController>();
        CurrentlyReloading = true;
        player.PlayAnimation(ReloadAnim);
        reloadCoroutine = player.StartCoroutine(player.Reload(1, this, 1, ReloadSFX));
    }

    public void StopReload()
    {
        if (!CurrentlyReloading) return;

        var player = FindObjectOfType<PlayerController>();
        player.StopCoroutine(reloadCoroutine);
        CurrentlyReloading = false;
    }
}