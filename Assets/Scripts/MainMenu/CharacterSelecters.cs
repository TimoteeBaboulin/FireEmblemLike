using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelecters : MonoBehaviour
{
    public List<CharacterSelecter> Selecters;

    private void OnEnable()
    {
        if (PlayerPrefs.GetString("Class1") == null)
        {
            PlayerPrefs.SetString("Class1", "Sword");
            PlayerPrefs.SetString("Class2", "Spear");
            PlayerPrefs.SetString("Class3", "Axe");
        }

        for (int x = 0; x < Selecters.Count; x++)
        {
            Selecters[x].SetPath("Class" + x.ToString());
        }
    }
}
