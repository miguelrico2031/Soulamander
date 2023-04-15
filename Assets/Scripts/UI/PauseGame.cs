using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public static PauseGame Instance;

    public bool Paused { get; private set; }

    [SerializeField] private GameObject _pausePanel;


    void Update()
    {
        if (Input.GetButtonDown("Pause")) Pause(!Paused);
    }

    public void Pause(bool pause)
    {
        if (pause) Pause();

        else Resume();
    }

    private void Pause()
    {
        Paused = true;
        _pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void Resume()
    {
        Paused = false;
        _pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
