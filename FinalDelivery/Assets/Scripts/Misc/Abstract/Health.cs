using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Can't be an interface because MonoBehavior is already a class
public abstract class Health : MonoBehaviour
{
    public abstract float GetHealth();
    public abstract float GetMaxHealth();
}
