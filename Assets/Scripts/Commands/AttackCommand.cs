using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public class AttackCommand : Command
{
    public bool undoable { get; set; }
    public Character user { get; set; }
    public Character target { get; set; }

    public AttackCommand(Character attacker)
    {
        undoable = false;
        user = attacker;
    }
    
    public AttackCommand(Character attacker, Character defender)
    {
        undoable = false;
        user = attacker;
        target = defender;
    }
    
    public bool Execute()
    {
        if (target == null)
            return false;

        if (user.GetPlayed())
            return false;
        
        Vector2Int userPosition = user.GetPosition();
        Vector2Int targetPosition = target.GetPosition();
        /*
        int distance = Math.Abs(userPosition.x - targetPosition.x) + Math.Abs(userPosition.y - targetPosition.y);
        */
        
        Dictionary<Vector2Int,int> possibleMovements = user.GetPossibleMovements();

        if (Vector2IntExtension.Distance(userPosition, targetPosition) >= user._Weapon.minRange &&
            Vector2IntExtension.Distance(userPosition, targetPosition) <= user._Weapon.maxRange)
        {
            target.GetHit(user._Weapon.damage, user._Weapon.type);
            user.Played();
            user.GetComponent<Animator>().SetBool("Attack", true);
            return true;
        }
        
        foreach (var movement in possibleMovements.Keys)
        {
            int distance = Vector2IntExtension.Distance(movement, targetPosition);
            if (distance >= user._Weapon.minRange && distance <= user._Weapon.maxRange && !Player.Instance.ContainPlayer(movement)) {
                if (user.Move(new Vector2((float) movement.x - userPosition.x, (float) movement.y - userPosition.y)))
                {
                    target.GetHit(user._Weapon.damage, user._Weapon.type);
                    user.Played();
                    user.GetComponent<Animator>().SetBool("Attack", true);
                    return true;
                }
            }
        }

        return false;
    }

    public bool Execute(Vector2Int attackPosition)
    {
        if (target == null)
            return false;

        if (user.GetPlayed())
            return false;
        
        Vector2Int userPosition = user.GetPosition();
        Vector2Int targetPosition = target.GetPosition();
        /*
        int distance = Math.Abs(userPosition.x - targetPosition.x) + Math.Abs(userPosition.y - targetPosition.y);
        */
        
        Dictionary<Vector2Int,int> possibleMovements = user.GetPossibleMovements();

        if (Vector2IntExtension.Distance(userPosition, targetPosition) >= user._Weapon.minRange &&
            Vector2IntExtension.Distance(userPosition, targetPosition) <= user._Weapon.maxRange)
        {
            target.GetHit(user._Weapon.damage, user._Weapon.type);
            user.Played();
            user.GetComponent<Animator>().SetBool("Attack", true);
            return true;
        }

        if (possibleMovements.ContainsKey(attackPosition) &&
            Vector2IntExtension.Distance(attackPosition, targetPosition) >= user._Weapon.minRange &&
            Vector2IntExtension.Distance(attackPosition, targetPosition) <= user._Weapon.maxRange)
        {
            if (user.Move(new Vector2((float) (attackPosition.x - userPosition.x),
                (float) (attackPosition.y - userPosition.y))))
            {
                target.GetHit(user._Weapon.damage, user._Weapon.type);
                user.GetComponent<Animator>().SetBool("Attack", true);
                return true;
            }
        }
        return false;
    }

    public void Undo()
    {
        return;
    }
}
