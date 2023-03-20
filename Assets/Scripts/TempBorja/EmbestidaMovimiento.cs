using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class EmbestidaMovimiento : MonoBehaviour
{
    private float _direction;
    private bool _isFacingRight;
    private bool _isRunning;
    private bool _isAtMaxSpeed;
    private float _speed;
    private float _horizontalInput;
    private bool _isPushing = false;

    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _initialSpeed;
    [SerializeField] private float _pushSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _wallCheckOffsetY;
    [SerializeField] private float _wallCheckOffsetX;

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _destructibleLayer;
    [SerializeField] private LayerMask _pushableLayer;


    private void Awake()
    {
        _direction = transform.localScale.x > 0 ? -1 : 1;
    }

    private void Update()
    {
        if (!_isRunning)
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            if (_horizontalInput != 0) _direction = _horizontalInput;
        }      

        // _runTime = _isRunning ? _runTime + Time.deltaTime : 0f;

        if (Input.GetButtonDown("Jump") && !_isRunning)
        {
            _isRunning = true;
            _speed = _initialSpeed;
        }

        if (_isAtMaxSpeed) Debug.Log("vel max");

        Flip();
    }


    private void FixedUpdate()
    {
        if (!_isRunning) return;

        if (_speed < _maxSpeed)
        {
            _isAtMaxSpeed = false;
            _speed += _acceleration * Time.fixedDeltaTime;
        }
        else
        {
            _isAtMaxSpeed = true;
            _speed = _maxSpeed;
        }


        _rb.velocity = new Vector2((_isPushing ? _pushSpeed : _speed) * _direction, _rb.velocity.y);
    } 
        
    

    public void StopRunning()
    {
        _speed = 0;
        _isAtMaxSpeed = false;
        _isRunning = false;
    }

    /*private GameObject RayCastHitWall(LayerMask layer)
    {
        float dirFlipper;
        if (_isFacingRight)
        {
            dirFlipper = 1;
        }
        else
        {
            dirFlipper = -1;
        }
        RaycastHit2D[] raycastHit = new RaycastHit2D[3];

        raycastHit[0] = Physics2D.Raycast(_collider.bounds.center, dirFlipper * Vector2.right, _collider.bounds.extents.x + _wallCheckOffsetX, layer);
        Debug.DrawRay(_collider.bounds.center, dirFlipper * Vector2.right * (_collider.bounds.extents.x + _wallCheckOffsetX));

        raycastHit[1] = Physics2D.Raycast(new Vector2(_collider.bounds.center.x, _collider.bounds.center.y + _collider.bounds.extents.y  - _wallCheckOffsetY), dirFlipper * Vector2.right, _collider.bounds.extents.y + _wallCheckOffsetX, layer);
        Debug.DrawRay(new Vector2(_collider.bounds.center.x, _collider.bounds.center.y + _collider.bounds.extents.y  - _wallCheckOffsetY), dirFlipper * Vector2.right * (_collider.bounds.extents.x + _wallCheckOffsetX));

        raycastHit[2] = Physics2D.Raycast(new Vector2(_collider.bounds.center.x, _collider.bounds.center.y - _collider.bounds.extents.y  + _wallCheckOffsetY), dirFlipper * Vector2.right, _collider.bounds.extents.y + _wallCheckOffsetX, layer);
        Debug.DrawRay(new Vector2(_collider.bounds.center.x, _collider.bounds.center.y - _collider.bounds.extents.y  + _wallCheckOffsetY), dirFlipper * Vector2.right * (_collider.bounds.extents.x + _wallCheckOffsetX));

        foreach (var hit in raycastHit)
        {
            if (hit.collider != null)
            {
                return hit.transform.gameObject;   
            }
        }
        return null;
    }*/


    private void Flip()
    {
        if (_isFacingRight && _direction < 0f || !_isFacingRight && _direction > 0f)
        {
            _isFacingRight = !_isFacingRight;

            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if ((_groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            if (Mathf.Abs(collision.contacts[0].normal.x) == 1f) StopRunning();
            return;
        }

        if ((_destructibleLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            if (Mathf.Abs(collision.contacts[0].normal.x) != 1f) return;

                if (_isAtMaxSpeed) collision.gameObject.GetComponent<DestructibleObject>().DestroyObstacle(gameObject);
            
            else StopRunning();
            
            return;
        }

        if ((_pushableLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            if (Mathf.Abs(collision.contacts[0].normal.x) != 1f) return;

            if (!collision.gameObject.GetComponent<PushableObject>().HasHitWall)
            {
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(_pushSpeed * _direction, _rb.velocity.y);
                _isPushing = true;
            }
            else
            {
                _isPushing = false;
                StopRunning();
            }
            
            return;
        }


    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if ((_pushableLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            _isPushing = false;
            return;
        }
    }
}
