using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Jumper : Golem
{
    [SerializeField] private LayerMask _groundLayers;
    [SerializeField] private float _minJumpForce, _maxJumpForce, _minJumpTime, _maxJumpTime, _gravityForce;
    [SerializeField] private ParticleSystem _dustEffect, _maxChargeEffect;

    private Vector2 _gravity;

    private float _horizontalInput, _nextHorizontalInput;
    private bool _isDelayingHorizontalInput = false;
    private float _horizontalDelayTimer = 0f;

    private bool _isHoldingJumpButton = false;
    private float _holdingJumpButtonTime = 0f;

    private bool _isGrounded = false;

    private Transform _northCollider, _westCollider, _eastCollider;

    private Collider2D _grapplingCollider = null, _lastGrapplingCollider = null;

    private bool _lerpingToGolem = false;
    private float _lerpTime = 0f;
    Vector2 _lerpTarget;

    private float _collisionAngle;



    protected override void Awake()
    {
        base.Awake();

        _gravity = _gravityForce * 9.81f * Vector2.down;

        _northCollider = transform.GetChild(1);
        _westCollider = transform.GetChild(2);
        _eastCollider = transform.GetChild(3);

        _northCollider.gameObject.SetActive(false);
        _westCollider.gameObject.SetActive(false);
        _eastCollider.gameObject.SetActive(false);

        _dustEffect.transform.parent = null;
    }

    private void Update()
    {
        if (State == GolemState.Available)
        {
            if (_isGrounded)
            {
                _rb.isKinematic = true;
                _rb.velocity = Vector2.zero;
            }
            else if(/*transform.parent == null*/ !IsBeingCarried) _rb.isKinematic = false;
        }
        if (State != GolemState.Enabled || IsTalking) return;

        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            if (!_isHoldingJumpButton)
            {
                _animator.SetBool("Charge", true); 
            }
            
            _isHoldingJumpButton = true;
            
        }
        

        if (Input.GetButtonUp("Jump") && _isGrounded) Jump();
        else
        {
            if (!_isDelayingHorizontalInput)
            {
                _horizontalInput = _nextHorizontalInput;
                _nextHorizontalInput = Input.GetAxisRaw("Horizontal");
                _nextHorizontalInput = _nextHorizontalInput > 0f ? 1f : (_nextHorizontalInput < 0f ? -1f : 0f);
            }

            if (_nextHorizontalInput == 0 && _horizontalInput != 0) _isDelayingHorizontalInput = true;

            if (_isDelayingHorizontalInput)
            {
                _horizontalDelayTimer += Time.deltaTime;
                if(_horizontalDelayTimer >= 0.1f)
                {
                    _isDelayingHorizontalInput = false;
                    _horizontalDelayTimer = 0f;
                }   
            }   
        }

        if (_isHoldingJumpButton)
        {

            if(_holdingJumpButtonTime < _maxJumpTime) _holdingJumpButtonTime += Time.deltaTime;

            else if(_maxChargeEffect.isStopped) _maxChargeEffect.Play();
            
        }

    }

    private void FixedUpdate()
    {
        if (State != GolemState.Enabled && State != GolemState.Available) return;

        if(_lerpingToGolem)
        {
            transform.position = Vector2.Lerp(transform.position, _lerpTarget, _lerpTime);
            _lerpTime += Time.fixedDeltaTime * 10f;

            if (Vector2.Distance(transform.position, _lerpTarget) > 0.05f) return;

            _lerpTime = 0f;
            _lerpingToGolem = false;
            _northCollider.gameObject.SetActive(true);
            _westCollider.gameObject.SetActive(false);
            _eastCollider.gameObject.SetActive(false);
        }

        if (!_isGrounded && !IsBeingCarried) _rb.velocity += _gravity * Time.fixedDeltaTime;

    }

    private void Jump()
    {
        _isHoldingJumpButton = false;
        _maxChargeEffect.Stop();

        if ((_horizontalInput == -1 && _westCollider.gameObject.activeSelf) 
            || (_horizontalInput == 1 && _eastCollider.gameObject.activeSelf) || IsTalking)
        {
            _holdingJumpButtonTime = 0f;
            _animator.SetBool("CancelJump", true);
            return;
        }

        _dustEffect.transform.SetPositionAndRotation(transform.position, transform.rotation);
        _dustEffect.Play();

        _isGrounded = false;
        _rb.SetRotation(0f);
        _northCollider.gameObject.SetActive(true);
        _westCollider.gameObject.SetActive(false);
        _eastCollider.gameObject.SetActive(false);

        //(valor de x - a) / (b - a)
        float jumpForceFactor = _holdingJumpButtonTime <= _minJumpTime ? 0f : (_holdingJumpButtonTime - _minJumpTime) / (_maxJumpTime - _minJumpTime);

        float jumpForce = Mathf.Lerp(_minJumpForce, _maxJumpForce, jumpForceFactor);

        Vector2 jumpDirection = new Vector2(_horizontalInput / 2f, 1f).normalized;
        

        _rb.velocity = jumpDirection * jumpForce;


        _holdingJumpButtonTime = 0f;

        _animator.SetBool("Jump", true);
        _animator.SetBool("Land", false);
        _animator.SetBool("Charge", false);
        
    }
        
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (State != GolemState.Enabled && State != GolemState.Available) return;

        if ((_groundLayers.value & (1 << collision.gameObject.layer)) <= 0) return;
        if (collision.transform.parent == transform) return;

        foreach(var contact in collision.contacts)
        {
            //if(contact.normal.y < 0f) continue;
            if (((Vector2)transform.position - contact.point).y < 0f) continue;

            _isGrounded = true;
            

            float newAngle = Vector2.SignedAngle(transform.up, collision.contacts[0].normal);

            _rb.velocity = Vector2.zero;
            _collisionAngle = newAngle;

            _grapplingCollider = collision.collider;
            _lastGrapplingCollider = _grapplingCollider;    

            if (((_groundGolemLayer.value & (1 << _grapplingCollider.gameObject.layer)) > 0)
                && !_grapplingCollider.transform.parent.TryGetComponent<Jumper>(out var c))
            {
                _lerpTime = 0f;
                _lerpingToGolem = true;
                _lerpTarget = (Vector2)(_grapplingCollider.transform.position + (transform.position - _feet.position) * 0.8f);
                _animator.SetBool("CancelJump", false);
                _animator.SetBool("Jump", false);
                _animator.SetBool("Land", true);
                
                return;
            }

            if (Mathf.Abs(_collisionAngle) < 10f)
            {
                _northCollider.gameObject.SetActive(true);
                _westCollider.gameObject.SetActive(false);
                _eastCollider.gameObject.SetActive(false);
            }
            else if(_collisionAngle < 0f)
            {
                _northCollider.gameObject.SetActive(false);
                _westCollider.gameObject.SetActive(true);
                _eastCollider.gameObject.SetActive(false);
            }
            else
            {
                _northCollider.gameObject.SetActive(false);
                _westCollider.gameObject.SetActive(false);
                _eastCollider.gameObject.SetActive(true);
            }

            _rb.SetRotation(_collisionAngle);
            _animator.SetBool("CancelJump", false);
            _animator.SetBool("Jump", false);
            _animator.SetBool("Land", true);

            return;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (State != GolemState.Enabled && State != GolemState.Available) return;
        if (_grapplingCollider != collision.collider) return;
        if (/*transform.parent != null*/ IsBeingCarried) return;


        _grapplingCollider = null;
        _isGrounded = false;
        _rb.SetRotation(0f);
        _northCollider.gameObject.SetActive(true);
        _westCollider.gameObject.SetActive(false);
        _eastCollider.gameObject.SetActive(false);

        _animator.SetBool("Jump", true);
        _animator.SetBool("Land", false);
        _animator.SetBool("Charge", false);
    }

    protected override void NewState()
    {
        base.NewState();

        if(State == GolemState.Enabled)
        {
            Collider2D col = Physics2D.OverlapCircle(_feet.position, 0.1f, _groundLayers);
            if(col)
            {
                _isGrounded = true;

            }
        }

        else if(State == GolemState.Available)
        {
            _animator.SetBool("CancelJump", false);
            _animator.SetBool("Jump", false);
            _animator.SetBool("Land", true);
            _maxChargeEffect.Stop();

            _isHoldingJumpButton = false;
            _holdingJumpButtonTime = 0f;
        }
    }

    protected override void ToggleCarryGolem(bool newState)
    {
        base.ToggleCarryGolem(newState);

    }
}
