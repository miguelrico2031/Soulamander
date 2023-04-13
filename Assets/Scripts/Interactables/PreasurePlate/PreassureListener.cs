using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PreassureListener : MonoBehaviour
{
    public GemColor _gemColor;
    public abstract void OnPlatePressed();
    public abstract void OnPlateUnpressed();
}
