using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public bool IsPaused;
    public GameObject PausePanel;
    public Image PlayerHPBar;
    public Image PlayerHPBar_Lazy;
    public Image Crosshair;

    [Header("Melee HUD References")]
    public GameObject MeleeResourceObj;
    public Image MeleeResourceBar;
    public Image MeleeResourceBar_Lazy;

    [Header("Special HUD References")]
    public GameObject SpecialResourceObj;
    public Image SpecialCooldownLight;
    public Image SpecialCooldownMedium;
    public Image SpecialCooldownHeavy;

    private PlayerControls playerControls;

    private float chipSpeed = 2f;

    [HideInInspector]
    public float lerpTimer;

    [HideInInspector]
    public float staminaLerpTimer;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerControls = new PlayerControls();
        playerControls.Enable();
        playerControls.Basic.PauseGame.performed += ctx => TogglePause();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
        PausePanel.SetActive(IsPaused);

        if (IsPaused)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void UpdateHPBars(PlayerController player)
    {
        float hpF = PlayerHPBar.fillAmount;
        float hp_LazyF = PlayerHPBar_Lazy.fillAmount;
        float hFraction = (float)player.CurrentHealth / player.MaxHealth;

        if (hp_LazyF > hFraction)
        {
            PlayerHPBar.fillAmount = hFraction;
            PlayerHPBar_Lazy.color = new Color(255f / 255f, 91f / 255f, 21f / 255f);
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            PlayerHPBar_Lazy.fillAmount = Mathf.Lerp(hp_LazyF, hFraction, percentComplete);
        }

        if (hpF < hFraction)
        {
            PlayerHPBar_Lazy.color = new Color(183f / 255f, 255f / 255f, 91f / 255f);
            PlayerHPBar_Lazy.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            PlayerHPBar.fillAmount = Mathf.Lerp(hpF, PlayerHPBar_Lazy.fillAmount, percentComplete);
        }
    }

    public void UpdateStaminaBar(PlayerController player)
    {
        float staminaF = MeleeResourceBar.fillAmount;
        float stamina_LazyF = MeleeResourceBar_Lazy.fillAmount;
        float hFraction = (float)player.CurrentStamina / player.MaxStamina;

        if (stamina_LazyF > hFraction)
        {
            MeleeResourceBar.fillAmount = hFraction;
            MeleeResourceBar_Lazy.color = Color.red;
            staminaLerpTimer += Time.deltaTime;
            float percentComplete = staminaLerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            MeleeResourceBar_Lazy.fillAmount = Mathf.Lerp(stamina_LazyF, hFraction, percentComplete);
        }

        if (staminaF < hFraction)
        {
            MeleeResourceBar.fillAmount = hFraction;
        }
    }

    public void SwitchHUD(WeaponType type)
    {
        SpecialResourceObj.SetActive(type == WeaponType.Special);
    }

    public void UpdateResources(PlayerController player)
    {
        Weapon equippedWeapon = player.EquippedWeapon;

        switch (equippedWeapon.Type)
        {
            case WeaponType.Special:
                SpecialWeapon SpecialWeapon = (SpecialWeapon)equippedWeapon;
                SpecialCooldownLight.fillAmount = 1f - (SpecialWeapon.LightSpecialCDTimer / SpecialWeapon.LightAttackCooldown);
                SpecialCooldownMedium.fillAmount = 1f - (SpecialWeapon.MediumSpecialCDTimer / SpecialWeapon.MediumAttackCooldown);
                SpecialCooldownHeavy.fillAmount = 1f - (SpecialWeapon.HeavySpecialCDTimer / SpecialWeapon.HeavyAttackCooldown);
                break;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}