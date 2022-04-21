using System;
using System.Collections;
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

        List<Vector2Int> possibleMovements = new List<Vector2Int>();
        possibleMovements.Add(GetPosition());
        
        for (int x = 0; x < movementPoints; x++)
        {
            List<Vector2Int> newList = new List<Vector2Int>();
            foreach (var movement in possibleMovements)
            {
                Vector2Int vector;

                newList.Add(movement);

                vector = movement + Vector2Int.up;
                if (!possibleMovements.Contains(vector) && Walls.GetTile((Vector3Int) vector) == null)
                    newList.Add(vector);

                vector = movement + Vector2Int.right;
                if (!possibleMovements.Contains(vector) && Walls.GetTile((Vector3Int) vector) == null)
                    newList.Add(vector);

                vector = movement + Vector2Int.down;
                if (!possibleMovements.Contains(vector) && Walls.GetTile((Vector3Int) vector) == null)
                    newList.Add(vector);

                vector = movement + Vector2Int.left;
                if (!possibleMovements.Contains(vector) && Walls.GetTile((Vector3Int) vector) == null)
                    newList.Add(vector);
            }

            possibleMovements = newList;
            foreach (var movement in possibleMovements)
            {
                if (movement == positionInt) {
                    Position = positionInt;
                    transform.position = Position;
                    movementPoints -= x + 1;
                    return true;
                }
            }
        }
        
        return false;
    }

    public bool UndoMove(int x, int y)
    {
        int distance = Math.Abs(x) + Math.Abs(y);
        movementPoints += distance;
        Text.text = movementPoints.ToString();
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
}
