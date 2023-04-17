using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Music : MonoBehaviour
{
    public static Music Instance;

    [SerializeField] private AudioClip _desertMusic, _mossMusic, _cityMusic;

    private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance)
        {
            if (Instance != this) Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _audioSource = GetComponent<AudioSource>();
        }
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
        
    }

    public void PlayDesertMusic()
    {
        if (_audioSource.clip == _desertMusic && _audioSource.isPlaying) return;

        _audioSource.clip = _desertMusic;
        _audioSource.Play();
    }

    public void PlayMossMusic()
    {
        if (_audioSource.clip == _mossMusic && _audioSource.isPlaying) return;
        _audioSource.clip = _mossMusic;
        //_audioSource.Play();
        StartCoroutine(MusicFadeIn(2f));
    }

    public void PlayCityMusic()
    {
        if (_audioSource.clip == _cityMusic && _audioSource.isPlaying) return;
        _audioSource.clip = _cityMusic;
        _audioSource.Play();
    }

    public void StopMusic()
    {
        _audioSource.Stop();
    }

    private IEnumerator MusicFadeIn(float duration)
    {
        _audioSource.Stop();
        _audioSource.volume = 0f;
        _audioSource.Play();
        float t = 0f;
        while (t < 1f)
        {
            _audioSource.volume = Mathf.Lerp(0f, 1f, t);

            t += Time.deltaTime / duration;

            yield return null;
        }
        _audioSource.volume = 1f;
    }
}
