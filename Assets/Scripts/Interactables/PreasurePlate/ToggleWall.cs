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
        if (!_initialStateClosed) _wall.SetActive(false);
    }

    public override void OnPlatePressed()
    {
        _pressurePlatesActive++;
        if (_pressurePlatesActive != _numberOfPressurePlates) return;

        if (_initialStateClosed)
        {
            OpenWall();
        }
        else
        {
            CloseWall();
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

    private void CloseWall()
    {
        _wall.SetActive(true);
    }
    private void OpenWall()
    {
        _wall.SetActive(false);
    }


}
