using UnityEngine;
using System.Collections;

public class SceneLoadEvent : EventBase
{
    public static string SCENE_LOAD_COMPLETE = "SCENE_LOAD_COMPLETE";

	public int param1;

	public SceneLoadEvent(string eventName)
        : base(eventName)
    {

    }
}
