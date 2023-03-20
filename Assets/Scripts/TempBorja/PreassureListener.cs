using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PreassureListener : MonoBehaviour
{
    public abstract void OnPlatePressed();
    public abstract void OnPlateUnpressed();
    public abstract void OnTimerEnd();
}
