using UnityEngine;

public class MoveStarViewfinder_Joystick : MonoBehaviour
{
    private SpriteRenderer _this;

    void Awake()
    {
        _this = GetComponent<SpriteRenderer>();
//        _this.enabled = false;
    }

    public void DisplayViewfinder(bool val)
    {
        if (val) _this.enabled = true;
        else _this.enabled = false;
    }
    
}