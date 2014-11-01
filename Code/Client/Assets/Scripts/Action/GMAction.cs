using System;
using System.Collections.Generic;
using Message;
using UnityEngine;

public class GMActionParam
{
    public string Cmd
    {
        set;
        get;
    }
}

public class GMAction : LogicAction<request_msg_gm, respond_msg_gm>
{
    private delegate bool CallFunction(ObjectBase obj, respond_msg_gm respond);
    private Dictionary<string, CallFunction> mHandlers = new Dictionary<string, CallFunction>();

    public GMAction()
        : base((int)MESSAGE_ID.ID_MSG_GM)
    {
        RegisterAllFunction();
    }

    private void RegisterFunction(string name, CallFunction func)
    {
        if (func == null)
            return;
        if (mHandlers.ContainsKey(name))
            return;
        mHandlers.Add(name, func);
    }

    protected override void OnRequest(request_msg_gm request, object userdata)
    {
        GMActionParam gmParam = userdata as GMActionParam;
        if (gmParam == null)
            return;

        request.cmd = gmParam.Cmd;
    }

    protected override void OnRespond(respond_msg_gm respond, object userdata)
    {
        if(!respond.rst)
        {
            GameDebug.Log("GM 指令：[" + respond.cmd + "] " + "后端执行失败. errmsg = " + respond.errmsg);
            return;
        }

        if(!DoHandler(PlayerController.Instance.GetControlObj(), respond))
        {
            GameDebug.Log("GM 指令：[" + respond.cmd + "] " + "前端执行失败");
        }
        else
        {
            GameDebug.Log("GM 指令：[" + respond.cmd + "] " + "执行成功");
        }
    }

    public bool DoHandler(ObjectBase obj, respond_msg_gm respond)
    {
        if (!mHandlers.ContainsKey(respond.name))
        {
            GameDebug.Log("gm命令 (" + respond.name + ")未找到");
            return false;
        }

        CallFunction func = mHandlers[respond.name] as CallFunction;
        return func(obj, respond);
    }

    private void RegisterAllFunction()
    {
        RegisterFunction("createnpc", CreateNPC);
        RegisterFunction("enterscene", EnterScene);
        RegisterFunction("kill", Kill);
        RegisterFunction("addskilleffect", AddSkillEffect);
        RegisterFunction("money", MoneyChange);
        RegisterFunction("passstage", PassStage);
        RegisterFunction("showprop", ShowProp); 
        RegisterFunction("setfloor", SetFloor);
        RegisterFunction("openui", OpenUI);
        RegisterFunction("fittings", OpenFittings);
        RegisterFunction("createitem", CreateItem);
        RegisterFunction("createpick", CreatePick);
        RegisterFunction("createbuild", CreateBuild);
        RegisterFunction("pause", Pause);
        RegisterFunction("resume", Resume);
        RegisterFunction("resettower", ResetTower);
        RegisterFunction("resetlayer", ResetCurLayer);
        RegisterFunction("drop", ShowDrop);
        RegisterFunction("logerr", LogError);
        RegisterFunction("wudi", Wudi);
        RegisterFunction("equipitem", EquipItem);
        RegisterFunction("resetact", ResetActivity);
        RegisterFunction("reset", ResetDaily);
		RegisterFunction("resetquest",ResetQuest);
        RegisterFunction("addexp", AddExp);
		RegisterFunction("acceptquest", AcceptQuest);
		RegisterFunction("finishquest", FinishQuest);
        RegisterFunction("level", Level);
        RegisterFunction("finishallquest", FinishAllQuest);
        RegisterFunction("wingactive", WingActive);
        RegisterFunction("resetwing", ResetWing);
		RegisterFunction("qualifyingaward", QualifyingAward);
		RegisterFunction("unlockall", UnlockAll);
        RegisterFunction("resetfashion", ResetFashion);

    }

    private bool ResetWing(ObjectBase obj, respond_msg_gm respond)
    {
        return true;
    }
    private bool WingActive(ObjectBase obj, respond_msg_gm respond)
    {
        if (respond == null)
            return false;

        if (obj == null)
            return false;

        return true;
    }
    private bool CreateNPC(ObjectBase obj, respond_msg_gm respond)
    {
        if (respond == null)
            return false;

        if (obj == null)
            return false;

        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        NpcInitParam initParam = new NpcInitParam();
        initParam.npc_res_id = System.Convert.ToInt32(respond.param1);
        initParam.init_pos = obj.GetPosition();
        initParam.init_dir = obj.GetDirection();

        return scn.CreateSprite(initParam) != null;
    }

    private bool EnterScene(ObjectBase obj, respond_msg_gm respond)
    {
        if (respond == null)
            return false;

        if (obj == null)
            return false;

        SceneManager.Instance.RequestEnterScene(System.Convert.ToInt32(respond.param1));
        return true;
    }

    private bool Kill(ObjectBase obj, respond_msg_gm respond)
    {
        if (respond == null)
            return false;

        if (obj == null)
            return false;

        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        obj = scn.FindObject((uint)System.Convert.ToInt32(respond.param1));

        if (obj != null && obj is BattleUnit)
        {
            BattleUnit battleunit = obj as BattleUnit;
            battleunit.Die(new AttackerAttr(battleunit));
            return true;
        }

        return false;
    }

    private bool AddSkillEffect(ObjectBase obj, respond_msg_gm respond)
    {
        if (obj == null || respond == null)
            return false;

        if (obj is BattleUnit)
        {
            BattleUnit battleunit = obj as BattleUnit;
            battleunit.AddSkillEffect(new AttackerAttr(battleunit),
                (SkillEffectType)System.Convert.ToUInt32(respond.param1), System.Convert.ToUInt32(respond.param2));
            return true;
        }

        return false;
    }

    private bool MoneyChange(ObjectBase obj, respond_msg_gm respond)
    {
        if (obj == null || respond == null)
            return false;

        if (obj is Player)
        {
            PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

            //module.ChangeProceeds((ProceedsType)(System.Convert.ToInt32(respond.param1)), System.Convert.ToInt32(respond.param2));
            //module.SetProceeds((ProceedsType)(System.Convert.ToInt32(respond.param1)), System.Convert.ToUInt32(respond.param2));
            return true;
        }

        return false;
    }

    private bool PassStage(ObjectBase obj, respond_msg_gm respond)
    {
        if (obj == null || respond == null)
            return false;

		List<string> list = new List<string>();
		list.Add("1");

		SceneInterface.SetResult(list);
		SceneInterface.PassStage(list);

        return true;
    }

    private bool ShowProp(ObjectBase obj, respond_msg_gm respond)
    {
        if (obj == null || respond == null)
            return false;

        BattleUnit owner = obj as Role;
        if (owner == null)
            return false;

        int propID = System.Convert.ToInt32(respond.param1);

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

    private bool SetFloor(ObjectBase obj, respond_msg_gm respond)
    {
        if (respond == null)
            return false;

        if (obj == null)
            return false;

        //ModuleManager.Instance.FindModule<ChallengeModule>().SetCurFloor(Convert.ToInt32(respond.param1));
        return true;
    }

    private bool OpenUI(ObjectBase obj, respond_msg_gm respond)
    {
        if (obj == null || respond == null)
            return false;


        WindowManager.Instance.OpenUI(respond.param1);
        return true;
    }

    private bool OpenFittings(ObjectBase obj, respond_msg_gm respond)
    {
        if (obj == null || respond == null)
            return false;


        if (obj is Player)
        {
            PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

            module.OpenFittings(System.Convert.ToUInt32(respond.param1), System.Convert.ToInt32(respond.param2));

            return true;
        }
        return false;
    }

    private bool CreateItem(ObjectBase obj, respond_msg_gm respond)
    {
        if (obj == null || respond == null)
            return false;

        return true;
    }
 private bool EquipItem(ObjectBase obj, respond_msg_gm respond)
    {
        if (obj == null || respond == null)
            return false;

        return true;
    }

    private bool CreatePick(ObjectBase obj, respond_msg_gm respond)
    {
        if (respond == null)
            return false;

        if (obj == null)
            return false;

        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        PickInitParam initParam = new PickInitParam();
        initParam.pick_res_id = System.Convert.ToInt32(respond.param1);
        initParam.init_pos = obj.GetPosition();
        initParam.init_dir = obj.GetDirection();

        return scn.CreateSprite(initParam) != null;
    }

    private bool CreateBuild(ObjectBase obj, respond_msg_gm respond)
    {
        if (respond == null)
            return false;

        if (obj == null)
            return false;

        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return false;

        BuildInitParam initParam = new BuildInitParam();
        initParam.build_res_id = System.Convert.ToInt32(respond.param1);
        initParam.init_pos = obj.GetPosition();
        initParam.init_dir = obj.GetDirection();
        initParam.build_barrier = System.Convert.ToInt32(respond.param2) != 0;

        return scn.CreateSprite(initParam) != null;
    }

    private bool Pause(ObjectBase obj, respond_msg_gm respond)
    {
        SceneManager.Instance.LogicPause();
        return true;
    }

    private bool Resume(ObjectBase obj, respond_msg_gm respond)
    {
        SceneManager.Instance.LogicResume();
        return true;
    }

    private bool ResetTower(ObjectBase obj, respond_msg_gm respond)
    {
        //var module = ModuleManager.Instance.FindModule<ChallengeModule>();
        //var pl = PlayerController.Instance.GetControlObj() as Player;
        ////pl.ModifyPropertyValue();

        //module.ResetTower();

        //pl.ModifyPropertyValue((int)PropertyTypeEnum.PropertyTypeHP, pl.GetMaxHP() - pl.GetHP());
        //pl.ModifyPropertyValue((int)(int)PropertyTypeEnum.PropertyTypeMana, pl.GetMaxMana() - pl.GetMana());

        return true;

    }

    private bool ResetCurLayer(ObjectBase obj, respond_msg_gm respond)
    {
        //var module = ModuleManager.Instance.FindModule<ChallengeModule>();

        //module.ResetCurLayer(Convert.ToInt32(respond.param1));
        return true;
    }

    private bool ShowDrop(ObjectBase obj, respond_msg_gm respond)
    {
        //ModuleManager.Instance.FindModule<ChallengeModule>().GetSweepDropInfo();
        return true;
    }

    private bool LogError(ObjectBase obj, respond_msg_gm respond)
    {
        bool log = System.Convert.ToInt32(respond.param1) != 0;
        GameConfig.LogSkillError = log;
        return true;
    }

    private bool Wudi(ObjectBase obj , respond_msg_gm respond)
    {
        BattleUnit ply = PlayerController.Instance.GetControlObj() as BattleUnit;
        if( ply == null )
        {
            return false;
        }
        ply.SetWudi( !ply.IsWudi() );
        return true;
    }

    private bool ResetActivity(ObjectBase obj, respond_msg_gm respond)
    {
        return true;
    }

    private bool ResetDaily(ObjectBase obj, respond_msg_gm respond)
    {
        return true;
    }


	private bool ResetQuest(ObjectBase obj, respond_msg_gm respond)
	{
		return true;
	}


    private bool AddExp(ObjectBase obj, respond_msg_gm respond)
    {
        return true;
    }

	private bool AcceptQuest(ObjectBase obj, respond_msg_gm respond)
	{
        QuestEvent evt = new QuestEvent(QuestEvent.QUEST_ACCEPT);
        evt.mQuestId = Convert.ToInt32(respond.param1);
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        pdm.AcceptQuest(evt.mQuestId);

        if (QuestHelper.IsInFightScene() || QuestHelper.IsLoading())
        {
            GameDebug.Log("gm接取任务缓存" + evt.mQuestId);
            QuestModule qm = ModuleManager.Instance.FindModule<QuestModule>();
            qm.mEventCache.Enqueue(evt);
        }
        else
        {
            GameDebug.Log("gm接取任务" + evt.mQuestId);
            EventSystem.Instance.PushEvent(evt);
        }
		return true;
	}

	private bool FinishQuest(ObjectBase obj, respond_msg_gm respond)
	{
	    if (!respond.rst) return false;

        FinishQuestEvent evt = new FinishQuestEvent(FinishQuestEvent.QUEST_FINISHED);
	    evt.mQuestId = Convert.ToInt32(respond.param1);
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        pdm.FinishQuest(evt.mQuestId);
        GameDebug.Log("gm 完成任务" + evt.mQuestId);  
        if (!QuestHelper.IsInFightScene() && !QuestHelper.IsLoading())
        {

            EventSystem.Instance.PushEvent(evt);
        }
        else
        {
            GameDebug.Log("gm任务缓存" + evt.mQuestId);
            QuestModule qm = ModuleManager.Instance.FindModule<QuestModule>();
            qm.mEventCache.Enqueue(evt);
        }
		return true;
	}

    private bool FinishAllQuest(ObjectBase obj, respond_msg_gm respond)
    {
        FinishQuestEvent evt = new FinishQuestEvent(FinishQuestEvent.QUEST_FINISHED_ALL);
        GameDebug.Log("gm finishall");
        EventSystem.Instance.PushEvent(evt);
        return true;
    }
    private bool Level(ObjectBase obj, respond_msg_gm respond)
	{	
		return true;
	}

	private bool QualifyingAward(ObjectBase obj, respond_msg_gm respond)
	{
		GameDebug.Log("gm 排位赛排行榜结算成功");
		return true;
	}

	private bool UnlockAll(ObjectBase obj, respond_msg_gm respond)
	{
		GameDebug.Log("gm 解锁所有关卡成功");

		return true;
	}

    private bool ResetFashion(ObjectBase obj, respond_msg_gm respond)
    {
        GameDebug.Log("重置时装数据");
        return true;
    }
}

