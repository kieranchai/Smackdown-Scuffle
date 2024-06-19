using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject equippedSlot;
    public GameObject unequippedSlot;

    public GameObject equippedPanel;
    public GameObject unequippedPanel;

    public Sprite meleeWeapon;
    public Sprite rangedWeapon;
    public Sprite specialWeapon;

    private void Awake()
    {
        InitializeInventory();
    }

    public void InitializeInventory()
    {
        ClearInventory();

        PlayerController playerController = FindObjectOfType<PlayerController>();
        List<Weapon> weaponInventory = playerController.WeaponInventory;

        for (int i = 0; i < weaponInventory.Count; i++)
        {
            Weapon weapon = weaponInventory[i];
            if (weapon == playerController.EquippedWeapon)
            {
                CreateEquippedSlot(weapon, i + 1);
                continue;
            }
            CreateWeaponSlot(weapon, i + 1);
        }
    }

    private void ClearInventory()
    {
        for (int i = equippedPanel.transform.childCount - 1; i >= 0; i--)
            Destroy(equippedPanel.transform.GetChild(i).gameObject);

        for (int i = unequippedPanel.transform.childCount - 1; i >= 0; i--)
            Destroy(unequippedPanel.transform.GetChild(i).gameObject);
    }

    private GameObject CreateEquippedSlot(Weapon weapon, int slotIndex)
    {
        GameObject slot = Instantiate(equippedSlot, equippedPanel.transform);

        switch (weapon.Type)
        {
            case WeaponType.Melee:
                slot.transform.Find("Weapon").GetComponent<Image>().sprite = meleeWeapon;
                slot.transform.Find("Ammo").GetComponent<TMP_Text>().text = "";
                break;
            case WeaponType.Ranged:
                slot.transform.Find("Weapon").GetComponent<Image>().sprite = rangedWeapon;
                RangedWeapon rweap = (RangedWeapon)FindObjectOfType<PlayerController>().EquippedWeapon;
                slot.transform.Find("Ammo").GetComponent<TMP_Text>().text = $"{rweap.CurrentAmmo}/{rweap.MaxAmmo}";
                break;
            case WeaponType.Special:
                slot.transform.Find("Weapon").GetComponent<Image>().sprite = specialWeapon;
                slot.transform.Find("Ammo").GetComponent<TMP_Text>().text = "";
                break;
        }

        slot.transform.Find("Key").GetComponent<TMP_Text>().text = slotIndex.ToString();

        return slot;
    }

    public void UpdateAmmoCount()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        RangedWeapon rweap = (RangedWeapon)FindObjectOfType<PlayerController>().EquippedWeapon;
        equippedPanel.transform.GetChild(0).gameObject.transform.Find("Ammo").GetComponent<TMP_Text>().text = $"{rweap.CurrentAmmo}/{rweap.MaxAmmo}";
    }

    private GameObject CreateWeaponSlot(Weapon weapon, int slotIndex)
    {
        GameObject slot = Instantiate(unequippedSlot, unequippedPanel.transform);

        switch (weapon.Type)
        {
            case WeaponType.Melee:
                slot.transform.Find("Weapon").GetComponent<Image>().sprite = meleeWeapon;
                break;
            case WeaponType.Ranged:
                slot.transform.Find("Weapon").GetComponent<Image>().sprite = rangedWeapon;
                break;
            case WeaponType.Special:
                slot.transform.Find("Weapon").GetComponent<Image>().sprite = specialWeapon;
                break;
        }

        slot.transform.Find("Key").GetComponent<TMP_Text>().text = slotIndex.ToString();

        return slot;
    }
}