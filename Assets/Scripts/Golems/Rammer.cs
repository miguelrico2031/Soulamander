using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rammer : Golem
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
    [SerializeField] private float _groundCheckOffset;

    [SerializeField] private LayerMask _interactableLayers;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _pushableLayer;
    [SerializeField] private LayerMask _destructibleLayer;

    [SerializeField] private Transform[] _wallCheckPoints;

    private Vector3[] _groundCheckPoints;

    Collider2D _wallCheckCollider;

    protected override void Awake()
    {
        base.Awake();

        _direction = transform.localScale.x > 0 ? -1 : 1;

        _groundCheckPoints = new Vector3[]
       {
            Vector3.zero,
            Vector3.right * _collider.bounds.extents.x,
            Vector3.left * _collider.bounds.extents.x
       };
    }

    private void Update()
    {
        if (State == GolemState.Available && !_isRunning)
        {
            _rb.isKinematic = true;
            _rb.velocity = Vector2.zero;
        }

        if (State == GolemState.BeingLaunched && _isRunning) StopRunning();
        if (State == GolemState.BeingLaunched && RayCastHitGround())
        {
            State = GolemState.Enabled;
            _rb.velocity = Vector3.zero;
        }
        if (State != GolemState.Enabled) return;

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

        //if (_isAtMaxSpeed) Debug.Log("vel max");

        Flip();
    }


    private void FixedUpdate()
    {

        if (State != GolemState.Enabled && State != GolemState.Available) return;
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

        WallCheck();

        _rb.velocity = new Vector2((_isPushing ? _pushSpeed : _speed) * _direction, _rb.velocity.y);
    } 
        
    

    public void StopRunning()
    {
        _speed = 0;
        _isAtMaxSpeed = false;
        _isRunning = false;
        if (State == GolemState.Available)
        {
            _rb.isKinematic = true;
            _rb.velocity = Vector2.zero;
        }
    }

    private void Flip()
    {
        if (_isFacingRight && _direction < 0f || !_isFacingRight && _direction > 0f)
        {
            _isFacingRight = !_isFacingRight;

            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private bool RayCastHitGround()
    {
        foreach (var v in _groundCheckPoints)
        {
            if (Physics2D.Raycast(_collider.bounds.center + v, Vector2.down, _collider.bounds.extents.y + _groundCheckOffset, _groundLayer).collider)
                return true;
        }

        return false;
    }
    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if ((_destructibleLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            if (Mathf.Abs(collision.contacts[0].normal.x) != 1f) return;

                if (_isAtMaxSpeed) collision.gameObject.GetComponent<DestructibleObject>().DestroyObstacle(this);
            
            else StopRunning();
            
            return;
        }
        
        if ((_pushableLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            Debug.Log("entra " + collision.gameObject.GetComponent<PushableObject>().HasHitWall);
            //if (Mathf.Abs(collision.contacts[0].normal.x) != 1f) return;

            if (!collision.gameObject.GetComponent<PushableObject>().HasHitWall)
            {
                
                //collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(_pushSpeed * _direction, _rb.velocity.y);
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
       */
    private void WallCheck()
    {
        foreach(var point in _wallCheckPoints)
        {
            _wallCheckCollider = Physics2D.Raycast(point.position, _isFacingRight ? Vector2.right : Vector2.left, 0.05f, _interactableLayers).collider;

            if (!_wallCheckCollider) continue;
            if (_wallCheckCollider.gameObject == gameObject) continue;

            if ((_groundLayer.value & (1 << _wallCheckCollider.gameObject.layer)) > 0 || _wallCheckCollider.gameObject.layer == gameObject.layer)
            {
                StopRunning();
            }

            else if ((_pushableLayer.value & (1 << _wallCheckCollider.gameObject.layer)) > 0)
            {
                if (!_wallCheckCollider.GetComponent<PushableObject>().HasHitWall)
                {
                    _isPushing = true;
                }
                else
                {
                    _isPushing = false;
                    StopRunning();
                }
            }
            else if ((_destructibleLayer.value & (1 << _wallCheckCollider.gameObject.layer)) > 0)
            {
                if (_isAtMaxSpeed)
                {
                    _wallCheckCollider.GetComponent<DestructibleObject>().DestroyObstacle(this);
                }
                else StopRunning();
            }

            return;
        }
    }

}