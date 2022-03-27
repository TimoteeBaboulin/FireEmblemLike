using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public Vector2Int position;

    public Tilemap tiles;

    public Sprite tileSprite;
    // Start is called before the first frame update
    void Start()
    {
        position = new Vector2Int();
    }

    // Update is called once per frame
    void Update()
    {
        float axis = 0f;
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
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

            tiles = new Tilemap();
            Vector3Int tempPosition = new Vector3Int(position.x, position.y, 0);
            tiles.SetTile((Vector3Int) position, tileSprite);
        }
    }
}
