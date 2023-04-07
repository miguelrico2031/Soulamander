using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : PreassureListener
{
    [Header("Instance Settings")]
    [SerializeField] private List<Transform> _waypoints;
    [SerializeField] private int _initialWaypoint;
    [SerializeField] private bool _cyclePath;
    [SerializeField] private bool _initialDirectionPositive;
    [SerializeField] private int _numberOfPressurePlates;
    [SerializeField] private float _speed;

    [Header("General Settings")]
    [SerializeField] private GameObject _platform;
    [SerializeField] private float _changeWaypointDistance;

    private bool _isActive;
    private bool _dirPositive;
    private int _currentWaypoint;
    private int _pressurePlatesActive;
    private Rigidbody2D _platformRb;
    

    private void Start()
    {
        _pressurePlatesActive = 0;
        _platform.transform.position = _waypoints[_initialWaypoint].position;
        _currentWaypoint = _initialWaypoint;
        _dirPositive = _initialDirectionPositive;
        _platformRb = _platform.GetComponent<Rigidbody2D>();
        foreach (Transform t in _waypoints)
        {
            t.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public override void OnPlatePressed()
    {
        _pressurePlatesActive++;
        if (_pressurePlatesActive != _numberOfPressurePlates) return;

        _isActive = true;
        _platform.GetComponent<MovingPlatformChild>().OnMove();
    }

    public override void OnPlateUnpressed()
    {
        if (_pressurePlatesActive == _numberOfPressurePlates)
        {
            _isActive = false;
            _platform.GetComponent<MovingPlatformChild>().OnStop();
        }
        _pressurePlatesActive--;
    }

    private void Update()
    {
        if (!_isActive) return;

        if (_dirPositive)
        {
            Vector3 dir = _waypoints[_currentWaypoint + 1].position - _platform.transform.position;
            MoveToNextWaypoint(_waypoints[_currentWaypoint + 1].position);
            if (dir.magnitude <= _changeWaypointDistance)
            {
                _currentWaypoint += 1;
                _platform.transform.position = _waypoints[_currentWaypoint].position;

                if (_currentWaypoint + 1 == _waypoints.Count)
                {
                    if (_cyclePath) _currentWaypoint = -1;
                    if (!_cyclePath) _dirPositive = false;
                }               
            }
        }
        else
        {
            Vector3 dir = _waypoints[_currentWaypoint - 1].position - _platform.transform.position;
            MoveToNextWaypoint(_waypoints[_currentWaypoint - 1].position);
            if (dir.magnitude <= _changeWaypointDistance)
            {
                _currentWaypoint -= 1;
                _platform.transform.position = _waypoints[_currentWaypoint].position;
                if (_currentWaypoint == 0) _dirPositive = true;
            }
        }        
    }

    private void MoveToNextWaypoint(Vector3 target)
    {
        _platform.transform.position = Vector3.MoveTowards(_platform.transform.position, target, _speed * Time.deltaTime);
    }
}
