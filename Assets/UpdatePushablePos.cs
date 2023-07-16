using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePushablePos : MonoBehaviour
{
    [SerializeField] Transform _pushable, _notPushable;

    private void FixedUpdate()
    {
        _notPushable.position = _pushable.position;
    }
}
