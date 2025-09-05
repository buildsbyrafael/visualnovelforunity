using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image faderImage;
    public float fadeInDuration = 3f;
    public float transitionDuration = 1f;

    private void Awake()
    {
        if (faderImage == null)
            faderImage = GetComponentInChildren<Image>();

        faderImage.color = Color.black;
    }

    public IEnumerator FadeIn()
    {
        yield return StartCoroutine(FadeTo(0f, fadeInDuration));
    }

    public IEnumerator FadeOut()
    {
        yield return StartCoroutine(FadeTo(1f, transitionDuration));
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = faderImage.color.a;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float blend = Mathf.Clamp01(t / duration);
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, blend);
            faderImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        faderImage.color = new Color(0, 0, 0, targetAlpha);
    }

    public IEnumerator FadeTransition(Sprite newSprite, Image backgroundImage)
    {
        yield return FadeOut();

        if (backgroundImage != null)
            backgroundImage.sprite = newSprite;

        yield return FadeIn();
    }
}