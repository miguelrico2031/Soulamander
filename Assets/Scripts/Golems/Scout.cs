using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scout : Golem
{
    private float _horizontal;
    private bool _isFacingRight;
    private float _flightTime;
    private bool _isGroundedForJump, _grounded; //el ultimo es nuevo y sirve para ahorrar llamar varias veces a RaycastHitGround

    private Queue<ButtonToQueue> _buttonQueue;

    [SerializeField] private float _speed;
    [SerializeField] private float _jumpingForce;
    [SerializeField] private float _holdDiff;
    [SerializeField] private float _groundCheckOffset;
    [SerializeField] private float _inputBufferTime;
    [SerializeField] private float _coyoteTime;

    [SerializeField] private LayerMask _groundLayer;

    private Vector3[] _groundCheckPoints;
    private RaycastHit2D _hit;

    private bool _lerpingToGolem = false;
    private float _lerpTime = 0f;
    Vector2 _lerpTarget;

    protected override void Awake()
    {
        base.Awake();

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
        _grounded = RayCastHitGround();
        if (State == GolemState.BeingLaunched && _grounded) State = GolemState.Enabled;
        if (State == GolemState.Available)
        {
            if(_grounded)
            {
                if (!_rb.isKinematic) TryToStickToGolem();

                _rb.isKinematic = true;
                _rb.velocity = Vector2.zero;
            }
            else if(!transform.parent)
            {
                _rb.isKinematic = false;
            }
            
        }

        if (State != GolemState.Enabled || IsTalking) return;

        _horizontal = Input.GetAxisRaw("Horizontal");    

        if (_grounded)
        {
            _flightTime = 0f;
            if (_rb.velocity.y <= 0.1f) _isGroundedForJump = true;
        }
        else _flightTime += Time.deltaTime;

        if(Input.GetButtonDown("Jump"))
        {
            _buttonQueue.Enqueue(ButtonToQueue.Jump);
            Invoke(nameof(ClearKeyInQueue), _inputBufferTime);         
        }
        if (_flightTime < _coyoteTime && _isGroundedForJump)
        {
            if (_buttonQueue.Count > 0)
            {
                if (_buttonQueue.Peek() == ButtonToQueue.Jump)
                {
                    _rb.velocity = new Vector2(_rb.velocity.x, _jumpingForce);
                    _isGroundedForJump = false;
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
        

        if (State != GolemState.Disabled &&_lerpingToGolem)
        {
            transform.position = Vector2.Lerp(transform.position, _lerpTarget, _lerpTime);
            _lerpTime += Time.fixedDeltaTime * 10f;

            if (Vector2.Distance(transform.position, _lerpTarget) > 0.02f) return;

            _lerpTime = 0f;
            _lerpingToGolem = false;
        }

        if (State != GolemState.Enabled || IsTalking) return;

        _rb.velocity = new Vector2(_horizontal * _speed, _rb.velocity.y);
    }

    private void ClearKeyInQueue()
    {
        if (_buttonQueue.Count > 0) _buttonQueue.Dequeue();
    }

    private bool RayCastHitGround()
    {
        foreach (var v in _groundCheckPoints)
        {
            _hit = Physics2D.Raycast(_collider.bounds.center + v, Vector2.down, _collider.bounds.extents.y + _groundCheckOffset, _groundLayer);
            if (_hit.collider) return true;
        }
        return false;
    }

    protected override void StickToGolem(Golem golemToStick)
    {
        base.StickToGolem(golemToStick);

        _lerpTime = 0f;
        _lerpingToGolem = true;
        _lerpTarget = new Vector2(golemToStick.TopCollider.transform.position.x, transform.position.y);
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
