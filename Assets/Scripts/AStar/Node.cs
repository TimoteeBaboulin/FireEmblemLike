using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private Node parent;

    public int h;
    public int g;
    public int f;
    
    public Vector2Int position;

    public Node(Vector2Int pos)
    {
        position = pos;
    }
    public Node(Vector2Int pos, bool walk)
    {
        position = pos;
    }

    public List<Node> GetPath()
    {
        List<Node> list;
        if (parent == null)
        {
            list = new List<Node>();
            list.Add(this);
            return list;
        }

        list = parent.GetPath();
        list.Add(this);
        return list;
    }

    private int GetLength()
    {
        if (parent == null)
            return 0;
        return parent.GetLength() + 1;
    }
    
    public void CalculateH(Vector2Int goal)
    {
        h = Vector2IntExtension.Distance(goal, position);
    }

    public void CalculateG()
    {
        g = GetLength();
    }

    public void CalculateF()
    {
        f = g + h;
    }

    public void CalculateHeuristics(Vector2Int goal)
    {
        CalculateH(goal);
        CalculateG();
        CalculateF();
    }

    public void SetParent(Node newParent)
    {
        parent = newParent;
    }
}
