using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    private Camera _camera;
    private Vector3 _targetPlayer;
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
    [SerializeField] private float _zOffset;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _camera.orthographicSize = _startingSize;

        foreach (CameraController camera in GameObject.FindObjectsOfType<CameraController>())
        {
            if (camera.gameObject != transform.gameObject) Debug.LogError("Hay más de una camara con el componente CameraController en la escena");
        }
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
        transform.position = new Vector3(transform.position.x, transform.position.y, _zOffset);
    }

    private void Update()
    {
        if (!_sceneIsEnding)
        {
            if (_camera.orthographicSize < _cameraSize)
            {
                _camera.orthographicSize = Mathf.Clamp((_camera.orthographicSize + _zoomSpeed * Time.deltaTime), 0, _cameraSize);
            }
            else if (_camera.orthographicSize > _cameraSize)
            {
                _camera.orthographicSize = Mathf.Clamp((_camera.orthographicSize - _zoomSpeed * Time.deltaTime), _cameraSize, 1000);
            }
            if (_followPlayer) return;
            MoveCamera(_staticCamPos, _cameraToStaticPosSpeed, true, false);
        }
        else
        {
            if (_camera.orthographicSize > _startingSize)
            {
                _camera.orthographicSize = Mathf.Clamp((_camera.orthographicSize - _zoomSpeed * 1.5f * Time.deltaTime), 0, _cameraSize);
            }
            MoveCamera(_goalPos, _cameraToStaticPosSpeed * 1.5f, true, false);
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

        if (_enabledGolems.Count == 0) _targetPlayer = GameObject.FindObjectOfType<SpiritMovement>().gameObject.transform.position;

        if (_enabledGolems.Count == 1) _targetPlayer = _enabledGolems[0].gameObject.transform.position;

        if (_enabledGolems.Count > 1) 
        {
            float x = 0;
            float y = 0;
            foreach (GameObject golem in _enabledGolems)
            {
                x += golem.transform.position.x;
                y += golem.transform.position.y;
            }
            _targetPlayer = new Vector3(x, y, 0);
        }
        MoveCamera(_targetPlayer, _cameraFollowSmoothSpeed, false, true);
    }

    private void OnRoomEnd(object sender, EventArgs e)
    {
        _sceneIsEnding = true;
        _goalPos = GameObject.FindObjectOfType<Goal>().gameObject.transform.position;
    }

    public void OnEffector(bool changeToFollowPlayer, Vector3 newCamStaticPos, float newCameraSize, float timer)
    {
        bool previousBoolFollowPlayer = _followPlayer;
        Vector3 previousCamStaticPos = _staticCamPos;
        float previousCameraSize = _cameraSize;

        _cameraSize = newCameraSize;
        if (changeToFollowPlayer) _followPlayer = true;
        else
        {
            _followPlayer = false;
            _staticCamPos = newCamStaticPos;
        }

        if (timer == 0) return;
        StartCoroutine(RestorePreviousValues(timer, previousBoolFollowPlayer, previousCamStaticPos, previousCameraSize));
    }

    IEnumerator RestorePreviousValues(float timer, bool previousBoolFollowPlayer, Vector3 previousCamStaticPos, float previousCameraSize)
    {
        yield return new WaitForSeconds(timer);

        _cameraSize = previousCameraSize;
        if (previousBoolFollowPlayer) _followPlayer = true;
        else
        {
            _followPlayer = false;
            _staticCamPos = previousCamStaticPos;
        }
    }

    private void MoveCamera(Vector3 desiredPosition, float speed, bool snapPosition, bool smoothSpeed)
    {
        Vector3 movePosition = Vector3.zero;
        if (smoothSpeed) movePosition = Vector3.Lerp(transform.position, desiredPosition, speed * Time.deltaTime);
        else movePosition = Vector3.MoveTowards(transform.position, desiredPosition, speed * Time.deltaTime);
        movePosition.z = _zOffset;
        if (!snapPosition)
        {
            transform.position = movePosition;       
        }
        else
        {
            Vector3 dir = desiredPosition - movePosition;
            if (dir.magnitude > 0.05f) transform.position = movePosition;
            else transform.position = desiredPosition;
        }       
    }
}
