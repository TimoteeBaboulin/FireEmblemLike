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

    private bool PlayerTurn;
    private bool _CursorPause;
    
    public List<TileBase> numbers;
    private Tilemap NumberTilemap;

    private List<Character> PlayerCharacters;
    private List<Character> EnemyCharacters;
    private List<Command> Commands;

    private CommandTypes CommandSetType;
    
    private Character CharacterChosen;
    private Command Command;
    
    private UITracking UI;

    private Vector2Int Position;

    [Header("Tilemap")] public Tilemap tiles;
    public Tilemap selectedTilemap;
    public TileBase redTileSprite;
    public TileBase selectedTile;
    public TileBase blueSquare;

    private Tilemap Walls;

    private void Awake()
    {
        _CursorPause = false;
        
        PlayerTurn = true;
        EnemyManager.OnEnemyTurnEnd += EnemyTurnOver;
        
        //Set up the different fields
        if (Player.Instance == null)
            Instance = this;

        GameObject tilemaps = new GameObject();
        tilemaps.AddComponent<Tilemap>();
        NumberTilemap = tilemaps.GetComponent<Tilemap>();

        Character.OnDeath += OnCharDeath;

        Walls = GameObject.FindWithTag("Walls").GetComponent<Tilemap>();

        UI = GameObject.FindWithTag("UI").GetComponent<UITracking>();
        UI.gameObject.SetActive(false);
        
        Commands = new List<Command>();

        CommandSetType = CommandTypes.None;
        CharacterChosen = null;
        
        PlayerCharacters = new List<Character>();
        EnemyCharacters = new List<Character>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject charactersParent = GameObject.FindWithTag("Characters");
        
        foreach (var character in charactersParent.GetComponentsInChildren<Character>())
        {
            if (character.team == Character.Team.Player)
                PlayerCharacters.Add(character);
            if (character.team == Character.Team.Enemy)
                EnemyCharacters.Add(character);
        }
        
        Position = PlayerCharacters[0].GetPosition();
        
        Camera.main.GetComponent<CameraMovement>().SetPosition(Position);
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerTurn)
            return;
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
        {
            if (!_CursorPause)
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
        
        Camera.main.GetComponent<CameraMovement>().SetGoal(Position);
    }

    void HandleCommand()
    {
        if (CheckCommand())
        {
            Command.Execute();
            
            if (!Command.undoable)
                Commands.Clear();
            else
                Commands.Add(Command);

            Command = null;
        }
        
        ResetCharacterChosen();
    }
    
    void Select()
    {
        if (CharacterChosen != null && CommandSetType == CommandTypes.None)
            return;
        //verifies si on a deja un personnage selectionne
        if (CharacterChosen != null && CommandSetType != CommandTypes.None)
        {
            HandleCommand();
        }
        else
        {
            if (ContainPlayer(Position, out Character character))
            {
                CharacterChosen = character;
                _CursorPause = true;
                UI.gameObject.SetActive(true);
                UI._Character = character.gameObject;
                UI.CalculateButtons();
            }
        }
    }

    public void Undo()
    {
        ResetUI();
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

        ResetCharacterChosen();
    }

    public void SetMoveCommand()
    {
        ResetUI();
        CommandSetType = CommandTypes.Move;
        Command = new MoveCommand(CharacterChosen);
        _CursorPause = false;
    }

    private void ShowMovements()
    {
        if (CharacterChosen.GetPlayed()) {
            tiles.SetTile((Vector3Int) CharacterChosen.GetPosition(), redTileSprite);
            return;
        }
        
        Dictionary<Vector2Int, int> possibleMovements = CharacterChosen.GetPossibleMovements();

        foreach (var tile in possibleMovements.Keys)
        {
            if (ContainPlayer(tile))
                tiles.SetTile((Vector3Int) tile, redTileSprite);
            else
                tiles.SetTile(new Vector3Int(tile.x, tile.y, 0), blueSquare);
            
            if (possibleMovements[tile] != 0)
            {
                NumberTilemap.SetTile((Vector3Int) tile, numbers[possibleMovements[tile] - 1]);
            }

            Character characterIn;
            if (ContainPlayer(tile, out characterIn) && characterIn != CharacterChosen)
                continue;
            
            foreach (var neighbor in Vector2IntExtension.neighbors)
            {
                if (!possibleMovements.ContainsKey(tile + neighbor))
                    tiles.SetTile((Vector3Int) (tile + neighbor), redTileSprite);
            }
        }
    }

    private void UpdateTilemap()
    {
        tiles.ClearAllTiles();
        selectedTilemap.ClearAllTiles();

        if (CharacterChosen != null)
        {
            ShowMovements();
        }

        selectedTilemap.SetTile((Vector3Int) Position, selectedTile);
    }

    public void SetAttackCommand()
    {
        CommandSetType = CommandTypes.Attack;
        Command = new AttackCommand(CharacterChosen);
        _CursorPause = false;
        ResetUI();
    }

    public bool CheckCommand()
    {
        switch (CommandSetType)
        {
            case CommandTypes.Move:
                if (ContainPlayer(Position))
                    return false;
                
                (Command as MoveCommand).SetMoveTarget(Position);
                CommandSetType = CommandTypes.None;
                return true;

            case CommandTypes.Attack:
                if (ContainPlayer(Position, out Character character) && character != CharacterChosen) {
                    (Command as AttackCommand).target = character;
                    return true;
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

        foreach (var character in EnemyCharacters)
        {
            if (character.GetPosition() == position)
                return true;
        }
        
        return false;
    }

    public void ChangeTurn()
    {
        ResetCharacterChosen();
        PlayerTurn = false;
        OnTurnChange.Invoke();
        Commands = new List<Command>();
    }

    public void OnCharDeath()
    {
        int index = -1;

        for (int x = 0; x < PlayerCharacters.Count; x++)
        {
            if (PlayerCharacters[x].health <= 0)
            {
                index = x;
            }
        }

        if (index != -1)
            PlayerCharacters.Remove(PlayerCharacters[index]);

        index = -1;

        for (int x = 0; x < EnemyCharacters.Count; x++)
        {
            if (EnemyCharacters[x].health <= 0)
            {
                index = x;
            }
        }

        if (index != -1)
            EnemyCharacters.Remove(EnemyCharacters[index]);

        /*if (PlayerCharacters.Count == 0)
        */
            
    }

    public void EnemyTurnOver()
    {
        _CursorPause = false;
        PlayerTurn = true;
        Camera.main.GetComponent<CameraMovement>().ForcePosition(PlayerCharacters[0].GetPosition());
        Position = PlayerCharacters[0].GetPosition();
    }

    public void ResetCharacterChosen()
    {
        _CursorPause = false;
        
        ResetUI();
        CharacterChosen = null;
        NumberTilemap.ClearAllTiles();
        tiles.ClearAllTiles();
        Command = null;
        CommandSetType = CommandTypes.None;
    }

    public void ResetUI()
    {
        UI.gameObject.SetActive(false);
    }
}
