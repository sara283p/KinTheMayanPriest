using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public GameObject selectedForAttack;
    // Start is called before the first frame update
    void Start()
    {
        selectedForAttack.SetActive(false);
    }

    public void Select()
    {
        selectedForAttack.SetActive(true);
    }

    public void Deselect()
    {
        selectedForAttack.SetActive(false);
    }
}
