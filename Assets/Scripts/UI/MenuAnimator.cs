using UnityEngine;
using System.Collections;

public class MenuAnimator : MonoBehaviour
{
    public Animator animator;
    public float stagger = 0.2f;
    public float closeDuration = 0.2f; // match your animation + stagger

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

    IEnumerator DisappearSequence()
    {
        for (int i = items.Length - 1; i >= 0; i--)
            StartCoroutine(items[i].Disappear((items.Length - 1 - i) * stagger));

        yield return new WaitForSeconds(0.25f);
        animator.Play("MenuClose");
        yield return new WaitForSeconds(0.25f);
        gameObject.SetActive(false);
    }
}
