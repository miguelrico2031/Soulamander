using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scout : Golem
{
    private float _horizontal;
    private bool _isFacingRight;
    private float _flightTime;
    private bool _isGrounded;

    //private Queue<KeyCode> _buttonQueue;
    private Queue<ButtonToQueue> _buttonQueue;

    [SerializeField] private float _speed;
    [SerializeField] private float _jumpingForce;
    [SerializeField] private float _holdDiff;
    [SerializeField] private float _groundCheckOffset;
    [SerializeField] private float _inputBufferTime;
    [SerializeField] private float _coyoteTime;

    [SerializeField] private LayerMask _groundLayer;

    private Vector3[] _groundCheckPoints;

    protected override void Awake()
    {
        base.Awake();

        //_buttonQueue = new Queue<KeyCode>();
        _buttonQueue = new Queue<ButtonToQueue>();

        _groundCheckPoints = new Vector3[]
        {
            Vector3.zero,            
            Vector3.right * _collider.bounds.extents.x,
            Vector3.left * _collider.bounds.extents.x
        };
    }
    private void Update()
    {
        if (State == GolemState.BeingLaunched && RayCastHitGround()) State = GolemState.Enabled;
        if (State == GolemState.Available && RayCastHitGround())
        {
            _rb.isKinematic = true;
            _rb.velocity = Vector2.zero;
        }
        if (State != GolemState.Enabled) return;

        _horizontal = Input.GetAxisRaw("Horizontal");    

        if (RayCastHitGround())
        {
            _flightTime = 0f;
            if (_rb.velocity.y <= 0.1f) _isGrounded = true;
        }
        else _flightTime += Time.deltaTime;

        
        //if (Input.GetKeyDown(KeyCode.Space))
        if(Input.GetButtonDown("Jump"))
        {
            _buttonQueue.Enqueue(ButtonToQueue.Jump);
            Invoke(nameof(ClearKeyInQueue), _inputBufferTime);         
        }
        if (_flightTime < _coyoteTime && _isGrounded)
        {
            if (_buttonQueue.Count > 0)
            {
                if (_buttonQueue.Peek() == ButtonToQueue.Jump)
                {
                    _rb.velocity = new Vector2(_rb.velocity.x, _jumpingForce);
                    _isGrounded = false;
                    _buttonQueue.Dequeue();
                }
            }         
        }
        if (Input.GetButtonUp("Jump") && _rb.velocity.y > 0f)
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * _holdDiff);
        

        Flip();
    }

    private void FixedUpdate()
    {
        if (State != GolemState.Enabled) return;

        _rb.velocity = new Vector2(_horizontal * _speed, _rb.velocity.y);
    }

    private void ClearKeyInQueue()
    {
        if (_buttonQueue.Count > 0) _buttonQueue.Dequeue();
    }

    private bool RayCastHitGround()
    {
        foreach(var v in _groundCheckPoints)
        {
            Debug.DrawRay(_collider.bounds.center + v, Vector2.down * (_collider.bounds.extents.y + _groundCheckOffset));
            if (Physics2D.Raycast(_collider.bounds.center + v, Vector2.down, _collider.bounds.extents.y + _groundCheckOffset, _groundLayer).collider)
                return true;
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

public enum ButtonToQueue
{
    Jump
}
