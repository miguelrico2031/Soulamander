using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    public static PauseGame Instance;

    public bool Paused { get; private set; }

    [SerializeField] private GameObject _pausePanel, _bgPanel, _levelSelectPanel, _cluePanel;
    [SerializeField] private TextMeshProUGUI _clueText;
    [SerializeField] private Clues _clues;


    private void Awake()
    {
        if (Instance)
        {
            if(Instance != this) Destroy(transform.parent.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(transform.parent.gameObject);
        }
        
        Resume();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _clueText.text = _clues.GetClue(scene.name);
        Resume();
    }

    private void Update()
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
        _bgPanel.SetActive(true);
        _levelSelectPanel.SetActive(false);
        _cluePanel.SetActive(false);
        Time.timeScale = 0f;
    }

    private void Resume()
    {
        Paused = false;
        _pausePanel.SetActive(false);
        _bgPanel.SetActive(false);
        _levelSelectPanel.SetActive(false);
        _cluePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
