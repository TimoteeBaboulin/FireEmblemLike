using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyManager : MonoBehaviour
{
    private AStar ASharp;
    private List<Character> Enemies;
    private List<Character> Players;

    public static Action OnEnemyTurnEnd;
    
    private bool EnemyTurn;
    private float Timer;
    private int Index;

    public Tilemap Walls;
    
    [Header("Visuals")]
    public Tilemap Visual;
    public TileBase Selected;
    
    private void Awake()
    {
        Player.OnTurnChange += StartEnemyTurn;
        Character.OnDeath += OnCharDeath;

        Players = new List<Character>();
        Enemies = new List<Character>();

        EnemyTurn = false;
        Visual.color = Color.cyan;
        
        foreach (var character in GameObject.FindWithTag("Characters").GetComponentsInChildren<Character>())
        {
            if (character.team == Character.Team.Player) {
                Players.Add(character);
                continue;
            }
            Enemies.Add(character);
        }
    }

    private void Update()
    {
        if (ASharp == null)
            ASharp = AStar.Instance;
        
        if (!EnemyTurn)
            return;
        
        Timer += Time.deltaTime;
        if (Timer >= 1f)
        {
            Timer = 0;
            NextAction();
        }

    }

    private void NextAction()
    {
        Character current = Enemies[Index];

        Dictionary<Vector2Int, int> possibleMovement = current.GetPossibleMovements();

        Dictionary<Vector2Int, int> attackPositions = new Dictionary<Vector2Int, int>();

        int _index = -1;
        foreach (var player in Players)
        {
            _index++;
            foreach (var neighbor in Vector2IntExtension.neighbors)
            {
                Character blockingPlayer;
                if ( Walls.GetTile((Vector3Int) (player.GetPosition()+neighbor)) != null || (Player.Instance.ContainPlayer(player.GetPosition() + neighbor, out blockingPlayer) &&
                    blockingPlayer != current))
                {
                    continue;
                }
                
                if (attackPositions.ContainsKey(player.GetPosition()+neighbor))
                    continue;
                Debug.Log("1");
                attackPositions.Add(player.GetPosition() + neighbor, _index);
            }
        }

        int index = 0;
        int distance = 100;

        List<Vector2Int> path = new List<Vector2Int>();

        foreach (Vector2Int attackPos in attackPositions.Keys)
        {
            List<Vector2Int> newPath = ASharp.GetPath(current.GetPosition(), attackPos);

            if (newPath != null && newPath.Count < distance)
            {
                path = newPath;
                distance = path.Count;
                index = attackPositions[attackPos];
            }
        }

        Visual.ClearAllTiles();
        foreach (var tile in path)
        {
            Visual.SetTile((Vector3Int) tile, Selected);
        }

        if (path.Count - 1 <= current.movementPoints)
        {
            AttackCommand attackCommand = new AttackCommand(current, Players[index]);

            attackCommand.Execute(path[path.Count - 1]);
        }
        else
        {
            MoveCommand moveCommand = new MoveCommand(current.GetPosition(), current);
            
            for (int x = 0; x < current.movementPoints; x++)
            {
                if (!Player.Instance.ContainPlayer(path[current.movementPoints - x]))
                {
                    moveCommand.SetMoveTarget(path[current.movementPoints - x]);
                    break;
                }
            }

            moveCommand.Execute();
        }

        Index++;
        if (Index >= Enemies.Count)
        {
            Index = 0;
            EnemyTurn = false;
            OnEnemyTurnEnd.Invoke();
            ReloadCharacters();
        }
    }

    private void StartEnemyTurn()
    {
        EnemyTurn = true;
        Index = 0;
        Timer = 0;
    }
    
    public void OnCharDeath()
    {
        return;
    }

    private void ReloadCharacters()
    {
        Players.Clear();
        Enemies.Clear();

        foreach (var character in GameObject.FindWithTag("Characters").GetComponentsInChildren<Character>())
        {
            if (character.team == Character.Team.Player)
            {
                Players.Add(character);
                continue;
            }
            
            Enemies.Add(character);
        }
    }
}
