using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
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

        rammer.ResetSpeed();

        Destroy(gameObject);
    }
}
