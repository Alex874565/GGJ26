using UnityEngine;
using UnityEngine.Events;

public enum MaskType
{
    SELFNESSNESS,
    HAPPINESS,
    CONFIDENCE
}

public class MaskManager : MonoBehaviour
{
    public UnityEvent<MaskType> maskPicked;

    public void PickMask(int maskTypeIndex)
    {
        MaskType maskType = (MaskType)maskTypeIndex;
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
