using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterDisplay : MonoBehaviour
{
    private Image image;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        image = GetComponent<Image>();

        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        image.enabled = false;
        canvasGroup.alpha = 0f;
    }

    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
        image.preserveAspect = true;
        image.enabled = true;
    }

    public void SetPosition(Vector2 position)
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
    }

    public void SetScale(float scale)
    {
        if (image.sprite == null) return;

        var rt = GetComponent<RectTransform>();
        float w = image.sprite.rect.width;
        float h = image.sprite.rect.height;
        rt.sizeDelta = new Vector2(w * scale, h * scale);
    }

    public void SetSize(float width, float height)
    {
        var rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, height);
    }

    public void SetFlip(bool flipX)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (flipX ? -1f : 1f);
        transform.localScale = scale;
    }

    public void ShowInstant()
    {
        canvasGroup.alpha = 1f;
        image.enabled = true;
    }

    public IEnumerator FadeIn(float duration)
    {
        image.enabled = true;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / duration);
            canvasGroup.alpha = Mathf.SmoothStep(0f, 1f, progress);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    public IEnumerator FadeOut(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / duration);
            canvasGroup.alpha = Mathf.SmoothStep(1f, 0f, progress);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        image.enabled = false;
    }

    public bool IsAvailable => image.sprite == null || canvasGroup.alpha == 0f;
    public string CurrentName { get; private set; } = null;

    public void AssignName(string name)
    {
        CurrentName = name;
    }

    public void ClearSlot()
    {
        CurrentName = null;
        image.sprite = null;
        image.enabled = false;
        canvasGroup.alpha = 0f;
    }
}