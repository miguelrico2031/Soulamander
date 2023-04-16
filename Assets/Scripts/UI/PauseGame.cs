using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    public static PauseGame Instance;

    public bool Paused { get; private set; }

    private bool _isOnClue, _isOnLevelSelect;

    [SerializeField] private GameObject _pausePanel, _bgPanel, _levelSelectPanel, _cluePanel;
    [SerializeField] private TextMeshProUGUI _clueText;
    [SerializeField] private Clues _clues;

    private EventSystem _eventSystem;

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
            _eventSystem = transform.parent.GetComponentInChildren<EventSystem>();
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
        foreach(var eS in FindObjectsOfType<EventSystem>())
        {
            if (eS != _eventSystem) Destroy(eS.gameObject);
        }

        _clueText.text = _clues.GetClue(scene.name);
        Resume();

        _eventSystem.firstSelectedGameObject = _pausePanel.transform.Find("Resume").gameObject;
        
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause")) Pause(!Paused);

        if(Input.GetButtonDown("Cancel"))
        {
            if(_isOnClue)
            {
                _cluePanel.SetActive(false);
                _pausePanel.SetActive(true);
                _eventSystem.SetSelectedGameObject(_pausePanel.transform.Find("Resume").gameObject);
                _isOnClue = false;
            }
            else if(_isOnLevelSelect)
            {
                _levelSelectPanel.SetActive(false);
                _pausePanel.SetActive(true);
                _eventSystem.SetSelectedGameObject(_pausePanel.transform.Find("Resume").gameObject);
                _isOnLevelSelect = false;
            }

            else Resume();
        }
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

        _eventSystem.SetSelectedGameObject(_pausePanel.transform.Find("Resume").gameObject);
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

    public void OnClue(bool onClue) => _isOnClue = onClue;
    public void OnLevelSelect(bool onLevelSelect) => _isOnLevelSelect = onLevelSelect;
}
