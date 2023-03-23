using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpiritDim : MonoBehaviour
{
    public bool IsFading
    {
        get { return _isFading;  }
        set
        {
            if(value != _isFading) _sprite.localScale = _initialScale;
            
            _isFading = value;
        }
    }

    [SerializeField] private Transform _sprite;
    [SerializeField] private float _attenuationDuration;

    private bool _isFading = true;
    private Vector3 _scaleDecrement, _initialScale;

    private void Awake()
    {
        IsFading = true;
        _scaleDecrement = new Vector3(Time.fixedDeltaTime, Time.fixedDeltaTime, Time.fixedDeltaTime) / _attenuationDuration;
        _initialScale = _sprite.localScale;

    }

    private void FixedUpdate()
    {
        if (!IsFading) return;

        if(_sprite.localScale.x > 0f) _sprite.localScale -= _scaleDecrement;
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
