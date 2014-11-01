using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Collections.Generic;

public static class GameConfig 
{
    public static string CONFIG_BEGIN_SERVER_ADDRESS = "ServerAddress";
    public static string CONFIG_CHAT_SRVER = "ChatServer";

    public static string CONFIG_QQ_LOGIN_SERVER_ADDRESS = "QQServerAddress";
    public static string CONFIG_QQ_LOGIN_CHAT_SRVER = "QQChatServer";

    public static string CONFIG_WX_LOGIN_SERVER_ADDRESS = "WXServerAddress";
    public static string CONFIG_WX_LOGIN_CHAT_SRVER = "WXChatServer";

    public static string CONFIG_BEGIN_CITY_ID = "BeginCityScene";
    public static string CONFIG_GUIDE_SCENE_ID = "GuideSceneID";

    // 场景单元格大小
    public static string CONFIG_SCENE_CELL_SIZE_X = "SceneCellSize_X";
    public static string CONFIG_SCENE_CELL_SIZE_Y = "SceneCellSize_Y";

	// 普通复活恢复比例
	public static string CONFIG_NORMAL_RELIVE_RATE = "NormalReliveRate";
	// 强力复活恢复比例
	public static string CONFIG_EXTRA_RELIVE_RATE = "ExtraReliveRate";

	public static string GRAVITATIONAL_ACCELERATION = "GravitationalAcceleration";

	public static string HOMING_MISSILE_DEFAULT_RADIUS = "HomingMissileDefaultRadius";

	public static string HIT_MATERIAL_CD = "HitMaterialEffectCd";

	public static string SPASTICITY_BEATBACK_SPEED = "SpasticityBeatbackSpeed";

	public static string MAX_SPASTICITY_RESISTANCE = "MaxSpasticityResistance";

	// 暴击伤害为实际伤害的多少倍.
    private static readonly string CRITICAL_DAMAGE_SCALE = "CriticalDamageScale";

	private static readonly string CRITICAL_STRIKE_EFFECT_BINDPOINT = "CriticalStrikeEffectBindPoint";
	private static readonly string CRITICAL_STRIKE_EFFECT_ID = "CriticalStrikeEffectID";

    private static readonly string LEVELUP_EFFECT_ID = "LevelUpEffectID";
    private static readonly string LEVELUP_SHAKECAMERA_AMOUNT = "LevelUpShakeCameraAmount";
    private static readonly string LEVELUP_SHAKECAMERA_TIME = "LevelUpShakeCameraTime";

    private static readonly string KILL_NUMBER_TIME = "KillNumberTime";
    private static readonly string CP_PLAYIDLE_TIME = "CPPlayIdlePerTime";

    private static readonly string WAVE_WING_FREQUENCY = "WaveWingFrequency";

	private static Hashtable mValues = new Hashtable();
	private const int CONFIG_FILE_COUNT = 2;
	private static string[] configFileNames = new string[CONFIG_FILE_COUNT] { "Config/config.xml", "@/Config/config.xml" };

	public static bool hasInit = false;
	private static int hasInitFileCount = 0;

    //------------------------------------系统的一些配置-------------------------------------------------

    private const string DefaultAddr = "http://127.0.0.1:8087";
    private const string DefaultChatAddr = "127.0.0.1:17788";

    public static readonly string AssetServerURL = "http://77.77.1.103/";
    public static readonly string AssetVersion = "version.txt";
    public static readonly string DataURL = "Data/";
    public static readonly string AppURL = "App/";
    public static readonly string AppPath = "client.apk";

    public static readonly string VersionID = "appVersionID";
    public static readonly string appUpdateFlag = "appUpdateFlag";
    public static readonly string appInstallFlag = "appInstallFlag";

    public static string FileListPath = "filelist.config";
    public static string AppVersionPath = "appversion.config";
    public static string DataversionPath = "dataversion.config";




    //-----------------------------Client/Config数据-----------------------------------------

    private static int sLevelUpEffectID;
    private static float sLevelUpShakeCameraAmount;
    
    private static float sLevelUpShakeCameraTime = 2;
    private static float sCPPlayIdlePerTime = 15f;
    // 挥动翅膀的频率
    private static float sWaveWingFrequency = 15f;

	// 竞技场场景Id
	private static int sArenaSceneID;
	// 排位赛场景Id
	private static int sQualifyingSceneID;
	// Pvp进场BuffId
	private static uint sPvpBuffId;

	// 塔防生命数
	private static int sTDLifeCount;
	// 塔防进门特效
	private static int sTDEffectId;
	// 塔防进门特效位置
	private static float sTDEffectPosX;
	// 塔防进门特效位置
	private static float sTDEffectPosZ;

	// 压制邪恶怪物上限
	private static int sYZXEMonsterMaxCount;

	// 逗比猫通关金币数
	private static int sMaoMaxGoldCount;
	// 逗比猫技术金币Id
	private static int sMaoGoldId1;
	private static int sMaoGoldId2;
	private static int sMaoGoldId3;

	// 佣兵脚下光圈BuffId
	private static uint sCropHaloBuffId;

	// 招财猫Id
	private static int sZhaoCaiMaoId;
	// 招财猫位置
	private static float sZhaoCaiMaoPosX;
	private static float sZhaoCaiMaoPosY;
	private static float sZhaoCaiMaoDir;

	//---------------------------------------------------------------------------------------

	
	//-----------------------------Common/Config数据-----------------------------------------

	// 竞技场购买挑战次数消耗钻石数
	private static uint sArenaBuyTimesCost;
	// 竞技场挑战冷却时间
	private static uint sArenaFightCD;
	// 排位赛购买挑战次数消耗钻石数
	private static uint sQualifyingBuyTimesCost;
	// 排位赛挑战冷却时间
	private static uint sQualifyingFightCD;
	// 排位赛获胜奖励声望
	private static uint sQualifyingWinPrestige;
	// 排位赛获胜奖励金币
	private static uint sQualifyingWinGold;
	// 排位赛失败奖励声望
	private static uint sQualifyingLosePrestige;
	// 排位赛失败奖励金币
	private static uint sQualifyingLoseGold;
	// 排位赛每日锁榜时刻
	private static int sQualifyingLockSecondOfDay;
	// 排位赛每日结算时刻
	private static int sQualifyingAwardSecondOfDay;
	// 招财猫准备时间
	private static uint sZhaoCaiMaoReadyTime;
	// 招财猫请求假人时间
	private static uint sZhaoCaiMaoRequestTime;
    //精英计划
   private static uint sJingying_jihua_zhuanshi;
    //至尊计划
    private static uint sZhizun_jihua_zhuanshi;

	//---------------------------------------------------------------------------------------

	// 是否记录错误.
	public static bool LogSkillError = false;

	// 是否记录严重错误, 比如配置错误.
	public static bool LogSkillFatalError = true;

	public delegate void CompleteCallback ();

	private static CompleteCallback mCallback = null;

	public static void SyncInit(CompleteCallback callback)
	{
		mCallback = callback;

		for (int i = 0; i < CONFIG_FILE_COUNT; ++i)
		{
			ResourceManager.Instance.LoadBytes(configFileNames[i], ReadConfigCallBack);
		}
	}

	static void ReadConfigCallBack(string path , byte[] bytes)
	{
		string txt = Encoding.UTF8.GetString (bytes);
		if( string.IsNullOrEmpty(txt) )
			return ;

		XmlDocument xDoc = new XmlDocument();

		MemoryStream ms = new MemoryStream(bytes);
		ms.Flush();
		ms.Position = 0;
		xDoc.Load(ms);

		//xDoc.LoadXml( txt );
		XmlNode node = xDoc.FirstChild;
		node = node.NextSibling;
		XmlNodeList nodeList = node.ChildNodes;
		for( int i = 0 ; i < nodeList.Count ; ++i )
		{
			XmlNode childNode = nodeList[i];
			
			if( !string.IsNullOrEmpty(childNode.Name) &&
			   childNode.LocalName == "#comment" )
			{
				continue;
			}
			
			string name = childNode.Attributes["name"].Value;
			string value = childNode.Attributes["value"].Value;
			
			mValues.Add( name , value ); 
		}

		hasInitFileCount++;

		if(hasInitFileCount >= CONFIG_FILE_COUNT)
		{
			ReadData();

			hasInit = true;

			if (mCallback != null)
			{
				mCallback();
			}
		}
	}


    static void ReadData()
    {
		//-----------------------------Client/Config数据-----------------------------------------
        sLevelUpEffectID = ReadInt(LEVELUP_EFFECT_ID, -1);
        sLevelUpShakeCameraAmount = ReadFloat(LEVELUP_SHAKECAMERA_AMOUNT, 0);
        sLevelUpShakeCameraTime = ReadFloat(LEVELUP_SHAKECAMERA_TIME, 2);
        sCPPlayIdlePerTime = ReadFloat(CP_PLAYIDLE_TIME, 15);
        sWaveWingFrequency = ReadFloat(WAVE_WING_FREQUENCY, 20);
		sArenaSceneID = ReadInt("ArenaSceneID", 1);
		sQualifyingSceneID = ReadInt("QualifyingSceneID", 1);
		sPvpBuffId = ReadUInt("PvpBuffId", uint.MaxValue);
		sTDLifeCount = ReadInt("TDLifeCount", 20);
		sTDEffectId = ReadInt("TDEffectId", 525);
		sTDEffectPosX = ReadFloat("TDEffectPosX", 0.0f);
		sTDEffectPosZ = ReadFloat("TDEffectPosZ", 0.0f);
		sYZXEMonsterMaxCount = ReadInt("YZXEMonsterMaxCount", 20);
		sMaoMaxGoldCount = ReadInt("MaoMaxGoldCount", 300);
		sMaoGoldId1 = ReadInt("MaoGoldId1", -1);
		sMaoGoldId2 = ReadInt("MaoGoldId2", -1);
		sMaoGoldId3 = ReadInt("MaoGoldId3", -1);
		sCropHaloBuffId = ReadUInt("CropHaloBuffId", 618);
		sZhaoCaiMaoId = ReadInt("ZhaoCaiMaoID", -1);
		sZhaoCaiMaoPosX = ReadFloat("ZhaoCaiMaoPosX", 0.0f);
		sZhaoCaiMaoPosY = ReadFloat("ZhaoCaiMaoPosY", 0.0f);
		sZhaoCaiMaoDir = ReadFloat("ZhaoCaiMaoDir", 0.0f);
		//-----------------------------Common/Config数据-----------------------------------------
		sArenaBuyTimesCost = ReadUInt("Arena_Buy_Times_Cost", 1);
		sArenaFightCD = ReadUInt("Arena_Fight_CD", uint.MaxValue);
		sQualifyingBuyTimesCost = ReadUInt("Qualifying_Buy_Times_Cost", 1);
		sQualifyingFightCD = ReadUInt("Qualifying_Fight_CD", uint.MaxValue);
		sQualifyingWinPrestige = ReadUInt("Qualifying_Win_Prestige", 0);
		sQualifyingWinGold = ReadUInt("Qualifying_Win_Gold", 0);
		sQualifyingLosePrestige = ReadUInt("Qualifying_Lose_Prestige", 0);
		sQualifyingLoseGold = ReadUInt("Qualifying_Lose_Gold", 0);
		sQualifyingLockSecondOfDay = ReadInt("Qualifying_Lock_Second_Of_Day", 0);
		sQualifyingAwardSecondOfDay = ReadInt("Qualifying_Award_Second_Of_Day", 0);
		sZhaoCaiMaoReadyTime = ReadUInt("ZhaoCaiMao_Ready_Time", 120000);
		sZhaoCaiMaoRequestTime = ReadUInt("ZhaoCaiMao_Request_Time", 85000);
        sJingying_jihua_zhuanshi = ReadUInt("Jingying_jihua_zhuanshi", 1);
        sZhizun_jihua_zhuanshi = ReadUInt("Zhizun_jihua_zhuanshi", 1);
    }

    static string ReadString(string name,string defvalue)
    {

        if (string.IsNullOrEmpty(name) || !mValues.ContainsKey(name))
        {
            badConfig(name);
            return defvalue;
        }
        return mValues[name] as string;
    }

    static int ReadInt(string name, int defvalue)
    {
        if (string.IsNullOrEmpty(name) ||!mValues.ContainsKey(name))
        {
            badConfig(name);
            return defvalue;
        }

        return int.Parse(mValues[name] as string);
    }
    static uint ReadUInt(string name, uint defvalue)
    {
        if (string.IsNullOrEmpty(name) || !mValues.ContainsKey(name))
        {
            badConfig(name);
            return defvalue;
        }

        return uint.Parse(mValues[name] as string);
    }
    static float ReadFloat(string name, float defvalue)
    {
        if (string.IsNullOrEmpty(name) || !mValues.ContainsKey(name))
        {
            badConfig(name);
            return defvalue;
        }

        return float.Parse(mValues[name] as string);
    }


    public static int BeginCityID
	{
		get{
            if (!mValues.ContainsKey(CONFIG_BEGIN_CITY_ID))
			{
				return 0;
			}

            return int.Parse(mValues[CONFIG_BEGIN_CITY_ID] as string);

		 }
	}

    public static int GuideSceneID
	{
		get
		{
            if (!mValues.ContainsKey(CONFIG_GUIDE_SCENE_ID))
			{
				return -1;
			}

            return int.Parse((mValues[CONFIG_GUIDE_SCENE_ID] as string));
		}
	}

	public static int ArenaSceneID
	{
		get
		{
			return sArenaSceneID;
		}
	}
	public static uint ArenaBuyTimesCost
	{
		get
		{
			return sArenaBuyTimesCost;
		}
	}

	public static uint ArenaFightCD
	{
		get
		{
			return sArenaFightCD;
		}
	}

	public static int QualifyingSceneID
	{
		get
		{
			return sQualifyingSceneID;
		}
	}
	public static uint QualifyingBuyTimesCost
	{
		get
		{
			return sQualifyingBuyTimesCost;
		}
	}

	public static uint QualifyingFightCD
	{
		get
		{
			return sQualifyingFightCD;
		}
	}

	public static uint QualifyingWinPrestige
	{
		get
		{
			return sQualifyingWinPrestige;
		}
	}

	public static uint QualifyingWinGold
	{
		get
		{
			return sQualifyingWinGold;
		}
	}

	public static uint QualifyingLosePrestige
	{
		get
		{
			return sQualifyingLosePrestige;
		}
	}

	public static uint QualifyingLoseGold
	{
		get
		{
			return sQualifyingLoseGold;
		}
	}

	public static int QualifyingLockSecondOfDay
	{
		get
		{
			return sQualifyingLockSecondOfDay;
		}
	}

	public static int QualifyingAwardSecondOfDay
	{
		get
		{
			return sQualifyingAwardSecondOfDay;
		}
	}

	public static uint PvpBuffId
	{
		get
		{
			return sPvpBuffId;
		}
	}

	public static uint ZhaoCaiMaoReadyTime
	{
		get
		{
			return sZhaoCaiMaoReadyTime;
		}
	}

	public static uint ZhaoCaiMaoRequestTime
	{
		get
		{
			return sZhaoCaiMaoRequestTime;
		}
	}

    public static uint Jingying_jihua_zhuanshi
    {
        get
        {
            return sJingying_jihua_zhuanshi;
        }
    }

    public static uint Zhizun_jihua_zhuanshi
    {
        get
        {
            return sZhizun_jihua_zhuanshi;
        }
    }

	public static int TDLifeCount
	{
		get
		{
			return sTDLifeCount;
		}
	}

	public static int TDEffectId
	{
		get
		{
			return sTDEffectId;
		}
	}

	public static float TDEffectPosX
	{
		get
		{
			return sTDEffectPosX;
		}
	}

	public static float TDEffectPosZ
	{
		get
		{
			return sTDEffectPosZ;
		}
	}

	public static int YZXEMonsterMaxCount
	{
		get
		{
			return sYZXEMonsterMaxCount;
		}
	}

	public static int MaoMaxGoldCount
	{
		get
		{
			return sMaoMaxGoldCount;
		}
	}

	public static int MaoGoldId1
	{
		get
		{
			return sMaoGoldId1;
		}
	}

	public static int MaoGoldId2
	{
		get
		{
			return sMaoGoldId2;
		}
	}

	public static int MaoGoldId3
	{
		get
		{
			return sMaoGoldId3;
		}
	}

	public static uint CropHaloBuffId
	{
		get
		{
			return sCropHaloBuffId;
		}
	}

	public static int ZhaoCaiMaoId
	{
		get
		{
			return sZhaoCaiMaoId;
		}
	}

	public static float ZhaoCaiMaoPosX
	{
		get
		{
			return sZhaoCaiMaoPosX;
		}
	}

	public static float ZhaoCaiMaoPosY
	{
		get
		{
			return sZhaoCaiMaoPosY;
		}
	}

	public static float ZhaoCaiMaoDir
	{
		get
		{
			return sZhaoCaiMaoDir;
		}
	}

	/// <summary>
	/// 暴击伤害为实际伤害的多少倍.
	/// </summary>
	public static float CriticalDamageScale
	{
		get {
			if (!mValues.ContainsKey(CRITICAL_DAMAGE_SCALE))
				return 1;
			return float.Parse(mValues[CRITICAL_DAMAGE_SCALE] as string);
		}
	}

    public static float SceneCellSizeX
    {
        get
        {
            if (!mValues.ContainsKey(CONFIG_SCENE_CELL_SIZE_X))
            {
                return 3.0f;
            }

            return float.Parse(mValues[CONFIG_SCENE_CELL_SIZE_X] as string);
        }
    }

    public static float SceneCellSizeY
    {
        get
        {
            if (!mValues.ContainsKey(CONFIG_SCENE_CELL_SIZE_Y))
            {
                return 3.0f;
            }

            return float.Parse(mValues[CONFIG_SCENE_CELL_SIZE_Y] as string);
        }
    }

	public static float GravitationalAcceleration
	{
		get {
			if (!mValues.ContainsKey(GRAVITATIONAL_ACCELERATION))
			{
				badConfig(GRAVITATIONAL_ACCELERATION);
				return 12f;
			}
			return float.Parse(mValues[GRAVITATIONAL_ACCELERATION] as string);
		}
	}

	public static float HomingMissileDefaultRadius
	{
		get {
			if (!mValues.ContainsKey(HOMING_MISSILE_DEFAULT_RADIUS))
			{
				badConfig(HOMING_MISSILE_DEFAULT_RADIUS);
				return 2f;
			}
			return float.Parse(mValues[HOMING_MISSILE_DEFAULT_RADIUS] as string);
		}
	}

	/// <summary>
	/// 播放击中材质特效的CD时间.
	/// </summary>
	public static uint HitMaterialCd
	{
		get {
			if (!mValues.ContainsKey(HIT_MATERIAL_CD))
			{
				badConfig(HIT_MATERIAL_CD);
				return 1500;
			}
			return uint.Parse(mValues[HIT_MATERIAL_CD] as string);
		}
	}



    public static float KillNumberTime
    {
        get
        {
            if (!mValues.ContainsKey(KILL_NUMBER_TIME))
            {
                return 5.0f;
            }
            return float.Parse(mValues[KILL_NUMBER_TIME] as string) / 1000;
        }
    }

	public static float NormalReliveRate
	{
		get
		{
			if (!mValues.ContainsKey(CONFIG_NORMAL_RELIVE_RATE))
			{
				return 0.2f;
			}
			return float.Parse(mValues[CONFIG_NORMAL_RELIVE_RATE] as string);
		}
	}

	public static float ExtraReliveRate
	{
		get
		{
			if (!mValues.ContainsKey(CONFIG_EXTRA_RELIVE_RATE))
			{
				return 1.0f;
			}
			return float.Parse(mValues[CONFIG_EXTRA_RELIVE_RATE] as string);
		}
	}

    public static string ServerAddress
    {
        get
        {
            if (!mValues.ContainsKey(CONFIG_BEGIN_SERVER_ADDRESS))
                return DefaultAddr;

            string address = mValues[CONFIG_BEGIN_SERVER_ADDRESS] as string;
            if(address == null)
                return DefaultAddr;

            return address;
        }
    }
    public static string ChatServerAddress
    {
        get
        {
            if (!mValues.ContainsKey(CONFIG_CHAT_SRVER))
                return DefaultAddr;

            string address = mValues[CONFIG_CHAT_SRVER] as string;
            if (address == null)
                return DefaultChatAddr;

            return address;
        }
    }

    public static string QQServerAddress
    {
        get
        {
            if (!mValues.ContainsKey(CONFIG_QQ_LOGIN_SERVER_ADDRESS))
                return DefaultAddr;

            string address = mValues[CONFIG_QQ_LOGIN_SERVER_ADDRESS] as string;
            if (address == null)
                return DefaultAddr;

            return address;
        }
    }

    public static string QQChatServerAddress
    {
        get
        {
            if (!mValues.ContainsKey(CONFIG_QQ_LOGIN_CHAT_SRVER))
                return DefaultAddr;

            string address = mValues[CONFIG_QQ_LOGIN_CHAT_SRVER] as string;
            if (address == null)
                return DefaultChatAddr;

            return address;
        }
    }

    public static string WXChatServerAddress
    {
        get
        {
            if (!mValues.ContainsKey(CONFIG_WX_LOGIN_CHAT_SRVER))
                return DefaultAddr;

            string address = mValues[CONFIG_WX_LOGIN_CHAT_SRVER] as string;
            if (address == null)
                return DefaultChatAddr;

            return address;
        }
    }

    public static string WXServerAddress
    {
        get
        {
            if (!mValues.ContainsKey(CONFIG_WX_LOGIN_SERVER_ADDRESS))
                return DefaultAddr;

            string address = mValues[CONFIG_WX_LOGIN_SERVER_ADDRESS] as string;
            if (address == null)
                return DefaultAddr;

            return address;
        }
    }

	public static float SpasticityBeatbackSpeed
	{
		get
		{
			if (!mValues.ContainsKey(SPASTICITY_BEATBACK_SPEED))
			{
				badConfig(SPASTICITY_BEATBACK_SPEED);
				return 5f;
			}
			return float.Parse(mValues[SPASTICITY_BEATBACK_SPEED] as string);
		}
	}

	public static float MaxSpasticityResistance
	{
		get {
			if (!mValues.ContainsKey(MAX_SPASTICITY_RESISTANCE))
			{
				badConfig(MAX_SPASTICITY_RESISTANCE);
				return 1000f;
			}
			return float.Parse(mValues[MAX_SPASTICITY_RESISTANCE] as string);
		}
	}

	public static string CriticalStrikeEffectBindPoint {
		get {
			if (!mValues.Contains(CRITICAL_STRIKE_EFFECT_BINDPOINT))
			{
				badConfig(CRITICAL_STRIKE_EFFECT_BINDPOINT);
				return "";
			}

			return mValues[CRITICAL_STRIKE_EFFECT_BINDPOINT] as string;
		}
	}

	public static uint CriticalStrikeEffectID
	{
		get
		{
			if (!mValues.Contains(CRITICAL_STRIKE_EFFECT_ID))
			{
				badConfig(CRITICAL_STRIKE_EFFECT_ID);
				return uint.MaxValue;
			}

			return (uint)int.Parse(mValues[CRITICAL_STRIKE_EFFECT_ID] as string); 
		}
	}

	private static void badConfig(string keyName)
	{
		GameDebug.LogError("没有在xml配置中找到键值: " + keyName);
	}

    public static int LevelUpEffectID
    {
        get
        {

            return sLevelUpEffectID;
        }
    }

    public static float LevelUpShakeCameraAmount
    {
        get
        {

            return sLevelUpShakeCameraAmount;
        }
    }

    public static float LevelUpShakeCameraTime
    {
        get
        {
            return sLevelUpShakeCameraTime;
        }
    }
    public static float CPPlayIdlePerTime
    {
        get
        {
            return sCPPlayIdlePerTime;
        }
    }
    public static float WaveWingFrequency
    {
        get
        {
            return sWaveWingFrequency;
        }
    }

    public static string ServerDataURL
    {
        get
        {
            return AssetServerURL + DataURL;
        }
    }
    public static string ServerAppURL
    {
        get
        {
            return AssetServerURL + AppURL;
        }
    }

	public static string CachePath
	{
		get
		{
			return Application.persistentDataPath + "/cache/";
		}
	}

    public static string GetRealSrvDataPath(string version)
    {
        return AssetServerURL + version + "/" + DataURL;
    }
}
