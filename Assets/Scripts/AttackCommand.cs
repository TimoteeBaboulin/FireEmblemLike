using System;
using System.Collections;
using System.Collections.Generic;
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

        if (Vector2IntExtension.Distance(userPosition, targetPosition) >= user.weapon.minRange &&
            Vector2IntExtension.Distance(userPosition, targetPosition) <= user.weapon.maxRange)
        {
            target.GetHit(user.weapon.damage);
            user.Played();
            return true;
        }
        
        foreach (var movement in possibleMovements.Keys)
        {
            int distance = Vector2IntExtension.Distance(movement, targetPosition);
            if (distance >= user.weapon.minRange && distance <= user.weapon.maxRange && !Player.Instance.ContainPlayer(movement)) {
                if (user.Move(new Vector2((float) movement.x - userPosition.x, (float) movement.y - userPosition.y)))
                {
                    target.GetHit(user.weapon.damage);
                    user.Played();

                    return true;
                }
            }
        }

        return false;
    }

    public void Undo()
    {
        return;
    }
}
