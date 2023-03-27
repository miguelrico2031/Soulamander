using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleWall : PreassureListener
{

    [SerializeField] private GameObject _wall;
    [SerializeField] private bool _initialStateClosed;
    [SerializeField] private int _numberOfPressurePlates;

    private int _pressurePlatesActive;

    private void Start()
    {
        _pressurePlatesActive = 0;
    }

    public override void OnPlatePressed()
    {
        if (_initialStateClosed)
        {
            _pressurePlatesActive++;
            if (_pressurePlatesActive == _numberOfPressurePlates)
            {
                OpenWall();
            }
        }
        else
        {
            _pressurePlatesActive++;
            if (_pressurePlatesActive == _numberOfPressurePlates)
            {
                CloseWall();
            }
        }
    }
    public override void OnPlateUnpressed()
    {
        if (_pressurePlatesActive == _numberOfPressurePlates)
        {
            
            if (_initialStateClosed)
            {
                CloseWall();
            }
            else
            {
                OpenWall();
            }            
        }
        _pressurePlatesActive--;
    }

    public override void OnTimerEnd()
    {
    }

    private void CloseWall()
    {
        _wall.SetActive(true);
    }
    private void OpenWall()
    {
        _wall.SetActive(false);
    }


}
