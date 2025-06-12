using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class FadeScript : MonoBehaviour
{
    public Image fadeImage; // imagem preta no canvas
    public float fadeDuration = 1.5f;
    private Action onFadeComplete;

    private void Awake()
    {
        Color c = fadeImage.color;
        c.a = 1f; // começa preta
        fadeImage.color = c;
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void StartFade(Action onComplete)
    {
        onFadeComplete = onComplete;
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float timer = 0f;
        Color c = fadeImage.color;

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
}
