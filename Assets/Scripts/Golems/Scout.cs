using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Scout : Golem
{
    public bool IsFacingRight = true;

    private float _horizontal;
    private float _flightTime;
    private bool _isGroundedForJump, _grounded; //el ultimo es nuevo y sirve para ahorrar llamar varias veces a RaycastHitGround
    private bool _lastGrounded, _isFalling;

    private Queue<ButtonToQueue> _buttonQueue;

    [SerializeField] private float _speed;
    [SerializeField] private float _jumpingForce;
    [SerializeField] private float _holdDiff;
    [SerializeField] private float _groundCheckOffset;
    [SerializeField] private float _inputBufferTime;
    [SerializeField] private float _coyoteTime;
    [SerializeField] private float _onPlatformSpeedMultiplier;

    [SerializeField] private LayerMask _groundLayer;

    private Vector3[] _groundCheckPoints;
    private RaycastHit2D _hit;

    private bool _lerpingToGolem = false;
    private float _lerpTime = 0f;
    Vector2 _lerpTarget;

    protected override void Awake()
    {
        base.Awake();
        IsOnMovingPlatform = false;
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
        if (State == GolemState.Disabled) return;

        _grounded = RayCastHitGround();

        if(_grounded)
        {
            if(!_lastGrounded)
            {
                _animator.SetBool("Landed", true);
                _animator.SetBool("Falling", false);
                _isFalling = false;
            }
            _lastGrounded = true;
        }
        else
        {
            if(!_isFalling && _rb.velocity.y < 0f)
            {
                _isFalling = true;
                _animator.SetBool("Falling", true);
                _animator.SetBool("Jump", false);
                _animator.SetBool("Landed", false);
            }

            _lastGrounded = false;
        }

        if (State == GolemState.BeingLaunched && _grounded) State = GolemState.Enabled;
        if (State == GolemState.Available)
        {
            if(_grounded)
            {
                if (!_rb.isKinematic) TryToStickToGolem();

                _rb.isKinematic = true;
                _rb.velocity = Vector2.zero;
            }
            else if(/*!transform.parent*/ !IsBeingCarried)
            {
                _rb.isKinematic = false;
            }
            
        }

        if (State != GolemState.Enabled || IsTalking) return;

        _horizontal = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));    

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
                    _animator.SetBool("Jump", true);
                    _animator.SetBool("Landed", false);
                    _animator.SetBool("Falling", false);
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

        if (!IsOnMovingPlatform) _rb.velocity = new Vector2(_horizontal * _speed, _rb.velocity.y);
        if (IsOnMovingPlatform)
        {
            _rb.velocity = new Vector2(_horizontal * _speed * _onPlatformSpeedMultiplier, _rb.velocity.y);
            Debug.Log("haiii");
        }
        _animator.SetFloat("Speed", Mathf.Abs(_horizontal * _speed));
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
        if (IsFacingRight && _horizontal < 0f || !IsFacingRight && _horizontal > 0f)
        {
            IsFacingRight = !IsFacingRight;

            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    protected override void NewState()
    {
        base.NewState();

        if(State == GolemState.Enabled)
        {
            IsFacingRight = transform.localScale.x > 0f;
        }
    }
}

public enum ButtonToQueue
{
    Jump
}
