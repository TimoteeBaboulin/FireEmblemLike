using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.XR;

public class AStar : MonoBehaviour
{
    private List<Node> Open;
    private List<Node> Closed;

    public Tilemap ground;
    public Tilemap walls;

    public Tilemap player;
    public TileBase Path;

    public void Start()
    {
        Open = new List<Node>();
        Closed = new List<Node>();
    }

    public List<Node> GetPath(Vector2Int Start, Vector2Int End)
    {
        Node StartNode = new Node(Start, true);
        Node EndNode = new Node(End, true);

        Open.Clear();
        Closed.Clear();
        
        StartNode.CalculateHeuristics(End);
        
        Open.Add(StartNode);

        while (Open[0] != null)
        {
            Node current = null;

            foreach (var Node in Open)
            {
                if (current == null || current.f > Node.f)
                    current = Node;
            }

            if (current == EndNode)
                return current.GetPath();

            foreach (var neighbor in Vector2IntExtension.neighbors) 
            {
                Node neighboringNode = new Node(current.position + neighbor);
                if (ground.GetTile((Vector3Int) neighboringNode.position) == null ||
                    walls.GetTile((Vector3Int) neighboringNode.position) != null)
                {
                    continue;
                }
                    
                if (ListContainsNode(Closed, neighboringNode))
                    continue;

                if (ListContainsNode(Open, neighboringNode))
                {
                    int index = GetNodeIndex(Open, neighboringNode);
                        
                    neighboringNode.SetParent(current);
                    neighboringNode.CalculateHeuristics(End);

                    if (Open[index].g > neighboringNode.g)
                    { 
                        Open[index] = neighboringNode;
                    }
                        
                    continue;
                }
                    
                neighboringNode.SetParent(current);
                Open.Add(neighboringNode);
            }
        }

        return null;
    }

    public void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            List<Node> path = GetPath(Vector2Int.zero, new Vector2Int(3, -2));

            if (path == null)
                return;
            
            foreach (var node in path)
            {
                player.SetTile((Vector3Int) node.position, Path);
            }
        }
    }
    
    bool ListContainsNode(List<Node>list, Node node)
    {
        foreach (var element in list)
        {
            if (element.position == node.position)
                return true;
        }

        return false;
    }

    int GetNodeIndex(List<Node> list, Node node)
    {
        for (int x = 0; x < list.Count; x++)
        {
            if (list[x].position == node.position)
                return x;
        }

        return -1;
    }
}
