using UnityEngine;
using System.Collections;

public class GuideTargetEvent : EventBase
{
	// 战斗指引目标变化
	public static string GUIDE_TARGET_CHANGED = "GUIDE_TARGET_CHANGED";

	// 道具指引目标变化
	public static string PICK_TARGET_CHANGED = "PICK_TARGET_CHANGED";

	public GuideTargetEvent(string eventName)
        : base(eventName)
    {

    }
}
