using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritDim : MonoBehaviour
{
    public bool IsFading = true;

    [SerializeField] private Transform _sprite;
    [SerializeField] private float _attenuationDuration;

    private Vector3 _scaleDecrement;

    private void Awake()
    {
        _scaleDecrement = new Vector3(Time.fixedDeltaTime, Time.fixedDeltaTime, Time.fixedDeltaTime) / _attenuationDuration;

    }

    private void FixedUpdate()
    {
        if (!IsFading) return;

        if(_sprite.localScale.x > 0f) _sprite.localScale -= _scaleDecrement;

    }
}
