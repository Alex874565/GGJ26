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

    Vector3 targetScale = Vector3.one;
    Vector3 velocity;

    bool interactable = true;

    void Update()
    {
        transform.localScale = Vector3.SmoothDamp(
            transform.localScale,
            targetScale,
            ref velocity,
            1f / animSpeed
        );
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (!interactable) return;
        targetScale = Vector3.one * hoverScale;
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (!interactable) return;
        targetScale = Vector3.one;
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
        targetScale = Vector3.one * pressedScale;
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (!interactable) return;
        targetScale = Vector3.one * hoverScale;
    }
}
