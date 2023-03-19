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
    private float _runTime;
    private float speed;

    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _initialSpeed;
    [SerializeField] private float _pushSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _wallCheckOffsetY;
    [SerializeField] private float _wallCheckOffsetX;

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private LayerMask _destructibleLayer;
    [SerializeField] private LayerMask _pushableLayer;

    private void Update()
    {
        if (!_isRunning)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            if (horizontal != 0) { _direction = horizontal; }
        }      

        if (_isRunning)
        {
            _runTime += Time.deltaTime;
        }
        else if(!_isRunning)
        {
            _runTime = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !_isRunning)
        {
            _isRunning = true;
            speed = _initialSpeed;
        }

        if (_isAtMaxSpeed)
        {
            Debug.Log("vel max");
        }

        
        Flip();
    }


    private void FixedUpdate()
    {
        if (_isRunning) 
        {           
            if (speed < _maxSpeed)
            {
                _isAtMaxSpeed = false;
                speed = Mathf.Clamp((speed + (_acceleration * Time.fixedDeltaTime)), -_maxSpeed, _maxSpeed);
            }
            else
            {
                _isAtMaxSpeed = true;
            }

            if (RayCastHitWall(_wallLayer) != null)
            {
                speed = 0;
                _isAtMaxSpeed = false;
                _isRunning = false;
            }
            else if (RayCastHitWall(_destructibleLayer) != null)
            {
                if (_isAtMaxSpeed)
                {
                    RayCastHitWall(_destructibleLayer).gameObject.SetActive(false);
                    _rb.velocity = new Vector2(speed * _direction, _rb.velocity.y);
                }
                else
                {
                    speed = 0;
                    _isAtMaxSpeed = false;
                    _isRunning = false;
                }                              
            }
            else if (RayCastHitWall(_pushableLayer) != null)
            {
                GameObject r = RayCastHitWall(_pushableLayer);
                if (!r.GetComponent<PushableObject>().HasHitWall)
                {
                    r.GetComponent<Rigidbody2D>().velocity = new Vector2(_pushSpeed * _direction, _rb.velocity.y);
                    _rb.velocity = new Vector2(_pushSpeed * _direction, _rb.velocity.y);
                }
                else
                {
                    speed = 0;
                    _isAtMaxSpeed = false;
                    _isRunning = false;
                }               
                
            }
            else
            {
                _rb.velocity = new Vector2(speed * _direction, _rb.velocity.y);
            }           
        } 
        
    }

    private GameObject RayCastHitWall(LayerMask layer)
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
    }


    //mover a otro script:
    private void Flip()
    {
        if (_isFacingRight && _direction < 0f || !_isFacingRight && _direction > 0f)
        {
            _isFacingRight = !_isFacingRight;

            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}
