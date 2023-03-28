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
    [SerializeField] private bool _followPlayerX;
    [SerializeField] private bool _followPlayerY;
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
        LateStart();
    }

    private void LateStart()
    {
        _sceneIsEnding = false;

        GameObject.FindObjectOfType<Goal>().gameObject.GetComponent<Goal>().OnGoalReached += OnRoomEnd;

        if (!_followPlayerX && !_followPlayerY) _staticCamPos = transform.position;
        _enabledGolems = new List<GameObject>();
        foreach (Golem golem in GameObject.FindObjectsOfType<Golem>())
        {
            if (golem.State == GolemState.Enabled)
            {
                Debug.Log("hei");
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
            if (_followPlayerX || _followPlayerY) return;
            MoveCamera(_staticCamPos, _cameraToStaticPosSpeed, true, false);
        }
        else
        {
            if (_camera.orthographicSize > _startingSize)
            {
                _camera.orthographicSize = Mathf.Clamp((_camera.orthographicSize - _zoomSpeed * Time.deltaTime), 0, _cameraSize);
            }
            MoveCamera(_goalPos, _cameraToStaticPosSpeed, true, false);
        }        
    }

    void LateUpdate()
    {
        if (_sceneIsEnding) return;
        if (!_followPlayerY && !_followPlayerX) return;

        _enabledGolems = new List<GameObject>();
        foreach (Golem golem in GameObject.FindObjectsOfType<Golem>())
        {
            if (golem.State == GolemState.Enabled)
            {
                _enabledGolems.Add(golem.gameObject);
            }
        }

        if (_enabledGolems.Count == 0)
        {
            if (_followPlayerX) _targetPlayer.x = GameObject.FindObjectOfType<SpiritMovement>().gameObject.transform.position.x;
            if (_followPlayerY) _targetPlayer.y = GameObject.FindObjectOfType<SpiritMovement>().gameObject.transform.position.y;
        }

        if (_enabledGolems.Count == 1) 
        {
            if (_followPlayerX) _targetPlayer.x = _enabledGolems[0].gameObject.transform.position.x;
            if (_followPlayerY) _targetPlayer.y = _enabledGolems[0].gameObject.transform.position.y;
        }

        if (_enabledGolems.Count > 1) 
        {
            float x = 0;
            if (!_followPlayerX) x = transform.position.x;
            float y = 0;
            if (!_followPlayerY) y = transform.position.y;
            foreach (GameObject golem in _enabledGolems)
            {
                if (_followPlayerX) x += golem.transform.position.x;
                if (_followPlayerY) y += golem.transform.position.y;
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

    public void OnEffector(bool changeToFollowPlayerX, bool changeToFollowPlayerY, Vector3 newCamStaticPos, float newCameraSize, float timer)
    {
        bool previousBoolFollowPlayerX = _followPlayerX;
        bool previousBoolFollowPlayerY = _followPlayerY;
        Vector3 previousCamStaticPos = _staticCamPos;
        float previousCameraSize = _cameraSize;

        _cameraSize = newCameraSize;
        if (changeToFollowPlayerX) _followPlayerX = true;
        else
        {
            _followPlayerX = false;
            if (!changeToFollowPlayerY) _staticCamPos = newCamStaticPos;
        }
        if (changeToFollowPlayerY) _followPlayerY = true;
        else
        {
            _followPlayerY = false;
        }

        if (timer == 0) return;
        StartCoroutine(RestorePreviousValues(timer, previousBoolFollowPlayerX, previousBoolFollowPlayerY, previousCamStaticPos, previousCameraSize));
    }

    IEnumerator RestorePreviousValues(float timer, bool previousBoolFollowPlayerX, bool previousBoolFollowPlayerY, Vector3 previousCamStaticPos, float previousCameraSize)
    {
        yield return new WaitForSeconds(timer);

        _cameraSize = previousCameraSize;
        if (previousBoolFollowPlayerX) _followPlayerX = true;
        else
        {
            _followPlayerX = false;
            if (!previousBoolFollowPlayerY) _staticCamPos = previousCamStaticPos;
        }
        if (previousBoolFollowPlayerY) _followPlayerY = true;
        else
        {
            _followPlayerY = false;
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
