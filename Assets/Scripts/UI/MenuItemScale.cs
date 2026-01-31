using UnityEngine;
using System.Collections;

public class MenuItemScale : MenuItemBase
{
    public float appearTime = 0.25f;
    public float disappearTime = 0.15f;

    [Range(1f, 2f)]
    public float overshoot = 2f;

    void Awake()
    {
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    public override IEnumerator Appear(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(true);

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / appearTime;

            // Ease out + overshoot
            float s = EaseOutBack(t, overshoot);
            transform.localScale = Vector3.one * s;

            yield return null;
        }

        transform.localScale = Vector3.one;
    }

    public override IEnumerator Disappear(float delay)
    {
        yield return new WaitForSeconds(delay);

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / disappearTime;

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
