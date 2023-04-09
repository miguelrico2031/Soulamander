using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private Color _atmosColor;
    [Header("Unity Setup")]
    [SerializeField] private List<SpriteRenderer> _atmosImages;
    private void Awake()
    {
        foreach (SpriteRenderer a in _atmosImages)
        {
            a.transform.gameObject.SetActive(true);
            a.color = _atmosColor;
        }
    }
}
