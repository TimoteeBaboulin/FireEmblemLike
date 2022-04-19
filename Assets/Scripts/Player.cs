using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Packages.Rider.Editor.UnitTesting;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    private enum CommandTypes
    {
        Move,
        ExchangePosition,
        None
    }
    
    private List<Character> Characters;
    private List<Command> Commands;

    private CommandTypes CommandSetType;

    private int Index;
    private Command Command;

    [Header("UI")]
    public GameObject UIObject;
    [SerializeField]
    private UITracking UI;
    
    private Vector2Int Position;

    [Header("Tilemap")]
    public Tilemap tiles;
    public TileBase tileSprite;
    public TileBase blueSquare;

    private Tilemap Walls;

    private void Awake()
    {
        //Set up the different fields

        Walls = GameObject.FindWithTag("Walls").GetComponent<Tilemap>();
        
        UI = UIObject.GetComponent<UITracking>();
        Commands = new List<Command>();
        Characters = new List<Character>();

        CommandSetType = CommandTypes.None;
        Index = -1;
        
        GameObject charactersParent = GameObject.FindWithTag("Characters");
        foreach (var character in charactersParent.GetComponentsInChildren<Character>())
        {
            Characters.Add(character);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Position = new Vector2Int();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
        {
            MoveCursor();
        }
        
        if (Input.GetButtonDown("Jump")) {
            Select();
        }

        UpdateTilemap();
    }

    void setNewTile(Vector2Int oldPosition, Vector2Int newPosition)
    {
        tiles.SetTile((Vector3Int) oldPosition, null);
        tiles.SetTile((Vector3Int) newPosition, tileSprite);
        
    }

    void MoveCursor()
    {
        if (Index != -1 && CommandSetType == CommandTypes.None)
            return;

        float horizontal = Input.GetAxis("Horizontal");
        
        if (horizontal > 0)
            Position.x += 1;
        else if (horizontal < 0)
            Position.x -= 1;

        float vertical = Input.GetAxis("Vertical");
        
        if (vertical > 0)
            Position.y += 1;
        else if (vertical < 0)
            Position.y -= 1;
    }

    void Select()
    {
        //verifies si on a deja un personnage selectionne
        if (Index != -1 && CommandSetType != CommandTypes.None) {
            switch (CommandSetType)
            {
                case CommandTypes.Move:
                    (Command as MoveCommand).SetMoveTarget(Position);
                    CommandSetType = CommandTypes.None;
                    break;
            }

            if (Command.Execute()) {
                Commands.Add(Command);
                Command = null;
            }
            
            //remets l'index pour pointer sur aucun perso
            Index = -1;
            UI.gameObject.SetActive(false);
        } else {
            //trouves si un personnage se trouve a l'endroit ou le pointeur est
            Index = 0;
            foreach (var character in Characters)
            {
                if (character.GetPosition() == Position) {
                    UI.gameObject.SetActive(true);
                    UI.Character = character.gameObject;
                    return;
                }

                Index++;
            }
            Index = -1;
        }
    }

    public void Undo()
    {
        //verifies qu'il y ait des commandes dans la liste
        if (Commands.Count > 0)
        {
            //Undo la derniere commande
            Commands[Commands.Count-1].Undo();

            //Supprime la de la liste
            Commands.Remove(Commands[Commands.Count - 1]);

            //Genere la nouvelle liste sans l'espace null
            List<Command> newList = new List<Command>();
            foreach (var command in Commands)
            {
                if (command != null)
                    newList.Add(command);
            }

            Commands = newList;
            
        }
        Index = -1;
        UI.gameObject.SetActive(false);
    }

    public void SetMoveCommand()
    {
        CommandSetType = CommandTypes.Move;
        Command = new MoveCommand(Characters[Index]);
    }

    private void ShowMovements()
    {
        Debug.Log(Position);
        var character = Characters[Index];

        List<Vector2Int> possibleMovements = new List<Vector2Int>();
        possibleMovements.Add(character.GetPosition());
        for (int x = 0; x < character.movementPoints; x++)
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
        }

        foreach (var tile in possibleMovements)
        {
            tiles.SetTile(new Vector3Int(tile.x, tile.y, 0), blueSquare);
        }
    }

    private void UpdateTilemap()
    {
        tiles.ClearAllTiles();
        
        if (Index != -1)
        {
            ShowMovements();
        }
        
        tiles.SetTile((Vector3Int) Position, tileSprite);
    }
}
