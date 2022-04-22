using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector2IntExtension : MonoBehaviour
{
    static public int Distance(Vector2Int a, Vector2Int b)
    {
        return (Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y));
    }
    
    static public Vector2Int[] neighbors = new []{Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left, };
}
