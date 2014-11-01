using UnityEngine;
using System.Collections;

// 主线关卡
public class Scene_StageSceneTableItem : SceneTableItem
{
	// 最多解锁条件3个
	public static uint MAX_UNLOCK_CONDITION_COUNT = 3;

	// 战区ID
	public int mZoneId;

    // 每日可进入次数;
    public int mEnterTimes;

    // 进入消耗行动力
    public int mEnterCostSP;

    // 结算消耗行动力
    public int mAwardCostSP;

	// 奖励经验
	public int mAwardExp;

    // 解锁条件ID
	public int mUnlockCondId0;

	// 解锁条件ID
	public int mUnlockCondId1;

	// 解锁条件ID
	public int mUnlockCondId2;

	// 复活次数
	public int mReliveTimes;

	// 普通复活消耗Id
	public int mReliveCostId0;

	// 强力复活消耗Id
	public int mReliveCostId1;

    // 自动瞄准
    public int mAutoAim;

    // 推荐战斗力
    public int mSuitableFC;

    // S时间
	public uint mTimeS;

    // A时间
	public uint mTimeA;

    // B时间
	public uint mTimeB;

	// 满分
	public uint mTotalScore;

    // 首次通关奖励道具ID
    public int mFirstAwardId;

    // 通关固定奖励道具ID
	public int mPassAwardId0;

    // 普通翻牌次数
    public int mRandomAwardTimes;

    // 普通翻牌消耗ID
    public int mRandomAwardCostId;

    // 普通翻牌奖励包ID
    public int mRandomAwardBoxId;

    // 钻石翻牌次数
    public int mExtraAwardTimes;

    // 钻石翻牌消耗ID
    public int mExtraAwardCostId;

    // 钻石翻牌掉落包ID
    public int mExtraAwardBoxId;

	// 关卡图片
	public string mStageBk;

	// Bosst头像
	public string mBossIcon;

	// 首次击杀掉落包Id
	public int mFirstKillDropBoxId;

	// 击杀掉落包Id
	public int mKillDropBoxId;

	// 击杀金币掉落(±10%)
	public uint mKillGoldDrop;

    //箭头指引id;
    public int mArrow;
}
