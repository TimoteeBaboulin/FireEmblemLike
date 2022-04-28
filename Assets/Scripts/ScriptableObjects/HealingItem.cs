using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[CreateAssetMenu(order = 3, fileName = "Potion", menuName = "Healing Item")]
public class HealingItem : Item
{
    public int healAmount = 5;
}
