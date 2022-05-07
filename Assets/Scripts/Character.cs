using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class Character : MonoBehaviour
{
    public enum Team
    {
        Player,
        Enemy
    }
    public static Action OnDeath;
    
    private bool HasPlayed;
    private Vector2 Position;

    [NonSerialized]public int maxMovement;
    private Tilemap Walls;
    [NonSerialized]public int MaxHealth;
    
    [Header("Character")]
    public int health = 10;
    public int movementPoints = 3;
    public Team team;   
    
    [Header("Equipment")]
    public Weapon _Weapon;
    public Armor _Armor;

    private void Awake()
    {
        Player.OnTurnChange += ChangeTurn;
    }

    private void LoadStats()
    {
        MaxHealth = health;
        maxMovement = movementPoints;
        HasPlayed = false;
        Position = new Vector2(transform.position.x, transform.position.y);
    }

    public void LoadGear(Weapon weapon, Armor armor, Team _team)
    {
        LoadStats();
        
        ChangeEquipment(weapon);
        ChangeEquipment(armor);
        team = _team;
        
        AnimatorOverrideController controller = GetController();
        
        if (controller == null)
            return;

        GetComponent<Animator>().runtimeAnimatorController = controller;
    }
    
    private AnimatorOverrideController GetController()
    {
        if (team == Team.Enemy && _Weapon.type == Weapon.WeaponType.Axe)
            return null;
        
        string path = "Animation/InGame/";

        switch (team)
        {
            case Team.Enemy:
                path += "Enemy";
                break;
            case Team.Player:
                path += "Player";
                break;
        }

        switch (_Weapon.type)
        {
            case Weapon.WeaponType.Axe:
                path += "Axe";
                break;
            
            case Weapon.WeaponType.Spear:
                path += "Spear";
                break;
            
            case Weapon.WeaponType.Sword:
                path += "Sword";
                break;
        }

        AnimatorOverrideController controller = Resources.Load<AnimatorOverrideController>(path);
        
        Debug.Log(controller == null);
        return controller;
    }

    public bool Move(Vector2 direction)
    {
        Vector2Int positionInt = new Vector2Int((int) (Position.x + direction.x), (int) (Position.y + direction.y));
        Dictionary<Vector2Int, int> possibleMovements = GetPossibleMovements();

        if (!possibleMovements.ContainsKey(positionInt))
            return false;

        Position = positionInt;
        transform.position = Position;
        movementPoints -= possibleMovements[positionInt];
        return true;
    }

    public bool UndoMove(int x, int y)
    {
        int distance = Math.Abs(x) + Math.Abs(y);
        movementPoints += distance;
        Position.Set(Position.x - x, Position.y - y);
        transform.position = Position;
        return true;
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int((int) Position.x, (int) Position.y);
    }

    private void UpdatePosition()
    {
        transform.position = Position;
    }

    public bool ExchangePositions(Vector2 position)
    {
        Position.Set(position.x, position.y);
        transform.position = Position;
        return true;
    }

    public void GetHit(int damage, Weapon.WeaponType type)
    {
        float damageEff = _Weapon.GetEfficiency(type);
        int trueDamage = Mathf.FloorToInt(damage * damageEff) - _Armor.defense;
        if (trueDamage <= 0)
            trueDamage = 1;
        health -= trueDamage;
        if (health <= 0) {
            OnDeath.Invoke();
            Destroy(gameObject);
        }
        
        gameObject.GetComponentInChildren<HealthBar>().UpdateHealth(this);
    }

    public Dictionary<Vector2Int, int> GetPossibleMovements()
    {
        Dictionary<Vector2Int, int> movementMap = new Dictionary<Vector2Int, int>();
        movementMap.Add(GetPosition(), 0);
        Tilemap walls = GameObject.FindWithTag("Walls").GetComponent<Tilemap>();
        Tilemap ground = GameObject.FindWithTag("Ground").GetComponent<Tilemap>();

        for (int x = 0; x < movementPoints; x++)
        {
            List<Vector2Int> newList = new List<Vector2Int>();
            foreach (var movement in movementMap)
            {
                newList.Add(movement.Key);

                foreach (var neighbor in Vector2IntExtension.neighbors)
                {
                    if (!movementMap.ContainsKey(movement.Key + neighbor) &&
                        walls.GetTile((Vector3Int) (movement.Key + neighbor)) == null &&
                        ground.GetTile((Vector3Int) (movement.Key + neighbor)) != null) 
                    {
                        newList.Add(movement.Key + neighbor);
                    }
                }
            }

            foreach (var movement in newList)
            {
                if (!movementMap.ContainsKey(movement))
                    movementMap.Add(movement, x + 1);
            }
        }

        return movementMap;
    }

    public void Played() {
        HasPlayed = true;
        movementPoints = 0;
    }

    public bool GetPlayed()
    {
        return HasPlayed;
    }

    public void OnDestroy()
    {
        Player.OnTurnChange -= ChangeTurn;
    }

    public void ChangeTurn()
    {
        HasPlayed = false;
        movementPoints = maxMovement + _Armor.movement;
        if (gameObject == null)
            return;
        GetComponent<Animator>().SetBool("Attack", false);
    }
    public bool ChangeEquipment(Armor armor)
    {
        _Armor = armor;
        movementPoints = maxMovement + _Armor.movement;
        return true;
    }
    
    public bool ChangeEquipment(Weapon weapon)
    {
        _Weapon = weapon;
        return true;
    }
}
