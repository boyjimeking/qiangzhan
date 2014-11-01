using UnityEngine;
using System.Collections;
using System.Text;

//货币类收益
public enum ProceedsType : int
{
    Invalid = -1,
    Money_Game = 0,
    Money_RMB = 1,
    Money_Prestige = 2,
    Money_Stren = 3,
	Money_Arena = 4,
    Money_Max = 5,
}

public enum ActionTypeDef : uint
{
	ActionTypeIdle,
	ActionTypeMove,
	ActionTypeSkill,
	ActionTypeSpasticity,
	ActionTypeDisplacement,
	ActionTypeDie,
	ActionTypeReload,

	ActionTypeCount,
}

public enum PackageType : int
{
    Invalid = -1,
    Pack_Bag = 0,
    Pack_Equip = 1,
    Pack_Weapon = 2,
    Pack_Gem= 3,
    Pack_Max = 4,
}

public enum EquipSlot : int
{
    EquipSlot_Head = 0, //头盔
    EquipSlot_Necklace = 1, //项链
    EquipSlot_Coat = 2, //上衣
    EquipSlot_Medal = 3, //勋章
    EquipSlot_Pants = 4, //裤子
    EquipSlot_Rings = 5, //戒指
    EquipSlot_MAX = 6,
}

public enum ChatChannelType:int
{
    ChannelType_City = 0,
    ChannelType_World = 1,
    ChannelType_System = 2,
    ChannelType_Max = 3,
}

public enum FittingsType : uint
{
    MAX_PROPERTY = 3,
    MAX_FITTGINS = 6,
}

public class FittingsProperty
{
    public const int RES_PROPERTY_MAX = 5;

    public const int PROPERTY_0 = 2001;
    public const int PROPERTY_1 = 2003;
    public const int PROPERTY_2 = 2005;
    public const int PROPERTY_3 = 2007;
    public const int PROPERTY_4 = 2009;

    public static int GetProperty(int index)
    {
        switch (index)
        {
            case 0:
                return PROPERTY_0;
            case 1:
                return PROPERTY_1;
            case 2:
                return PROPERTY_2;
            case 3:
                return PROPERTY_3;
            case 4:
                return PROPERTY_4;
            default:
                return -1;
        }
    }

    public static int GetResId(int proId)
    {
        switch (proId)
        {
            case PROPERTY_0:
                return 0;
            case PROPERTY_1:
                return 1;
            case PROPERTY_2:
                return 2;
            case PROPERTY_3:
                return 3;
            case PROPERTY_4:
                return 4;
            default:
                return -1;
        }
    }
}

// 条件/奖励类型
public enum ConditionType : int
{
	LEVEL = 0,				// 等级
	MONEY = 1,				// 货币
	EXP = 2,				// 经验
	ITEM = 3,				// 道具
	STAGE_UNLOCK = 4,		// 关卡解锁
	STAGE_GRADE = 5,		// 关卡评价
	BATTLE_SCORE = 6,		// 战斗力
	WING_LEVEL = 7,			// 翅膀等级
	COST_DIAMOND = 8,		// 消费钻石
	QUEST_FINISH = 9,		// 任务完成
	PASSTED_STAGE = 10,		// 通关关卡
	PASSTED_ZONE = 11,		// 通关战区
	QUEST_ACCEPT = 12,		// 任务接受
}

// 关卡评分
public enum StageGrade : int
{
	StageGrade_Invalid = -1,
	StageGrade_C = 0,
	StageGrade_B = 1,
	StageGrade_A = 2,
	StageGrade_S = 3,
}

// 复活方式
public enum ReliveType : int
{
	ReliveType_Normal = 0,
	ReliveType_Extra = 1,
}

public enum ItemTypeIdRangle : uint
{
    Item_Weapon_Res_Id_Min = 0,
    Item_Weapon_Res_Id_Max = 999999,

    Item_Normal_Id_Min = 1000000,
    Item_Normal_Id_Max = 1999999,

    Item_Defence_Id_Min = 2000000,
    Item_Defence_Id_Max = 2999999,

    Item_Stone_Id_Min = 3000000,
    Item_Stone_Id_Max = 3999999,

    Item_Money_Id_Min = 4000000,
    Item_Money_Id_Max = 4999999,

    Item_Crops_Id_Min = 5000000,
    Item_Crops_Id_Max = 6999999,

    Item_Box_Id_Min = 7000000,
    Item_Box_Id_Max = 8999999,
}

//道具初始类型
public enum ItemType : int
{
    Invalid = -1,
    Normal = 0,
    Defence = 1,
    Weapon = 2,
    Stone = 3,
    Money = 4,
    Fittings = 5,
    Crops = 6,
    Box = 7,
}

public enum PlayerGradeEnum:int
{
    PlayerGrade_Level = 0,
    PlayerGrade_Equip = 1,
    PlayerGrade_Weapon = 2,
    PlayerGrade_Title = 3,
    PlayerGrade_Skill = 4,
    PlayerGrade_Wing = 5,
    PlayerGrade_Fashion = 6,
    PlayerGradeEnumMax,
}

public enum RoleState:int
{
	RoleState_Invaild = -1,
	RoleState_Idle  = 0,
	RoleState_Moving = 1,
    RoleState_Died  = 2,    //死亡
}

public enum UpdateRetCode : uint
{
	Continue,		// 继续Update.
	Finished,		// 当前行为的Update完成.
	Aborted,		// 当前行为被中断.
}

public enum RoleAnimationDir:int
{
	AnimationDir_Front = 0,
	AnimationDir_Back,
	AnimationDir_Left,
	AnimationDir_Right,
}

//游戏对象层级定义
public enum ObjectLayerType:int
{
	ObjectLayerDefault = 0,
	ObjectLayerUI = 5,
	ObjectLayer3DUI = 8,
    ObjectLayerPlayer = 9,     //Player层
    ObjectLayerNPC = 10,        //NPC层
    ObjectLayerObjects = 11,        //场景里各种物件层
}
//移动方式
public enum MovingType : int
{
    MoveType_Lowwer,
    MoveType_Name,
}

// 主属性
public enum MainPropertyType : int
{
	PropertyType_Level = 101,			// 等级
	PropertyType_Job = 102,				// 职业
	PropertyType_Exp = 103,				// 当前经验
	PropertyType_SP = 104,				// 行动力
}

/// <summary>
/// Idle时的状态定义.
/// </summary>
public enum IdleStateDef : uint
{
	Invalid,	//
	Fight,		// 备战状态.
	Rest,		// 休息.
	Count,
};

//menu的操作类型
public enum MenuOpType:uint
{
    MenuOpType_OpenUI = 0,
    MenuOpType_Scene = 1,
    MenuOpType_ParentUI = 2,
}

/// <summary>
/// 进入每个Idle状态时, 播放的动画.
/// </summary>
public class IdleStateAnimationDef
{
	static readonly string[] StateAnimation = new string[] { 
		"LogicError_InvalidAnimationState",
		AnimationNameDef.PrefixBeiZhan,
		AnimationNameDef.PrefixXiuxi 
	};

	static public string GetAnimationNameByState(IdleStateDef state) {
		return StateAnimation[(int)state];
	}
};

// 战斗属性枚举 (对外暴露)
public enum PropertyTypeEnum:int
{
	PropertyTypeHP = 1001,				    //当前生命值
    PropertyTypeMana = 1002,			    //当前魔法值
    PropertyTypeSpeed = 1003,			    //当前移动速度

    PropertyTypeSpeed_Rate = 1004,			    //当前移动速度百分比

    PropertyTypeMaxHP = 2001,			    //生命上限
    PropertyTypeMaxHP_Rate = 2002,		    //生命上限百分比

    PropertyTypeDamage = 2003,			    //伤害
    PropertyTypeDamage_Rate = 2004,			//伤害百分比
    PropertyTypeCrticalLV = 2005, 		    //暴击等级
    PropertyTypeCrticalLV_Rate = 2006, 		//暴击等级百分比
    PropertyTypeDefance = 2007, 			//防护.
    PropertyTypeDefance_Rate = 2008, 		//防护百分比
    PropertyTypeMaxMana = 2009,				//能量上限
    PropertyTypeMaxEnergy_Rate = 2010,		//能量上限百分比

	PropertyTypeAlpha = 2011,				// 透明度.
	PropertyTypeScale_Rate = 2012,			// 模型缩放百分比, 实际的缩放为(模型缩放值 * 该值 / 100).

	PropertyTypeSpasticityResistance  = 2013,	// 硬直抗性.
}


public enum PROPERTY_DEFINE : int
{
    MAX_PROPERTY_NUMBER = 11,
}

public enum STARS_RANK : int
{ 
    MAX_STARS_RANK_NUMBER = 10,
}

public enum STONE_RANK : int
{ 
    MAX_STONE_RANK_NUMBER = 4,
}

public static class PropertyBind
{
    public static int ToBind(PropertyTypeEnum type)
    {
        switch( type )
        {
            case PropertyTypeEnum.PropertyTypeHP:
                return 0;
            case PropertyTypeEnum.PropertyTypeMana:
                return 1;
            case PropertyTypeEnum.PropertyTypeSpeed:
            case PropertyTypeEnum.PropertyTypeSpeed_Rate:
                return 2;
            case PropertyTypeEnum.PropertyTypeMaxHP:
            case PropertyTypeEnum.PropertyTypeMaxHP_Rate:
                return 3;
            case PropertyTypeEnum.PropertyTypeDamage:
            case PropertyTypeEnum.PropertyTypeDamage_Rate:
                return 4;
            case PropertyTypeEnum.PropertyTypeCrticalLV:
            case PropertyTypeEnum.PropertyTypeCrticalLV_Rate:
                return 5;
            case PropertyTypeEnum.PropertyTypeDefance:
            case PropertyTypeEnum.PropertyTypeDefance_Rate:
                return 6;
            case PropertyTypeEnum.PropertyTypeMaxMana:
            case PropertyTypeEnum.PropertyTypeMaxEnergy_Rate:
                return 7;
			case PropertyTypeEnum.PropertyTypeAlpha:
				return 8;
			case PropertyTypeEnum.PropertyTypeScale_Rate:
				return 9;
			case PropertyTypeEnum.PropertyTypeSpasticityResistance:
				return 10;
        }
        return -1;
    }

    public static bool IsRate(PropertyTypeEnum type)
    {
        if (type == PropertyTypeEnum.PropertyTypeMaxHP_Rate ||
            type == PropertyTypeEnum.PropertyTypeDamage_Rate ||
            type == PropertyTypeEnum.PropertyTypeCrticalLV_Rate ||
            type == PropertyTypeEnum.PropertyTypeDefance_Rate ||
            type == PropertyTypeEnum.PropertyTypeMaxEnergy_Rate ||
            type == PropertyTypeEnum.PropertyTypeSpeed_Rate ||
			type == PropertyTypeEnum.PropertyTypeScale_Rate)
            return true;
        return false;
    }

    public static float GetMinValue(PropertyTypeEnum type)
    {
//         if (type == PropertyTypeEnum.PropertyTypeHP ||
//             type == PropertyTypeEnum.PropertyTypeMana ||
//             type == PropertyTypeEnum.PropertyTypeSpeed ||
//             type == PropertyTypeEnum.PropertyTypeSpeed_Rate ||
// 			type == PropertyTypeEnum.PropertyTypeAlpha ||
// 			type == PropertyTypeEnum.PropertyTypeScale_Rate ||
// 			type == PropertyTypeEnum.PropertyTypeSpasticityResistance ||
//             type == PropertyTypeEnum.PropertyTypeDamage ||
//             type == PropertyTypeEnum.PropertyTypeDamage_Rate
//             )
//             return 0.0f;
//         return 2000000.0f;
        return 0.0f;
    }
}

public enum PropertyOpTypeEnum:int
{
	PropertyOpTypeNone = 0,				
	PropertyOpTypeRate = 1,				
}

public enum FunctionType:int
{
    FunctionActivtiy = 0,
    FunctionFunc = 1,

}

public enum ArenaRankLevel : uint
{
	ArenaRankLevel_0 = 0,
	ArenaRankLevel_1 = 1,
	ArenaRankLevel_2 = 2,
	ArenaRankLevel_3 = 3,

	ArenaRankLevel_End = ArenaRankLevel_3,
}



public class ObjectLayerMask
{
    public static string Player = "Player";
    public static string NPC = "NPC";
    public static string Bullet = "Bullet";

    public static int GetLayer(string name)
    {
        return LayerMask.NameToLayer(name);
    }
}

//玩家tag类型
public class ObjectType
{
	public static string ObjectTagNone = "None";
	public static string ObjectTagPlayer = "Player";
	public static string ObjectTagNPC = "NPC";
	public static string ObjectTagTrap = "Trap";
    public static string ObjectTagPick = "Pick";
    public static string ObjectTagBuild = "Build";
	public static string ObjectTagGhost = "Ghost";

    public static int OBJ_INVAILD = -1;

    public static int OBJ_NPC = 0;
    public static int OBJ_PLAYER = 1;
	public static int OBJ_GHOST = 2;
    public static int OBJ_TRAP = 3;
    public static int OBJ_BUILD = 4;

	public static int OBJ_PICK = 5;

    public const int OBJ_SEARCH_BATTLEUNIT = 0;
    public const int OBJ_SEARCH_NPC = 1;
    public const int OBJ_SEARCH_PLAYER = 2;
    public const int OBJ_SEARCH_BUILD = 3;

    public static bool IsPlayer(int type)
    {
        return type == OBJ_PLAYER;
    }
    public static bool IsNPC(int type)
    {
        return type == OBJ_NPC;
    }
	public static bool IsPick(int type)
	{
		return type == OBJ_PICK;
	}
    public static bool IsBuild(int type)
    {
        return type == OBJ_BUILD;
    }
    public static bool IsTrap(int type)
    {
        return type == OBJ_TRAP;
    }
	public static bool IsGhost(int type)
	{
		return type == OBJ_GHOST;
	}
    public static bool IsBattleUnit(int type)
    {
		return (type >= OBJ_NPC && type <= OBJ_BUILD);
    }
    public static bool IsRole(int type)
    {
		return (type >= OBJ_NPC && type <= OBJ_GHOST);
    }

    public static bool IsCanSearch(int searchType , int type)
    {
        switch( searchType )
        {
            case OBJ_SEARCH_BATTLEUNIT:
                return IsBattleUnit(type);
            case OBJ_SEARCH_NPC:
                return IsNPC(type);
            case OBJ_SEARCH_PLAYER:
                return IsPlayer(type);
            case OBJ_SEARCH_BUILD:
                return IsBuild(type);
        }
        return false;
    }
}

public static class AnimationNameDef
{
	public static readonly string XiuXi = "xiuxi";
	public static readonly string PrefixXiuxi = "%" + XiuXi;

	public static readonly string BeiZhan = "beizhan";
	public static readonly string PrefixBeiZhan = "%" + BeiZhan;

	public static readonly string SiWang = "die01";
	public static readonly string PrefixSiWang = "%" + SiWang;

	public static readonly string BeiJi = "beiji";
	public static readonly string PrefixBeiJi = "%" + BeiJi;

    public static readonly string Pao = "pao";
    public static readonly string PrefixPao = "%" + Pao;
    public static readonly string PrefixZhanliXiuxi = "%zhanlixiuxi";

	public static readonly string JiangZhi = "jiangzhi";


    //--------------------------武器动作------------------------------
    public static readonly string WeaponPrefix = "Base Layer.";
    public static readonly string WeaponEmpty = WeaponPrefix + "emptyState";
    public static readonly string WeaponDefault = WeaponPrefix + "default";
    public static readonly string WeaponFire = WeaponPrefix + "fire";

    //--------------------------翅膀动作------------------------------
    public static readonly string WingPrefix = "Base Layer.";
    public static readonly string WingEmpty = WeaponPrefix + "emptyState";
    public static readonly string WingDefault = WeaponPrefix + "xiuxi";
    public static readonly string WingFei = WeaponPrefix + "fei";


    private  static StringBuilder mTempAnimStr = new StringBuilder(256);
    public static string GetAnimNameByStatename(string statename,string aniname )
    {
        if (string.IsNullOrEmpty(aniname))
            return aniname;

        mTempAnimStr.Length = 0;
        int nStart = 0;
        int nLength = aniname.Length;
        if (aniname[0] == '%' && !string.IsNullOrEmpty(statename))
        {
            mTempAnimStr.Append(statename);
            mTempAnimStr.Append(".");
            nStart = 1;
            nLength -= 1;
        }
        else
        {
            mTempAnimStr.Append("Base Layer.");
        }
        mTempAnimStr.Append(aniname, nStart, nLength);
        return mTempAnimStr.ToString();
    }
}

/// <summary>
/// 错误码. 在StrError方法中, 加入解析.
/// </summary>
public enum ErrorCode : int
{
	Succeeded = 0,			// 成功.
	InvalidParam,			// 无效参数.

	/// <summary>
	/// 严重系统错误.
	/// </summary>
	LogicError,				// 逻辑错误.
	ConfigError,			// 配置错误.

	/// <summary>
	/// 技能使用错误.
	/// </summary>
	InvalidModel,			// 模型尚未加载完成, 不能被添加技能效果.
	TooClose,				// 目标点太近.
	TooFar,					// 目标点太远.
	InsufficientHp,			// 没有足够的血量.
	InsufficientMana,		// 没有足够的魔法.
	InsufficientSoul,		// 没有足够的魂能.
	SkillDisabled,			// 技能未启用.
	CoolingDown,			// 正在CD中.
	SkillNotExist,			// 技能不存在.
	SkillAlreadyUsing,		// 技能正在使用, 在使用状态的技能再次接收到使用的通知.
	SkillUninterruptable,	// 技能无法被打断.
	UnableToAttack,			// 无法普通攻击.
	UnableToUseSkill,		// 无法使用技能.
	UnableToMove,			// 无法移动.
	TargetIsDead,			// 目标已经死亡.
	MaxStackCount,			// 添加skilleffect失败, 已达最大叠加次数.

	/// <summary>
	/// 技能效果错误.
	/// </summary>
	AddBuffFailedBuffMutex,				// 添加buff失败, buff之间互斥.
	AddEffectFailedSkillEffectImmunity,	// 无法添加技能效果, 目标免疫.

    /// <summary>
    /// 任务错误
    /// </summary>
    Precondition, // 任务接取前提条件不符合
    CompleteConditon, //任务完成条件不符合；
    MaxAcceptNum,//接取的任务达到上限

    InsufficientBullet,     //没有足够的弹药

    NotBattleScene ,        //非战斗场景
}

public enum ActivityType : int
{
    Activity_Type_Invalid = -1,
    Activity_Type_QiangLinDanYu = 0,
	Activity_Type_YaZhiXieE = 1,
	Activity_Type_ZhaoCaiMao = 2,
    Activity_Type_MAX,
};

public static class ErrorHandler
{
	/// <summary>
	/// 错误处理. err为错误码, msg为额外输出(附加到错误输出之后).
	/// </summary>
	/// <returns>返回err是否为succeeded.</returns>
	public static bool Parse(ErrorCode err, string msg = "")
	{
		if (msg == null)
			msg = "";

		switch (err)
		{
			case ErrorCode.Succeeded:
				break;
			case ErrorCode.LogicError:
				fatalErrorHandler("logic error: " + msg);
				break;
			case ErrorCode.ConfigError:
				configErrorHandler("config error: " + msg);
				break;
			default:
				if (GameConfig.LogSkillError) GameDebug.LogWarning(Error2String(err) + ": " + msg);
				break;
		}

		return err == ErrorCode.Succeeded;
	}

    public static string Error2Prompt(ErrorCode err)
    {
        string errMessage = "";
        switch (err)
        {
            case ErrorCode.InsufficientMana:
                errMessage += "能量值不足，无法使用技能";
                break;
            case ErrorCode.CoolingDown:
                errMessage += "技能冷却中，无法使用技能";
                break;
            case ErrorCode.UnableToUseSkill:
                errMessage += "无法使用技能";
                break;
            case ErrorCode.UnableToMove:
                errMessage += "无法移动";
                break;
        }
        return errMessage;
    }

	public static string Error2String(ErrorCode err)
	{
		string errMessage = "error " + (int)err + ": ";
		switch (err)
		{
			case ErrorCode.LogicError:
				errMessage += "logic error";
				break;
			case ErrorCode.InvalidParam:
				errMessage += "invalid param";
				break;
			case ErrorCode.InvalidModel:
				errMessage += "loading model";
				break;
			case ErrorCode.TooClose:
				errMessage += "target position too close";
				break;
			case ErrorCode.TooFar:
				errMessage += "target position too far";
				break;
			case ErrorCode.InsufficientHp:
				errMessage += "insufficient hp";
				break;
			case ErrorCode.InsufficientMana:
				errMessage += "insufficient mana";
				break;
			case ErrorCode.InsufficientSoul:
				errMessage += "insufficient soul";
				break;
			case ErrorCode.InsufficientBullet:
				errMessage += "insufficient bullet";
				break;
			case ErrorCode.SkillDisabled:
				errMessage += "skill disabled";
				break;
			case ErrorCode.CoolingDown:
				errMessage += "skill is cooling down";
				break;
			case ErrorCode.SkillNotExist:
				errMessage += "skill doesn't exist";
				break;
			case ErrorCode.SkillUninterruptable:
				errMessage += "skill is uninterruptable";
				break;
			case ErrorCode.SkillAlreadyUsing:
				errMessage += "skill is already in using state";
				break;
			case ErrorCode.UnableToAttack:
				errMessage += "unable to attack";
				break;
			case ErrorCode.UnableToUseSkill:
				errMessage += "unable to use skill";
				break;
			case ErrorCode.UnableToMove:
				errMessage += "unable to move";
				break;
			case ErrorCode.TargetIsDead:
				errMessage += "target is dead";
				break;
			case ErrorCode.MaxStackCount:
				errMessage += "failed to add skill effect, max stack count";
				break;
			case ErrorCode.AddBuffFailedBuffMutex:
				errMessage += "failed to add skill buff, buff mutex";
				break;
			case ErrorCode.AddEffectFailedSkillEffectImmunity:
				errMessage += "failed to add skill effect, target is immune";
				break;
            case ErrorCode.Precondition:
		        errMessage += "precondition ,failed to accept,not enough preconditon";
                break;
            case ErrorCode.CompleteConditon:
		        errMessage += "CompleteCondition,not enough CompleteCondition";
		        break;
            case ErrorCode.MaxAcceptNum:
		        errMessage += "failed to accept,MaxAccpteNum is 30";
		        break;
			default:
				errMessage += "unrecognized error";
				break;
		}

		return errMessage;
	}



	/// <summary>
	/// 触发逻辑异常.
	/// </summary>
	private static void fatalErrorHandler(string msg)
	{
		if (GameConfig.LogSkillFatalError)
			GameDebug.LogError(msg);
	}

	private static void configErrorHandler(string msg)
	{
		GameDebug.LogError(msg);
	}

}

public enum QuestDefine
{
    MaxCount = 1024,//最大任务数
    MaxAcceptCount = 30,//最大接取数
}
public enum QuestState
{
    Unaccepted,
    Accepted,
    Finish
}

//关卡子类型
public enum StageSubType
{
    Tower,//爬塔
    Zone,//战区
    Resource,//资源本
    Money,
    Count
}

public class QuestType
{
    public const int Main = 0;
    public const int Side = 1;
    public const int Daily = 2;
}


public class QuestConditionType
{
    public const int Level = 0;
    //等级

    public const int Money_ = 1;
    //货币

    public const int Exp = 2;
    //经验

    public const int Item = 3;
    //道具 

    public const int Stage_Unlock = 4;
    //关卡解锁

    public const int Stage_Grade = 5;
    //关卡评价

    public const int Battle_Score = 6;
    //战斗力

    public const int Wing_Level = 7;
    //翅膀等级

    public const int Cost_Diamond = 8;
    //消费钻石

    public const int Quest = 9;
    //任务

    public const int Stren_Defence = 10;
    //强化装备

    public const int Rising_Stars = 11;
    //升星

    public const int Wing_Forge = 12;
    //翅膀精炼

    public const int Stone_Inlay = 13;
    //宝石镶嵌

    public const int Stren_Weapon = 14;
    //强化武器

    public const int Pass_Stage = 15;
    //通关

    public const int Add_Friend = 16;
    //添加好友

    public const int Stage_Zone = 17;
    //战区

    public const int Passted_Stage = 18;
    //通过某个关卡

    public const int Passted_Zone = 19;

    //僵尸本
    public const int Pass_Zombies = 20;
    public const int Pass_Tower = 21; //爬塔成功
    public const int Pass_Mao = 22; //逗比猫
    public const int Pass_Arena = 23; //通关竞技场
    public const int Enter_Arena = 24; //进入竞技场
    public const int Enter_Tower = 25; //参与爬塔
    public const int Enter_Qinglin = 26; //参与枪林弹雨
    public const int Enter_Qualifying = 27; //参与排位赛
    public const int Pass_Qualifying = 28; //排位赛胜出
    public const int Enter_Egg = 29; //砸蛋  
    public const int Weapon_Promote = 30;
    public const int Top_Level = 31; //等级上限值
    public const int Tower_Floor = 32; //爬塔层数
}

public class WingDefine
{
    public const int Max_Wing_Level = 100;
    public const int PropertyNum = 5;
    public const int MaxConditonNum = 5;
    public const int MaxWingNum = 10;
}

public class ChallengeDefine
{
    public const uint Rank_Num = 10;
    public const int Rank_Rest_Time = 86400000;

}
public class CropsDefine
{
    public const int PRORATIONNUM = 10;
}
public class FashionDefine
{
    public const uint Max_Fashion_Count = 16;
    public const uint Starnum_Per_Level = 10;
}