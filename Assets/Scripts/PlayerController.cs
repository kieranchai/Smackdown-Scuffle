using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed = 5f;
    public float Gravity = -9.81f;
    public float JumpStrength = 1.5f;
    public bool IsGrounded;
    public Vector3 Velocity;

    [Header("Combat")]
    public int MaxHealth = 10;
    public int CurrentHealth = 10;
    public bool IsAttacking;

    [Header("Weapons")]
    public Weapon EquippedWeapon;
    public List<Weapon> WeaponInventory;
    public GameObject MeleeAnchor;
    public GameObject RangedAnchor;
    public GameObject SpecialAnchor;

    [Header("Camera")]
    public GameObject CameraRig;
    public Camera PlayerCamera;
    public float Sensitivity;

    [Header("Animations")]
    public AnimationClip LightDamageAnim;
    public AnimationClip MediumDamageAnim;
    public AnimationClip HeavyDamageAnim;
    public AnimationClip DeathAnim;

    [Header("Sound Effects")]
    public AudioClip LightDamageSFX;
    public AudioClip MediumDamageSFX;
    public AudioClip HeavyDamageSFX;
    public AudioClip DeathSFX;

    [Header("Debug Properties")]
    public List<DebugWarpParams> WarpPoints = new List<DebugWarpParams>();
    public KeyCode HealKey;
    public int DebugHealAmount = 1;

    [HideInInspector]
    public PlayerControls.BasicActions Controls;
    private GameObject EquippedWeaponModel;
    private CharacterController CC;
    private PlayerControls PC;
    private AudioSource AS;
    private HUDManager HUD;
    private Animator PA;
    float xRotation;
    float _Gravity;

    private void Awake()
    {
        _Gravity = Gravity;
        CC = GetComponent<CharacterController>();
        PC = new PlayerControls();
        AS = GetComponent<AudioSource>();
        PA = GetComponentInChildren<Animator>();
        HUD = FindObjectOfType<HUDManager>();
        EquipWeapon(WeaponInventory[0]);
    }

    private void OnEnable()
    {
        PC.Enable();

        //Assign and reference our methods to our controls from the IM
        Controls = PC.Basic;
        Controls.Jump.performed += ctx => Jump();
        Controls.LightAttack.performed += ctx => PerformLightAttack();
        Controls.MediumAttack.performed += ctx => PerformMediumAttack();
        Controls.HeavyAttack.performed += ctx => PerformHeavyAttack();
        Controls.ToggleInventory.performed += ctx => ToggleInventory();
        Controls.EnemyLightAttack.performed += ctx => EnemyAttack(AttackStrength.Light);
        Controls.EnemyMediumAttack.performed += ctx => EnemyAttack(AttackStrength.Medium);
        Controls.EnemyHeavyAttack.performed += ctx => EnemyAttack(AttackStrength.Heavy);
    }

    private void OnDisable()
    {
        PC.Disable();
    }

    private void Update()
    {
        GroundedCheck();
        GetInventoryInput();
        HandleResources();
        GetDebugInputs();
    }


    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void LateUpdate()
    {
        CameraLook();
    }

    #region Input

    private void GetDebugInputs()
    {
        foreach (var entry in WarpPoints)
        {
            GameObject playerWarp = entry.playerWarpPoint;
            GameObject enemyWarp = entry.enemyWarpPoint;
            KeyCode keyCode = entry.warpKey;

            if (Input.GetKeyDown(keyCode))
            {
                var EC = FindObjectOfType<EnemyController>();

                CC.enabled = false;
                CC.transform.position = playerWarp.transform.position;
                CC.transform.forward = playerWarp.transform.forward;
                CC.enabled = true;

                EC.transform.position = enemyWarp.transform.position;
                EC.transform.transform.forward = enemyWarp.transform.forward;

                Debug.Log("Warped to " + playerWarp.gameObject.name);

                return;
            }
        }

        if (Input.GetKeyDown(HealKey))
        {
            HealPlayer(DebugHealAmount);
        }
    }

    public int GetPressedNumber()
    {
        //Helper function-- if the keycode's ToString returns a number, return it.
        for (int number = 0; number <= 9; number++)
        {
            if (Input.GetKeyDown(number.ToString()))
                return number;
        }
        return -1;
    }

    private void GetInventoryInput()
    {
        //If the player presses a number, attempt to equip a weapon at that inventory slot
        if (GetPressedNumber() > -1)
            try { EquipWeapon(WeaponInventory[GetPressedNumber() - 1]); }
            catch { Debug.Log("No weapon at index " + (GetPressedNumber() - 1).ToString()); return; }
    }

    public void ToggleInventory()
    {
        if (FindObjectOfType<InventoryManager>().GetComponent<CanvasGroup>().alpha == 1)
            FindObjectOfType<InventoryManager>().GetComponent<CanvasGroup>().alpha = 0;
        else
            FindObjectOfType<InventoryManager>().GetComponent<CanvasGroup>().alpha = 1;
    }

    #endregion

    #region Movement

    private void CameraLook()
    {
        //Grab the movement values from our Control's mouseLook config
        float mouseX = Controls.Look.ReadValue<Vector2>().x;
        float mouseY = Controls.Look.ReadValue<Vector2>().y;

        //Store our vertical rotation movement into a value to apply to transform
        xRotation -= (mouseY * Time.deltaTime) * Sensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 85f);

        //Apply our transforms to both our camera (vertical) and our player (horizontal)
        CameraRig.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * Sensitivity);
    }

    private void MovePlayer()
    {
        //Grab our WASD input as a vector2 and normalize it with our transform
        Vector2 controlAxis = Controls.Movement.ReadValue<Vector2>();
        Vector3 inputDir = new Vector3(controlAxis.x, 0, controlAxis.y);
        Vector3 inputVelocity = transform.TransformDirection(inputDir.normalized);

        //Add velocity each frame
        Velocity.y += Gravity * Time.deltaTime;

        //If we're grounded, apply a constant downwards force of baseGravity / 2 for slopes.
        if (IsGrounded && Velocity.y < 0)
            Velocity.y = _Gravity / 2;

        //Apply our movements
        CC.Move((inputVelocity * MoveSpeed) * Time.deltaTime + //PlayerInput Movement
                 Velocity * Time.deltaTime); //Additional Velocities (Jump/Gravity)
    }
    private void GroundedCheck()
    {
        IsGrounded = CC.isGrounded;
    }

    private void Jump()
    {
        if (IsGrounded)
            Velocity.y = JumpStrength;
    }

    #endregion

    #region Combat

    public void EquipWeapon(Weapon weapon)
    {
        if (EquippedWeaponModel != null)
            Destroy(EquippedWeaponModel);

        EquippedWeapon = weapon;

        if (!WeaponInventory.Contains(weapon))
            WeaponInventory.Add(weapon);

        switch (weapon.Type)
        {
            case WeaponType.Melee:
                EquippedWeaponModel = Instantiate(weapon.Prefab, MeleeAnchor.transform);
                break;
            case WeaponType.Ranged:
                EquippedWeaponModel = Instantiate(weapon.Prefab, RangedAnchor.transform);
                break;
            case WeaponType.Special:
                EquippedWeaponModel = Instantiate(weapon.Prefab, SpecialAnchor.transform);
                break;
        }

        EquippedWeaponModel.transform.localScale = Vector3.one;
        EquippedWeaponModel.transform.localPosition = Vector3.zero;
        EquippedWeaponModel.transform.localRotation = Quaternion.identity;

        HUD.SwitchHUD(weapon.Type);

        if (weapon.Crosshair != null)
        {
            HUD.Crosshair.enabled = true;
            HUD.Crosshair.sprite = weapon.Crosshair;
        }
        else
        {
            HUD.Crosshair.enabled = false;
            HUD.Crosshair.sprite = null;
        }

        FindObjectOfType<InventoryManager>().InitializeInventory();
        EquippedWeapon.InitializeControls(this);
    }

    private void HandleResources()
    {
        if (EquippedWeapon != null)
        {
            switch (EquippedWeapon.Type)
            {
                case WeaponType.Melee:
                    ((MeleeWeapon)EquippedWeapon).RegenerateStamina();

                    break;
                case WeaponType.Special:
                    ((SpecialWeapon)EquippedWeapon).UpdateCooldowns();
                    break;
            }
        }
        HUD.UpdateResources(this);
    }

    public void PerformLightAttack()
    {
        if (IsAttacking || EquippedWeapon == null) return;

        if (EquippedWeapon.ExpendResource(AttackStrength.Light))
        {
            IsAttacking = true;
            EquippedWeapon.PerformLightAttack(this);

            if (EquippedWeapon.Type != WeaponType.Special)
                StartCoroutine(ResetAttackAfterDelay(EquippedWeapon.LightAttackCooldown));
            else
                IsAttacking = false;
        }
    }

    public void PerformMediumAttack()
    {
        if (IsAttacking || EquippedWeapon == null) return;

        if (EquippedWeapon.ExpendResource(AttackStrength.Medium))
        {
            IsAttacking = true;
            EquippedWeapon.PerformMediumAttack(this);

            if (EquippedWeapon.Type != WeaponType.Special)
                StartCoroutine(ResetAttackAfterDelay(EquippedWeapon.MediumAttackCooldown));
            else
                IsAttacking = false;
        }
    }

    public void PerformHeavyAttack()
    {
        if (IsAttacking || EquippedWeapon == null) return;

        if (EquippedWeapon.ExpendResource(AttackStrength.Heavy))
        {
            IsAttacking = true;
            EquippedWeapon.PerformHeavyAttack(this);

            if (EquippedWeapon.Type != WeaponType.Special)
                StartCoroutine(ResetAttackAfterDelay(EquippedWeapon.HeavyAttackCooldown));
            else
                IsAttacking = false;
        }
    }

    public System.Collections.IEnumerator TargetImpact(AttackStrength strength, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out RaycastHit hit, EquippedWeapon.AttackRange))
        {
            EnemyController enemy = hit.collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                switch (strength)
                {
                    case AttackStrength.Light:
                        enemy.TakeDamage(EquippedWeapon.LightAttackDamage, strength);
                        break;
                    case AttackStrength.Medium:
                        enemy.TakeDamage(EquippedWeapon.MediumAttackDamage, strength);
                        break;
                    case AttackStrength.Heavy:
                        enemy.TakeDamage(EquippedWeapon.HeavyAttackDamage, strength);
                        break;
                }
            }
            switch (strength)
            {
                case AttackStrength.Light:
                    if (EquippedWeapon.LightImpactSFX != null)
                        AS.PlayOneShot(EquippedWeapon.LightImpactSFX);
                    if (EquippedWeapon.LightHitEffect != null)
                        Instantiate(EquippedWeapon.LightHitEffect, hit.point, Quaternion.identity);
                    break;
                case AttackStrength.Medium:
                    if (EquippedWeapon.MediumImpactSFX != null)
                        AS.PlayOneShot(EquippedWeapon.MediumImpactSFX);
                    if (EquippedWeapon.MediumHitEffect != null)
                        Instantiate(EquippedWeapon.MediumHitEffect, hit.point, Quaternion.identity);
                    break;
                case AttackStrength.Heavy:
                    if (EquippedWeapon.HeavyImpactSFX != null)
                        AS.PlayOneShot(EquippedWeapon.HeavyImpactSFX);
                    if (EquippedWeapon.HeavyHitEffect != null)
                        Instantiate(EquippedWeapon.HeavyHitEffect, hit.point, Quaternion.identity);
                    break;
            }
        }
    }

    public System.Collections.IEnumerator CastSpecial(AttackStrength strength, GameObject attackPrefab, float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Casting Special");
        var Special = Instantiate(attackPrefab, SpecialAnchor.transform.position, Quaternion.identity);
        Special.GetComponent<SpecialProjectile>().Strength = strength;
        switch (strength)
        {
            case AttackStrength.Light:
                Special.GetComponent<SpecialProjectile>().Damage = EquippedWeapon.LightAttackDamage;
                Special.GetComponent<SpecialProjectile>().LifeSpan = EquippedWeapon.LightAttackCooldown;
                break;

            case AttackStrength.Medium:
                Special.GetComponent<SpecialProjectile>().Damage = EquippedWeapon.MediumAttackDamage;
                Special.GetComponent<SpecialProjectile>().LifeSpan = EquippedWeapon.MediumAttackCooldown;
                break;

            case AttackStrength.Heavy:
                Special.GetComponent<SpecialProjectile>().Damage = EquippedWeapon.HeavyAttackDamage;
                Special.GetComponent<SpecialProjectile>().LifeSpan = EquippedWeapon.HeavyAttackCooldown;
                break;
        }
    }

    public void PlayAnimation(AnimationClip clip)
    {
        if (EquippedWeaponModel != null && clip != null)
            EquippedWeaponModel.GetComponent<Animator>().Play(clip.name);
    }

    public System.Collections.IEnumerator ResetAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        IsAttacking = false;
    }

    private void EnemyAttack(AttackStrength strength)
    {
        if (FindObjectOfType<EnemyController>() != null)
        {
            var EC = FindObjectOfType<EnemyController>();

            switch (strength)
            {
                case AttackStrength.Light:
                    EC.EnemyAnimator.Play(EC.LightAttackAnim.name);
                    TakeDamage(strength, EC.LightAttackDamage);
                    break;

                case AttackStrength.Medium:
                    EC.EnemyAnimator.Play(EC.MediumAttackAnim.name);
                    TakeDamage(strength, EC.MediumAttackDamage);
                    break;

                case AttackStrength.Heavy:
                    EC.EnemyAnimator.Play(EC.HeavyAttackAnim.name);
                    TakeDamage(strength, EC.HeavyAttackDamage);
                    break;
            }
        }
    }

    public void TakeDamage(AttackStrength strength, int damage)
    {
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        switch (strength)
        {
            case AttackStrength.Light:
                AS.PlayOneShot(LightDamageSFX);
                PA.Play(LightDamageAnim.name);
                break;

            case AttackStrength.Medium:
                AS.PlayOneShot(MediumDamageSFX);
                PA.Play(MediumDamageAnim.name);
                break;

            case AttackStrength.Heavy:
                AS.PlayOneShot(HeavyDamageSFX);
                PA.Play(HeavyDamageAnim.name);
                break;
        }

        HUD.UpdateHPBars(); 

        if (CurrentHealth == 0)
            Die();
    }

    internal System.Collections.IEnumerator Reload(float reloadDuration, RangedWeapon wep)
    {
        yield return new WaitForSeconds(reloadDuration);
        wep.CurrentlyReloading = false;
        wep.CurrentAmmo = Mathf.Min(wep.CurrentAmmo + wep.ReloadAmount, wep.MaxAmmo);
    }

    private void HealPlayer(int healAmount)
    {
        CurrentHealth += healAmount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        HUD.UpdateHPBars();
    }

    private void Die()
    {
        this.enabled = false;
        AS.PlayOneShot(DeathSFX);
        PA.Play(DeathAnim.name);
        Invoke("ResetScene", 2f);
    }

    #endregion

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}

[System.Serializable]
public class DebugWarpParams
{
    public GameObject playerWarpPoint;
    public GameObject enemyWarpPoint;
    public KeyCode warpKey;
}