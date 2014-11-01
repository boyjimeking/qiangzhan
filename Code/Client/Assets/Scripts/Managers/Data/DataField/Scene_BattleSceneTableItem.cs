using UnityEngine;
using System.Collections;

// 战场
public class Scene_BattleSceneTableItem
{
    // ID
    public int mId;

    // 描述
    public string mDesc;

    // 名称
    public string mName;

	// 子类型
	public SceneType mSubType;

    // 场景ID
    public int mSceneId;

    // 每日可进入次数;
    public int mDailyTimes;

    // 最长游戏时间
	public uint mMaxTime;
}
