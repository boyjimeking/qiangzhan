using UnityEngine;
using System.Collections;

public class StoryEvent : EventBase
{
	// 情节步骤完成
	public static string STORY_STEP_FINISH = "STORY_STEP_FINISH";

	// 情节步骤开始
	public static string STORY_STEP_BEGIN = "STORY_STEP_BEGIN";

	// 情节结束
	public static string STORY_END = "STORY_END";

	// 情节跳过
	public static string STORY_SKIP = "STORY_SKIP";

	// 情节Id
	public int mStoryId = -1;

	public StoryEvent(string eventName, int storyId = -1)
        : base(eventName)
    {
		mStoryId = storyId;
    }
}
