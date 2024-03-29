using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonKeyboardController : MonoBehaviour
{
    private List<Button> Buttons = new List<Button>();
    private int Index;

    private bool CommandChosen;

    private void OnEnable()
    {
        Index = 0;
    }

    public void Update()
    {
        
        Buttons.Clear();
        foreach (var button in GetComponentsInChildren<Button>())
        {
            Buttons.Add(button);
        }

        if (Buttons.Count == 0)
            return;
        
        if (Input.GetButtonDown("Vertical")) {
            if (Input.GetAxis("Vertical") < 0) {
                Index++;
                if (Index >= Buttons.Count) {
                    Index = 0;
                }
            } else {
                Index--;
                if (Index <= -1) {
                    Index = Buttons.Count - 1;
                }
            }
        }
        
        foreach (var button in Buttons)
        {
            button.image.color = Color.white;
        }
        Buttons[Index].image.color = Color.red;

        if (Input.GetButtonDown("Jump")) {
            Buttons[Index].image.color = Color.magenta;
            Buttons[Index].onClick.Invoke();
        }
    }
}
