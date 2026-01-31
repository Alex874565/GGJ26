using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Events;

public class TextPlayable : PlayableAsset, ITimelineClipAsset
{
    public string message;
    public UnityAction onEnter;
    public UnityAction onExit;

    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TextBehaviour>.Create(graph);
        playable.GetBehaviour().message = message;
        playable.GetBehaviour().onEnter = onEnter;
        playable.GetBehaviour().onExit = onExit;
        return playable;
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
