using UnityEngine;
using System.Collections;

public class MenuAnimator : MonoBehaviour
{
    public Animator animator;
    public float stagger = 0.08f;

    private MenuItemAnim[] items;

    void Awake()
    {
        items = GetComponentsInChildren<MenuItemAnim>(true);
    }

    void Start()
    {
        Open(); // TEMP: auto open when scene starts
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
