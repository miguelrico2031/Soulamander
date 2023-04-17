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

    [SerializeField] private DummyScout _dummyScoutPrefab;

    [Header("Custom Scene Change")]
    [SerializeField] private bool _customSceneChange = false;
    [SerializeField] private string _customSceneName;

    public event EventHandler OnGoalReached;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((_golemLayer.value & (1 << collision.gameObject.layer)) <= 0) return;
        if (collision.gameObject.GetComponent<Golem>().State != GolemState.Enabled) return;
        if (OnGoalReached == null) return;
        OnGoalReached(this, EventArgs.Empty);

        var scout = collision.GetComponent<Scout>();
        if(scout)
        {
            var dummy = Instantiate(_dummyScoutPrefab);
            dummy.DummyStart(scout);
        }

        PauseGame.Instance.FadeOut();

        StartCoroutine(WaitBeforeSceneChange());
    }

    IEnumerator WaitBeforeSceneChange()
    {

        yield return new WaitForSeconds(_secondsBeforeSceneChange);


        if (_customSceneChange) SceneManager.LoadScene(_customSceneName);
        
        else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
