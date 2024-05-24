using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public bool IsPaused;
    public GameObject PausePanel;
    public Image PlayerHPBar;
    public Image EnemyHPBar;
    public Image Crosshair;

    [Header("Melee HUD References")]
    public GameObject MeleeResourceObj;
    public Image MeleeResourceBar;

    [Header("Ranged HUD References")]
    public GameObject RangedResourceObj;
    public TextMeshProUGUI RangedResourceText;

    [Header("Special HUD References")]
    public GameObject SpecialResourceObj;
    public Image SpecialCooldownLight;
    public Image SpecialCooldownMedium;
    public Image SpecialCooldownHeavy;

    private PlayerControls playerControls;

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

    public void UpdateHPBars()
    {
        var player = FindObjectOfType<PlayerController>();
        var enemy = FindObjectOfType<EnemyController>();
        PlayerHPBar.fillAmount = (float)player.CurrentHealth / player.MaxHealth;
        EnemyHPBar.fillAmount = (float)enemy.CurrentHealth / enemy.MaxHealth;
    }

    public void SwitchHUD(WeaponType type)
    {
        MeleeResourceObj.SetActive(type == WeaponType.Melee);
        RangedResourceObj.SetActive(type == WeaponType.Ranged);
        SpecialResourceObj.SetActive(type == WeaponType.Special);
    }

    public void UpdateResources(PlayerController player)
    {
        Weapon equippedWeapon = player.EquippedWeapon;

        switch (equippedWeapon.Type)
        {
            case WeaponType.Melee:
                MeleeWeapon meleeWeapon = (MeleeWeapon)equippedWeapon;
                MeleeResourceBar.fillAmount = meleeWeapon.CurrentStamina / meleeWeapon.MaxStamina;
                break;

            case WeaponType.Ranged:
                RangedWeapon rangedWeapon = (RangedWeapon)equippedWeapon;
                RangedResourceText.text = $"{rangedWeapon.CurrentAmmo} / {rangedWeapon.MaxAmmo}";
                break;

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