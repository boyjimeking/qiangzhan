using UnityEngine;
using System.Collections;

//场景表
public class SceneTableItem 
{
    public int    resID;
    public string name; // 名称
    public string desc;
    public string sceneFile;
	public string sceneScript;
    public string sceneBlock;   // 场景阻挡（高阻、低阻)、可行走面
	public string uiconfig;
    //正常战斗背景音乐
    public int normalSound;
    //boss战斗声音
    public int bossSound;

	public string bossSprite;
	public string bossName;
	public string bossDetail;

	// 准备时间
	public uint mReadyTime;

	// 最长游戏时间
	public uint mLogicTime;

    // 佣兵复活次数
    public int mCropsReliveTimes;
    // 佣兵复活CD
    public int mCropsReliveCds;
}
