using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    [SerializeField] private bool _stopsGolem;
    public void DestroyObstacle(Rammer rammer)
    {
        //if (_stopsGolem)
        //{
        //    //particulas y eso
        //    rammer.StopRunning();
        //    Destroy(gameObject);
        //}
        //else
        //{
        //    //particulas y eso
        //    Destroy(gameObject);
        //}

        if (_stopsGolem) rammer.StopRunning();

        Destroy(gameObject);
    }
}
