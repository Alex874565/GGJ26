using UnityEngine;
using System.Collections;

public class MenuItemScale : MenuItemBase
{
    [Range(1f, 2f)]
    public float overshoot = 2f;

    void Awake()
    {
        transform.localScale = Vector3.zero;
        //gameObject.SetActive(false);
    }

    public override IEnumerator Appear(float delay)
{
    if (!allowAppear) yield break;

    // Use Realtime delay
    yield return new WaitForSecondsRealtime(delay);
    gameObject.SetActive(true);

    float t = 0;
    while (t < 1f)
    {
        // Use unscaledDeltaTime to ignore Time.timeScale = 0
        t += Time.unscaledDeltaTime / appearTime;

        float s = EaseOutBack(Mathf.Clamp01(t), overshoot);
        transform.localScale = Vector3.one * s;

        yield return null;
    }
    transform.localScale = Vector3.one;
}

public override IEnumerator Disappear(float delay)
{
    yield return new WaitForSecondsRealtime(delay);

    float t = 0;
    while (t < 1f)
    {
        t += Time.unscaledDeltaTime / disappearTime;
        float s = Mathf.Lerp(1f, 0f, t);
        transform.localScale = Vector3.one * s;
        yield return null;
    }

    transform.localScale = Vector3.zero;
    gameObject.SetActive(false);
}

    // Overshoot easing (like DOTween OutBack)
    float EaseOutBack(float t, float s)
    {
        t -= 1f;
        return 1f + t * t * ((s + 1f) * t + s);
    }
}
