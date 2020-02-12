using System;
using UnityEngine;

[Serializable]
public class Dialogue
{
    public String name;
    [TextArea(3,5)]
    public String[] sentences;
}