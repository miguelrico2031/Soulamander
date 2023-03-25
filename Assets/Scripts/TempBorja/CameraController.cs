using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    private Camera _camera;
    private Vector3 _target;
    private List<GameObject> _enabledGolems;
    private Vector3 _staticCamPos;
    private bool _sceneIsEnding;
    private Vector3 _goalPos;

    [Header("Scene Settings")]
    [SerializeField] private bool _followPlayer;
    [SerializeField] private float _cameraSize;

    [Header("General Settings")]
    [SerializeField] private float _startingSize;
    [SerializeField] private float _zoomSpeed;
    [SerializeField] private float _cameraFollowSmoothSpeed;
    [SerializeField] private float _cameraToStaticPosSpeed;
    [SerializeField] private Vector3 _offset;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _camera.orthographicSize = _startingSize;
    }

    private void Start()
    {
        _sceneIsEnding = false;

        GameObject.FindObjectOfType<Goal>().gameObject.GetComponent<Goal>().OnGoalReached += OnRoomEnd;

        if (!_followPlayer) _staticCamPos = transform.position;
        _enabledGolems = new List<GameObject>();
        foreach (Golem golem in GameObject.FindObjectsOfType<Golem>())
        {
            if (golem.State == GolemState.Enabled)
            {
                _enabledGolems.Add(golem.gameObject);
            }
        }
        if (_enabledGolems.Count == 0) transform.position = GameObject.FindObjectOfType<SpiritMovement>().gameObject.transform.position;
        else transform.position = _enabledGolems[0].gameObject.transform.position;

    }

    private void OnRoomEnd(object sender, EventArgs e)
    {
        _sceneIsEnding = true;
        _goalPos = GameObject.FindObjectOfType<Goal>().gameObject.transform.position;
    }

    private void Update()
    {
        if (!_sceneIsEnding)
        {
            if (_camera.orthographicSize < _cameraSize)
            {
                _camera.orthographicSize += _zoomSpeed * Time.deltaTime;
            }
            if (_followPlayer) return;

            Vector3 desiredPosition = _staticCamPos;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _cameraToStaticPosSpeed * Time.deltaTime);
            smoothedPosition.z = -10;
            Vector3 dir = _staticCamPos - smoothedPosition;
            if (dir.magnitude > 0.05f) transform.position = smoothedPosition;
            else transform.position = _staticCamPos;
        }
        else
        {
            if (_camera.orthographicSize > _startingSize)
            {
                _camera.orthographicSize -= _zoomSpeed * Time.deltaTime;
            }
            Vector3 desiredPosition = _goalPos;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _cameraToStaticPosSpeed * Time.deltaTime);
            smoothedPosition.z = -10;
            transform.position = smoothedPosition;
        }        
    }

    void LateUpdate()
    {
        if (!_followPlayer || _sceneIsEnding) return;

        _enabledGolems = new List<GameObject>(); 
        foreach (Golem golem in GameObject.FindObjectsOfType<Golem>())
        {
            if (golem.State == GolemState.Enabled)
            {
                _enabledGolems.Add(golem.gameObject);
            }
        }

        if (_enabledGolems.Count == 0) _target = GameObject.FindObjectOfType<SpiritMovement>().gameObject.transform.position;

        if (_enabledGolems.Count == 1) _target = _enabledGolems[0].gameObject.transform.position;

        if (_enabledGolems.Count > 1) 
        {
            float x = 0;
            float y = 0;
            foreach (GameObject golem in _enabledGolems)
            {
                x += golem.transform.position.x;
                y += golem.transform.position.y;
            }
            _target = new Vector3(x, y, 0);
        }
        
        Vector3 desiredPosition = _target + _offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _cameraFollowSmoothSpeed * Time.deltaTime);  
        smoothedPosition.z = -10; 
        transform.position = smoothedPosition;
    }
}
