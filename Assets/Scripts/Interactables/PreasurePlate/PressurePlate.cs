using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] List<GameObject> _listeners;

    [SerializeField] private LayerMask _golemLayer;
    //[SerializeField] private BoxCollider2D _collider;   

    private bool _isBeingPressed;
    private bool _listenersActive;

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
        
        if (collision.contacts[0].normal.y < 0f)
        {
            _isBeingPressed = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if ((_golemLayer.value & (1 << collision.gameObject.layer)) <= 0) return;
        _isBeingPressed = false;
    }
}
