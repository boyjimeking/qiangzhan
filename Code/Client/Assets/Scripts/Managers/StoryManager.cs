using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class StoryManager
{
    private static StoryManager instance;

	// 是否在情节中
	private bool mIsRunning = false;

	// 初始情节Id
	private int mOriginalId = -1;

	// 当前情节
	private StoryTableItem mCurRes = null;

    private bool mNeedHint = true;

	public StoryManager()
	{
		instance = this;

		EventSystem.Instance.addEventListener(StoryEvent.STORY_STEP_FINISH, onStoryStepFinish);
		EventSystem.Instance.addEventListener(StoryEvent.STORY_SKIP, onStorySkip);
	}

	public static StoryManager Instance
	{
		get
		{
			return instance;
		}
	}

	// 启动情节
	public bool StartStory(int id)
	{
		if (mIsRunning)
		{
			GameDebug.LogError("不要在一个未结束的情节中启动另一情节！");
			return false;
		}

		if(!DataManager.StoryTable.ContainsKey(id))
		{
			return false;
		}

		mCurRes = DataManager.StoryTable[id] as StoryTableItem;
		if(mCurRes == null)
		{
			return false;
		}

		mOriginalId = id;

		WindowManager.Instance.OpenUI("story");

		BaseScene scene = SceneManager.Instance.GetCurScene();
		if(scene != null)
		{
			scene.RemoveAllActionFlag();
		}

// 		BattleUnit unit = PlayerController.Instance.GetControlObj() as BattleUnit;
// 		if (unit != null)
// 		{
// 			unit.SetLeague(LeagueDef.Neutral);
// 		}

		return mIsRunning = true;
	}

	// 是否在情节中
	public bool IsRunning()
	{
		return mIsRunning;
	}

	// 当前情节Id
	public int GetCurStoryId()
	{
		if(mCurRes == null)
		{
			return -1;
		}

		return mCurRes.id;
	}

	// 当前情节Res
	public StoryTableItem GetCurStoryRes()
	{
		return mCurRes;
	}

	// 初始情节Id
	public int GetOriginalStoryId()
	{
		return mOriginalId;
	}

	// 情节结束
	private void StoryEnd()
	{
        BaseScene scene = SceneManager.Instance.GetCurScene();
        if (scene != null)
        {
            scene.AddAllActionFlag();
        }

//         BattleUnit unit = PlayerController.Instance.GetControlObj() as BattleUnit;
//         if (unit != null)
//         {
//             unit.SetLeague(LeagueDef.Red);
//         }
        mCurRes = null;
        mIsRunning = false;
		EventSystem.Instance.PushEvent(new StoryEvent(StoryEvent.STORY_END, mOriginalId));

		mOriginalId = -1;		
	}

	// 跳过情节
	private void onStorySkip(EventBase e)
	{
		// 没有后续 情节结束
		if(mCurRes == null || mCurRes.nextId < 0 || !DataManager.StoryTable.ContainsKey(mCurRes.nextId))
		{
			StoryEnd();
			return;
		}

		// 执行后续步骤的跳过触发器
		StoryTableItem res = DataManager.StoryTable[mCurRes.nextId] as StoryTableItem;
		while(res != null)
		{
			if(!string.IsNullOrEmpty(res.skipTrigger))
			{
				doFunciton(mCurRes.skipTrigger);
			}

			if(res.nextId < 0 || !DataManager.StoryTable.ContainsKey(res.nextId))
			{
				break;
			}

			res = DataManager.StoryTable[res.nextId] as StoryTableItem;
		}

		// 情节结束
		StoryEnd();
	}

	// 情节步骤结束
	private void onStoryStepFinish(EventBase e)
	{
		// 没有后续 情节结束
		if(mCurRes.nextId < 0 || !DataManager.StoryTable.ContainsKey(mCurRes.nextId))
		{
			StoryEnd();
			return;
		}

		// 更新当前情节
		mCurRes = DataManager.StoryTable[mCurRes.nextId] as StoryTableItem;

		// 执行情节的功能触发器
		doFunciton(mCurRes.normalTrigger);

		EventSystem.Instance.PushEvent(new StoryEvent(StoryEvent.STORY_STEP_BEGIN));
	}

	// 执行情节功能
	private void doFunciton(string name)
	{
		if(string.IsNullOrEmpty(name))
		{
			return;
		}

		SceneManager.Instance.StartTrigger(name);
	}
}
