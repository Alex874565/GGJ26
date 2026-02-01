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
    // Force the menu to be invisible and inactive immediately after setup
    gameObject.SetActive(false); 
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
    // If this line doesn't run, the object stays gray.
    this.gameObject.SetActive(true); 
    
    // Safety check: is the object actually the one in the scene?
    Debug.Log(gameObject.name + " was told to turn on. Current status: " + gameObject.activeSelf);

    Rebuild();
    if (animator != null)
    {
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.Play("MenuOpen");
    }
    StartCoroutine(AppearSequence());
}


    IEnumerator AppearSequence()
{
    for (int i = 0; i < items.Length; i++)
    {
        // IMPORTANT: The GameObject must be active for its Coroutines to run
        items[i].gameObject.SetActive(true); 
        StartCoroutine(items[i].Appear(i * stagger));
    }
    yield return null;
}

    private Coroutine closeRoutine;

public void Close()
{
    // If the object is already off, don't do anything
    if (!gameObject.activeInHierarchy) return;

    // If we are already closing, don't start it again
    if (closeRoutine != null) StopCoroutine(closeRoutine);
    
    closeRoutine = StartCoroutine(DisappearSequence());
}

public IEnumerator DisappearSequence()
{
    float maxDisappearTime = 0f;

    for (int i = 0; i < items.Length; i++)
    {
        float delay = i * stagger; 
        StartCoroutine(items[i].Disappear(delay));

        float duration = delay + items[i].disappearTime;
        if (duration > maxDisappearTime) maxDisappearTime = duration;
    }

    // Wait using Realtime since we might still be at timeScale 0
    yield return new WaitForSecondsRealtime(maxDisappearTime);

    if (animator != null)
    {
        animator.Play("MenuClose");
        yield return new WaitForSecondsRealtime(0.1f); 
    }

    gameObject.SetActive(false);
    closeRoutine = null; // Reset the tracker
}



}
