using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritDim : MonoBehaviour
{
    [SerializeField] private float _attenuationDuration;

    private Vector3 _scaleDecrement;

    private void Awake()
    {
        _scaleDecrement = new Vector3(Time.fixedDeltaTime, Time.fixedDeltaTime, Time.fixedDeltaTime) / _attenuationDuration;

    }

    private void FixedUpdate()
    {
        if(transform.localScale.x > 0f) transform.localScale -= _scaleDecrement;

    }
}
