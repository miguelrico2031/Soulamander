using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomChanger : MonoBehaviour
{
    [Header("Instance Settings")]
    [SerializeField] int _roomID;
    [SerializeField] private GameObject _progressWall;

    [Header("Unity Setup")]
    [SerializeField] private LayerMask _golemLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((_golemLayer.value & (1 << collision.gameObject.layer)) <= 0) return;
        if (collision.gameObject.GetComponent<Golem>().State != GolemState.Enabled) return;

        ChangeRoom();
    }

    private void ChangeRoom()
    {
        foreach (Golem golem in GameObject.FindObjectsOfType<Golem>())
        {
            if (golem.State == GolemState.Available)
            {
                golem.State = GolemState.Disabled;
            }
        }
        _progressWall.SetActive(true);

        GameObject.FindObjectOfType<SpiritUnion>().RefreshAvailableGolems();
    }
}
