using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Experimental.Rendering.Universal;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    //multiplicador segun la distancia

    private Camera _camera;
    private PixelPerfectCamera _ppCamera;
    private Vector3 _targetPlayer;
    private List<GameObject> _enabledGolems;
    private Vector3 _staticCamPos;
    private bool _sceneIsEnding;
    private Vector3 _goalPos;
    private float minX, maxX, minY, maxY;

    [Header("Scene Settings")]
    [SerializeField] private bool _followPlayerX;
    [SerializeField] private bool _followPlayerY;
    [SerializeField] private bool _zoomOutStart;
    [SerializeField] private int _cameraSizePPU;

    [Header("General Settings")]
    [SerializeField] private int _startingSizePPU;
    [SerializeField] private float _initialZoomSpeed;
    [SerializeField] private float _cameraFollowSmoothSpeed;
    [SerializeField] private float _cameraToStaticPosSpeed;
    [SerializeField] private float _zOffset;
    [SerializeField] private Transform _cameraBounds;

    public void FollowPlayerY() => _followPlayerY = true;
    public void FollowPlayerX() => _followPlayerX = true;

    private void Awake()
    {
        _cameraBounds.GetComponent<SpriteRenderer>().enabled = false;
        _camera = GetComponent<Camera>();
        _ppCamera = GetComponent<PixelPerfectCamera>();

        if (_zoomOutStart) _ppCamera.assetsPPU = _startingSizePPU;
        else _ppCamera.assetsPPU = _cameraSizePPU;

        foreach (CameraController camera in GameObject.FindObjectsOfType<CameraController>())
        {
            if (camera.gameObject != transform.gameObject) Debug.LogError("Hay m�s de una camara con el componente CameraController en la escena");
        }
    }

    private void Start()
    {
        _sceneIsEnding = false;
       
        FindAnyObjectByType<Goal>().OnGoalReached += OnRoomEnd;
        _staticCamPos = transform.position;
        _enabledGolems = new List<GameObject>();
        foreach (Golem golem in GameObject.FindObjectsOfType<Golem>())
        {
            if (golem.State == GolemState.Enabled)
            {
                _enabledGolems.Add(golem.gameObject);
            }
        }
        if (_followPlayerX)
        {
            if (_enabledGolems.Count == 0)
            {
                transform.position = new Vector3(GameObject.FindObjectOfType<SpiritMovement>().gameObject.transform.position.x, transform.position.y, _zOffset);
            }
            else transform.position = new Vector3(_enabledGolems[0].gameObject.transform.position.x, transform.position.y, _zOffset);
        }
        if (_followPlayerY)
        {
            if (_enabledGolems.Count == 0)
            {
                transform.position = new Vector3(transform.position.x, GameObject.FindObjectOfType<SpiritMovement>().gameObject.transform.position.y, _zOffset);
            }
            else transform.position = new Vector3(transform.position.x, _enabledGolems[0].gameObject.transform.position.y, _zOffset);
        }
        if (!_zoomOutStart)
        {
            if (!_followPlayerX && !_followPlayerY) transform.position = _staticCamPos;
            else
            {
                if (!_followPlayerX) transform.position = new Vector3(_staticCamPos.x, transform.position.y, transform.position.z);
                if (!_followPlayerY) transform.position = new Vector3(transform.position.x, _staticCamPos.x, transform.position.z);
            }
        }
        ReloadBounds();
        if (_zoomOutStart) StartCoroutine(Zoom(_cameraSizePPU, _initialZoomSpeed));
    }

    private IEnumerator Zoom(int targetPPU, float zoomSpeed)
    {
        Debug.Log("hi");
        if (!_ppCamera.enabled) Debug.LogError("Zoom en curso");
        int i = 0;
        while (i < 5) // goofy bugfix
        {
            yield return new WaitForSeconds(Time.deltaTime);
            i++;
        }                  
        bool add = false;
        int orthograficToPPUConversionConstant = Mathf.RoundToInt(_ppCamera.assetsPPU * _camera.orthographicSize);
        //Debug.Log(_ppCamera.assetsPPU + " * " + _camera.orthographicSize + " = " + orthograficToPPUConversionConstant);
        if (targetPPU < _ppCamera.assetsPPU) add = true;        
        _ppCamera.enabled = false;
        while (!add)
        {
            
            _camera.orthographicSize -= zoomSpeed * Time.deltaTime;

            if (orthograficToPPUConversionConstant / _camera.orthographicSize >= targetPPU) break;

            yield return null;
        }
        while (add)
        {
            
            _camera.orthographicSize += zoomSpeed * Time.deltaTime;
            
            if (orthograficToPPUConversionConstant / _camera.orthographicSize <= targetPPU) break;
           
            yield return null;
        }
        _ppCamera.enabled = true;
        _ppCamera.assetsPPU = targetPPU;
    }

    private void Update()
    {
        //Debug.Log(_camera.orthographicSize);
        if (!_sceneIsEnding)
        {
            /*
            if (_camera.orthographicSize < _cameraSize)
            {
                _camera.orthographicSize = Mathf.Clamp((_camera.orthographicSize + _zoomSpeed * Time.deltaTime), 0, _cameraSize);
            }
            else if (_camera.orthographicSize > _cameraSize)
            {
                _camera.orthographicSize = Mathf.Clamp((_camera.orthographicSize - _zoomSpeed * Time.deltaTime), _cameraSize, 1000);
            }
            */
            if (!_followPlayerX) MoveCameraX(_staticCamPos.x, _cameraToStaticPosSpeed, false, false);
            if (!_followPlayerY) MoveCameraY(_staticCamPos.y, _cameraToStaticPosSpeed, false, false);
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
            if (GameObject.FindObjectOfType<SpiritMovement>().gameObject != null)
            {
                if (_followPlayerX) _targetPlayer.x = GameObject.FindObjectOfType<SpiritMovement>().gameObject.transform.position.x;
                if (_followPlayerY) _targetPlayer.y = GameObject.FindObjectOfType<SpiritMovement>().gameObject.transform.position.y;
            }
            
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
        if (_followPlayerX) MoveCameraX(_targetPlayer.x, _cameraFollowSmoothSpeed, false, true);
        if (_followPlayerY) MoveCameraY(_targetPlayer.y, _cameraFollowSmoothSpeed, false, true);
        
    }

    private void OnRoomEnd(object sender, EventArgs e)
    {
        _sceneIsEnding = true;
        _goalPos = GameObject.FindObjectOfType<Goal>().gameObject.transform.position;
    }

    public void OnEffector(bool changeToFollowPlayerX, bool changeToFollowPlayerY, Vector3 newCamStaticPos, int newCameraSize, Transform newCameraBounds, float timer)
    {
        bool previousBoolFollowPlayerX = _followPlayerX;
        bool previousBoolFollowPlayerY = _followPlayerY;
        Vector3 previousCamStaticPos = _staticCamPos;
        int previousCameraSize = _cameraSizePPU;
        Transform previuousCameraBounds = _cameraBounds;

        StartCoroutine(Zoom(newCameraSize, _initialZoomSpeed));

        _cameraSizePPU = newCameraSize;
        _staticCamPos = newCamStaticPos;
        if (newCameraBounds != null) _cameraBounds = newCameraBounds;
        ReloadBounds();
        if (changeToFollowPlayerX) _followPlayerX = true;
        else
        {
            _followPlayerX = false;
        }
        if (changeToFollowPlayerY) _followPlayerY = true;
        else
        {
            _followPlayerY = false;
        }        
        if (timer == 0) return;
        StartCoroutine(RestorePreviousValues(timer, previousBoolFollowPlayerX, previousBoolFollowPlayerY, previousCamStaticPos, previuousCameraBounds, previousCameraSize));
    }

    IEnumerator RestorePreviousValues(float timer, bool previousBoolFollowPlayerX, bool previousBoolFollowPlayerY, Vector3 previousCamStaticPos, Transform previousCameraBounds, int previousCameraSize)
    {
        yield return new WaitForSeconds(timer);

        _cameraBounds = previousCameraBounds;
        ReloadBounds();
        _cameraSizePPU = previousCameraSize;
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
    void ReloadBounds()
    {
        minX = _cameraBounds.position.x - _cameraBounds.localScale.x / 2;
        maxX = _cameraBounds.position.x + _cameraBounds.localScale.x / 2;
        minY = _cameraBounds.position.y - _cameraBounds.localScale.y / 2;
        maxY = _cameraBounds.position.y + _cameraBounds.localScale.y / 2;
    }

    float movePositionX = 0;
    float movePositionY = 0;
    float disY = 0;
    float disX = 0;
    private void MoveCameraX(float desiredPositionX, float speed, bool snapPosition, bool smoothSpeed)
    {
        movePositionX = 0;
        if (smoothSpeed) movePositionX = Mathf.Lerp(transform.position.x, desiredPositionX, speed * Time.deltaTime);
        else movePositionX = Mathf.MoveTowards(transform.position.x, desiredPositionX, speed * Time.deltaTime); 
        if (!snapPosition)
        {
            movePositionX = Mathf.Clamp(movePositionX, minX, maxX);
            transform.position = new Vector3(movePositionX, transform.position.y, _zOffset);       
        }
        else
        {
            movePositionX = Mathf.Clamp(movePositionX, minX, maxX);
            disX = desiredPositionX - movePositionX;
            if (disX > 0.05f) transform.position = new Vector3(movePositionX, transform.position.y, _zOffset);
            else transform.position = new Vector3(desiredPositionX, transform.position.y, _zOffset);
        }
    }
    private void MoveCameraY(float desiredPositionY, float speed, bool snapPosition, bool smoothSpeed)
    {
        movePositionY = 0;
        if (smoothSpeed) movePositionY = Mathf.Lerp(transform.position.y, desiredPositionY, speed * Time.deltaTime);
        else movePositionY = Mathf.MoveTowards(transform.position.y, desiredPositionY, speed * Time.deltaTime);
        if (!snapPosition)
        {
            movePositionY = Mathf.Clamp(movePositionY, minY, maxY);
            transform.position = new Vector3(transform.position.x, movePositionY, _zOffset);
        }
        else
        {
            movePositionY = Mathf.Clamp(movePositionY, minY, maxY);
            disY = desiredPositionY - movePositionY;
            if (disY > 0.05f) transform.position = new Vector3(transform.position.x, movePositionY, _zOffset);
            else transform.position = new Vector3(transform.position.x, desiredPositionY, _zOffset);
        }
    }

}
