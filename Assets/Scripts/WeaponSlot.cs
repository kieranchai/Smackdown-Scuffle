using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSlot : MonoBehaviour
{
    public Weapon Equip;
    public TextMeshProUGUI WeaponName;
    public Image WeaponIcon;
    public TextMeshProUGUI EquipKey;

    private PlayerController playerController;
    private HUDManager hudManager;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        hudManager = FindObjectOfType<HUDManager>();
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        WeaponName.text = Equip.Name.ToUpper();
        WeaponIcon.sprite = Equip.Icon;
    }

    public void EquipWeapon()
    {
        playerController.EquipWeapon(Equip);
        hudManager.SwitchHUD(Equip.Type);
    }
}