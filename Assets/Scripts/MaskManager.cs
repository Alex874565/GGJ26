using UnityEngine;
using UnityEngine.Events;

public enum MaskType
{
    BASIC,
    SELFNESSNESS,
    HAPPINESS
}

public class MaskManager : MonoBehaviour
{
    public UnityEvent<MaskType> maskPicked;

    public void PickMask(int maskTypeIndex)
    {
        MaskType maskType = (MaskType)maskTypeIndex;
        Debug.Log("Picked mask: " + maskType);
        maskPicked.Invoke(maskType);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
