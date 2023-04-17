using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] private LayerMask _golemLayer;
    [SerializeField] private float _secondsBeforeSceneChange;

    public event EventHandler OnGoalReached;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((_golemLayer.value & (1 << collision.gameObject.layer)) <= 0) return;
        if (collision.gameObject.GetComponent<Golem>().State != GolemState.Enabled) return;
        if (OnGoalReached == null) return;
        OnGoalReached(this, EventArgs.Empty);
        StartCoroutine(WaitBeforeSceneChange());
    }
    
    IEnumerator WaitBeforeSceneChange()
    {
        yield return new WaitForSeconds(_secondsBeforeSceneChange);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
