using System;
using System.Text;
using Assets.Scripts.Managers.Data.DataField;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

//正在加载的信息
public class LoadHandler
{
	public DataType		dataType;
	public string		path;
	public Type			type;
	public bool			isXML;
	public int 			keyIdx;
	public LoadHandler()
	{
		keyIdx = 0;
		isXML = false;
	}
}

public enum DataType:int
{
	DATA_SCENE = 0,
    DATA_SCENE_CITY,
    DATA_SCENE_QIANGLINDANYU,
    DATA_SCENE_TOWER_STAGE,
    DATA_SCENE_ZOMBIES_STAGE,
    DATA_SCENE_MONSTERFLOOD,
    DATA_SCENE_STAGESCENE,
    DATA_SCENE_BATTLESCENE,
    DATA_SCENE_STAGELIST,
	DATA_SCENE_ARENA,
	DATA_SCENE_QUALIFYING,
    DATA_SCENE_GOLD,
    DATA_SCENE_HUNNENG,
	DATA_SCENE_WANTED,
	DATA_SCENE_TD,
	DATA_SCENE_YAZHIXIEE,
	DATA_SCENE_ZHAOCAIMAO,
	DATA_PLAYER ,
	DATA_NPC,
	DATA_TRAP,
	DATA_MODEL,
	DATA_UICONFIG,
	DATA_SKILL_COMMON,
	DATA_SKILL_BEHAVIOUR,
	DATA_SKILL_EFFECT,
	DATA_SKILL_BULLET,
	DATA_WEAPON,
    DATA_MONEY_ITEM,
	DATA_SKILL_CREATION,
	DATA_SKILL_TARGET_SELECTION,
	DATA_SKILL_BUFF,
	DATA_SKILL_IMPACT,
	DATA_SKILL_DISPLACEMENT,
	DATA_SKILL_RAND_EVENT,
	DATA_SKILL_SPASTICITY,
	DATA_BULLET_DISTRIBUTION,
	DATA_PROJECTILE_SETTINGS,
	DATA_MATERIAL,
	DATA_AI,
    DATA_PROPERTY,
    DATA_LEVEL,
    DATA_CONDITION,
    DATA_EFFECT,
    DATA_ITEM,
    DATA_NORMALITEM,
    DATA_DEFENCE,
    DATA_ITEMDESCRIBE,
    DATA_PRESTIGE,
    DATA_SKILL_LEARN,
    DATA_SKILL_LEVEL,
    DATA_QUEST,
    DATA_SOUND,
	DATA_DROPBOX,
	DATA_DROPGROUP,
    DATA_CHALLENGE,
    DATA_STRENGTH,
    DATA_STRENPROPERTY,
    DATA_PROMOTE,
    DATA_FITTINGS,
    DATA_FITTODDS,
    DATA_FITTCOLOR,
    DATA_FITTPOS,
    DATA_FITTSCORE,
    DATA_MENU,
	DATA_PICK,
    DATA_BUILD,
	DATA_STORY,
    DATA_LEVEL_COMMON_PROPERTIES,
    DATA_NPC_TALK,
    DATA_GUIDE ,
    DATA_GUIDE_STEP,
    DATA_ERROR_STRING,
    DATA_STRING,
    DATA_MALL,
    DATA_STONE,
    DATA_DEFENCE_STREN_PRO,
    DATA_DEFENCE_STREN,
    DATA_DEFENCE_STARS_PRO,
    DATA_DEFENCE_STARS,
    DATA_DEFENCE_COMB,
	DATA_ARENA,
	DATA_ARENA_RANDOM,
	DATA_VIP,
    DATA_SHOW_WEAPON,
    DATA_SHOP,
	DATA_WING_COMMON,
	DATA_WING_LEVEL,
    DATA_ACTIVITY_TYPE,
    DATA_ACTIVITY,
    DATA_PRAT_MODELS,
	DATA_QUALIFYING_AWARD,
	DATA_QUALIFYING_RANDOM,
    DATA_ANNOUNCEMENT,
    DATA_QUACK_NUMBER,
    DATA_EGG,
    DATA_CONFIG,
    DATA_EGG_CONFIG,
    DATA_PACKAGE_EXTEND,
    DATA_CREATE_PLAYER,
    DATA_STR_FILTER,
    DATA_SUPER_WEAPON,
    DATA_CHALLENGE_RANK_AWARD,
    DATA_CHALLENGE_RANK_AWARD_ITEM,
    DATA_WEAPON_SKILL,
    DATA_TITLE_GROUP,
    DATA_TITLE_ITEM,
    DATA_ZHUSHOU,
    DATA_SCENE_STAGE_REWARD,
    DATA_FASHION,
    DATA_FASHION_PROP,
    DATA_FASHION_STAR,
    DATA_CROPS,
    DATA_AWARD,
    DATA_CROPS_LEVEL,
    DATA_CROPS_RATIO,
    DATA_FUND,
    DATA_ASSISTER,
    DATA_PLAN,
    DATA_ASSISTERITEM,
	DATA_ZHAOCAIMAO_AWARD,
    DATA_FIRSTCHARGE_REWARD,
    DATA_CROPSSKILLLEARN,
    DATA_CROPSSKILLLEVEL,
    DATA_BOX_ITEM,
    DATA_TOTAL_CHARGE,
    DATA_UI_EFFECT,
    DATA_CHARGE_REWARDS,
}

class CustomEqualityComparer : IEqualityComparer
{
    static StringBuilder tempCompareStrA = new StringBuilder(256);
    static StringBuilder tempCompareStrB = new StringBuilder(256);

    static Type intType = typeof(int);
    static Type uintType = typeof(uint);

	public bool Equals(object x, object y)    
	{
        //频繁的调用object.ToString()会产生内存开销，积累到一定程度会有GC问题
        //return x.ToString() == y.ToString();

        Type tx = x.GetType();
        Type ty = y.GetType();
        if ((tx == intType || tx == uintType) &&
            (ty == intType || ty == uintType))
        {
            return Convert.ToInt32(x) == Convert.ToInt32(y);
        }
        tempCompareStrA.Length = 0;
        tempCompareStrA.Append(x);

        tempCompareStrB.Length = 0;
        tempCompareStrB.Append(y);

        if( tempCompareStrA.Length != tempCompareStrB.Length )
        {
            return false;
        }
        for (int i = 0; i < tempCompareStrA.Length; ++i )
        {
            if (tempCompareStrA[i] != tempCompareStrB[i])
                return false;
        }
        return true;
        //return tempCompareStrA.ToString() == tempCompareStrB.ToString();
	}
	  
	public int GetHashCode(object obj)  
	{
        if( obj is string )
        {
            return ((string)obj).GetHashCode();
        }
		return obj.ToString().GetHashCode();
	}
}

/// <summary>
/// 数据管理器.
/// </summary>
public class DataManager
{
	private Dictionary<string, LoadHandler> mLoadHandles = new Dictionary<string, LoadHandler>();

    private Dictionary<int, DataTable> mTableList = new Dictionary<int, DataTable>();


	public delegate void CompleteCallback();

	private CompleteCallback mCallback = null;

	private ulong mReadFileStartTime = 0;

	private bool mDataInited = false;

    private DataTable mSceneTable = new DataTable();
    private DataTable mStageTable = new DataTable();

	private static DataManager instance;
	public DataManager()
	{
		instance = this;
	}

	public static DataManager Instance
	{
		get
		{
			return instance;
		}
	}

    public DataTable GetTable(DataType dataType)
	{
        int key = (int)dataType;
        if (!mTableList.ContainsKey(key))
	    {
            GameDebug.Log("没有" + dataType + "表数据");
            return new DataTable();
	    }
        return mTableList[key];
	}

	public void SyncInit(CompleteCallback callback)
	{
		if (mDataInited)
			return;

		mCallback = callback;

		mReadFileStartTime = TimeUtilities.GetNow();

        AddLoader<Scene_CitySceneTableItem>(DataType.DATA_SCENE_CITY, "@/Data/scene/scene_city.txt");
        AddLoader<Scene_QiangLinDanYuSceneTableItem>(DataType.DATA_SCENE_QIANGLINDANYU, "@/Data/scene/scene_qianglindanyu.txt");
        AddLoader<Scene_StageSceneTableItem>(DataType.DATA_SCENE_STAGESCENE, "@/Data/scene/scene_stagescene.txt");
        AddLoader<Scene_StageSceneTableItem>(DataType.DATA_SCENE_TOWER_STAGE, "@/Data/scene/scene_tower.txt");
        AddLoader<Scene_StageSceneTableItem>(DataType.DATA_SCENE_ZOMBIES_STAGE, "@/Data/scene/scene_zombies.txt");
        AddLoader<Scene_StageSceneTableItem>(DataType.DATA_SCENE_MONSTERFLOOD, "@/Data/scene/scene_monsterflood.txt");
        AddLoader<Scene_ArenaSceneTableItem>(DataType.DATA_SCENE_ARENA, "@/Data/scene/scene_arena.txt");
        AddLoader<Scene_QualifyingSceneTableItem>(DataType.DATA_SCENE_QUALIFYING, "@/Data/scene/scene_qualifying.txt");
        AddLoader<Scene_StageSceneTableItem>(DataType.DATA_SCENE_GOLD, "@/Data/scene/scene_mao.txt");
        AddLoader<Scene_StageSceneTableItem>(DataType.DATA_SCENE_HUNNENG, "@/Data/scene/scene_hunneng.txt");
        AddLoader<Scene_StageSceneTableItem>(DataType.DATA_SCENE_WANTED, "@/Data/scene/scene_wantedscene.txt");
        AddLoader<Scene_TDSceneTableItem>(DataType.DATA_SCENE_TD, "@/Data/scene/scene_td.txt");
        AddLoader<Scene_YaZhiXieESceneTableItem>(DataType.DATA_SCENE_YAZHIXIEE, "@/Data/scene/scene_yazhixiee.txt");
        AddLoader<Scene_ZhaoCaiMaoSceneTableItem>(DataType.DATA_SCENE_ZHAOCAIMAO, "@/Data/scene/scene_zhaocaimao.txt");

        //AddLoader<SceneTableItem>(DataType.DATA_SCENE, "@/Data/scene.txt");
        //AddLoader<Scene_StageSceneTableItem>(DataType.DATA_SCENE_STAGESCENE, "@/Data/scene_stagescene.txt");
        AddLoader<Scene_BattleSceneTableItem>(DataType.DATA_SCENE_BATTLESCENE, "@/Data/scene_battlescene.txt");

        AddLoader<PlayerTableItem>(DataType.DATA_PLAYER, "@/Data/player.txt");
        AddLoader<NPCTableItem>(DataType.DATA_NPC, "Data/npc.txt");
        AddLoader<TrapTableItem>(DataType.DATA_TRAP, "Data/trap.txt");
        AddLoader<ModelTableItem>(DataType.DATA_MODEL, "Data/model.txt");
        AddLoader<UITableItem>(DataType.DATA_UICONFIG, "Data/uiconfig.txt", 1);
        AddLoader<SkillCommonTableItem>(DataType.DATA_SKILL_COMMON, "Data/skillcommon.txt");
        AddLoader<SkillClientBehaviourItem>(DataType.DATA_SKILL_BEHAVIOUR, "Data/skillclientbehaviour.txt");
        AddLoader<SkillEffectTableItem>(DataType.DATA_SKILL_EFFECT, "Data/skilleffect.txt");
        AddLoader<AITableItem>(DataType.DATA_AI, "Data/ai.txt");
        AddLoader<SkillCreationTableItem>(DataType.DATA_SKILL_CREATION, "Data/creation.txt");
        AddLoader<PropertyTableItem>(DataType.DATA_PROPERTY, "Data/property.xml", 0, true);
        AddLoader<BulletTableItem>(DataType.DATA_SKILL_BULLET, "Data/bullet.txt");
        AddLoader<TargetSelectionTableItem>(DataType.DATA_SKILL_TARGET_SELECTION, "Data/targetselection.txt");
        AddLoader<SkillBuffTableItem>(DataType.DATA_SKILL_BUFF, "Data/buff.txt");
        AddLoader<SkillImpactTableItem>(DataType.DATA_SKILL_IMPACT, "Data/impact.txt");
        AddLoader<SkillRandEventTableItem>(DataType.DATA_SKILL_RAND_EVENT, "Data/skillrandevent.txt");
        AddLoader<SkillDisplacementTableItem>(DataType.DATA_SKILL_DISPLACEMENT, "Data/displacement.txt");
        AddLoader<SkillSpasticityTableItem>(DataType.DATA_SKILL_SPASTICITY, "Data/skillspasticity.txt");
        AddLoader<BulletDistributionTableItem>(DataType.DATA_BULLET_DISTRIBUTION, "Data/bulletdistribution.txt");
        AddLoader<MaterialTableItem>(DataType.DATA_MATERIAL, "Data/material.txt");
        AddLoader<ProjectileSettingsTableItem>(DataType.DATA_PROJECTILE_SETTINGS, "Data/projectilesettings.txt");
        AddLoader<LevelTableItem>(DataType.DATA_LEVEL, "@/Data/level.txt");
        AddLoader<Scene_StageListTableItem>(DataType.DATA_SCENE_STAGELIST, "@/Data/scene_stagelist.txt");
        AddLoader<ConditionTableItem>(DataType.DATA_CONDITION, "@/Data/condition.txt");
        AddLoader<EffectTableItem>(DataType.DATA_EFFECT, "Data/effect.txt");

        AddLoader<WeaponTableItem>(DataType.DATA_WEAPON, "@/Data/item/weapon.txt");
        AddLoader<NormalItemTableItem>(DataType.DATA_NORMALITEM, "@/Data/item/normalitem.txt");
        AddLoader<DefenceTableItem>(DataType.DATA_DEFENCE, "@/Data/item/defence.txt");
        AddLoader<StoneTableItem>(DataType.DATA_STONE, "@/Data/item/stones.txt");
        AddLoader<MoneyItemTableItem>(DataType.DATA_MONEY_ITEM, "@/Data/item/moneyitem.txt");

        AddLoader<PrestigeTableItem>(DataType.DATA_PRESTIGE, "@/Data/prestige.txt");
        AddLoader<StrenTableItem>(DataType.DATA_STRENGTH, "@/Data/strength.txt");
        AddLoader<StrProTableItem>(DataType.DATA_STRENPROPERTY, "@/Data/strenproperty.txt");
        AddLoader<PromoteTableItem>(DataType.DATA_PROMOTE, "@/Data/promote.txt");
        AddLoader<FittingsTableItem>(DataType.DATA_FITTINGS, "@/Data/item/fittings.txt");
        AddLoader<FittoddsTableItem>(DataType.DATA_FITTODDS, "@/Data/fittodds.txt");
        AddLoader<FittcolorTableItem>(DataType.DATA_FITTCOLOR, "Data/fittcolor.txt");
        AddLoader<FittposTableItem>(DataType.DATA_FITTPOS, "Data/fittpos.txt");
        AddLoader<FittingsFightValueItem>(DataType.DATA_FITTSCORE, "@/Data/fittingfightvalue.txt");
        AddLoader<SkillLearnTableItem>(DataType.DATA_SKILL_LEARN, "@/Data/skilllearn.txt");
        AddLoader<SkillLevelTableItem>(DataType.DATA_SKILL_LEVEL, "@/Data/skilllevel.txt");
        AddLoader<QuestTableItem>(DataType.DATA_QUEST, "@/Data/quest.txt");
        AddLoader<SoundTableItem>(DataType.DATA_SOUND, "Data/sound.txt");
        AddLoader<DropBoxTableItem>(DataType.DATA_DROPBOX, "@/Data/dropbox.txt");
        AddLoader<DropGroupTableItem>(DataType.DATA_DROPGROUP, "@/Data/dropgroup.txt");
        AddLoader<ChallengeTableItem>(DataType.DATA_CHALLENGE, "@/Data/challenge.txt");
        AddLoader<MenuTableItem>(DataType.DATA_MENU, "Data/menu.txt");
        AddLoader<PickTableItem>(DataType.DATA_PICK, "Data/pick.txt");
        AddLoader<BuildTableItem>(DataType.DATA_BUILD, "Data/build.txt");
        AddLoader<StoryTableItem>(DataType.DATA_STORY, "Data/story.txt");
        AddLoader<LevelCommonPropertiesItem>(DataType.DATA_LEVEL_COMMON_PROPERTIES, "Data/levelcommonproperties.txt");
        AddLoader<NpcTalkTableItem>(DataType.DATA_NPC_TALK, "Data/npctalk.txt");
        AddLoader<GuideTableItem>(DataType.DATA_GUIDE, "Data/guide.txt");
        AddLoader<GuideStepTableItem>(DataType.DATA_GUIDE_STEP, "Data/guidestep.txt");
        AddLoader<MallTableItemBase>(DataType.DATA_MALL, "@/Data/mall.txt");
        AddLoader<StringTableItem>(DataType.DATA_STRING, "String/string_cn.txt", 1);
        AddLoader<ErrorStringTableItem>(DataType.DATA_ERROR_STRING, "String/errorstring_cn.txt");
        AddLoader<DefenceStrenItem>(DataType.DATA_DEFENCE_STREN, "@/Data/defencestren.txt");
        AddLoader<DefenceStrenProItem>(DataType.DATA_DEFENCE_STREN_PRO, "@/Data/defencestrenpro.txt");
        AddLoader<DefenceStarsItem>(DataType.DATA_DEFENCE_STARS, "@/Data/defencestars.txt");
        AddLoader<DefenceStarsProItem>(DataType.DATA_DEFENCE_STARS_PRO, "@/Data/defencestarspro.txt");
        AddLoader<DefenceCombItem>(DataType.DATA_DEFENCE_COMB, "@/Data/defencecomb.txt");
        AddLoader<ArenaTableItem>(DataType.DATA_ARENA, "@/Data/arena.txt");
        AddLoader<ArenaRandomTableItem>(DataType.DATA_ARENA_RANDOM, "@/Data/arenarandom.txt");
        AddLoader<VipTableItem>(DataType.DATA_VIP, "@/Data/vip.txt");
        AddLoader<ShowWeaponItem>(DataType.DATA_SHOW_WEAPON, "Data/showweapon.txt");
        AddLoader<ShopTableItem>(DataType.DATA_SHOP, "@/Data/shop.txt");
        AddLoader<WingCommonTableItem>(DataType.DATA_WING_COMMON, "@/Data/wingcommon.txt");
        AddLoader<WingLevelTableItem>(DataType.DATA_WING_LEVEL, "@/Data/winglevel.txt");
        AddLoader<ActivityTableItem>(DataType.DATA_ACTIVITY, "@/Data/activity.txt");
        AddLoader<ActivityTypeTableItem>(DataType.DATA_ACTIVITY_TYPE, "@/Data/activitytype.txt");
        AddLoader<PartModelTableItem>(DataType.DATA_PRAT_MODELS, "Data/partsmodel.txt");
        AddLoader<QualifyingAwardTableItem>(DataType.DATA_QUALIFYING_AWARD, "@/Data/qualifying_award.txt");
        AddLoader<QualifyingRandomTableItem>(DataType.DATA_QUALIFYING_RANDOM, "@/Data/qualifying_random.txt");
        AddLoader<AnnouncementItem>(DataType.DATA_ANNOUNCEMENT, "Data/announcement.txt");
        AddLoader<EggTableItem>(DataType.DATA_EGG, "@/Data/egg.txt");
        AddLoader<QuackNumberTableItem>(DataType.DATA_QUACK_NUMBER, "Data/quacknumber.txt");
        AddLoader<ConfigTableItem>(DataType.DATA_CONFIG, "@/Data/config.txt");
        AddLoader<EggConfigTableItem>(DataType.DATA_EGG_CONFIG, "@/Data/egg_config.txt");
        AddLoader<PackageTableItem>(DataType.DATA_PACKAGE_EXTEND, "@/Data/package.txt");
        AddLoader<CreatePlayerItem>(DataType.DATA_CREATE_PLAYER, "Data/createplayer.txt");
        AddLoader<StrFilterTableItem>(DataType.DATA_STR_FILTER, "String/StrFilter.txt");
        AddLoader<SuperWeaponTableItem>(DataType.DATA_SUPER_WEAPON, "Data/superweapon.txt");
        AddLoader<ChallengeRankAwardTableItem>(DataType.DATA_CHALLENGE_RANK_AWARD, "@/Data/challenge_rank_award.txt");
        AddLoader<ChaRankAwardItemTableItem>(DataType.DATA_CHALLENGE_RANK_AWARD_ITEM, "@/Data/challenge_rank_award_item.txt");
        AddLoader<WeaponSkillTableItem>(DataType.DATA_WEAPON_SKILL, "@/Data/weaponskill.txt");
        AddLoader<ZhushouTableItem>(DataType.DATA_ZHUSHOU, "Data/zhushou.txt");
        AddLoader<TitleGroupTableItem>(DataType.DATA_TITLE_GROUP, "@/Data/titlegroup.txt");
        AddLoader<TitleItemTableItem>(DataType.DATA_TITLE_ITEM, "@/Data/titleitem.txt");
        AddLoader<ZoneRewardItem>(DataType.DATA_SCENE_STAGE_REWARD,"@/Data/scene/scene_zonereward.txt");
        AddLoader<FashionTableItem>(DataType.DATA_FASHION, "@/Data/fashion.txt");
        AddLoader<FashionPropTableItem>(DataType.DATA_FASHION_PROP, "@/Data/fashionprop.txt");
        AddLoader<FashionStarsTableItem>(DataType.DATA_FASHION_STAR, "@/Data/fashionstars.txt");
        AddLoader<CropsTableItem>(DataType.DATA_CROPS, "@/Data/item/crops.txt");
        AddLoader<SevenTableItem>(DataType.DATA_AWARD, "@/Data/seven_award.txt");
        AddLoader<CropsLevelTableItem>(DataType.DATA_CROPS_LEVEL, "@/Data/cropslevel.txt");
        AddLoader<CropsProRatioTableItem>(DataType.DATA_CROPS_RATIO, "@/Data/cropsproratio.txt");
        AddLoader<FundTableItem>(DataType.DATA_FUND, "@/Data/foundation.txt");
        AddLoader<FirstChargeRewardTableItemBase>(DataType.DATA_FIRSTCHARGE_REWARD, "@/Data/firstcharge_reward.txt");
        AddLoader<AssisterItemTableItem>(DataType.DATA_ASSISTERITEM, "Data/assisteritem.txt");
        AddLoader<AssisterTableItem>(DataType.DATA_ASSISTER, "Data/assister.txt");

		AddLoader<ZhaoCaiMaoAwardTableItem>(DataType.DATA_ZHAOCAIMAO_AWARD, "@/Data/zhaocaimao_award.txt");
        AddLoader<CropsSkillLearnTableItem>(DataType.DATA_CROPSSKILLLEARN, "@/Data/cropsskilllearn.txt");
        AddLoader<CropsSkillLevelTableItem>(DataType.DATA_CROPSSKILLLEVEL, "@/Data/cropsskilllevel.txt");

        AddLoader<PlayerPlanTableItem>(DataType.DATA_PLAN, "@/Data/playerplan_award.txt");

        AddLoader<BoxItemTableItem>(DataType.DATA_BOX_ITEM, "@/Data/item/boxitem.txt");
        AddLoader<TotalChargeTableItem>(DataType.DATA_TOTAL_CHARGE, "@/Data/totalcharge.txt");
        AddLoader<UIEffectTableItem>(DataType.DATA_UI_EFFECT, "Data/uieffect.txt");       
        AddLoader<ChargeRewardsTableItem>(DataType.DATA_CHARGE_REWARDS, "@/Data/charge_rewards.txt");
        mDataInited = true;
    }

	private void AddLoader<T>(DataType dataType, string filename, int keyIdx = 0, bool isXML = false)
	{
		LoadHandler handle = new LoadHandler();
		handle.dataType = dataType;
		handle.path = filename;
		handle.type = typeof(T);
		handle.isXML = isXML;
		handle.keyIdx = keyIdx;
		mLoadHandles.Add(filename, handle);

		DataChecker.GetInstance().Append(dataType, filename);
		ResourceManager.Instance.LoadBytes(filename, OnLoadTextCallback);
	}

	private void OnLoadTextCallback(string path, byte[] bytes)
	{

		if (!mLoadHandles.ContainsKey(path))
		{
			//出现严重错误
			return;
		}
		string text = "";

		LoadHandler handle = mLoadHandles[path] as LoadHandler;

		if (handle.isXML)
		{
			text = System.Text.Encoding.UTF8.GetString(bytes);
		}
		else
		{
			text = System.Text.Encoding.Unicode.GetString(bytes);
		}

		if (string.IsNullOrEmpty(text))
		{
			return;
		}

		//Hashtable tb = new Hashtable(new CustomEqualityComparer());

        DataTable tb = new DataTable();

		if (!handle.isXML)
		{
			WDBData dbData = null;
			try
			{
			   
				dbData = TextAnalyze.Analyze(text);
               
			}
			catch (Exception analyzeException)
			{
				throw new Exception("加载" + path + "失败: " + analyzeException.Message);
			}

			for (int i = 0; i < dbData.GetRecordCount(); ++i)
			{
				WDBSheetLine line = dbData.GetDataByNumber(i);
               
				if (line != null)
				{
					object item = System.Activator.CreateInstance(handle.type);
					try
					{
						tb.Add(loadLine(line, item, handle.keyIdx), item);
					}
					catch (Exception exp)
					{
						throw new Exception("解析" + path + "失败: " + exp.Message);
					}
				}
			}
		}
		else
		{
			XmlDocument xDoc = new XmlDocument();
			xDoc.LoadXml(text);
			XmlNode node = xDoc.FirstChild;
			node = node.NextSibling;
			XmlNodeList nodeList = node.ChildNodes;
			for (int i = 0; i < nodeList.Count; ++i)
			{
				XmlNode childNode = nodeList[i];

				if (!string.IsNullOrEmpty(childNode.Name) &&
				   childNode.LocalName == "#comment")
				{
					continue;
				}
				object item = System.Activator.CreateInstance(handle.type);

				Type itemType = item.GetType();
				System.Reflection.FieldInfo[] fields = itemType.GetFields();

				foreach (System.Reflection.FieldInfo f in fields)
				{
					XmlNode nd = childNode.Attributes.GetNamedItem(f.Name);
					if (nd != null)
					{
						if (f.FieldType.Name == "Int32")
							f.SetValue(item, System.Convert.ToInt32(nd.Value));
						if (f.FieldType.Name == "Single")
							f.SetValue(item, System.Convert.ToSingle(nd.Value));
						if (f.FieldType.Name == "String")
							f.SetValue(item, nd.Value);

					}
				}

				tb.Add(fields[0].GetValue(item), item);
			}
		}

		mLoadHandles.Remove(path);
		mTableList.Add((int)handle.dataType, tb);


		if (mLoadHandles.Count <= 0)
        {
            OnAllLoad();
		}
	}

    private void UpdateSceneTable(DataTable dataTable, bool scene, bool stage)
    {
        IDictionaryEnumerator itr = dataTable.GetEnumerator();
        while (itr.MoveNext())
        {
            if (itr.Value != null)
            {
                if( scene )
                    mSceneTable.Add(itr.Key, itr.Value);
                if(stage)
                    mStageTable.Add(itr.Key, itr.Value);
            }
        }
    }

    private void OnAllLoad()
    {
        //ulong start = TimeUtilities.GetNow();

        if (!DataChecker.GetInstance().Run())
        {
            GameDebug.LogError("数据填写错误, 需要修正后才可以正常运行!");
			return;
        }

        DataChecker.DestroyInstance();

        DataTable dataTable = DataManager.Instance.GetTable(DataType.DATA_SCENE_CITY);

        UpdateSceneTable(dataTable, true , false);

        dataTable = DataManager.instance.GetTable(DataType.DATA_SCENE_QIANGLINDANYU);
        UpdateSceneTable(dataTable, true, false);

        dataTable = DataManager.instance.GetTable(DataType.DATA_SCENE_QIANGLINDANYU);
        UpdateSceneTable(dataTable, true, false);

        dataTable = DataManager.instance.GetTable(DataType.DATA_SCENE_MONSTERFLOOD);
        UpdateSceneTable(dataTable, true, true);

        dataTable = DataManager.instance.GetTable(DataType.DATA_SCENE_STAGESCENE);
        UpdateSceneTable(dataTable, true, true);

        dataTable = DataManager.instance.GetTable(DataType.DATA_SCENE_TOWER_STAGE);
        UpdateSceneTable(dataTable, true, true);

        dataTable = DataManager.instance.GetTable(DataType.DATA_SCENE_ZOMBIES_STAGE);
        UpdateSceneTable(dataTable, true, true);

        dataTable = DataManager.instance.GetTable(DataType.DATA_SCENE_GOLD);
        UpdateSceneTable(dataTable, true, true);

        dataTable = DataManager.instance.GetTable(DataType.DATA_SCENE_HUNNENG);
        UpdateSceneTable(dataTable, true, true);


        dataTable = DataManager.Instance.GetTable(DataType.DATA_SCENE_ARENA);
        UpdateSceneTable(dataTable, true, false);

        dataTable = DataManager.Instance.GetTable(DataType.DATA_SCENE_QUALIFYING);
        UpdateSceneTable(dataTable, true, false);

        dataTable = DataManager.Instance.GetTable(DataType.DATA_SCENE_WANTED);
        UpdateSceneTable(dataTable, true, true);

        dataTable = DataManager.Instance.GetTable(DataType.DATA_SCENE_TD);
        UpdateSceneTable(dataTable, true, true);

        dataTable = DataManager.Instance.GetTable(DataType.DATA_SCENE_YAZHIXIEE);
        UpdateSceneTable(dataTable, true, false);

        dataTable = DataManager.Instance.GetTable(DataType.DATA_SCENE_ZHAOCAIMAO);
        UpdateSceneTable(dataTable, true, false);

        //SoundManager Init里需要SceneTable
        DataTable table = DataManager.Instance.GetTable(DataType.DATA_SOUND);
        SoundManager.Instance.Init(table);

		GameDebug.Log("load file successfully, time cost = " + (TimeUtilities.GetNow() - mReadFileStartTime) + "ms.");



		if (mCallback != null)
			mCallback();
    }

	/// <summary>
	/// 填写target的数据.
	/// </summary>
	/// <param name="fromType">target被解析为的类型, 它与target.GetType()不一定相同, 可能是该类型的基类</param>
	/// <param name="target">需要被填写的对象</param>
	/// <param name="paramArray">参数列表</param>
	/// <param name="paramIndex">参数列表的开始位置, 从该位置开始的内容被填写到target中</param>
	static private void loadLine(Type fromType, object target, WDBSheetLine paramArray, ref int paramIndex)
	{
		// 如果fromType有基类, 先读取基类的信息.
		if (fromType.BaseType != typeof(object))
			loadLine(fromType.BaseType, target, paramArray, ref paramIndex);

		// 不读取static成员.
		System.Reflection.FieldInfo[] fields = fromType.GetFields(
			System.Reflection.BindingFlags.Public
			| System.Reflection.BindingFlags.DeclaredOnly
			| System.Reflection.BindingFlags.Instance
			);

		foreach (System.Reflection.FieldInfo f in fields)
		{
			Type elementType = f.FieldType;
			int count = 1;

			// 如果该域是数组, 那么取数组长度和数组的元素类型.
			if (f.FieldType.IsArray)
			{
				count = ((object[])f.GetValue(target)).Length;
				elementType = f.FieldType.GetElementType();
			}

			// 
			object[] value = new object[count];

			for (int i = 0; i < count; ++i)
			{
				// 如果是内置类型(string的IsPrimitive为false, 特殊处理).
				if (!elementType.IsPrimitive && (elementType != typeof(string)))
				{
					value[i] = Activator.CreateInstance(elementType);
					loadLine(elementType, value[i], paramArray, ref paramIndex);
				}
				else
				{
					if (paramIndex >= paramArray.m_Line.Length)
						throw new Exception("文件中的列数和结构中定义的不一致");

					value[i] = paramArray.GetData(paramIndex++);
				}
			}

			// 拷贝数组到结果中.
			if (f.FieldType.IsArray)
				Array.Copy(value, (Array)f.GetValue(target), count);
			else
				f.SetValue(target, value[0]);
		}
	}

	private static object loadLine(WDBSheetLine line, object item, int keyIdx)
	{
		int index = 0;

		loadLine(item.GetType(), item, line, ref index);

		if (index != line.m_Line.Length)
			throw new Exception("文件中的列数和结构中定义的不一致");

		return line.GetData(keyIdx);
	}

	public static DataTable SceneTable
	{
		get
		{
            return DataManager.Instance.mSceneTable;
		}
	}

	public static DataTable PlayerTable
	{
		get
		{
			return DataManager.Instance.GetTable(DataType.DATA_PLAYER);
		}
	}

	public static DataTable ModelTable
	{
		get
		{
			return DataManager.Instance.GetTable(DataType.DATA_MODEL);
		}
	}

	public static DataTable UITable
	{
		get
		{
			return DataManager.Instance.GetTable(DataType.DATA_UICONFIG);
		}
	}

	public static DataTable NPCTable
	{
		get
		{
			return DataManager.Instance.GetTable(DataType.DATA_NPC);

		}
	}

	public static DataTable TrapTable
	{
		get { return DataManager.Instance.GetTable(DataType.DATA_TRAP); }
	}

	public static DataTable PropertyTable
	{
		get
		{
			return DataManager.Instance.GetTable(DataType.DATA_PROPERTY);

		}
	}

    public static DataTable StrenTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_STRENGTH);

        }
    }

    public static DataTable StrProTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_STRENPROPERTY);

        }
    }

    public static DataTable PromoteTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_PROMOTE);

        }
    }

	public static DataTable WeaponTable
	{
		get
		{
			return DataManager.Instance.GetTable(DataType.DATA_WEAPON);

		}
	}

    //public static DataTable ItemTable
    //{
    //    get
    //    {
    //        return DataManager.Instance.GetTable(DataType.DATA_ITEM);

    //    }
    //}

    public static DataTable NormalItemTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_NORMALITEM);

        }
    }

    public static DataTable MoneyItemTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_MONEY_ITEM);

        }
    }

    public static DataTable DefenceTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_DEFENCE);

        }
    }

    public static DataTable ItemDescribeTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_ITEMDESCRIBE);

        }
    }

    public static DataTable FittingsTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_FITTINGS);
        }
    }

    public static DataTable FittoddsTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_FITTODDS);
        }
    }

    public static DataTable FittcolorTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_FITTCOLOR);
        }
    }

    public static DataTable FittposTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_FITTPOS);
        }
    }

    public static DataTable FittingsFightValueTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_FITTSCORE);
        }
    }

    public static DataTable PrestigeTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_PRESTIGE);

        }
    }

	public static DataTable SkillCommonTable
	{
		get
		{
			return DataManager.Instance.GetTable(DataType.DATA_SKILL_COMMON);

		}
	}

	public static DataTable SkillClientBehaviourTable
	{
		get
		{
			return DataManager.Instance.GetTable(DataType.DATA_SKILL_BEHAVIOUR);

		}
	}

	public static DataTable SkillEffectTable
	{
		get
		{
			return DataManager.Instance.GetTable(DataType.DATA_SKILL_EFFECT);

		}
	}

	public static DataTable BulletTable
	{
		get
		{
			return DataManager.Instance.GetTable(DataType.DATA_SKILL_BULLET);

		}
	}

	public static DataTable SkillSpasticityTable
	{
		get
		{
			return Instance.GetTable(DataType.DATA_SKILL_SPASTICITY);
		}
	}

	public static DataTable CreationTable
	{
		get
		{
			return DataManager.Instance.GetTable(DataType.DATA_SKILL_CREATION);
		}
	}

	public static DataTable TargetSelectionTable
	{
		get {
			return DataManager.Instance.GetTable(DataType.DATA_SKILL_TARGET_SELECTION);
		}
	}

	public static DataTable BuffTable
	{
		get {
			return DataManager.Instance.GetTable(DataType.DATA_SKILL_BUFF);
		}
	}

	public static DataTable ImpactTable
	{
		get {
			return DataManager.Instance.GetTable(DataType.DATA_SKILL_IMPACT);
		}
	}

	public static DataTable DisplacementTable
	{
		get {
			return DataManager.Instance.GetTable(DataType.DATA_SKILL_DISPLACEMENT);
		}
	}

	public static DataTable RandEventTable
	{
		get {
			return DataManager.Instance.GetTable(DataType.DATA_SKILL_RAND_EVENT);
		}
	}

	public static DataTable ProjectileSettingsTable
	{
		get {
			return DataManager.Instance.GetTable(DataType.DATA_PROJECTILE_SETTINGS);
		}
	}

	public static DataTable BulletDistributionTable
	{
		get { return DataManager.Instance.GetTable(DataType.DATA_BULLET_DISTRIBUTION); }
	}

	public static DataTable MaterialTable
	{
		get { return DataManager.instance.GetTable(DataType.DATA_MATERIAL); }
	}

	public static DataTable AITable
	{
		get
		{
			return DataManager.Instance.GetTable(DataType.DATA_AI);

		}
	}
    public static DataTable LevelTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_LEVEL);

        }
    }

    public static DataTable Scene_StageSceneTable
    {
        get
        {
            return DataManager.Instance.mStageTable;
        }
    }

	public static DataTable Scene_ArenaSceneTable
	{
		get
		{
			return DataManager.Instance.GetTable(DataType.DATA_SCENE_ARENA);
		}
	}

	public static DataTable Scene_QualifyingSceneTable
	{
		get
		{
			return DataManager.Instance.GetTable(DataType.DATA_SCENE_QUALIFYING);
		}
	}

	public static DataTable Scene_BattleSceneTable
	{
		get
		{
			return DataManager.Instance.GetTable(DataType.DATA_SCENE_BATTLESCENE);
		}
	}

	public static DataTable Scene_WantedSceneTable
	{
		get
		{
			return DataManager.Instance.GetTable(DataType.DATA_SCENE_WANTED);
		}
	}

    public static DataTable Scene_StageListTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_SCENE_STAGELIST);
        }
    }

    public static DataTable ConditionTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_CONDITION);
        }
    }

    public static DataTable EffectTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_EFFECT);
        }
    }

    public static DataTable SkillLearnTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_SKILL_LEARN);
        }
    }
    public static DataTable SkillLevelTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_SKILL_LEVEL);
        }
    }

    public static DataTable QuestTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_QUEST);
        }
    }

    public static DataTable SoundTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_SOUND);
        }
    }

	public static DataTable DropBoxTable
	{
		get
		{
			return Instance.GetTable(DataType.DATA_DROPBOX);
		}
	}

	public static DataTable DropGroupTable
	{
		get
		{
			return Instance.GetTable(DataType.DATA_DROPGROUP);
		}
	}

    public static DataTable ChallengeTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_CHALLENGE);
        }
    }
    public static DataTable MenuTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_MENU);
        }
    }

	public static DataTable PickTable
	{
		get
		{
			return Instance.GetTable(DataType.DATA_PICK);
		}
	}
    public static DataTable BuildTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_BUILD);
        }
    }

	public static DataTable StoryTable
	{
		get
		{
			return Instance.GetTable(DataType.DATA_STORY);
		}
	}

	public static DataTable LevelCommonPropertiesTable
	{
		get
		{
			return Instance.GetTable(DataType.DATA_LEVEL_COMMON_PROPERTIES);
		}
	}

    public static DataTable NpcTalkTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_NPC_TALK);
        }
    }
    public static DataTable GuideTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_GUIDE);
        }
    }

    public static DataTable GuideStepTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_GUIDE_STEP);
        }
    }

    public static DataTable StringTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_STRING);
        }
    }
    public static DataTable ErrorCodeStringTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_ERROR_STRING);
        }
    }

    public static DataTable MallTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_MALL);
        }
    }
    public static DataTable StoneTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_STONE);
        }
    }

    public static DataTable DefenceStrenTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_DEFENCE_STREN);
        }
    }

    public static DataTable DefenceStrenProTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_DEFENCE_STREN_PRO);
        }
    }

    public static DataTable DefenceStarsTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_DEFENCE_STARS);
        }
    }

    public static DataTable DefenceStarsProTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_DEFENCE_STARS_PRO);
        }
    }

    public static DataTable DefenceCombTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_DEFENCE_COMB);
        }
    }
    public static DataTable ShowWeaponTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_SHOW_WEAPON);
        }
    }

    public static DataTable SceneQiangLinDanYuTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_SCENE_QIANGLINDANYU);
        }
    }

    public static DataTable ShopTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_SHOP);
        }
    }

    public static DataTable ActivityTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_ACTIVITY);
        }
    }

    public static DataTable ActivityTypeTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_ACTIVITY_TYPE);
        }
    }


    public static DataTable SceneCityTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_SCENE_CITY);
        }
    }


	public static DataTable ArenaTable
	{
		get
		{
			return Instance.GetTable(DataType.DATA_ARENA);
		}
	}

	public static DataTable ArenaRandomTable
	{
		get
		{
			return Instance.GetTable(DataType.DATA_ARENA_RANDOM);
		}
	}

	public static DataTable WingCommonTable
	{
		get
		{
			return Instance.GetTable(DataType.DATA_WING_COMMON);
		}
	}

    public static DataTable ChaRankAwardTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_CHALLENGE_RANK_AWARD);
        }
    }

    public static DataTable ChaRankAwardItemTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_CHALLENGE_RANK_AWARD_ITEM);
        }
    }

	public static DataTable WingLevelTable
	{
		get
		{
			return Instance.GetTable(DataType.DATA_WING_LEVEL);
		}
	}

    public static DataTable PartModelTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_PRAT_MODELS);
        }
    }

	public static DataTable QualifyingAwardTable
	{
		get
		{
			return Instance.GetTable(DataType.DATA_QUALIFYING_AWARD);
		}
	}

	public static DataTable QualifyingRandomTable
	{
		get
		{
			return Instance.GetTable(DataType.DATA_QUALIFYING_RANDOM);
		}
	}

    public static DataTable AnnouncementTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_ANNOUNCEMENT);
        }
    }

	public static DataTable VipTable
	{
		get
		{
			return Instance.GetTable(DataType.DATA_VIP);
		}
	}
    public static DataTable QuackNumberTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_QUACK_NUMBER);
        }
    }

    public static DataTable EggTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_EGG);
        }
    }

    public static DataTable ConfigTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_CONFIG);
        }
    }

    public static DataTable EggConfigTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_EGG_CONFIG);
        }
    }

    public static DataTable PackageExtendTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_PACKAGE_EXTEND);
        }
    }
    public static DataTable CreatePlayerTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_CREATE_PLAYER);
        }
    }
    public static DataTable SuperWeaponTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_SUPER_WEAPON);
        }
    }

    public static DataTable WeaponSkillTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_WEAPON_SKILL);
        }
    }

    public static DataTable TitleGroupTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_TITLE_GROUP);
        }
    }

    public static DataTable TitleItemTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_TITLE_ITEM);
        }
    }

    public static DataTable ZoneRewardTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_SCENE_STAGE_REWARD);
        }
    }

    public static DataTable FashionTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_FASHION);
        }
    }

    public static DataTable FashionPropTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_FASHION_PROP);
        }
    }

    public static DataTable FashionStarTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_FASHION_STAR);
        }
    }


    public static DataTable CropsTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_CROPS);
        }
    }
    public static DataTable SevenTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_AWARD);
        }
    }

    public static DataTable CropsLevelTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_CROPS_LEVEL);
        }
    }

    public static DataTable CropsProRatioTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_CROPS_RATIO);
        }
    }

    public static DataTable FundTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_FUND);
        }
    }

    public static DataTable FirstChargeTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_FIRSTCHARGE_REWARD);
        }
    }
    public static DataTable ChargeRewardsTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_CHARGE_REWARDS);
        }
    }
    public static DataTable AssisterTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_ASSISTER);
        }
    }

    public static DataTable AssisterItemTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_ASSISTERITEM);
        }
    }

   
	public static DataTable ZhaoCaiMaoAwardTable
	{
		get
		{
			return DataManager.Instance.GetTable(DataType.DATA_ZHAOCAIMAO_AWARD);

		}
	}

    public static DataTable CropsSkillLearnTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_CROPSSKILLLEARN);
        }
    }
    public static DataTable CropsSkillLevelTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_CROPSSKILLLEVEL);
        }
    }

    public static DataTable PlanTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_PLAN);
        }
    }

    public static DataTable BoxItemTable
    {
        get
        {
            return Instance.GetTable(DataType.DATA_BOX_ITEM);
        }
    }

    public static DataTable TotalChargeTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_TOTAL_CHARGE);
        }
    }
    public static DataTable UIEffectTable
    {
        get
        {
            return DataManager.Instance.GetTable(DataType.DATA_UI_EFFECT);
        }
    }
}
