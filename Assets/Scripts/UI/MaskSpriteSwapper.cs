using UnityEngine;
using UnityEngine.UI;

public class MaskSpriteSwapper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image targetImage;
    [SerializeField] private MaskManager maskManager;

    [Header("Sprites")]
    public Sprite selflessnessSprite;
    public Sprite happinessSprite;
    public Sprite confidenceSprite;

    private void OnEnable()
    {
        // Subscribe to the event when this object is active
        if (maskManager != null)
            maskManager.maskPicked.AddListener(UpdateMaskSprite);
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks or errors
        if (maskManager != null)
            maskManager.maskPicked.RemoveListener(UpdateMaskSprite);
    }

    public void UpdateMaskSprite(MaskType type)
    {
        switch (type)
        {
            case MaskType.SELFNESSNESS:
                targetImage.sprite = selflessnessSprite;
                break;
            case MaskType.HAPPINESS:
                targetImage.sprite = happinessSprite;
                break;
            case MaskType.CONFIDENCE:
                targetImage.sprite = confidenceSprite;
                break;
        }
        
        Debug.Log($"Sprite changed to: {type}");
    }
}