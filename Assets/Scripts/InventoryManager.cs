using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject equippedSlot;
    public GameObject unequippedSlot;

    public GameObject equippedPanel;
    public GameObject unequippedPanel;

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
            /*            GameObject slot = CreateWeaponSlot(weapon, i + 1);*/

            /*            if (weapon == playerController.EquippedWeapon)
                            HighlightSelectedWeapon(slot);*/
        }
    }

    private void ClearInventory()
    {
        Destroy(equippedPanel.transform.GetChild(0).gameObject);

        for (int i = unequippedPanel.transform.childCount - 1; i >= 0; i--)
            Destroy(unequippedPanel.transform.GetChild(i).gameObject);
    }

    /*    private GameObject CreateWeaponSlot(Weapon weapon, int slotIndex)
        {
            GameObject slot = Instantiate(WeaponSlotPrefab, transform);
            WeaponSlot weaponSlot = slot.GetComponent<WeaponSlot>();

            weaponSlot.Equip = weapon;
            weaponSlot.Initialize();
            weaponSlot.EquipKey.text = slotIndex.ToString();

            return slot;
        }*/

    /*    private void HighlightSelectedWeapon(GameObject slot)
        {
            Instantiate(SelectionPrefab, slot.transform);
        }*/
}