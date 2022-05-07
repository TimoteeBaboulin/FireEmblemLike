using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelecter : MonoBehaviour
{
    public string StringName;
    public Text Text;

    public List<string> Classes = new List<string>();
    private int Index;

    private bool _IsSet;


    private void Awake()
    {
        _IsSet = false;
        
        Index = 0;
    }

    private void OnEnable()
    {
        Text.text = Classes[Index];
    }

    public void SetPath(string pathName)
    {
        StringName = pathName;
        _IsSet = true;

        string ClassName = PlayerPrefs.GetString(StringName);

        if (ClassName == null)
            return;
        

        for (int x = 0; x < Classes.Count; x++) {
            if (Classes[x] == ClassName) {
                Index = x;
                break;
            }
        }

        Text.text = ClassName;
    }

    public void Next()
    {
        Index++;
        if (Index == Classes.Count)
            Index = 0;
        
        PlayerPrefs.SetString(StringName, Classes[Index]);
        Text.text = Classes[Index];
    }

    public void Before()
    {
        Index--;
        if (Index < 0)
            Index = Classes.Count - 1;
        
        PlayerPrefs.SetString(StringName, Classes[Index]);
        Text.text = Classes[Index];
    }
}
