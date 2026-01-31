using UnityEngine;
using TMPro;

public class NarrativeManager : MonoBehaviour
{
    [SerializeField] private TMP_Text narrativeText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowText(string message)
    {
        narrativeText.text = message;
        narrativeText.gameObject.SetActive(true);
    }

    public void HideText()
    {
        narrativeText.gameObject.SetActive(false);
    }
}
