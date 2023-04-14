using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class juju : MonoBehaviour
{
    [SerializeField] private float _radius;

    private float _angle, _lastRadius;

    void Start()
    {
        _angle = 360f / transform.childCount;
        _lastRadius = _radius;
    }

    // Update is called once per frame
    void Update()
    {
        if (_radius == _lastRadius) return;
        _angle = 360f / transform.childCount;

        for (int i = 0; i < transform.childCount; i++)
        {
            Vector2 newPos = new Vector2(Mathf.Cos(Mathf.Deg2Rad * i * _angle), Mathf.Sin(Mathf.Deg2Rad * i * _angle)) * _radius;
            transform.GetChild(i).localPosition = newPos;
        }

        _lastRadius = _radius;
    }
}
