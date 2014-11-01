using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GMHandler
{
    private delegate bool CallFunction(ObjectBase obj , ArrayList param);
    private Dictionary<string , CallFunction> mHandlers = new Dictionary<string,CallFunction>();

    private static GMHandler instance;
    public static GMHandler Instance
	{
		get
		{
			return instance;
		}
	}
    public GMHandler()
    {
        instance = this;

        //RegisterFunction("createnpc", CreateNPC);
        //RegisterFunction("enterscene", EnterScene);
        //RegisterFunction("kill", Kill);
        //RegisterFunction("addskilleffect", AddSkillEffect);
        RegisterFunction("logerr", LogError);
        //RegisterFunction("money", MoneyChange);
        RegisterFunction("passstage", PassStage);
        //RegisterFunction("showprop", ShowProp);
        //RegisterFunction("SetFloor",SetFloor);
        //RegisterFunction("openui", OpenUI);
        //RegisterFunction("fittings",OpenFittings);
        RegisterFunction("item", CreateItem);
        //RegisterFunction("createpick", CreatePick);
        //RegisterFunction("createbuild", CreateBuild);
        //RegisterFunction("pause", Pause);
        //RegisterFunction("resume", Resume);
        //RegisterFunction("doaction", DoAction);
        //RegisterFunction("resettower",ResetTower);
        //RegisterFunction("resetlayer", ResetCurLayer);
        //RegisterFunction("drop", ShowDrop);
		RegisterFunction("createeffect", CreateEffect);
        RegisterFunction("playerlevel",SetPlayerLevel);
        RegisterFunction("aq",AcceptQuest);
        RegisterFunction("fq",FinishQuest);
		RegisterFunction("wa",ActiveWing);
		RegisterFunction("wf",ForgeWing);
		RegisterFunction("we",EquipWing);
		RegisterFunction("createcreation", CreateCreation);
		RegisterFunction("play", PlayAni);
        RegisterFunction("buy", buy);
    }

    private bool AcceptQuest(ObjectBase obj, ArrayList param)
    {
        if (param == null || param.Count != 1)
            return false;
        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_QUEST_ACCPET, param[0]);
       // ModuleManager.Instance.FindModule<QuestModule>().AcceptQuest(Convert.ToInt32(param[0]));
        return true;
    }

    private bool FinishQuest(ObjectBase obj, ArrayList param)
    {
        if (param == null || param.Count != 1)
            return false;
        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_QUEST_FINISH, param[0]);
       // ModuleManager.Instance.FindModule<QuestModule>().CompletQuest(Convert.ToInt32(param[0]));
        return true;
    }

	private bool ActiveWing(ObjectBase obj,ArrayList param)
	{
		if (param == null || param.Count != 1) 
		{
			return false;
		}

		Net.Instance.DoAction((int) Message.MESSAGE_ID.ID_MSG_WING_ACTIVE,param[0]);
		return true;
	}

	private bool ForgeWing(ObjectBase obj,ArrayList param)
	{
		if(param == null || param.Count!=1)
		{
			return false;
		}

		Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_WING_FORGE,param[0]);
		return true;
		
	}

	private bool EquipWing(ObjectBase obj,ArrayList param)
	{
		if(param == null || param.Count!=1)
		{
			return false;
		}

		Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_WING_EQUIP,param[0]);
		return true;
	}

	private bool CreateCreation(ObjectBase obj, ArrayList param)
	{
		if (param.Count == 0) return false;

		uint creationResID = Convert.ToUInt32(param[0]);

		return ErrorHandler.Parse(SkillDetails.CreateCreationAround(
			new AttackerAttr(obj as BattleUnit), creationResID, 
			obj.GetPosition(), obj.GetDirection())
			);
	}

    private bool SetPlayerLevel(ObjectBase obj, ArrayList param)
    {
        if (param == null || param.Count != 1)
            return false;
        return true;
    }
    private bool ShowDrop(ObjectBase obj, ArrayList param)
    {
        //ModuleManager.Instance.FindModule<ChallengeModule>().GetSweepDropInfo();
        return true;
    }
    private bool SetFloor(ObjectBase obj, ArrayList param)
    {
        if (param == null || param.Count != 1)
            return false;

        if (obj == null)
            return false;

        //ModuleManager.Instance.FindModule<ChallengeModule>().SetCurFloor(Convert.ToInt32(param[0]));
        return true;
    }
    private void RegisterFunction(string name , CallFunction func)
    {
        if( func == null )
            return ;
        if (mHandlers.ContainsKey(name))
            return;
        mHandlers.Add(name, func);
    }

    public bool DoHandler(ObjectBase obj, string name , ArrayList param)
    {
        if (!mHandlers.ContainsKey(name))
        {
            GameDebug.Log("gm命令 (" + name + ")未找到");
            return false;
        }
       CallFunction func = mHandlers[name] as CallFunction;
       func(obj  , param);
       return true;
    }

    private bool CreateNPC(ObjectBase obj, ArrayList param)
    {
        if (param == null || param.Count != 1)
            return false;

        if (obj == null)
            return false;

        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        NpcInitParam initParam = new NpcInitParam();
        initParam.npc_res_id = System.Convert.ToInt32(param[0]);
        initParam.init_pos = obj.GetPosition();
        initParam.init_dir = obj.GetDirection();

        return scn.CreateSprite(initParam) != null;
    }

    private bool CreateBuild(ObjectBase obj, ArrayList param)
    {
        if (param == null || param.Count < 1 || param.Count > 2)
            return false;

        if (obj == null)
            return false;

        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        BuildInitParam initParam = new BuildInitParam();
        initParam.build_res_id = System.Convert.ToInt32(param[0]);
        initParam.init_pos = obj.GetPosition();
        initParam.init_dir = obj.GetDirection();

		if(param.Count > 1)
		{
			initParam.build_barrier = System.Convert.ToBoolean(param[1]);
		}

        return scn.CreateSprite(initParam) != null;
    }

    private bool EnterScene(ObjectBase obj, ArrayList param)
    {
		if(obj == null || param == null)
			return false;

		if(param.Count != 2)
		{
			GameDebug.LogError("参数改为2个，场景类型(0:城镇,1:关卡,2:战场)和场景Id。");
			return false;
		}

        SceneManager.Instance.RequestEnterScene(System.Convert.ToInt32(param[1]));
		return true;
    }

	private bool Kill(ObjectBase obj, ArrayList param)
	{
		if (obj == null || param == null)
			return false;

		BaseScene scn = SceneManager.Instance.GetCurScene();
		if (scn == null)
			return false;

		if (param.Count != 0)
			obj = scn.FindObject((uint)System.Convert.ToInt32(param[0]));

		if (obj != null && obj is BattleUnit)
		{
			BattleUnit battleunit = obj as BattleUnit;
			battleunit.Die(new AttackerAttr(battleunit));
			return true;
		}

		return false;
	}

	private bool AddSkillEffect(ObjectBase obj, ArrayList param)
	{
		if (obj == null || param == null)
			return false;

		if (param.Count != 2)
		{
			GameDebug.Log("usage: .addskilleffect effect_type[0:buff, 1:impact, 2:displacement] effect_id");
			return false;
		}

		if (obj is BattleUnit)
		{
			BattleUnit battleunit = obj as BattleUnit;
			battleunit.AddSkillEffect(new AttackerAttr(battleunit),
				(SkillEffectType)System.Convert.ToUInt32(param[0]), System.Convert.ToUInt32(param[1]));
			return true;
		}

		return false;
	}

    private bool OpenUI(ObjectBase obj, ArrayList param)
    {
        if (obj == null || param == null)
            return false;

        if (param.Count != 1)
        {
            return false;
        }

        WindowManager.Instance.OpenUI(param[0] as string);

        return true;
    }

    private bool MoneyChange(ObjectBase obj, ArrayList param)
    {
        if (obj == null || param == null)
            return false;

        if (param.Count != 2)
        {
            GameDebug.Log("usage: .money [0:type 1:value] ");
            return false;
        }

        if (obj is Player)
        {
            //PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
            
            //module.ChangeProceeds((ProceedsType)(System.Convert.ToInt32(param[0])), System.Convert.ToInt32(param[1]));

            return true;
        }

        return false;
    }

    private bool CreateItem(ObjectBase obj, ArrayList param)
    {
        if (obj == null || param == null)
            return false;

        if (param.Count != 1 && param.Count != 2)
        {
            GameDebug.LogError("usage: .item [0:资源id 1:包裹（暂时填0或者可以不填）] ");
            return false;
        }

        if (obj is Player)
        {
            PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

            //PackageType type = param.Count == 1 ? PackageType.Invalid : (PackageType)System.Convert.ToUInt32(param[1]);

            //module.CreateItemUnreal((System.Convert.ToInt32(param[0])), PackageType.Pack_Bag);

            return true;
        }
        return false;
    }

    private bool OpenFittings(ObjectBase obj, ArrayList param)
    {
        if (obj == null || param == null)
            return false;

        if (param.Count != 2)
        {
            GameDebug.LogError("usage: .fittings [0:pos, 1:id] ");
            return false;
        }

        if (obj is Player)
        {
            PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

            module.OpenFittings(System.Convert.ToUInt32(param[0]), System.Convert.ToInt32(param[1]));

            return true;
        }
        return false;
    }

	private bool LogError(ObjectBase obj, ArrayList param)
	{
		bool log = param.Count <= 0 || System.Convert.ToBoolean(param[0]);
		GameConfig.LogSkillError = log;
		return true;
	}

	private bool PassStage(ObjectBase obj, ArrayList param)
	{
		if (obj == null || param == null|| param.Count==0)
			return false;

		List<string> list = new List<string>();
		list.Add("1");

		SceneInterface.SetResult(list);
		SceneInterface.PassStage(list);

		return true;
	}

	private bool ShowProp(ObjectBase obj, ArrayList param)
	{
		if(obj == null || param == null)
			return false;

		BattleUnit owner = obj as Role;
		if (owner == null)
			return false;

		int propID = -1;
		if (param.Count >= 1)
			propID = System.Convert.ToInt32(param[0]);

		if (propID != -1)
		{
			GameDebug.Log("prop id = " + propID + ", value = " + owner.GetPropertyValue(propID));
		}
		else
		{
 			// 列出所有属性.
		}

		return true;
	}

	private bool CreatePick(ObjectBase obj, ArrayList param)
	{
		if (param == null || param.Count != 1)
			return false;

		if (obj == null)
			return false;

		BaseScene scn = SceneManager.Instance.GetCurScene();
		if (scn == null)
			return false;

		PickInitParam initParam = new PickInitParam();
		initParam.pick_res_id = System.Convert.ToInt32(param[0]);
		initParam.init_pos = obj.GetPosition();
		initParam.init_dir = obj.GetDirection();

		return scn.CreateSprite(initParam) != null;
	}

    private bool DoAction(ObjectBase obj, ArrayList param)
    {
        if (param == null || param.Count != 1)
            return false;

        if (obj == null)
            return false;

        Net.Instance.DoAction(System.Convert.ToInt32(param[0]), 456789, true);
        return true;
    }

    private bool ResetTower(ObjectBase obj, ArrayList param)
    {
       //var module= ModuleManager.Instance.FindModule<ChallengeModule>();
       // var pl=PlayerController.Instance.GetControlObj() as Player;
       // //pl.ModifyPropertyValue();
        
       // module.ResetTower();

       // pl.ModifyPropertyValue((int)PropertyTypeEnum.PropertyTypeHP, pl.GetMaxHP() - pl.GetHP());
       // pl.ModifyPropertyValue((int)(int)PropertyTypeEnum.PropertyTypeMana, pl.GetMaxMana() - pl.GetMana());
                
        return true;

    }

    private bool ResetCurLayer(ObjectBase obj, ArrayList param)
    {
        //if (param.Count < 1)
        //{
        //    return false;
        //}
        //var module = ModuleManager.Instance.FindModule<ChallengeModule>();

        //module.ResetCurLayer(Convert.ToInt32(param[0]));
        return true;
    }


	private bool Pause(ObjectBase obj, ArrayList param)
	{
		SceneManager.Instance.LogicPause();
		return true;
	}

	private bool Resume(ObjectBase obj, ArrayList param)
	{
		SceneManager.Instance.LogicResume();
		return true;
	}

	private bool CreateEffect(ObjectBase obj, ArrayList param)
	{
		if (param == null || param.Count != 1)
			return false;

		if (obj == null)
			return false;

		BaseScene scn = SceneManager.Instance.GetCurScene();
		if (scn == null)
			return false;

		return scn.CreateEffect(System.Convert.ToInt32(param[0]), Vector3.one, obj.GetPosition(), obj.GetDirection()) != uint.MaxValue;
	}

	private bool PlayAni(ObjectBase obj, ArrayList param)
	{
		if (param == null || param.Count < 2)
			return false;

		Player player = obj as Player;
		if (player == null)
			return false;

		AnimActionPlayAnim action = AnimActionFactory.Create(AnimActionFactory.E_Type.PlayAnim) as AnimActionPlayAnim;

		if (System.Convert.ToInt32(param[0]) == 0)
		{
			action.AnimName = "Base Layer." + (param[1] as string);
		}
		else
		{
			action.AnimName = "Base Layer." + player.CombineAnimname("%" + param[1] as string);
		}

		player.GetStateController().DoAction(action);

		WindowManager.Instance.CloseUI("chat");

		return true;
	}

    private bool buy(ObjectBase obj, ArrayList param)
    {
        if (param == null || param.Count < 1)
            return false;

        Player player = obj as Player;
		if (player == null)
			return false;

        PlatformSDK.BuyGameCoins(System.Convert.ToUInt32(param[0]), player.GetLevel());
        return true;
    }
}