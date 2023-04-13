using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("Instance Settings")]
    

    [Header("Unity Setup")]
    public GemColor _gemColor;
    [SerializeField] private float _animationSpeed;
    [SerializeField] private LayerMask _golemLayer;
    [SerializeField] private Transform _ramps;
    //[SerializeField] private BoxCollider2D _collider;   
    [SerializeField] private Transform _animationTargetPos;

    private List<GameObject> _listeners;
    private bool revertingAnimation;
    private float _startingPositionY;
    private bool _isBeingPressed;
    private bool _listenersActive;

    private void Awake()
    {
        _startingPositionY = transform.position.y;
        _listeners = new List<GameObject>();
        foreach (PreassureListener door in GameObject.FindObjectsOfType<PreassureListener>()) if (door._gemColor == _gemColor) _listeners.Add(door.gameObject);
    }

    private void Update()
    {
        if (_isBeingPressed)
        {
            if (!_listenersActive)
            {
                foreach (var listener in _listeners)
                {
                    listener.GetComponent<PreassureListener>().OnPlatePressed();
                }
                _listenersActive = true;
            }
        }
        else
        {
            if (_listenersActive)
            {
                foreach (var listener in _listeners)
                {
                    listener.GetComponent<PreassureListener>().OnPlateUnpressed();
                }
                _listenersActive = false;
            }
        }       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((_golemLayer.value & (1 << collision.gameObject.layer)) <= 0) return;
        
        if (collision.contacts[0].normal.y >= 0f) return;
        collision.transform.SetParent(transform);
        _isBeingPressed = true;
        StartCoroutine(PlateAnimation());

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if ((_golemLayer.value & (1 << collision.gameObject.layer)) <= 0) return;
        collision.transform.SetParent(null);
        _isBeingPressed = false;
        StartCoroutine(RevertAnimation());
    }

    IEnumerator PlateAnimation()
    {
        revertingAnimation = false;
        while (!revertingAnimation)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - _animationSpeed * Time.fixedDeltaTime, transform.position.z);
            _ramps.position  = new Vector3(_ramps.position.x, _ramps.position.y - _animationSpeed * Time.fixedDeltaTime, _ramps.position.z);
            if (transform.position.y <= _animationTargetPos.position.y) break;
            yield return null;        
        }
    }
    IEnumerator RevertAnimation()
    {
        
        revertingAnimation = true;
        while (revertingAnimation)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + _animationSpeed / 4 * Time.fixedDeltaTime, transform.position.z);
            _ramps.position  = new Vector3(_ramps.position.x, _ramps.position.y + _animationSpeed / 4 * Time.fixedDeltaTime, _ramps.position.z);
            if (transform.position.y >= _startingPositionY) break;
            yield return null;            
        }
        revertingAnimation = false;
    }
}

public enum GemColor
{
    Pink,
    Purple,
    Yellow,
    Green,
    Blue
}
