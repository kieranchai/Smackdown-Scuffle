using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class HUDManager : MonoBehaviour
{
    public bool IsPaused;
    public GameObject PausePanel;
    public Image PlayerHPBar;
    public Image PlayerHPBar_Lazy;
    public Image Crosshair;
    public GameObject PlayerHPPanel;
    public GameObject StaminaPanel;
    public GameObject KeysPanel;
    public GameObject HealPanel;

    [Header("Melee HUD References")]
    public GameObject MeleeResourceObj;
    public Image MeleeResourceBar;
    public Image MeleeResourceBar_Lazy;

    [Header("Special HUD References")]
    public GameObject SpecialResourceObj;
    public Image SpecialCooldownLight;
    public Image SpecialCooldownMedium;
    public Image SpecialCooldownHeavy;

    [Header("Damage Taken References")]
    public Image Vignette_Left;
    public Image Vignette_Right;
    public Image Vignette_Top;
    public Image Vignette_Bottom;

    private PlayerControls playerControls;

    private float chipSpeed = 2f;
    private bool hasFlashedLight = true;
    private bool hasFlashedMedium = true;
    private bool hasFlashedHeavy = true;

    [HideInInspector]
    public float lerpTimer;

    [HideInInspector]
    public float staminaLerpTimer;

    private Dictionary<Image, Coroutine> fadeOutCoroutines = new Dictionary<Image, Coroutine>();
    private Dictionary<Image, float> accumulatedFadeStrengths = new Dictionary<Image, float>();

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


    }

    public void RegenStaminaBar(PlayerController player)
    {
        float staminaF = MeleeResourceBar.fillAmount;
        float stamina_LazyF = MeleeResourceBar_Lazy.fillAmount;
        float hFraction = (float)player.CurrentStamina / player.MaxStamina;

        if (staminaF < hFraction)
        {
            MeleeResourceBar.fillAmount = hFraction;
            MeleeResourceBar_Lazy.fillAmount = hFraction;
        }
    }

    public void FlashHPBars(PlayerController player)
    {
        PlayerHPBar.color = Color.white;
        StartCoroutine(FlashHealth(player));
    }
    IEnumerator FlashHealth(PlayerController player)
    {
        bool isFlickering = false;
        PlayerHPBar.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        PlayerHPBar.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        PlayerHPBar.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        PlayerHPBar.color = new Color(255f, 255f, 255f);

        if (player.CurrentHealth <= player.MaxHealth / 3)
        {
            isFlickering = true;
        }

        while (isFlickering)
        {
            PlayerHPBar.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            PlayerHPBar.color = Color.white;
            yield return new WaitForSeconds(0.1f);

            if (player.CurrentHealth > player.MaxHealth / 3)
            {
                isFlickering = false;
                //stop heartbeat
            }
            yield return null;
        }

        yield return null;
    }

    public void SwitchHUD(Weapon weapon)
    {
        KeysPanel.SetActive(weapon.Type != WeaponType.Special);
        SpecialResourceObj.SetActive(weapon.Type == WeaponType.Special);
        Crosshair.gameObject.GetComponent<Animator>().Play("Intro");
    }

    public void UpdateResources(PlayerController player)
    {
        Weapon equippedWeapon = player.EquippedWeapon;

        switch (equippedWeapon.Type)
        {
            case WeaponType.Special:
                SpecialWeapon SpecialWeapon = (SpecialWeapon)equippedWeapon;
                SpecialCooldownLight.fillAmount = (SpecialWeapon.LightSpecialCDTimer / SpecialWeapon.LightAttackCooldown);
                if (SpecialCooldownLight.fillAmount >= 1 && !hasFlashedLight)
                {
                    hasFlashedLight = true;
                    SpecialCooldownLight.gameObject.transform.parent.GetComponent<Animator>().Play("Ready");
                }
                else if (SpecialCooldownLight.fillAmount < 1 && hasFlashedLight)
                {
                    hasFlashedLight = false;
                }

                SpecialCooldownMedium.fillAmount = (SpecialWeapon.MediumSpecialCDTimer / SpecialWeapon.MediumAttackCooldown);
                if (SpecialCooldownMedium.fillAmount >= 1 && !hasFlashedMedium)
                {
                    hasFlashedMedium = true;
                    SpecialCooldownMedium.gameObject.transform.parent.GetComponent<Animator>().Play("Ready");
                }
                else if (SpecialCooldownMedium.fillAmount < 1 && hasFlashedMedium)
                {
                    hasFlashedMedium = false;
                }
                SpecialCooldownHeavy.fillAmount = (SpecialWeapon.HeavySpecialCDTimer / SpecialWeapon.HeavyAttackCooldown);
                if (SpecialCooldownHeavy.fillAmount >= 1 && !hasFlashedHeavy)
                {
                    hasFlashedHeavy = true;
                    SpecialCooldownHeavy.gameObject.transform.parent.GetComponent<Animator>().Play("Ready");
                }
                else if (SpecialCooldownHeavy.fillAmount < 1 && hasFlashedHeavy)
                {
                    hasFlashedHeavy = false;
                }
                break;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void FadeInDamageDealt(AttackStrength strength, string direction, bool isLowHealth = false)
    {
        float fadeStrength = 0f;

        switch (strength)
        {
            case AttackStrength.Light:
                fadeStrength = 1 / 3f;
                break;
            case AttackStrength.Medium:
                fadeStrength = 2 / 3f;
                break;
            case AttackStrength.Heavy:
                fadeStrength = 3 / 3f;
                break;
        }
        if (isLowHealth) fadeStrength = 1f;

        Image edge = null;
        switch (direction)
        {
            case "left":
                edge = Vignette_Left;
                break;
            case "right":
                edge = Vignette_Right;
                break;
            case "up":
                edge = Vignette_Top;
                break;
            case "down":
                edge = Vignette_Bottom;
                break;
        }

        StartCoroutine(FadeInVignette(fadeStrength, edge, isLowHealth));
    }

    public void FadeOutLowHealth()
    {
        StartCoroutine(FadeOutVignette(1f, Vignette_Left));
        StartCoroutine(FadeOutVignette(1f, Vignette_Right));
        StartCoroutine(FadeOutVignette(1f, Vignette_Top));
        StartCoroutine(FadeOutVignette(1f, Vignette_Bottom));
    }

    IEnumerator FadeInVignette(float fadeStrength, Image edge, bool isLowHealth)
    {
        float a = 0f;

        // Initialize or retrieve accumulated fade strength for this Image
        float accumulatedFadeStrength = 0f;
        if (accumulatedFadeStrengths.ContainsKey(edge))
        {
            accumulatedFadeStrength = accumulatedFadeStrengths[edge];
        }
        // Accumulate the fade strength based on baseFadeStrength
        accumulatedFadeStrength += fadeStrength;
        // Store the accumulated fade strength back into the dictionary
        accumulatedFadeStrengths[edge] = accumulatedFadeStrength;

        // Stop FadeOut coroutine if already running
        if (fadeOutCoroutines.ContainsKey(edge) && fadeOutCoroutines[edge] != null)
        {
            StopCoroutine(fadeOutCoroutines[edge]);
            fadeOutCoroutines.Remove(edge); // Remove coroutine from dictionary when done
        }

        while (edge.color.a <= accumulatedFadeStrength)
        {
            a += Time.deltaTime * 3f;
            a = Mathf.Max(a, accumulatedFadeStrength);
            edge.color = new Color(edge.color.r, edge.color.g, edge.color.b, a);
            yield return null;
        }

        if (!isLowHealth)
        {
            yield return new WaitForSeconds(1f);
            // Start FadeOut coroutine after fading in
            Coroutine fadeOutCoroutine = StartCoroutine(FadeOutVignette(accumulatedFadeStrength, edge));
            fadeOutCoroutines[edge] = fadeOutCoroutine;
        }
    }

    IEnumerator FadeOutVignette(float fadeStrength, Image edge)
    {
        if (FindAnyObjectByType<PlayerController>())
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player.CurrentHealth <= player.MaxHealth / 3)
            {
                fadeOutCoroutines.Remove(edge); // Remove coroutine from dictionary when done
                accumulatedFadeStrengths.Remove(edge); // Remove fadeStrength from dictionary when done
                yield break;
            }
        }

        float a = fadeStrength;
        while (edge.color.a > 0f)
        {
            a -= Time.deltaTime * 3f;
            a = Mathf.Max(a, 0f);
            edge.color = new Color(edge.color.r, edge.color.g, edge.color.b, a);
            yield return null;
        }
        edge.color = new Color(edge.color.r, edge.color.g, edge.color.b, 0f);

        fadeOutCoroutines.Remove(edge); // Remove coroutine from dictionary when done
        accumulatedFadeStrengths.Remove(edge); // Remove fadeStrength from dictionary when done
    }

    public void PlayHealAnim()
    {
        HealPanel.GetComponent<Animator>().Play("Heal");
    }
}