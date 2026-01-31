using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;

public class TextBehaviour : PlayableBehaviour
{
    public string message;
    private bool hasPaused;
    public UnityAction onEnter;
    public UnityEvent onExit;
    private NarrativeManager narrativeManager;
    private TimelineController timelineController;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (narrativeManager == null)
            narrativeManager = GameObject.FindFirstObjectByType<NarrativeManager>();

        narrativeManager.ShowText(message);

        if (timelineController == null)
            timelineController = GameObject.FindFirstObjectByType<TimelineController>();
        
        /*
        if (!hasPaused)
        {
            timelineController.Pause();
            onEnter?.Invoke();
            hasPaused = true;
        }
        */
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        /*
        if (narrativeManager != null)
        {
            narrativeManager.HideText();
        }
        
        onExit?.Invoke();
        */
    }
}