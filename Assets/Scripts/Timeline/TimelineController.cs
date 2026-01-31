using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    public PlayableDirector director;

    public void Pause() => director.Pause();
    public void Resume() => director.Play();
    
}
