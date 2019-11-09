using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class ConstellationGenerator : MonoBehaviour
{
    public GameObject emptyConstellationPrefab;
    public GameObject starPrefab;

    private static ConstellationGenerator _instance;
    private Constellation _emptyConstellation;
    private float _minDistance = 3f;
    private float _initialRadius = 0.2f;
    private int _maxStarNumber = 7;
    private static ContactFilter2D _contactFilter;
    private static System.Random _rand;

    public static ConstellationGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ConstellationGenerator>();
                if (_instance == null)
                {
                    Debug.Log("There must be an active ConstellationGenerator!");
                }
                else
                {
                    Init();
                }
            }

            return _instance;
        }
    }

    private static void Init()
    {
        _contactFilter = new ContactFilter2D {useTriggers = true};
        _contactFilter.SetLayerMask(1 << LayerMask.NameToLayer("Star"));
        _contactFilter.useLayerMask = true;
        _rand = new System.Random();
    }

    public GameObject GenerateConstellation()
    {
        List<GameObject> addedStars = new List<GameObject>();
        GameObject constellation = Instantiate(emptyConstellationPrefab);
        constellation.SetActive(true);
        Constellation constellationComponent = constellation.GetComponent<Constellation>();
        float width = constellationComponent.GetWidth();
        float height = constellationComponent.GetHeight();
        Collider2D[] overlapBuffer = new Collider2D[1];

        for (int i = 0; i < _maxStarNumber; i++)
        {
            
            Vector2 position = Vector2.zero;
            bool goodPosition = false;
            
            GameObject star = Instantiate(starPrefab, constellation.transform);
            star.SetActive(true);
            CircleCollider2D collider = star.GetComponent<CircleCollider2D>();
            collider.radius = _minDistance;
            int j = 0;
            while (!goodPosition)
            {
                position.x = UnityEngine.Random.Range(- width / 2f, width / 2f);
                position.y = UnityEngine.Random.Range(- height / 2f, height / 2f);
                star.transform.position = (Vector2) constellation.transform.position + position;
                bool overlap = false;
                foreach (GameObject go in addedStars)
                {
                    float distance = (go.transform.position - star.transform.position).magnitude;
                    if (distance < _minDistance + _initialRadius)
                    {
                        overlap = true;
                        break;
                    }
                }
                if (!overlap)
                {
                    goodPosition = true;
                }

                j++;
            }
            print("i: " + i + "j: " + j);
            collider.radius = _initialRadius;
            addedStars.Add(star);

        }


        return constellation;
    }
}
