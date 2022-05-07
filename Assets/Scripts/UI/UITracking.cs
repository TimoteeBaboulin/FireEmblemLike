using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITracking : MonoBehaviour
{
    private Camera Camera;
    private RectTransform Rt;
    
    public GameObject _Character;
    public GameObject ButtonParent;

    public GameObject ButtonPrefab;

    private void Awake()
    {
        Camera = Camera.main;
        Rt = GetComponent<RectTransform>();
        foreach (Transform Component in ButtonParent.transform) {
            if (Component!=ButtonParent.transform)
                Destroy(Component.gameObject);
        }
    }

    private void OnEnable()
    {
        
        
    }

    public void CalculateButtons()
    {
        GetComponentInChildren<UICharacterSheet>().SetUIElements(_Character.GetComponent<Character>());
        List<GameObject> buttons = new List<GameObject>();

        Character character = _Character.GetComponent<Character>();

        if (character.team == Character.Team.Player && !character.GetPlayed())
        {
            if (character.movementPoints > 0)
            {
                buttons.Add(Instantiate(ButtonPrefab, ButtonParent.transform));
                buttons[buttons.Count-1].GetComponentInChildren<Text>().text = "Move";
                buttons[buttons.Count-1].GetComponent<Button>().onClick.AddListener(Player.Instance.SetMoveCommand);
            }
            
            buttons.Add(Instantiate(ButtonPrefab, ButtonParent.transform));
            buttons[buttons.Count-1].GetComponentInChildren<Text>().text = "Attack";
            buttons[buttons.Count-1].GetComponent<Button>().onClick.AddListener(Player.Instance.SetAttackCommand);
            
        }

        if (character.team == Character.Team.Player)
        {
            buttons.Add(Instantiate(ButtonPrefab, ButtonParent.transform));
            buttons[buttons.Count-1].GetComponentInChildren<Text>().text = "Next Turn";
            buttons[buttons.Count-1].GetComponent<Button>().onClick.AddListener(Player.Instance.ChangeTurn);
            
        }
        
        buttons.Add(Instantiate(ButtonPrefab, ButtonParent.transform));
        buttons[buttons.Count - 1].GetComponentInChildren<Text>().text = "Return";
        buttons[buttons.Count - 1].GetComponent<Button>().onClick.AddListener(Player.Instance.ResetCharacterChosen);
        
        ButtonParent.GetComponent<ButtonKeyboardController>().enabled = true;
    }

    private void OnDisable()
    {
        ButtonParent.GetComponent<ButtonKeyboardController>().enabled = false;
        foreach (Transform Component in ButtonParent.transform)
        {
            if (Component!= ButtonParent.transform)
                Destroy(Component.gameObject);
        }
    }

    private void Update()
    {
        if (_Character != null) {
            Vector2 pos = new Vector2();
            pos = _Character.transform.position;
            pos.x += 1;
            pos.y -= 2;
            
            pos = RectTransformUtility.WorldToScreenPoint(Camera, pos);

            pos.x = (pos.x / Screen.width) * 1920;
            pos.y = (pos.y / Screen.height) * 1080;
            
            /*pos.x += Character.GetComponent<SpriteRenderer>().sprite.border.y / 2;
            pos.y += Character.GetComponent<SpriteRenderer>().sprite.border.x / 2;*/
            
            /*if (pos.x > Screen.width * 1 / 2)
                pos.x -= Rt.rect.width;
            if (pos.y > Screen.height * 1 / 2)
                pos.y -= Rt.rect.height;*/
            Rt.anchoredPosition =pos;
        }
    }
}
