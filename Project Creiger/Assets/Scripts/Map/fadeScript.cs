using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class FadeScript : MonoBehaviour
{
    public static FadeScript Instance { get; private set; }

    public Image fadeImage;
    public float fadeDuration = 1.5f;
    private Action onFadeComplete;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Este método é chamado automaticamente sempre que uma cena é carregada.
    // Vamos usá-lo para iniciar o FadeIn.
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeIn());
    }

    public void StartFade(Action onComplete)
    {
        onFadeComplete = onComplete;
        StartCoroutine(FadeOut());
    }

    public IEnumerator FadeOut()
    {
        float timer = 0f;
        Color c = fadeImage.color;
        c.a = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1f;
        fadeImage.color = c;

        onFadeComplete?.Invoke();
    }

    public IEnumerator FadeIn()
    {
        float timer = 0f;
        Color c = fadeImage.color;
        c.a = 1f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 0f;
        fadeImage.color = c;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}