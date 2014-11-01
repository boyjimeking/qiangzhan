using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 场景类型
public enum SceneType:int
{
	SceneType_Invaild   = -1,
	SceneType_City      = 0,		// 城镇
    SceneType_Stage     = 1,		// 副本
    SceneType_Tower     = 2,		// 闯塔
    SceneType_Zombies   = 3,        // 生存玩法--打僵尸（经验本）
    SceneType_MonsterFlood = 4,     // 挑战本
    SceneType_QiangLinDanYu = 5,	// 枪林弹雨
	SceneType_Arena = 6,			// 竞技场
	SceneType_Qualifying = 7,		// 排位赛
    SceneType_Mao = 8,              // 逗比猫;
    SceneType_Battle = 10,          // 战场
    SceneType_Resource = 11,	    // 资源本
    SceneType_HunNeng = 12,         // 魂能
	SceneType_Wanted = 13,			// 通缉令
    SceneType_TD = 14,				// 塔防（钻石本）
	SceneType_YaZhiXieE = 15,		// 压制邪恶
	SceneType_ZhaoCaiMao = 16,		// 招财猫
}

// 关卡子类型
//public enum StageSceneType : int
//{
//    StageSceneType_Invalid = -1,
//    StageSceneType_Main = 0,		// 主线
//    StageSceneType_Resource = 1,	// 资源本
//    StageSceneType_Tower = 2,		// 闯塔
//    StageSceneType_Zombies = 3,     // 生存玩法--打僵尸
//    StageSceneType_MonsterFlood=4,   //挑战本
//}

// 战场子类型
public enum BattleSceneType : int
{
	BattleSceneType_Invalid = -1,
	BattleSceneType_Rank = 0,		// 排位赛
	BattleSceneType_Arena = 1,		// 竞技场
}

// 场景状态
public enum SceneState:int
{
    SceneState_Invalid = -1,        // 未初始化
    SceneState_Loading = 0,         // 载入中
    SceneState_Initialize = 1,      // 初始化
    SceneState_LogicRun = 2,        // 运行逻辑
    SceneState_Destroy = 3,         // 销毁中
}

// 场景逻辑状态
public enum SceneLogicState:int
{
    SceneLogicState_Invalid = -1,   // 无效
    SceneLogicState_Ready = 0,      // 准备
    SceneLogicState_Working = 1,    // 工作中
    SceneLogicState_Closing  = 2,   // 清理
	SceneLogicState_Destroy = 3,	// 销毁
}

// 场景逻辑运行结果
public enum SceneLogicResult: int
{
    SceneLogicResult_Destroy = -1,  // 严重错误 销毁场景
    SceneLogicResult_Continue = 0,  // 继续
    SceneLogicResult_NextState = 1, // 切换下一状态
}

// 行为权限
public enum SceneActionFlag : int
{
	SceneActionFlag_None = 0,

	SceneActionFlag_Move = 1,		// 可行走
	SceneActionFlag_UseSkill = 2,	// 可使用技能
	SceneActionFlag_Ai = 4,			// 可执行Ai
	SceneActionFlag_Attack = 8,		// 可普通攻击

	SceneActionFlag_All = 0x7FFFFFFF,
}
