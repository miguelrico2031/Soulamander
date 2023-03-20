using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling;
using UnityEngine;

public class Golem1Movement : MonoBehaviour
{
    private float _horizontal;
    private bool _isFacingRight;
    private float _flightTime;
    private Queue<KeyCode> _keyQueue;
    private bool _isGrounded; 

    [SerializeField] private float _speed;
    [SerializeField] private float _jumpingForce;
    [SerializeField] private float _holdDiff;
    [SerializeField] private float _groundCheckOffset;
    [SerializeField] private float _inputBufferTime;
    [SerializeField] private float _coyoteTime;

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private LayerMask _groundLayer;
    private void Start()
    {
        _keyQueue = new Queue<KeyCode>();
    }
    private void Update()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");    

        if (RayCastHitGround())
        {
            _flightTime = 0f;
            if (_rb.velocity.y <= 0)
            {
                _isGrounded = true;
            }
            
        }
        else
        {
            _flightTime += Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _keyQueue.Enqueue(KeyCode.Space);
            Invoke("ClearKeyInQueue", _inputBufferTime);         
        }
        if (_flightTime < _coyoteTime && _isGrounded)
        {
            if (_keyQueue.Count > 0)
            {
                if (_keyQueue.Peek() == KeyCode.Space)
                {
                    _rb.velocity = new Vector2(_rb.velocity.x, _jumpingForce);
                    _isGrounded = false;
                    _keyQueue.Dequeue();
                }
            }         
        }
        if (Input.GetKeyUp(KeyCode.Space) && _rb.velocity.y > 0f)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * _holdDiff);
        }

        Flip();
    }

    private void ClearKeyInQueue()
    {
        if (_keyQueue.Count > 0) _keyQueue.Dequeue();
    }


    private void FixedUpdate()
    {
        _rb.velocity = new Vector2(_horizontal * _speed, _rb.velocity.y);
    }

    private bool RayCastHitGround()
    {
        RaycastHit2D[] raycastHit = new RaycastHit2D[3];

        raycastHit[0] = Physics2D.Raycast(_collider.bounds.center, Vector2.down, _collider.bounds.extents.y + _groundCheckOffset, _groundLayer);

        raycastHit[1] = Physics2D.Raycast(new Vector2(_collider.bounds.center.x + _collider.bounds.extents.x, _collider.bounds.center.y), Vector2.down, _collider.bounds.extents.y + _groundCheckOffset, _groundLayer);

        raycastHit[2] = Physics2D.Raycast(new Vector2(_collider.bounds.center.x - _collider.bounds.extents.x, _collider.bounds.center.y), Vector2.down, _collider.bounds.extents.y + _groundCheckOffset, _groundLayer);

        foreach (var hit in raycastHit)
        {
            if (hit.collider != null)
            {
                return true;
            }
        }
        return false;
    }
    
    //mover a otro script:
    private void Flip()
    {
        if (_isFacingRight && _horizontal < 0f || !_isFacingRight && _horizontal > 0f)
        {
            _isFacingRight = !_isFacingRight;

            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}
