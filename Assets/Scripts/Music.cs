using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Music : MonoBehaviour
{
    public static Music Instance;

    [SerializeField] private AudioClip _desertMusic, _mossMusic, _cityMusic, _menuMusic;

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
        if (scene.name == "Desert1" || scene.name == "Mossy1") return;

        if(scene.name.Contains("Desert"))
        {
            Debug.Log("desert");
            PlayDesertMusic();
        }

        else if (scene.name.Contains("Moss"))
        {
            Debug.Log("moss");
            PlayMossMusic();

        }

        else if (scene.name.Contains("City"))
        {
            Debug.Log("city");
            PlayCityMusic();
        }
    }

    public void PlayMenuMusic()
    {
        if (_audioSource.clip == _menuMusic && _audioSource.isPlaying) return;

        _audioSource.clip = _menuMusic;
        _audioSource.Play();
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
        StartCoroutine(MusicFade(2f, true));
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

    public void FadeOutMusic()
    {
        StartCoroutine(MusicFade(2f, false));
    }

    public void FadeOutMusic(float duration)
    {
        StartCoroutine(MusicFade(duration, false));
    }
    

    private IEnumerator MusicFade(float duration, bool fadeIn)
    {
        float t = 0f;

        if (fadeIn)
        {
            _audioSource.Stop();
            _audioSource.volume = 0f;
            _audioSource.Play();
        }

        while (t < 1f)
        {
            if(fadeIn) _audioSource.volume = Mathf.Lerp(0f, 1f, t);
            else _audioSource.volume = Mathf.Lerp(1f, 0f, t);

            t += Time.deltaTime / duration;

            yield return null;
        }

        if (!fadeIn) _audioSource.Stop();

        _audioSource.volume = 1f;
    }
}
