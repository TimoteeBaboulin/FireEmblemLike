using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<Node> list = new List<Node>();

        Node first = new Node(Vector2Int.zero);
        Node second = new Node(Vector2Int.one);
        
        list.Add(first);
        list.Add(second);

        list.Remove(first);
        
        Debug.Log(list[0].position);
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
}
