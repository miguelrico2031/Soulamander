using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleWall : PreassureListener
{
    [SerializeField] private bool _initialStateClosed;
    public override void OnPlatePressed()
    {
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

    }

    public override void OnTimerEnd()
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

    private void CloseWall()
    {
        transform.gameObject.SetActive(true);
    }
    private void OpenWall()
    {
        transform.gameObject.SetActive(false);
    }


}
