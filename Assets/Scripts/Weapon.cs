using UnityEngine;

public enum WeaponType { Melee, Ranged, Special, None }
public enum AttackStrength { Light, Medium, Heavy }
public abstract class Weapon : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public Sprite Crosshair_Default;
    public Sprite Crosshair_Enemy;
    public WeaponType Type;
    public GameObject Prefab;
    public PlayerControls.BasicActions Controls;
    public float AttackRange = 2.5f;

    [Header("Light Attack Properties")]
    public float LightAttackDelay = 0.5f;
    public float LightAttackCooldown = 1f;
    public int LightAttackDamage = 1;
    public GameObject LightHitEffect;

    [Header("Medium Attack Properties")]
    public float MediumAttackDelay = 0.5f;
    public float MediumAttackCooldown = 1f;
    public int MediumAttackDamage = 2;
    public GameObject MediumHitEffect;

    [Header("Heavy Attack Properties")]
    public float HeavyAttackDelay = 0.5f;
    public float HeavyAttackCooldown = 1f;
    public int HeavyAttackDamage = 3;
    public GameObject HeavyHitEffect;

    [Header("Animations")]
    public AnimationClip LightAttackAnim;
    public AnimationClip LightAttack2Anim;
    public AnimationClip MediumAttackAnim;
    public AnimationClip HeavyAttackAnim;

    public void InitializeControls(PlayerController player)
    {
        Controls = player.Controls;
        OnInitialize();
    }

    public virtual void OnInitialize() { return; }
    public abstract void PerformLightAttack(PlayerController player);
    public abstract void PerformMediumAttack(PlayerController player);
    public abstract void PerformHeavyAttack(PlayerController player);
    public abstract bool ExpendResource(AttackStrength strength);
}