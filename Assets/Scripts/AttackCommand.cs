using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AttackCommand : MonoBehaviour, Command
{
    public Character user { get; set; }
    public Character target { get; set; }

    public AttackCommand(Character attacker)
    {
        user = attacker;
    }
    
    public AttackCommand(Character attacker, Character defender)
    {
        user = attacker;
        target = defender;
    }
    
    public bool Execute()
    {
        if (target == null)
            return false;
        target.GetHit(user.weapon.damage);
        return true;
    }

    public void Undo()
    {
        target.health += user.weapon.damage;
    }
}
