using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonJuice : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    ISelectHandler, IDeselectHandler
{
    public float hoverScale = 1.1f;
    public float pressedScale = 0.9f;
    public float animSpeed = 12f;
    public float pulseAmount = 0.01f;   // how far it pulses
    public float pulseSpeed = 1f;       // how fast it pulses

    Vector3 targetScale = Vector3.one;
    Vector3 velocity;

    bool interactable = true;
    bool isHovered;
    bool isPulsing;
    float pulseTimer;


    void Update()
    {
        if (isPulsing)
        {
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                Vector3.one * pulse,
                Time.deltaTime * animSpeed
            );
        }
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (!interactable) return;
        isHovered = true;
        isPulsing = true;
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (!interactable) return;
        isHovered = false;
        isPulsing = false;
        transform.localScale = Vector3.one;
    }

    public void OnSelect(BaseEventData e)
    {
        if (!interactable) return;
        targetScale = Vector3.one * hoverScale;
    }

    public void OnDeselect(BaseEventData e)
    {
        if (!interactable) return;
        targetScale = Vector3.one;
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (!interactable) return;

        isPulsing = false;

        if (bounceRoutine != null)
            StopCoroutine(bounceRoutine);

        bounceRoutine = StartCoroutine(PressBounce());
    }

    IEnumerator PressBounce()
    {
        Vector3 start = transform.localScale;

        // 1 → 0.85
        yield return ScaleTo(Vector3.one * pressDown, bounceTime * 0.4f);

        // 0.85 → 1.15
        yield return ScaleTo(Vector3.one * overshoot, bounceTime * 0.35f);

        // 1.15 → 1
        yield return ScaleTo(Vector3.one, bounceTime * 0.25f);

        bounceRoutine = null;

        if (isHovered)
            isPulsing = true;

    }

    IEnumerator ScaleTo(Vector3 target, float time)
    {
        Vector3 start = transform.localScale;
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, target, t / time);
            yield return null;
        }

        transform.localScale = target;
    }


    public void OnPointerUp(PointerEventData e)
    {
        if (!interactable) return;
        targetScale = Vector3.one * hoverScale;
    }

    Coroutine bounceRoutine;

    public float pressDown = 1.25f;
    public float overshoot = 1.15f;
    public float bounceTime = 0.15f;

}
