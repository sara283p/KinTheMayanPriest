using System;
using UnityEngine;

[Serializable]
public class Dialogue
{
    public String name;
    [TextArea(2, 2)]
    public String[] sentences;
}