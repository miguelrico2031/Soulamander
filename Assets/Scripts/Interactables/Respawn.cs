using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] private int _maxIterations;
    [SerializeField] private LayerMask _golemLayer;


    [SerializeField] private Transform _start, _end, _alternative;


    public Vector2 GetRespawnPoint()
    {
        Vector2 point;
        for (int i = 0; i < _maxIterations; i++)
        {
            point = Vector2.Lerp(_start.position, _end.position, Random.value);

            if (!Physics2D.OverlapCircle(point, 0.4f, _golemLayer)) return point;
        }

        return _alternative.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_start.position, _end.position);
    }
}
