using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    private enum CommandTypes
    {
        Move,
        Attack,
        None
    }

    public static Action OnTurnChange;

    public static Player Instance;
    public List<TileBase> numbers;
    private Tilemap NumberTilemap;

    private List<Character> PlayerCharacters;
    private List<Character> EnemyCharacters;
    private List<Command> Commands;

    private CommandTypes CommandSetType;

    private int Index;
    private Command Command;

    [Header("UI")] public GameObject UIObject;
    [SerializeField] private UITracking UI;

    private Vector2Int Position;

    [Header("Tilemap")] public Tilemap tiles;
    public Tilemap selectedTilemap;
    public TileBase redTileSprite;
    public TileBase selectedTile;
    public TileBase blueSquare;

    private Tilemap Walls;

    private void Awake()
    {
        EnemyManager.OnEnemyTurnEnd += EnemyTurnOver;
        
        //Set up the different fields
        if (Player.Instance == null)
            Instance = this;
        NumberTilemap = GameObject.FindWithTag("NumberTiles").GetComponent<Tilemap>();

        Character.OnDeath += OnCharDeath;

        Walls = GameObject.FindWithTag("Walls").GetComponent<Tilemap>();

        UI = UIObject.GetComponent<UITracking>();
        Commands = new List<Command>();
        PlayerCharacters = new List<Character>();
        EnemyCharacters = new List<Character>();

        CommandSetType = CommandTypes.None;
        Index = -1;

        GameObject charactersParent = GameObject.FindWithTag("Characters");
        foreach (var character in charactersParent.GetComponentsInChildren<Character>())
        {
            if (character.team == Character.Team.Player)
                PlayerCharacters.Add(character);
            if (character.team == Character.Team.Enemy)
                EnemyCharacters.Add(character);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Position = PlayerCharacters[0].GetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
        {
            MoveCursor();
        }

        if (Input.GetButtonDown("Jump"))
        {
            Select();
        }

        UpdateTilemap();
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
        if (Index != -1 && CommandSetType != CommandTypes.None)
        {
            if (CheckCommand() && Command.Execute())
            {
                Commands.Add(Command);

                if (!Command.undoable)
                    Commands = new List<Command>();

                Command = null;
            }

            //remets l'index pour pointer sur aucun perso
            Index = -1;
            NumberTilemap.ClearAllTiles();
            UI.gameObject.SetActive(false);
            CommandSetType = CommandTypes.None;
        }
        else
        {
            //trouves si un personnage se trouve a l'endroit ou le pointeur est
            Index = 0;
            foreach (var character in PlayerCharacters)
            {
                if (character.GetPosition() == Position)
                {
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
            Commands[Commands.Count - 1].Undo();

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
        NumberTilemap.ClearAllTiles();
    }

    public void SetMoveCommand()
    {
        CommandSetType = CommandTypes.Move;
        Command = new MoveCommand(PlayerCharacters[Index]);
    }

    private void ShowMovements()
    {
        var character = PlayerCharacters[Index];

        Dictionary<Vector2Int, int> possibleMovements = character.GetPossibleMovements();

        foreach (var tile in possibleMovements.Keys)
        {
            tiles.SetTile(new Vector3Int(tile.x, tile.y, 0), blueSquare);
            if (possibleMovements[tile] != 0)
            {
                NumberTilemap.SetTile((Vector3Int) tile, numbers[possibleMovements[tile] - 1]);
            }

            Character characterIn;
            if (character.GetPlayed() || (ContainPlayer(tile, out characterIn) && characterIn != character))
                continue;
            foreach (var neighbor in Vector2IntExtension.neighbors)
            {
                if (!possibleMovements.ContainsKey(tile + neighbor))
                    tiles.SetTile((Vector3Int) (tile + neighbor), redTileSprite);
            }
        }

        if (character.GetPlayed())
            return;
        foreach (var charactersVar in PlayerCharacters)
        {
            Vector2Int characterPosition = charactersVar.GetPosition();
            if (possibleMovements.ContainsKey(characterPosition))
            {
                tiles.SetTile((Vector3Int) characterPosition, redTileSprite);
                NumberTilemap.SetTile((Vector3Int) characterPosition, null);
            }
        }
    }

    private void UpdateTilemap()
    {
        tiles.ClearAllTiles();
        selectedTilemap.ClearAllTiles();

        if (Index != -1)
        {
            ShowMovements();
        }

        selectedTilemap.SetTile((Vector3Int) Position, selectedTile);
    }

    public void SetAttackCommand()
    {
        CommandSetType = CommandTypes.Attack;
        Command = new AttackCommand(PlayerCharacters[Index]);
    }

    public bool CheckCommand()
    {
        switch (CommandSetType)
        {
            case CommandTypes.Move:
                foreach (var character in PlayerCharacters)
                {
                    if (character.GetPosition() == Position)
                    {
                        CommandSetType = CommandTypes.None;
                        Command = null;
                        UI.gameObject.SetActive(false);
                        NumberTilemap.ClearAllTiles();
                        Index = -1;
                        return false;
                    }
                }

                (Command as MoveCommand).SetMoveTarget(Position);
                CommandSetType = CommandTypes.None;
                return true;

            case CommandTypes.Attack:
                foreach (var character in PlayerCharacters)
                {
                    if (character == Command.user)
                        continue;
                    if (character.GetPosition() == Position)
                    {
                        (Command as AttackCommand).target = character;
                        CommandSetType = CommandTypes.None;
                        return true;
                    }
                }

                return false;
        }

        return false;
    }

    public bool ContainPlayer(Vector2Int position, out Character playerIn)
    {
        foreach (var character in PlayerCharacters)
        {
            if (character.GetPosition() == position)
            {
                playerIn = character;
                return true;
            }
        }

        foreach (var character in EnemyCharacters)
        {
            if (character.GetPosition() == position)
            {
                playerIn = character;
                return true;
            }
        }

        playerIn = null;
        return false;
    }

    public bool ContainPlayer(Vector2Int position)
    {
        foreach (var character in PlayerCharacters)
        {
            if (character.GetPosition() == position)
                return true;
        }

        return false;
    }

    public void ChangeTurn()
    {
        OnTurnChange.Invoke();
        UI.gameObject.SetActive(false);
        Index = 1;
        CommandSetType = CommandTypes.None;
        Command = null;
        Commands = new List<Command>();
        NumberTilemap.ClearAllTiles();
        tiles.ClearAllTiles();
    }

    public void OnCharDeath()
    {
        foreach (var character in PlayerCharacters)
        {
            if (character.health <= 0)
            {
                PlayerCharacters.Remove(character);
            }
        }
    }

    public void EnemyTurnOver()
    {
        Index = -1;
    }
}
