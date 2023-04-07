using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MovingPlatformChild : MonoBehaviour
{
    [SerializeField] private LayerMask _golemLayer;
    private List<Golem> _passengers;
    private bool _isMoving;
    private void Awake()
    {
        _passengers = new List<Golem>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((_golemLayer.value & (1 << collision.gameObject.layer)) <= 0) return;
        if (collision.contacts[0].normal.y < 0f)
        {
            collision.transform.SetParent(transform);
            if(_isMoving) collision.transform.gameObject.GetComponent<Golem>().IsOnMovingPlatform = true;
            _passengers.Add(collision.gameObject.GetComponent<Golem>());
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if ((_golemLayer.value & (1 << collision.gameObject.layer)) <= 0) return;
        collision.transform.SetParent(null);
        collision.transform.gameObject.GetComponent<Golem>().IsOnMovingPlatform = false;
        foreach (Golem g in _passengers)
        {
            if (collision.transform.gameObject == g.transform.gameObject) _passengers.Remove(g);
            break;
        }     
    }

    public void OnMove()
    {
        _isMoving = true;
        foreach (Golem g in _passengers)
        {
            g.IsOnMovingPlatform = true;
        }
    }
    public void OnStop()
    {
        _isMoving = false;
        foreach (Golem g in _passengers)
        {
            g.IsOnMovingPlatform = false;
        }
    }
}
