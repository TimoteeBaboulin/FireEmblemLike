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
        Debug.Log("Execute working");
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

        if (Vector2IntExtension.Distance(userPosition, targetPosition) >= user.Weapon.minRange &&
            Vector2IntExtension.Distance(userPosition, targetPosition) <= user.Weapon.maxRange)
        {
            target.GetHit(user.Weapon.damage, user.Weapon.type);
            user.Played();
            return true;
        }
        
        foreach (var movement in possibleMovements.Keys)
        {
            int distance = Vector2IntExtension.Distance(movement, targetPosition);
            if (distance >= user.Weapon.minRange && distance <= user.Weapon.maxRange && !Player.Instance.ContainPlayer(movement)) {
                if (user.Move(new Vector2((float) movement.x - userPosition.x, (float) movement.y - userPosition.y)))
                {
                    target.GetHit(user.Weapon.damage, user.Weapon.type);
                    user.Played();

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

        if (Vector2IntExtension.Distance(userPosition, targetPosition) >= user.Weapon.minRange &&
            Vector2IntExtension.Distance(userPosition, targetPosition) <= user.Weapon.maxRange)
        {
            target.GetHit(user.Weapon.damage, user.Weapon.type);
            user.Played();
            return true;
        }

        if (possibleMovements.ContainsKey(attackPosition) &&
            Vector2IntExtension.Distance(attackPosition, targetPosition) >= user.Weapon.minRange &&
            Vector2IntExtension.Distance(attackPosition, targetPosition) <= user.Weapon.maxRange)
        {
            if (user.Move(new Vector2((float) (attackPosition.x - userPosition.x),
                (float) (attackPosition.y - userPosition.y))))
            {
                target.GetHit(user.Weapon.damage, user.Weapon.type);
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
