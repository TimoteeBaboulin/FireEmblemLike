using System.Collections;
using System.Collections.Generic;
using Packages.Rider.Editor.UnitTesting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public Vector2Int position;

    public Tilemap tiles;

    public TileBase tileSprite;
    // Start is called before the first frame update
    void Start()
    {
        position = new Vector2Int();
    }

    // Update is called once per frame
    void Update()
    {
        float axis = 0f;

        Vector2Int oldPosition = position;
        
        if (Input.GetButtonDown("Horizontal"))
        {
            if (Input.GetAxis("Horizontal") > 0)
                position.x += 1;
            else
                position.x -= 1;
            
        }

        if (Input.GetButtonDown("Vertical"))
        {
            if (Input.GetAxis("Vertical") > 0)
                position.y += 1;
            else
                position.y -= 1;
            
            
        }
        
        setNewTile(oldPosition, position);
        
        /*if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            axis = Input.GetAxis("Horizontal");
            if (axis > 0.5f)
            {
                position.x += 1;
            } else if (axis < -0.5f)
            {
                position.x -= 1;
            }

            axis = Input.GetAxis("Vertical");
            if (axis > 0.5f)
            {
                position.y += 1;
            } else if (axis < -0.5f)
            {
                position.y -= 1;
            }

            //tiles = new Tilemap();
            Vector3Int tempPosition = new Vector3Int(position.x, position.y, 0);
            tiles.SetTile(new Vector3Int(position.x, position.y, 0), tileSprite);
        }*/
    }

    void setNewTile(Vector2Int oldPosition, Vector2Int newPosition)
    {
        tiles.SetTile((Vector3Int) oldPosition, null);
        tiles.SetTile((Vector3Int) newPosition, tileSprite);
        
    }
}
