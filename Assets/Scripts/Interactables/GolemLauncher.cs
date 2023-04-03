using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class GolemLauncher : MonoBehaviour
{
    [SerializeField] float _launchVelocity;
    [SerializeField] float _maxAngle;
    [SerializeField] float _initialAngle;
    [SerializeField] float _rotationSpeed;
    [SerializeField] Transform _golemHolder;
    [SerializeField] float _activationDistance;
    [SerializeField] float _reloadTimer;

    private Vector2 _direction;
    private GameObject _golem;
    private float _angle;
    private float _timeElapsedSinceLaunch;
    private bool _hasGolem;
    private bool _canBeActivated;

    private void Awake()
    {
        _angle = _initialAngle;
    }

    public void GetGolem(GameObject golem)
    {
        _golem = golem;
        _golem.GetComponent<Golem>().State = GolemState.Available;
        _canBeActivated = false;
    }
    private void Launch()
    {
        _golem.GetComponent<Golem>().State = GolemState.BeingLaunched;
        _golem.GetComponent<Rigidbody2D>().velocity = new Vector2(_launchVelocity * _direction.x, _launchVelocity * _direction.y);
        _golem.transform.rotation = Quaternion.Euler(0, 0, 0);
        _golem = null;
        _hasGolem = false;        
        _timeElapsedSinceLaunch = 0;
    }
    
    private void Update()
    {
        if (!_hasGolem) 
        {
            if (_timeElapsedSinceLaunch < _reloadTimer)
            {
                _timeElapsedSinceLaunch += Time.deltaTime;
            }
            else
            {
                _canBeActivated = true;
            }
            if (!_canBeActivated) return;      
            foreach (Golem golem in GameObject.FindObjectsOfType<Golem>())
            {
                if (golem.State == GolemState.Enabled && golem.CanBeLaunched)
                {
                    Vector3 dir = golem.transform.position - transform.position;
                    if (dir.magnitude <= _activationDistance)
                    {
                        GetGolem(golem.gameObject);
                        _hasGolem = true;
                    }
                }
            }
        }
        
        else
        {
            _golem.transform.position = _golemHolder.position;
            _golem.transform.rotation = _golemHolder.rotation;

            _angle = Mathf.Clamp(_angle - (int)Input.GetAxis("Horizontal") * _rotationSpeed * Time.deltaTime, -_maxAngle, _maxAngle);
            
            if (Input.GetButtonDown("Jump"))
            {
                Launch();
            }
        }

        _direction = transform.up;

        transform.rotation = Quaternion.Euler(0, 0, _angle);
    }
}
