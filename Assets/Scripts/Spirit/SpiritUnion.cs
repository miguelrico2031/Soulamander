using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritUnion : MonoBehaviour
{
    public bool CheckGolems = true;

    [SerializeField] private float _unionRadius;
    [SerializeField] private LayerMask _golemLayers;

    private List<Golem> _golemsInArea;
    private Golem _nearestGolem;

    private void Awake()
    {
        _golemsInArea = new List<Golem>();
    }

    private void FixedUpdate()
    {
        if(_golemsInArea.Count >= 2)
        {
            RecalcNearestGolem();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (!CheckGolems) return;
        if ((_golemLayers.value & (1 << collision.gameObject.layer)) <= 0) return;
        

        Golem golem = collision.GetComponentInParent<Golem>();

        AddGolem(golem);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!CheckGolems) return;
        if ((_golemLayers.value & (1 << collision.gameObject.layer)) <= 0) return;

        Golem golem = collision.GetComponentInParent<Golem>();
        RemoveGolem(golem);
    }

    private void AddGolem(Golem golem)
    {
        if (_golemsInArea.Contains(golem)) return;
           
        _golemsInArea.Add(golem);
        RecalcNearestGolem();
    }

    private void RemoveGolem(Golem golem)
    {
        if (!_golemsInArea.Contains(golem)) return;

        _golemsInArea.Remove(golem);

        if(golem == _nearestGolem)
        {
            _nearestGolem.GetComponent<SpriteRenderer>().color = Color.green;
            _nearestGolem = null;
        }

        RecalcNearestGolem();

    }

    private void RecalcNearestGolem()
    {
        if (_golemsInArea.Count == 0) return;

        if (_golemsInArea.Count == 1)
        {
            _nearestGolem = _golemsInArea[0];
            _nearestGolem.GetComponent<SpriteRenderer>().color = Color.red;
            return;
        }

        float currentDistance = float.MaxValue;
        float distance = 0f;
        foreach(Golem golem in _golemsInArea)
        {
            if(!_nearestGolem)
            {
                _nearestGolem = golem;
                currentDistance = Vector2.Distance(golem.transform.position, transform.position);
                continue;
            }

            distance = Vector2.Distance(golem.transform.position, transform.position);
            if (distance < currentDistance)
            {
                _nearestGolem.GetComponent<SpriteRenderer>().color = Color.green;
                _nearestGolem = golem;
                currentDistance = distance;
            }
        }
        _nearestGolem.GetComponent<SpriteRenderer>().color = Color.red;
    }
}
