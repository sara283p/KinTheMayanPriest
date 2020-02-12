using System.Collections;
using UnityEngine;

public class MoveStarViewfinder_Joystick : MonoBehaviour
{
    private SpriteRenderer _renderer;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
//        _this.enabled = false;
    }

    public void DisplayViewfinder(bool val)
    {
        _renderer.enabled = val;
    }

    public void EnemyTooFarEffect()
    {
        StartCoroutine(TooFarAnimation());
    }
    
    IEnumerator TooFarAnimation()
    {
        var solidColor = _renderer.color;
        Color newColor = new Color(255f, 0f, 0f);

        for (int i = 0; i < 3; i++)
        {
            _renderer.material.color = newColor;
            yield return new WaitForSeconds(0.3f);
            _renderer.material.color = solidColor;
            yield return new WaitForSeconds(0.3f);
        }
        
    }
    
    
}