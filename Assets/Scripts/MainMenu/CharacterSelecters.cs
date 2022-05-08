using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelecters : MonoBehaviour
{
    public List<CharacterSelecter> Selecters;

    private void OnEnable()
    {
        if (PlayerPrefs.GetString("Class1") == "")
        {
            PlayerPrefs.SetString("Class1", "Sword");
        }

        if (PlayerPrefs.GetString("Class2") == "")
        {
            PlayerPrefs.SetString("Class2", "Spear");
        }
        
        if (PlayerPrefs.GetString("Class3") == "")
        {
            PlayerPrefs.SetString("Class3", "Axe");
        }

        for (int x = 0; x < Selecters.Count; x++)
        {
            Selecters[x].SetPath("Class" + (x+1).ToString());
        }
    }
}
