using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector2 Position;
    public Vector2 Goal;

    public void SetGoal(Vector2Int goal)
    {
        Goal = goal;
    }

    public void ForcePosition(Vector2Int goal)
    {
        Position = goal;
        Goal = goal;

        transform.position = Position;
    }

    public void SetPosition(Vector2Int pos)
    {
        SetGoal(pos);
        
        Position = pos;
        transform.position = Position;
        
        SetY();
    }

    private void Update()
    {
        if (Position == Goal)
            return;

        if (Vector2.Distance(Position, Goal) < 0.01f) {
            Position = Goal;
            transform.position = Position;
            SetY();
            return;
        }

        Vector2 movement = Goal - Position;
        movement.Normalize();
        Position += movement * 0.01f;
        transform.position = Position;
        
        SetY();
    }

    void SetY()
    {
        Vector3 newPosition = transform.position;
        newPosition.z = -1;
        transform.position = newPosition;
    }
}
