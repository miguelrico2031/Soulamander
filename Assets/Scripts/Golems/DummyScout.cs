using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyScout : MonoBehaviour
{
    private Animator _animator;
    private bool _move = false;
    private float t = 0f, _speed = 0f;
    private bool _right = true;
    private Rigidbody2D _rb;
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    public void DummyStart(Scout scout)
    {
        _animator.SetBool("Enabled", true);
        _speed = scout.GetSpeed();  
        _animator.SetFloat("Speed", _speed / 1.5f);

        transform.position = scout.transform.position;
        transform.localScale = scout.transform.localScale;
        if (transform.localScale.x < 0f) _right = false;

        scout.gameObject.SetActive(false);

        _move = true;
        //_rb.velocity = 2f * _speed * (_right ? Vector2.right : Vector2.left);
    }

    private void FixedUpdate()
    {
        if (!_move) return;

        //transform.position =
        //    Vector2.LerpUnclamped(transform.position, transform.position + transform.right * (_right ? 1f : -1f), t);

        //t += Time.fixedDeltaTime * _speed / 30f;

        _rb.velocity = new Vector2(_speed * (_right ? 1f : -1f), _rb.velocity.y);
    }
}
