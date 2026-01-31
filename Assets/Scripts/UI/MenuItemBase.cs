using UnityEngine;
using System.Collections;

public abstract class MenuItemBase : MonoBehaviour
{
    public abstract IEnumerator Appear(float delay);
    public abstract IEnumerator Disappear(float delay);
}
