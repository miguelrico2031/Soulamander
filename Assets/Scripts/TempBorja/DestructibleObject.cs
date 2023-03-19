using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    [SerializeField] private bool _stopsGolem;
    public void DestroyObstacle(GameObject golem)
    {
        if (_stopsGolem)
        {
            //particulas y eso
            golem.GetComponent<EmbestidaMovimiento>().StopRunning();
            gameObject.SetActive(false);
        }
        else
        {
            //particulas y eso
            gameObject.SetActive(false);
        }
    }
}
