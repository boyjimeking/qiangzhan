using UnityEngine;
using System.Collections;

// 关卡事件
public class StageEvent : EventBase
{
	public StageEvent(string eventName)
		: base(eventName)
	{

	}
}

// 翻牌UI事件
public class StageBalanceUIEvent : StageEvent
{
	// 翻牌
	public static string STAGE_BALANCE_SELECT_CARD = "STAGE_BALANCE_SELECT_CARD";

	// 普通牌/黄金牌
	public bool mIsNormalCard;

	// 索引
	public int mIdx;

	public StageBalanceUIEvent(string eventName, bool isNormalCard, int idx)
		: base(eventName)
	{
		mIsNormalCard = isNormalCard;
		mIdx = idx;
	}
}

// 结算界面事件
public class StageEndUIEvent : StageEvent
{
	// 评价界面显示结束
	public static string STAGE_END_FINISH = "STAGE_END_FINISH";

	public StageEndUIEvent(string eventName)
		: base(eventName)
	{

	}
}

// 进入关卡
public class StageEnterEvent : StageEvent
{
	// 进入关卡答复
	public const string STAGE_ENTER_RESPOND_EVENT = "STAGE_ENTER_RESPOND_EVENT";

	public SceneType mSceneType;

	public int mStageId;

	public StageEnterEvent(string eventName, SceneType sceneType, int stageId)
		: base(eventName)
	{
		mSceneType = sceneType;
		mStageId = stageId;
	}
}

// 复活
public class StageReliveEvent : StageEvent
{
	// 请求复活
	public static string STAGE_RELIVE_REQUEST = "STAGE_RELIVE_REQUEST";

	// 复活答复
	public static string STAGE_RELIVE_RESPOND = "STAGE_RELIVE_RESPOND";

	// 复活方式(0普通 1强力)
	public ReliveType mReliveType;

	public StageReliveEvent(string eventName, ReliveType type)
		: base(eventName)
	{
		mReliveType = type;
	}
}

// 关卡解锁
public class StageUnlockEvent : StageEvent
{
	// 关卡解锁
	public static string STAGE_UNLOCK = "STAGE_UNLOCK";

	public int stageId = -1;

	public StageUnlockEvent(string eventName)
		: base(eventName)
	{

	}
}

// 服务器同步关卡数据
public class StageSyncServerEvent : StageEvent
{
	// 同步关卡数据
	public static string STAGE_SYNC_SERVER_EVENT = "STAGE_SYNC_SERVER_EVENT";

	// 同步关卡日常数据
	public static string STAGE_DAILY_SYNC_SERVER_EVENT = "STAGE_DAILY_SYNC_SERVER_EVENT";

	public StageSyncServerEvent(string eventName)
		: base(eventName)
	{

	}
}

// 服务器通知关卡通关
public class StagePassServerEvent : StageEvent
{
	// 关卡通关
	public static string STAGE_PASS_SERVER_EVENT = "STAGE_PASS_SERVER_EVENT";

	public StageData mStageData = null;

	public StagePassServerEvent(string eventName)
		: base(eventName)
	{

	}
}

// 战区领取奖励事件
public class ZoneRewardEvent : StageEvent
{
    // 战区奖励领取
    public static string ZONE_REWARD_OBTAIN = "ZONE_REWARD_OBTAIN";

    public ZoneRewardEvent(string eventName)
        : base(eventName)
    {

    }
}
