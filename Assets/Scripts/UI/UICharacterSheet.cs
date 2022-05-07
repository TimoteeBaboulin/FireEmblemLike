using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterSheet : MonoBehaviour
{
    [Header("Weapon Sprites")]
    public Sprite Sword;
    public Sprite Axe;
    public Sprite Spear;

    [Header("Armor Sprites")]
    public Sprite Platemail;
    public Sprite Chainmail;
    public Sprite Leather;
    
    [Header("UI GameObjects")] 
    public GameObject Weapon;
    public GameObject Armor;
    public GameObject Character;
    public GameObject Life;
    public GameObject Movement;

    public void SetUIElements(Character character)
    {
        switch (character._Weapon.type)
        {
            case global::Weapon.WeaponType.Sword:
                Weapon.GetComponentsInChildren<Image>()[1].sprite = Sword;
                break;
            
            case global::Weapon.WeaponType.Axe:
                Weapon.GetComponentsInChildren<Image>()[1].sprite = Axe;
                break;
            
            case global::Weapon.WeaponType.Spear:
                Weapon.GetComponentsInChildren<Image>()[1].sprite = Spear;
                break;
        }

        switch (character._Armor.movement)
        {
            case -1:
                Armor.GetComponentsInChildren<Image>()[1].sprite = Platemail;
                break;
            
            case 1:
                Armor.GetComponentsInChildren<Image>()[1].sprite = Leather;
                break;
            
            case 0:
                Armor.GetComponentsInChildren<Image>()[1].sprite = Chainmail;
                break;
        }

        Weapon.GetComponentInChildren<Text>().text = "Damage:\n" + character._Weapon.damage.ToString();
        Armor.GetComponentInChildren<Text>().text = "Defense:\n" + character._Armor.defense.ToString();

        Life.GetComponent<Text>().text = "HP: " + character.health + "/" + character.MaxHealth;
        int maxMovement = character.maxMovement + character._Armor.movement;
        Movement.GetComponent<Text>().text = "Movement:\n" + character.movementPoints + "/" + maxMovement;
    }
}
