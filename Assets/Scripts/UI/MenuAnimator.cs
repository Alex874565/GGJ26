using UnityEngine;
using System.Collections;

public class MenuAnimator : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager;

    public Animator animator;
    public float stagger = 0.1f;
    public float closeDuration = 0.1f; // match your animation + stagger

    private MenuItemBase[] items;

    void Start()
    {
        Rebuild();
    }

    public void Rebuild()
    {
        var all = GetComponentsInChildren<MenuItemBase>(true);
        bool hasSave = saveManager != null && saveManager.HasSaveFile();

        var list = new System.Collections.Generic.List<MenuItemBase>();

        foreach (var item in all)
        {
            if (item.requiresSave && !hasSave)
            {
                item.gameObject.SetActive(false);
                item.allowAppear = false;   // <- IMPORTANT
                continue;
            }

            item.allowAppear = true;
            list.Add(item);
        }

        items = list.ToArray();
    }

    public void Open()
    {
        Rebuild(); // <- IMPORTANT
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
