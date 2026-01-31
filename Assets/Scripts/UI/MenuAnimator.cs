using UnityEngine;
using System.Collections;

public class MenuAnimator : MonoBehaviour
{
    public Animator animator;
    public float stagger = 0.1f;
    public float closeDuration = 0.1f; // match your animation + stagger

    private MenuItemBase[] items;

    void Awake()
    {
        items = GetComponentsInChildren<MenuItemBase>(true);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        animator.Play("MenuOpen");
        StartCoroutine(AppearSequence());
    }

    public void Close()
    {
        StartCoroutine(DisappearSequence());
    }

    IEnumerator AppearSequence()
    {
        for (int i = 0; i < items.Length; i++)
            StartCoroutine(items[i].Appear(i * stagger));

        yield return null; // <-- this fixes the error
    }

    public IEnumerator DisappearSequence()
    {
        float maxDisappearTime = 0f;

        for (int i = 0; i < items.Length; i++)
        {
            float delay = i * stagger; 
            StartCoroutine(items[i].Disappear(delay));

            // track total time
            float duration = delay + items[i].disappearTime;
            if (duration > maxDisappearTime) maxDisappearTime = duration;
        }

        yield return new WaitForSeconds(maxDisappearTime);

        animator.Play("MenuClose");
        yield return new WaitForSeconds(0.1f);

        gameObject.SetActive(false);
    }



}
