using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.XR;

public class AStar : MonoBehaviour
{
    public static AStar Instance;
        
    private List<Node> Open;
    private List<Node> Closed;

    public Tilemap ground;
    public Tilemap walls;

    public Tilemap player;
    public TileBase Path;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void Start()
    {
        Open = new List<Node>();
        Closed = new List<Node>();
    }

    public List<Vector2Int> GetPath(Vector2Int Start, Vector2Int End)
    {
        Node StartNode = new Node(Start, true);
        Node EndNode = new Node(End, true);

        Open = new List<Node>();
        Closed = new List<Node>();
        
        StartNode.CalculateHeuristics(End);
        
        Open.Add(StartNode);

        int iteration = 0;

        while (Open.Count>0)
        {
            Node current = null;

            foreach (var Node in Open)
            {
                if (current == null || current.f > Node.f)
                    current = Node;
            }

            if (CheckNodesSimilarity(current, EndNode))
            {
                List<Node> path = current.GetPath();

                List<Vector2Int> pathInt = new List<Vector2Int>();

                foreach (var node in path)
                {
                    pathInt.Add(new Vector2Int(node.position.x, node.position.y));
                }

                return pathInt;
            }

            List<Node> newList = Open;

            newList.Remove(current);
            
            Closed.Add(current);

            foreach (var neighbor in Vector2IntExtension.neighbors) 
            {
                Node neighboringNode = new Node(current.position + neighbor);
                
                neighboringNode.SetParent(current);
                
                if (ground.GetTile((Vector3Int) neighboringNode.position) == null ||
                    walls.GetTile((Vector3Int) neighboringNode.position) != null)
                {
                    continue;
                }
                    
                if (ListContainsNode(Closed, neighboringNode))
                    continue;

                if (ListContainsNode(newList, neighboringNode))
                {
                    int index = GetNodeIndex(newList, neighboringNode);
                    
                    neighboringNode.CalculateHeuristics(End);

                    if (newList[index].g > neighboringNode.g)
                    { 
                        newList[index] = neighboringNode;
                    }
                        
                    continue;
                }
                
                
                neighboringNode.CalculateHeuristics(End);
                newList.Add(neighboringNode);

                Open = newList;
            }
            
        }

        return null;
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

    private bool CheckNodesSimilarity(Node first, Node second)
    {
        return first.position == second.position;
    }
}
