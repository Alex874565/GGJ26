using UnityEngine;
using System.Collections;

public class MenuItemSlideUp : MenuItemBase
{
    public float moveDistance = 200f;

    RectTransform rect;
    Vector2 startPos;
    Vector2 hiddenPos;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
        hiddenPos = startPos + Vector2.up * moveDistance;

        rect.anchoredPosition = hiddenPos;
        gameObject.SetActive(false);
    }

    public override IEnumerator Appear(float delay)
{
    yield return new WaitForSecondsRealtime(delay);
    gameObject.SetActive(true);

    float t = 0;
    while (t < 1f)
    {
        t += Time.unscaledDeltaTime / appearTime; // Use unscaled
        rect.anchoredPosition = Vector2.Lerp(hiddenPos, startPos, t);
        yield return null;
    }
    rect.anchoredPosition = startPos;
}

public override IEnumerator Disappear(float delay)
{
    yield return new WaitForSecondsRealtime(delay);

    float t = 0;
    while (t < 1f)
    {
        t += Time.unscaledDeltaTime / disappearTime; // Use unscaled
        rect.anchoredPosition = Vector2.Lerp(startPos, hiddenPos, t);
        yield return null;
    }
    rect.anchoredPosition = hiddenPos;
    gameObject.SetActive(false);
}
}
