using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Character : MonoBehaviour
{
    private bool HasPlayed;
    private Vector2 Position;
    private TextMeshPro Text;

    private int maxMovement;
    private Tilemap Walls;

    public int health;
    public Weapon weapon;
    public int movementPoints;

    private void Awake()
    {
        Walls = GameObject.FindWithTag("Walls").GetComponent<Tilemap>();
        
        maxMovement = movementPoints;
        Text = GetComponentInChildren<TextMeshPro>();
        Text.text = health.ToString();
        
        HasPlayed = false;
        Position = new Vector2(transform.position.x, transform.position.y);
    }

    
    public bool Move(Vector2 direction)
    {
        //check easy conditions like if it's too far, or if we're trying to get to the same place we're already at
        if (direction == Vector2.zero)
            return false;
        
        int distance = (int) Math.Abs(direction.x) + (int) Math.Abs(direction.y);
        if (distance > movementPoints)
            return false;
        
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

    public void GetHit(int damage)
    {
        health -= damage;
        Text.text = health.ToString();
        if (health <= 0)
            Destroy(gameObject);
    }

    public Dictionary<Vector2Int, int> GetPossibleMovements()
    {
        Dictionary<Vector2Int, int> movementMap = new Dictionary<Vector2Int, int>();
        movementMap.Add(GetPosition(), 0);
        Tilemap walls = GameObject.FindWithTag("Walls").GetComponent<Tilemap>();

        List<Vector2Int> neighbors = new List<Vector2Int>();
        neighbors.Add(Vector2Int.up);
        neighbors.Add(Vector2Int.right);
        neighbors.Add(Vector2Int.down);
        neighbors.Add(Vector2Int.left);
        
        for (int x = 0; x < movementPoints; x++)
        {
            List<Vector2Int> newList = new List<Vector2Int>();
            foreach (var movement in movementMap)
            {
                newList.Add(movement.Key);

                foreach (var neighbor in neighbors)
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
    }

    public bool GetPlayed()
    {
        return HasPlayed;
    }
}
