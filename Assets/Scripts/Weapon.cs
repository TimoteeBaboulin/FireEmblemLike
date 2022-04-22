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
}
