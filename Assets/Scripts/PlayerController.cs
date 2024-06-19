using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed = 5f;
    public float Gravity = -9.81f;
    public float JumpStrength = 1.5f;
    public bool IsGrounded;
    public bool isJumping;
    private Vector3 inputVelocity;
    public Vector3 Velocity;
    public bool canMove = true;
    public bool canJump = true;
    public bool canTurn = true;

    [Header("Combat")]
    public int MaxHealth = 10;
    public int CurrentHealth = 10;
    public bool IsAttacking;
    public bool isUsingSpecial = false;
    public bool hitObstacle = false;
    public GameObject tagTeamerPrefab;
    public Transform garbageCanSpawnLocation;
    public Transform garbageSpawnLocation;
    public Transform multiGarbageSpawnLocation;
    public float StaminaRegenRate = 5f;
    public float MaxStamina = 100;
    public float CurrentStamina = 100;
    public ParticleSystem chargeParticles;

    [Header("Weapons")]
    public Weapon EquippedWeapon;
    public List<Weapon> WeaponInventory;
    public GameObject MeleeAnchor;
    public GameObject RangedAnchor;
    public GameObject SpecialAnchor;

    [Header("Camera")]
    public GameObject CameraRig;
    public Camera PlayerCamera;
    public GameObject PlayerHead;
    public Camera DeathCamera;
    public float Sensitivity;

    [Header("UI Refs")]
    public GameObject ammoCircle;
    public GameObject light_comicVFX;
    public GameObject heavy_comicVFX;
    public GameObject medium_comicVFX;
    public GameObject ranged_heavy_comicVFX;
    public GameObject ranged_light_comicVFX;
    public ParticleSystem speedLines;

    [Header("Animations")]
    public AnimationClip LightDamageAnim;
    public AnimationClip MediumDamageAnim;
    public AnimationClip HeavyDamageAnim;
    public AnimationClip DeathAnim;
    public AnimationClip RangedIdleAnim;

    [Header("Sound Effects")]
    public AudioClip LightDamageSFX;
    public AudioClip MediumDamageSFX;
    public AudioClip HeavyDamageSFX;
    public AudioClip DeathSFX;

    [Header("Debug Properties")]
    public KeyCode HealKey;
    public int DebugHealAmount = 1;

    [HideInInspector]
    public PlayerControls.BasicActions Controls;
    public GameObject EquippedWeaponModel;
    public CharacterController CC;
    private PlayerControls PC;
    public AudioSource AS;
    public HUDManager HUD;
    private Animator PA;
    float xRotation;
    float _Gravity;
    bool initialSpecialEquip = true;

    private void Awake()
    {
        _Gravity = Gravity;
        CC = GetComponent<CharacterController>();
        PC = new PlayerControls();
        AS = GetComponent<AudioSource>();
        PA = GetComponentInChildren<Animator>();
        HUD = FindObjectOfType<HUDManager>();
        EquipWeapon(WeaponInventory[0]);
        CurrentStamina = MaxStamina;
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
        RegenerateStamina();
        HandleResources();
        GetDebugInputs();

        // UI
        CheckIfXHairOverEnemy();
        HUD.UpdateHPBars(this);
    }


    private void FixedUpdate()
    {
        MovePlayer();
        if (IsGrounded) isJumping = false;
    }

    private void LateUpdate()
    {
        CameraLook();
    }

    #region Input

    private void GetDebugInputs()
    {
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

    #endregion

    #region Movement

    private void CameraLook()
    {
        if (!canTurn) return;
        if (DeathCamera.enabled) return;

        //Grab the movement values from our Control's mouseLook config
        float mouseX = Controls.Look.ReadValue<Vector2>().x;
        float mouseY = Controls.Look.ReadValue<Vector2>().y;

        //Store our vertical rotation movement into a value to apply to transform
        xRotation -= (mouseY * Time.deltaTime) * Sensitivity;

        /*        if (TPCamera.enabled) xRotation = Mathf.Clamp(xRotation, -40f, 40f);
                else */
        xRotation = Mathf.Clamp(xRotation, -80f, 85f);

        //Apply our transforms to both our camera (vertical) and our player (horizontal)
        CameraRig.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        /*        TPCameraRig.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);*/
        PlayerHead.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * Sensitivity);
    }

    private void MovePlayer()
    {
        if (!canMove) return;

        //Grab our WASD input as a vector2 and normalize it with our transform
        Vector2 controlAxis = Controls.Movement.ReadValue<Vector2>();
        Vector3 inputDir = new Vector3(controlAxis.x, 0, controlAxis.y);
        inputVelocity = transform.TransformDirection(inputDir.normalized);

        //Add velocity each frame
        Velocity.y += Gravity * Time.deltaTime;
        if (IsGrounded)
        {
            if (Velocity.y < 0) Velocity.y = _Gravity / 2;
        }

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
        if (!canMove) return;
        if (!canJump) return;

        if (IsGrounded)
        {
            isJumping = true;
            Velocity.y = JumpStrength;
        }
    }

    #endregion

    #region Combat

    public void EquipWeapon(Weapon weapon)
    {
        if (isUsingSpecial) return;

        if (EquippedWeaponModel != null)
            Destroy(EquippedWeaponModel);

        EquippedWeapon = weapon;

        if (!WeaponInventory.Contains(weapon))
            WeaponInventory.Add(weapon);

        switch (weapon.Type)
        {
            case WeaponType.Melee:
                RemoveHUDAmmoCircles();
                EquippedWeaponModel = Instantiate(weapon.Prefab, MeleeAnchor.transform);
                break;
            case WeaponType.Ranged:
                EquippedWeaponModel = Instantiate(weapon.Prefab, RangedAnchor.transform);
                break;
            case WeaponType.Special:
                RemoveHUDAmmoCircles();
                EquippedWeaponModel = Instantiate(weapon.Prefab, SpecialAnchor.transform);
                break;
        }

        HUD.SwitchHUD(weapon.Type);

        if (weapon.Crosshair_Default != null)
        {
            HUD.Crosshair.enabled = true;
            HUD.Crosshair.sprite = weapon.Crosshair_Default;
        }
        else
        {
            HUD.Crosshair.enabled = false;
            HUD.Crosshair.sprite = null;
        }

        FindObjectOfType<InventoryManager>().InitializeInventory();
        EquippedWeapon.InitializeControls(this);
    }

    public void CheckIfXHairOverEnemy()
    {
        if (EquippedWeapon.Crosshair_Default == null) return;

        RaycastHit hit;
        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hit, 50f, LayerMask.GetMask("Enemy")))
        {
            HUD.Crosshair.sprite = EquippedWeapon.Crosshair_Enemy;
            hit.transform.gameObject.GetComponent<EnemyController>().enemyHPBar.ShowName();
        }
        else
        {
            HUD.Crosshair.sprite = EquippedWeapon.Crosshair_Default;
        }
    }

    private void HandleResources()
    {
        if (EquippedWeapon != null)
        {
            switch (EquippedWeapon.Type)
            {
                case WeaponType.Special:
                    if (initialSpecialEquip)
                    {
                        initialSpecialEquip = false;
                        ((SpecialWeapon)EquippedWeapon).InitSpecials();
                    }
                    else
                    {
                        ((SpecialWeapon)EquippedWeapon).UpdateCooldowns();
                    }
                    break;
            }
        }
        HUD.UpdateResources(this);
        HUD.UpdateStaminaBar(this);
    }

    public void RegenerateStamina()
    {
        if (CurrentStamina < MaxStamina)
        {
            CurrentStamina += StaminaRegenRate * Time.deltaTime;
        }
    }

    public void RemoveHUDAmmoCircles()
    {
        if (HUD.Crosshair.transform.Find("Ammo Counter").childCount > 0)
        {
            for (int i = 0; i < HUD.Crosshair.transform.Find("Ammo Counter").childCount; i++)
            {
                Destroy(HUD.Crosshair.transform.Find("Ammo Counter").GetChild(i).gameObject);
            }
        }
    }

    public void UpdateHUDAmmoCircles(int currentAmmo, int maxAmmo)
    {
        for (int i = 1; i <= maxAmmo / 2; i++)
        {
            GameObject ammoCircleUI = Instantiate(ammoCircle, HUD.Crosshair.transform.Find("Ammo Counter"));

            if (i * 2 > currentAmmo) ammoCircleUI.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
        }
    }

    public bool ExpendResource(AttackStrength strength, WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Melee:
                MeleeWeapon meleeWeapon = (MeleeWeapon)EquippedWeapon;
                switch (strength)
                {
                    case AttackStrength.Light:
                        if (CurrentStamina < meleeWeapon.LightStaminaCost) return false;
                        CurrentStamina -= meleeWeapon.LightStaminaCost;
                        break;
                    case AttackStrength.Medium:
                        if (CurrentStamina < meleeWeapon.MediumStaminaCost) return false;
                        CurrentStamina -= meleeWeapon.MediumStaminaCost;
                        break;
                    case AttackStrength.Heavy:
                        if (CurrentStamina < meleeWeapon.HeavyStaminaCost) return false;
                        CurrentStamina -= meleeWeapon.HeavyStaminaCost;
                        break;
                }
                HUD.staminaLerpTimer = 0f;
                break;
            case WeaponType.Ranged:
                RangedWeapon rangedWeapon = (RangedWeapon)EquippedWeapon;
                switch (strength)
                {
                    case AttackStrength.Light:
                        if (rangedWeapon.CurrentAmmo < rangedWeapon.LightAmmoCost) return false;
                        rangedWeapon.CurrentAmmo -= rangedWeapon.LightAmmoCost;
                        RemoveHUDAmmoCircles();
                        UpdateHUDAmmoCircles(rangedWeapon.CurrentAmmo, rangedWeapon.MaxAmmo);
                        break;
                    case AttackStrength.Medium:
                        if (rangedWeapon.CurrentAmmo < rangedWeapon.MediumAmmoCost) return false;
                        rangedWeapon.CurrentAmmo -= rangedWeapon.MediumAmmoCost;
                        RemoveHUDAmmoCircles();
                        UpdateHUDAmmoCircles(rangedWeapon.CurrentAmmo, rangedWeapon.MaxAmmo);
                        break;
                    case AttackStrength.Heavy:
                        if (rangedWeapon.CurrentAmmo < rangedWeapon.LightAmmoCost) return false;
                        rangedWeapon.CurrentAmmo -= rangedWeapon.CurrentAmmo;
                        RemoveHUDAmmoCircles();
                        UpdateHUDAmmoCircles(rangedWeapon.CurrentAmmo, rangedWeapon.MaxAmmo);
                        break;
                }
                FindObjectOfType<InventoryManager>().UpdateAmmoCount();
                break;
            case WeaponType.Special:
                SpecialWeapon specialWeapon = (SpecialWeapon)EquippedWeapon;
                switch (strength)
                {
                    case AttackStrength.Light:
                        if (specialWeapon.LightSpecialCDTimer < specialWeapon.LightAttackCooldown) return false;
                        specialWeapon.LightSpecialCDTimer = 0;
                        break;
                    case AttackStrength.Medium:
                        if (specialWeapon.MediumSpecialCDTimer < specialWeapon.MediumAttackCooldown) return false;
                        specialWeapon.MediumSpecialCDTimer = 0;
                        break;
                    case AttackStrength.Heavy:
                        if (specialWeapon.HeavySpecialCDTimer < specialWeapon.HeavyAttackCooldown) return false;
                        specialWeapon.HeavySpecialCDTimer = 0;
                        break;
                }
                break;
        }


        return true;
    }

    public void PerformLightAttack()
    {
        if (isUsingSpecial || IsAttacking || EquippedWeapon == null) return;

        if (EquippedWeapon.Type == WeaponType.Special && !IsGrounded) return;

        if (ExpendResource(AttackStrength.Light, EquippedWeapon.Type))
        {
            if (EquippedWeapon.Type == WeaponType.Special) isUsingSpecial = true;
            if (EquippedWeapon.Type == WeaponType.Melee) EquippedWeaponModel.transform.Find("Folding Chair").GetComponent<ChairEvents>().hasHit = false;

            IsAttacking = true;
            EquippedWeapon.PerformLightAttack(this);

            if (EquippedWeapon.Type != WeaponType.Special) StartCoroutine(ResetAttackAfterDelay(EquippedWeapon.LightAttackCooldown));
            else IsAttacking = false;
        }
    }

    public void PerformMediumAttack()
    {
        if (isUsingSpecial || IsAttacking || EquippedWeapon == null) return;

        if (ExpendResource(AttackStrength.Medium, EquippedWeapon.Type))
        {
            if (EquippedWeapon.Type == WeaponType.Special) isUsingSpecial = true;

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
        if (isUsingSpecial || IsAttacking || EquippedWeapon == null) return;

        if (ExpendResource(AttackStrength.Heavy, EquippedWeapon.Type))
        {
            if (EquippedWeapon.Type == WeaponType.Special) isUsingSpecial = true;
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
        yield return new WaitForSecondsRealtime(delay);
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
                        if (EquippedWeapon.Type == WeaponType.Melee)
                        {
                            Instantiate(medium_comicVFX, hit.point, Quaternion.identity);
                        }
                        enemy.Knockback(transform.forward, 30f);
                        enemy.TakeDamage(EquippedWeapon.MediumAttackDamage, strength);
                        break;
                    case AttackStrength.Heavy:
                        if (EquippedWeapon.Type == WeaponType.Melee)
                        {
                            Instantiate(heavy_comicVFX, hit.point + new Vector3(0, 0, 0.2f), Quaternion.identity);
                        }
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

    public System.Collections.IEnumerator CastSpecial(AttackStrength strength, float delay)
    {
        yield return new WaitForSeconds(delay);
        switch (strength)
        {
            case AttackStrength.Light:
                StartCoroutine(ForwardCharge());
                break;
            case AttackStrength.Medium:
                StartCoroutine(TagTeam(EquippedWeapon.MediumAttackDamage));
                break;
            case AttackStrength.Heavy:
                StartCoroutine(Choke());
                break;
        }
    }

    public bool GetShootHitPos(out RaycastHit hit)
    {
        return Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hit);
    }

    public System.Collections.IEnumerator TagTeam(int damage)
    {
        RaycastHit hit;
        if (GetShootHitPos(out hit))
        {
            GameObject tagTeamer = Instantiate(tagTeamerPrefab, hit.point + new Vector3(0, 10f, 0f), transform.rotation);
            tagTeamer.GetComponent<TagTeamer>().damage = damage;
        }

        EndSpecialAttack();
        yield return null;
    }

    public System.Collections.IEnumerator ForwardCharge()
    {
        canMove = false;
        canTurn = false;
        Velocity = Vector3.zero;
        float chargeTime = 0f;
        chargeParticles.Play();
        speedLines.Play();
        while (!hitObstacle && chargeTime < 0.5f)
        {
            PlayerCamera.fieldOfView -= 20f * Time.deltaTime;
            CC.Move(transform.forward * 18f * Time.deltaTime);
            CheckSpecialHit(AttackStrength.Light);
            chargeTime += Time.deltaTime;
            yield return null;
        }
        chargeParticles.Stop();
        speedLines.Stop();
        PlayerCamera.fieldOfView = 75f;
        EquippedWeaponModel.GetComponent<Animator>().Play("End Light");
        EndSpecialAttack();
    }

    public System.Collections.IEnumerator Choke()
    {
        if (CheckChokeLand())
        {
            EnemyController enemy = CheckChokeLand();
            enemy.gameObject.transform.SetParent(EquippedWeaponModel.transform.Find("Basiccharacter")
                .Find("clavicle_r").Find("upperarm_r").Find("lowerarm_r").Find("hand_r"));
            enemy.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            enemy.gameObject.GetComponent<Collider>().enabled = false;
            enemy.transform.localPosition = new Vector3(0.0774f, -0.0529f, -0.0563f);
            enemy.transform.localRotation = Quaternion.Euler(23.144f, -124.586f, -54.422f);
            enemy.EnemyAnimator.Play("Choking");

            // Play Choking Anim
            EquippedWeaponModel.GetComponent<Animator>().Play("Choking");

            MoveSpeed = 1.0f;
            canJump = false;

            float chokeTime = 0f;
            while (chokeTime < 3.9f)
            {
                chokeTime += Time.deltaTime;
                yield return null;
            }

            enemy.gameObject.transform.parent = null;
            enemy.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            enemy.gameObject.GetComponent<Collider>().enabled = true;
            enemy.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 90f, ForceMode.Impulse);
            enemy.transform.rotation = Quaternion.Euler(enemy.transform.rotation.x, enemy.transform.rotation.y, 0f);
            enemy.StartCoroutine(enemy.ChokeDeath(enemy.MaxHealth));
            MoveSpeed = 5.0f;
            canJump = true;
            EndSpecialAttack();
        }
        else
        {
            EndSpecialAttack();
        }

        yield return null;
    }


    /*    void OnDrawGizmos()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position + CC.center, CC.height / 3f);
        }
    */
    public void CheckSpecialHit(AttackStrength strength)
    {
        if (Physics.SphereCast(transform.position + CC.center, CC.height / 3.25f, transform.forward, out RaycastHit hit, 0.2f))
        {
            hitObstacle = true;
            Velocity = Vector3.zero;
            EnemyController enemy = hit.collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                switch (strength)
                {
                    case AttackStrength.Light:
                        Instantiate(light_comicVFX, hit.point, Quaternion.identity);
                        enemy.Knockback(transform.forward, 120f);
                        enemy.TakeDamage(EquippedWeapon.LightAttackDamage, strength);
                        break;
                    case AttackStrength.Medium:
                        enemy.TakeDamage(EquippedWeapon.MediumAttackDamage, strength);
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
            }
        }
    }

    public EnemyController CheckChokeLand()
    {
        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out RaycastHit hit, 1.0f))
        {
            EnemyController enemy = hit.collider.GetComponent<EnemyController>();
            if (enemy != null && enemy.CurrentHealth > 0)
            {
                return enemy;
            }
        }

        return null;
    }


    public void EndSpecialAttack()
    {
        canMove = true;
        canTurn = true;
        hitObstacle = false;
        isUsingSpecial = false;
    }

    public void PlayAnimation(AnimationClip clip)
    {
        if (EquippedWeaponModel != null && clip != null)
        {
            EquippedWeaponModel.GetComponent<Animator>().Play(clip.name);
        }
    }

    public void InitialiseChairScript(AttackStrength strength, int damage)
    {
        EquippedWeaponModel.GetComponent<ChairEvents>().player = this;
        EquippedWeaponModel.transform.Find("Folding Chair").GetComponent<ChairEvents>().player = this;
        EquippedWeaponModel.transform.Find("Folding Chair").GetComponent<ChairEvents>().strength = strength;
        EquippedWeaponModel.transform.Find("Folding Chair").GetComponent<ChairEvents>().damage = damage;
    }

    public void InitialiseGarbageScript()
    {
        EquippedWeaponModel.GetComponent<GarbageEvents>().player = this;
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
        HUD.lerpTimer = 0f;

        switch (strength)
        {
            case AttackStrength.Light:
                AS.PlayOneShot(LightDamageSFX);
/*                PA.Play(LightDamageAnim.name);*/
                break;

            case AttackStrength.Medium:
                AS.PlayOneShot(MediumDamageSFX);
/*                PA.Play(MediumDamageAnim.name);*/
                break;

            case AttackStrength.Heavy:
                AS.PlayOneShot(HeavyDamageSFX);
/*                PA.Play(HeavyDamageAnim.name);*/
                break;
        }

        if (CurrentHealth == 0)
            Die();
    }

    internal System.Collections.IEnumerator Reload(float reloadDuration, RangedWeapon wep, int reloadAmount, AudioClip ReloadSFX = null)
    {
        while (wep.CurrentAmmo < wep.MaxAmmo)
        {
            yield return new WaitForSeconds(reloadDuration);
            wep.CurrentAmmo = Mathf.Min(wep.CurrentAmmo + reloadAmount, wep.MaxAmmo);
            RemoveHUDAmmoCircles();
            UpdateHUDAmmoCircles(wep.CurrentAmmo, wep.MaxAmmo);
            FindObjectOfType<InventoryManager>().UpdateAmmoCount();
            if (ReloadSFX != null) GetComponent<AudioSource>().PlayOneShot(ReloadSFX);
        }
        wep.CurrentlyReloading = false;
        if (EquippedWeaponModel.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name == "Reload")
        {
            EquippedWeaponModel.GetComponent<Animator>().SetTrigger("ReloadDone");
        }
    }

    private void HealPlayer(int healAmount)
    {
        CurrentHealth += healAmount;
        HUD.lerpTimer = 0f;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
    }

    private void Die()
    {
        canMove = false;
        canJump = false;
        canTurn = false;
        IsAttacking = true;
        PlayerCamera.enabled = false;
        DeathCamera.enabled = true;
        AS.PlayOneShot(DeathSFX);
        PA.Play(DeathAnim.name);
        Invoke("ResetScene", 2f);
    }

    #endregion

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.name == "Car_03")
        {
            TakeDamage(AttackStrength.Medium, 10);
        }
    }
}