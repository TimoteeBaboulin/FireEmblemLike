using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(order = 0, fileName = "Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public enum WeaponType {
        Sword,
        Axe,
        Spear,
        None
    }
    
    [Header("Type")]
    public WeaponType type;
    
    [Header("Stats")]
    public int damage;
    public int minRange;
    public int maxRange;

    public float GetEfficiency(WeaponType _type)
    {
        if (type == Weapon.WeaponType.Sword && _type == Weapon.WeaponType.Spear
            || type == Weapon.WeaponType.Spear && _type == Weapon.WeaponType.Axe
            || type == Weapon.WeaponType.Axe && _type == Weapon.WeaponType.Sword)
            return 2;
        if (type == _type)
            return 1;

        return 0.5f;
    }
}
