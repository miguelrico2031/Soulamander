using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomChanger : MonoBehaviour
{
    [SerializeField] private GameObject _progressWall;
    [SerializeField] private LayerMask _golemLayer;
    [SerializeField] private List<GameObject> _golemsToDeactivate;

    private void Start()
    {
        _progressWall.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((_golemLayer.value & (1 << collision.gameObject.layer)) <= 0) return;
        if (collision.gameObject.GetComponent<Golem>().State != GolemState.Enabled) return;

        ChangeRoom();
    }

    private void ChangeRoom()
    {
        if(_golemsToDeactivate.Count == 0) {
            Debug.Log(_golemsToDeactivate.Count);
            foreach (Golem golem in GameObject.FindObjectsOfType<Golem>())
            {
                if (golem.State == GolemState.Available)
                {
                    golem.State = GolemState.Disabled;
                }
            }
        }
        else
        {
            foreach (GameObject golem in _golemsToDeactivate)
            {
                if (golem.GetComponent<Golem>().State == GolemState.Available)
                {
                    golem.GetComponent<Golem>().State = GolemState.Disabled;
                }
            }
        }
        GameObject.FindObjectOfType<SpiritUnion>().RefreshAvailableGolems();
        _progressWall.SetActive(true);
    }
}
