using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Command
{
    public Character user { get; set; }
    public bool Execute();
    public void Undo();
}
