using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritMovement : MonoBehaviour
{
    public bool CanMove = true;

    [SerializeField] private float _moveSpeed;

    private Rigidbody2D _rb;
    private Vector2 _moveInput;

    private bool _isPressingMove = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!CanMove) return;
        _moveInput.x = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        _moveInput.y = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

        _isPressingMove = _moveInput.x != 0f || _moveInput.y != 0f;
    }

    private void FixedUpdate()
    {
        if(!CanMove) return;
        if(_isPressingMove) _rb.MovePosition(_rb.position + _moveInput.normalized * _moveSpeed * Time.fixedDeltaTime);
    }
}
