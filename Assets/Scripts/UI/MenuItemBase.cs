using UnityEngine;
using System.Collections;

public abstract class MenuItemBase : MonoBehaviour
{
    public float appearTime = 0.1f;
    public float disappearTime = 0.1f;

    public abstract IEnumerator Appear(float delay);
    public abstract IEnumerator Disappear(float delay);
}
