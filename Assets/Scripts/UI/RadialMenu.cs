using UnityEngine;

public class RadialMenu : MonoBehaviour
{
    public RectTransform[] items;
    public float radius = 140f;

    void Awake()
    {
        PositionItems();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying)
            PositionItems();
    }
#endif

    void PositionItems()
    {
        if (items == null || items.Length == 0) return;

        int count = items.Length;
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            items[i].anchoredPosition = new Vector2(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius
            );
        }
    }
}
