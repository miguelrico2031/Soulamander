using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleWall : PreassureListener
{
    [Header("Instance Settings")]
    [SerializeField] private bool _initialStateClosed;
    [SerializeField] private int _numberOfPressurePlates;

    [Header("Unity Setup")]
    [SerializeField] private GameObject _collider;
    [SerializeField] private Animator _spriteDoorAnimator;
    [SerializeField] private float _colliderDelayAnim;
    

    private int _pressurePlatesActive;

    private void Start()
    {
        _pressurePlatesActive = 0;
        if (!_initialStateClosed) _collider.SetActive(false);
    }

    public override void OnPlatePressed()
    {
        _pressurePlatesActive++;
        if (_pressurePlatesActive != _numberOfPressurePlates) return;

        if (_initialStateClosed)
        {
            StartCoroutine(OpenWall());
        }
        else
        {
            StartCoroutine(CloseWall());
        }
    }
    public override void OnPlateUnpressed()
    {
        if (_pressurePlatesActive == _numberOfPressurePlates)
        {
            
            if (_initialStateClosed)
            {
                StartCoroutine(CloseWall());
            }
            else
            {
                StartCoroutine(OpenWall());
            }            
        }
        _pressurePlatesActive--;
    }

    private IEnumerator CloseWall()
    {
        _spriteDoorAnimator.SetBool("OnOpen", false);
        _spriteDoorAnimator.SetBool("OnClose", true);
        yield return new WaitForSeconds(_colliderDelayAnim);
        _collider.SetActive(true);
    }
    private IEnumerator OpenWall()
    {
        _spriteDoorAnimator.SetBool("OnClose", false);
        _spriteDoorAnimator.SetBool("OnOpen", true);
        yield return new WaitForSeconds(_colliderDelayAnim);
        _collider.SetActive(false);
    }


}
