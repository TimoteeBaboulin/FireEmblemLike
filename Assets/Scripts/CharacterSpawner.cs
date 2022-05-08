using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class CharacterSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject PlayerPrefab;
    
    [Header("References")]
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject EnemyManager;
    [SerializeField] private GameObject CharacterParent;
    
    [Header("Weapons")]
    [SerializeField] private Weapon Sword;
    [SerializeField] private Weapon Spear;
    [SerializeField] private Weapon Axe;
    
    [Header("Armors")]
    [SerializeField] private Armor Platemail;
    [SerializeField] private Armor Chainmail;
    [SerializeField] private Armor Leather;
    
    private int _Index;
    [Header("Spawns")]
    [SerializeField] private List<Vector2> Positions;

    private void Awake()
    {
        _Index = 0;
        SetUpGame();
        
    }

    private void SetUpGame()
    {
        SetUpPlayer();
        SetUpEnemies();

        Player.SetActive(true);
        EnemyManager.SetActive(true);
    }

    private void SetUpPlayer()
    {
        while (_Index < 3)
        {
            InstantiateNewPlayer(Positions[_Index]);
            _Index++;
        }

        return;
    }

    private void SetUpEnemies()
    {
        while (_Index < Positions.Count)
        {
            InstantiateNewEnemy(Positions[_Index]);
            _Index++;
        }
    }
    
    private void InstantiateNewPlayer(Vector2 position)
    {
        GameObject player = Instantiate(PlayerPrefab, CharacterParent.transform);
        player.transform.position = position;
        player.GetComponent<Character>().movementPoints = 5;

        string Class = PlayerPrefs.GetString("Class" + (_Index + 1));

        switch (Class)
        {
            case "Sword":
                player.GetComponent<Character>().LoadGear(Sword, Platemail, Character.Team.Player);
                break;
            
            case "Spear":
                player.GetComponent<Character>().LoadGear(Spear, Leather, Character.Team.Player);
                break;
            
            case "Axe":
                player.GetComponent<Character>().LoadGear(Axe, Chainmail, Character.Team.Player);
                break;
        }
        
    }
    
    private void InstantiateNewEnemy(Vector2 position)
    {
        //Instantiate object
        GameObject player = Instantiate(PlayerPrefab, CharacterParent.transform);
        player.transform.position = position;
        player.GetComponent<Character>().movementPoints = 3;

        //Random class
        int classRNG = Random.Range(0, 3);

        switch (classRNG)
        {
            case 0:
                player.GetComponent<Character>().LoadGear(Sword, Platemail, Character.Team.Enemy);
                break;
            
            case 1:
                player.GetComponent<Character>().LoadGear(Axe, Chainmail, Character.Team.Enemy);
                break;
            
            case 2:
                player.GetComponent<Character>().LoadGear(Spear, Leather, Character.Team.Enemy);
                break;
        }
    }
}
