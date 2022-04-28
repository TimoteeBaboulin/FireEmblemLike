using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    private TextMeshPro Text;

    private int maxMovement;
    private Tilemap Walls;

    [Header("Character")]
    public int health;
    public int movementPoints;
    public Team team;   
    
    [Header("Equipment")]
    public Weapon weapon;
    public Armor armor;

    [Header("Inventory")] 
    public Item[] inventory;


    private void Awake()
    {
        GetComponent<SpriteRenderer>().sprite =
            Resources.Load<Sprite>("Characters/" + weapon.type.ToString().ToLower() + ".png");
        Walls = GameObject.FindWithTag("Walls").GetComponent<Tilemap>();

        Player.OnTurnChange += ChangeTurn;
        
        maxMovement = movementPoints;
        movementPoints += armor.movement;
        Text = GetComponentInChildren<TextMeshPro>();
        Text.text = health.ToString();
        
        HasPlayed = false;
        Position = new Vector2(transform.position.x, transform.position.y);
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
        float damageEff = weapon.GetEfficiency(type);
        health -= Mathf.FloorToInt(damage * damageEff) - armor.defense;
        Text.text = health.ToString();
        if (health <= 0) {
            OnDeath.Invoke();
            Destroy(gameObject);
        }
    }

    public Dictionary<Vector2Int, int> GetPossibleMovements()
    {
        Dictionary<Vector2Int, int> movementMap = new Dictionary<Vector2Int, int>();
        movementMap.Add(GetPosition(), 0);
        Tilemap walls = GameObject.FindWithTag("Walls").GetComponent<Tilemap>();

        for (int x = 0; x < movementPoints; x++)
        {
            List<Vector2Int> newList = new List<Vector2Int>();
            foreach (var movement in movementMap)
            {
                newList.Add(movement.Key);

                foreach (var neighbor in Vector2IntExtension.neighbors)
                {
                    if (!movementMap.ContainsKey(movement.Key + neighbor) &&
                        walls.GetTile((Vector3Int) (movement.Key + neighbor)) == null) 
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

    public void ChangeTurn()
    {
        HasPlayed = false;
        movementPoints = maxMovement + armor.movement;
    }

    public bool UseItem()
    {
        return false;
    }
}
