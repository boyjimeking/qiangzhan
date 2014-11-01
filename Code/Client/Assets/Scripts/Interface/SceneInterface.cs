
using System;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// [StructLayout(LayoutKind.Explicit, Pack = 1)]
// public struct sFuncParam
// {
//     [FieldOffset(0)]
//     public int i;
// 
//     [FieldOffset(0)]
//     public float f;
// 
//     [FieldOffset(0)]
//     [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
//     public string s;
// }

public delegate bool SceneFunctionInterface(List<string> paramList);

public class SceneInterface
{
    // 启动触发器
	public static bool StartTrigger(List<string> paramList)
    {
        if (paramList == null || paramList.Count != 1)
        {
            return false;
        }

        return SceneManager.Instance.StartTrigger(paramList[0]);
    }

    // 停止触发器
	public static bool StopTrigger(List<string> paramList)
    {
        if (paramList == null || paramList.Count != 1)
        {
            return false;
        }

        return SceneManager.Instance.StopTrigger(paramList[0]);
    }


    // 关卡结果
	public static bool SetResult(List<string> paramList)
    {
		if (paramList == null || paramList.Count != 1)
		{
			return false;
		}

		GameScene gamescene = SceneManager.Instance.GetCurScene() as GameScene;
		if (gamescene == null)
			return false;

		gamescene.SetResult(int.Parse(paramList[0]));

        return true;
    }


    // 被攻击的时候掉落
    public static bool onHpDamageAward(List<string> paramList)
    {
        if (paramList == null || paramList.Count != 2)
        {
            return false;
        }

		GameScene gamescene = SceneManager.Instance.GetCurScene() as GameScene;
		if (gamescene == null)
			return false;

		List<BattleUnit> objs = gamescene.SearchObjsByAlias<BattleUnit>(paramList[0]);
          if (null == objs || 0 == objs.Count)
        {
            GameDebug.Log("当前没有找到此别名的对象");
            return false;
        }

    
        foreach (var obj in objs)
        {
            obj.HpDamageAward(obj.InstanceID,int.Parse(paramList[1]));
           
        }


        return true;
    }

    // 通关结算
	public static bool PassStage(List<string> paramList)
    {
		BaseScene scene = SceneManager.Instance.GetCurScene();
		if (scene == null)
		{
			return false;
		}

		scene.pass();

		return true;
    }

    // 聊天泡泡
	public static bool TalkPop(List<string> paramList)
    {
        return true;
    }

    // 检查场景变量
	public static bool CheckIntParam(List<string> paramList)
    {
        if(paramList == null || paramList.Count != 3)
        {
            return false;
        }

		BaseScene scene = SceneManager.Instance.GetCurScene();
		if (scene == null)
		{
			return false;
		}

		int param = -1;
		bool ret = scene.GetIntParam(int.Parse(paramList[1]), ref param);
		if (!ret)
		{
			return false;
		}

		switch (paramList[0])
		{
			case "eq":
				{
					return int.Parse(paramList[2]) == param;
				}
				break;
			case "mt":
				{
					return param > int.Parse(paramList[2]);
				}
				break;
			case "lt":
				{
					return param < int.Parse(paramList[2]);
				}
				break;
			case "mteq":
				{
					return param >= int.Parse(paramList[2]);
				}
				break;
			case "lteq":
				{
					return param <= int.Parse(paramList[2]);
				}
				break;
			case "neq":
				{
					return int.Parse(paramList[2]) != param;
				}
				break;
			default:
				return false;
		}

		return true;
    }

    // 检查场景变量
	public static bool CheckFloatParam(List<string> paramList)
    {
        if (paramList == null || paramList.Count != 3)
        {
            return false;
        }

		BaseScene scene = SceneManager.Instance.GetCurScene();
		if (scene == null)
		{
			return false;
		}

		float param = -1.0f;
		bool ret = scene.GetFloatParam(int.Parse(paramList[1]), ref param);
		if (!ret)
		{
			return false;
		}

		switch (paramList[0])
		{
			case "eq":
				{
					return float.Equals(float.Parse(paramList[2]), param);
				}
				break;
			case "mt":
				{
					return param > float.Parse(paramList[2]);
				}
				break;
			case "lt":
				{
					return param < float.Parse(paramList[2]);
				}
				break;
			case "mteq":
				{
					return param >= float.Parse(paramList[2]);
				}
				break;
			case "lteq":
				{
					return param <= float.Parse(paramList[2]);
				}
				break;
			case "neq":
				{
					return !float.Equals(float.Parse(paramList[2]), param);
				}
				break;
			default:
				return false;
		}

		return true;
    }

    // 设置场景变量
	public static bool SetIntParam(List<string> paramList)
    {
        if (paramList == null || paramList.Count != 2)
        {
            return false;
        }

		BaseScene scene = SceneManager.Instance.GetCurScene();
		if (scene == null)
		{
			return false;
		}

        return scene.SetIntParam(int.Parse(paramList[0]), int.Parse(paramList[1]));
    }

    // 设置场景变量
	public static bool SetFloatParam(List<string> paramList)
    {
        if (paramList == null || paramList.Count != 2)
        {
            return false;
        }

		BaseScene scene = SceneManager.Instance.GetCurScene();
		if (scene == null)
		{
			return false;
		}

        return scene.SetFloatParam(int.Parse(paramList[0]), float.Parse(paramList[1]));
    }

    // 增减场景变量
	public static bool IncIntParam(List<string> paramList)
    {
        if (paramList == null || paramList.Count != 2)
        {
            return false;
        }

		BaseScene scene = SceneManager.Instance.GetCurScene();
		if (scene == null)
		{
			return false;
		}

		int idx = int.Parse(paramList[0]);
		int param = -1;
		bool ret = scene.GetIntParam(idx, ref param);
		if (!ret)
		{
			return false;
		}

		return scene.SetIntParam(idx, param + int.Parse(paramList[1]));
    }

    // 增减场景变量
	public static bool IncFloatParam(List<string> paramList)
    {
        if (paramList == null || paramList.Count != 2)
        {
            return false;
        }

		BaseScene scene = SceneManager.Instance.GetCurScene();
		if (scene == null)
		{
			return false;
		}

		int idx = int.Parse(paramList[0]);
		float param = -1.0f;
		bool ret = scene.GetFloatParam(idx, ref param);
		if (!ret)
		{
			return false;
		}

		return scene.SetFloatParam(idx, param + float.Parse(paramList[1]));
    }


	// 通过别名删除场景内角色
	public static bool RemoveObjectByAlias(List<string> paramList)
	{
		if (paramList == null || paramList.Count != 1)
		{
			return false;
		}

		BaseScene scene = SceneManager.Instance.GetCurScene();
		if (scene == null)
		{
			return false;
		}

		return scene.RemoveObjectByAlias(paramList[0]);
	}

	// 通过别名删除场景内特效
	public static bool RemoveParticleByAlias(List<string> paramList)
	{
		if (paramList == null || paramList.Count != 1)
		{
			return false;
		}

		BaseScene scene = SceneManager.Instance.GetCurScene();
		if (scene == null)
		{
			return false;
		}

		return scene.RemoveParticleByAlias(paramList[0]);
	}

    /// <summary>
    /// 爬塔时启动当前层的trigger，只有stagescene使用
    /// </summary>
    /// <param name="paramList"></param>
    /// <returns></returns>
	public static bool StartCurTrigger(List<string> paramList)
    {
     
        int curFloor = ModuleManager.Instance.FindModule<ChallengeModule>().GetDoingFloor();
        //GameDebug.Log("StartCurTrigger"+curFloor);
        
        return SceneManager.Instance.RestartTrigger("t" + curFloor) &&
                    SceneManager.Instance.RestartTrigger("tf" + curFloor);
    }

    public static bool RestartTrigger(List<string> paramList)
    {
        GameDebug.Log("RestartTrigger" + paramList[0]);
        return SceneManager.Instance.RestartTrigger(paramList[0]);
    }

	// 打开界面
	public static bool OpenUI(List<string> paramList)
	{
		if (paramList == null || paramList.Count < 1)
		{
			return false;
		}

		string uiname = paramList[0];

		if (uiname.Equals("stagelist"))
		{
			if (paramList.Count != 3)
			{
				return false;
			}

			StageListModule module = ModuleManager.Instance.FindModule<StageListModule>();
			module.OpenStageListUI((SceneType)System.Enum.Parse(typeof(SceneType), paramList[1]), int.Parse(paramList[2]));
		}else if (uiname.Equals("mainmap"))
		{

            WindowManager.Instance.OpenUI(uiname);
           
		}
		else
		{
			if (!WindowManager.Instance.OpenUI(uiname))
			{
				return false;
			}
		}

		return true;
	}

	// 关闭界面
	public static bool CloseUI(List<string> paramList)
	{
		if (paramList == null || paramList.Count != 1)
		{
			return false;
		}

		WindowManager.Instance.CloseUI(paramList[0]);

		return true;
	}

	// 启动摄像机路径
	public static bool StartCameraPath(List<string> paramList)
	{
		if (paramList == null || paramList.Count != 1)
		{
			return false;
		}

		CameraController.Instance.PlayCameraAnimation(paramList[0]);

		return true;
	}

	// 停止摄像机路径
	public static bool StopCameraPath(List<string> paramList)
	{
		if (paramList == null || paramList.Count != 0)
		{
			return false;
		}

		CameraController.Instance.StopCameraAniamtion();

		return true;
	}

	// 暂停摄像机路径
	public static bool PauseCameraPath(List<string> paramList)
	{
		if (paramList == null || paramList.Count != 0)
		{
			return false;
		}

		CameraController.Instance.PauseCameraAniamtion();

		return true;
	}

   
	// Boss警报
	public static bool BossAlert(List<string> paramList)
	{
		if (paramList == null || paramList.Count != 2)
		{
			return false;
		}

		WindowManager.Instance.OpenUI(paramList[0]);
        SceneManager.Instance.GetCurScene().StopBgSound();
        var bossAlertSoundId=SoundManager.Instance.Play(int.Parse(paramList[1]), false);
        SceneManager.Instance.GetCurScene().PlayBossBgSound();
        SoundManager.Instance.AddFinishCallback(bossAlertSoundId, BossAlertSoundFinish);

		return true;
	}

    public static void BossAlertSoundFinish()
    {
        SceneManager.Instance.GetCurScene().PlayBossBgSound();
    }
	// 暂停
	public static bool LogicPause(List<string> paramList)
	{
		SceneManager.Instance.LogicPause();
		return true;
	}

	// 恢复
	public static bool LogicResume(List<string> paramList)
	{
		SceneManager.Instance.LogicResume();
		return true;
	}

	// 启动情节
	public static bool StartStory(List<string> paramList)
	{
		if (paramList == null || paramList.Count != 1)
		{
			return false;
		}

		return StoryManager.Instance.StartStory(int.Parse(paramList[0]));
	}

	// 设置摄像机
	public static bool SetCamera(List<string> paramList)
	{
		if (paramList == null || paramList.Count != 3)
		{
			return false;
		}

		CameraController.Instance.SetCameraInfo(float.Parse(paramList[0]), float.Parse(paramList[1]), float.Parse(paramList[2]));

		return true;
	}

	// 锁定摄像机
	public static bool LockCamera(List<string> paramList)
	{
		if (paramList == null || paramList.Count != 1)
		{
			return false;
		}

		CameraController.Instance.LockCamera = bool.Parse(paramList[0]);

		return true;
	}

	// 通过别名杀死obj 仅开放建筑死亡 其他建议用Remove
	public static bool KillObjectByAlias(List<string> paramList)
	{
		if (paramList == null || paramList.Count != 1)
		{
			return false;
		}

		BaseScene scene = SceneManager.Instance.GetCurScene();
		if(scene == null)
		{
			return false;
		}

		return scene.KillObjectByAlias(paramList[0]);
	}

	public static bool ZombieCrazy(List<string> paramList)
    {
        return ZombiesStageModule.ZombieCrazy();
    }

	public static bool SaySomething(List<string> paramList)
    {
        if (paramList == null || paramList.Count != 1)
            return false;

        PromptUIManager.Instance.AddNewPrompt(StringHelper.GetString(paramList[0]));

        return true;
    }

	public static bool ZombieTenSecond(List<string> paramList)
    {
        if (paramList == null || paramList.Count != 2)
            return false;

        return ZombiesStageModule.ZombieTenSecond(System.Convert.ToInt32(paramList[0]), System.Convert.ToInt32(paramList[1]));
    }

    public static bool ZombieSetEnermyNum(List<string> paramList)
    {
        if (paramList == null || paramList.Count != 1)
            return false;

        return ZombiesStageModule.SetTotalEnermyNum(System.Convert.ToUInt32(paramList[0]));
    }

    public static bool SetChallengeTotalNum(List<string> paramList)
    {
        if (paramList == null || paramList.Count != 1)
            return false;
        ModuleManager.Instance.FindModule<MonsterFloodModule>().TotalNum = Convert.ToInt32(paramList[0]);
        ModuleManager.Instance.FindModule<MonsterFloodModule>().mCurNum = 1;
        BattleUIEvent evt= new BattleUIEvent(BattleUIEvent.BATTLE_UI_UPDATE_MONSTER_FlOOD);
        EventSystem.Instance.PushEvent(evt);     
        return true;
    }

    public static bool SetChallengeNum(List<string> paramList)
    {
        if (paramList == null || paramList.Count != 1)
            return false;
        ModuleManager.Instance.FindModule<MonsterFloodModule>().mCurNum =  Convert.ToInt32(paramList[0]);
        BattleUIEvent evt = new BattleUIEvent(BattleUIEvent.BATTLE_UI_UPDATE_MONSTER_FlOOD);
        GameDebug.Log("设置波数" + ModuleManager.Instance.FindModule<MonsterFloodModule>().mCurNum);
        EventSystem.Instance.PushEvent(evt);
        return true;
    }

    public static bool CheckPickable(List<string> paramList)
    {
        GameDebug.Log("interface_checkPick");
        if (paramList == null || paramList.Count != 1)
            return false;
        ModuleManager.Instance.FindModule<MonsterFloodModule>().CheckPicksable(paramList[0]);
        return true;
    }

	public static bool AddBuff(List<string> paramList)
	{
		if (paramList == null || paramList.Count != 1)
			return false;

		Player player = PlayerController.Instance.GetControlObj() as Player;
		if (player == null)
			return false;

		player.AddSkillEffect(new AttackerAttr(player), SkillEffectType.Buff, uint.Parse(paramList[0]));

		return true;
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="paramList"></param>
    /// <returns></returns>
	public static bool PlaySceneAni(List<string> paramList)
	{
		if (paramList == null || paramList.Count != 1)
			return false;

		BaseScene scene = SceneManager.Instance.GetCurScene();
		if (scene == null)
			return false;

		scene.PlayGameObjAnim(paramList[0]);
        
		return true;
	}
    public static bool StopDynamicOBJToTarget(List<string> paramList)
    {
		BaseScene scene = SceneManager.Instance.GetCurScene();
		if (scene == null)
			return false;

		scene.StopDynamicToTarget();

		return true;
    }
    public static bool PauseDynamicOBJ(List<string> paramList)
    {
        BaseScene scene = SceneManager.Instance.GetCurScene();
        if (scene == null)
            return false;

        scene.PauseDynamic();

		return true;
    }
    public static bool PlayDynamicOBJ(List<string> paramList)
    {
        BaseScene scene = SceneManager.Instance.GetCurScene();
        if (scene == null)
            return false;

        scene.PlayDynamicAnimation();

		return true;
    }

    /// <summary>
    /// 变暗场景
    /// </summary>
    /// <param name="paramList">排除指定物体</param>
    public static bool DarkScene(List<string> paramList)
    {
		BaseScene scene = SceneManager.Instance.GetCurScene();
		if (scene == null)
			return false;   

		List<uint> instID = new List<uint>();
		if(paramList != null)
		{
			foreach(string alias in paramList)
			{
                if (alias == "Player")
                {
                    instID.Add(PlayerController.Instance.GetControl());

                }
                else
                {
                    List<VisualObject> objList = scene.GetSceneObjManager().SearchObjsByAlias<VisualObject>(alias);
                    foreach (VisualObject obj in objList)
                    {
                        if (instID.IndexOf(obj.InstanceID) < 0)
                            instID.Add(obj.InstanceID);
                    }
                }
			}
		}

        scene.GetEffectManager().DarkenScene(instID.ToArray(), instID.Count == 0 ? 1 : 0.7f);

		return true;
    }
	public static bool DeDarkScene(List<string> paramList)
    {
		BaseScene scene = SceneManager.Instance.GetCurScene();
		if (scene == null)
			return false;

		scene.GetEffectManager().RecoverScene();

		return true;
    }

	public static bool ShowGuide(List<string> paramList)
	{
		if (paramList == null || paramList.Count != 1)
			return false;

        GuideModule module = ModuleManager.Instance.FindModule<GuideModule>();
        module.EnterTrigger(int.Parse(paramList[0]));
        
		return true;
	}

	public static bool ShowTimer(List<string> paramList)
	{
		if (paramList == null || paramList.Count < 1 || paramList.Count > 2)
			return false;

		GameScene scene = SceneManager.Instance.GetCurScene() as GameScene;
		if (scene == null)
			return false;

		if(paramList.Count == 2)
		{
			scene.ResetLogicRunTime();
			SceneManager.Instance.SetCountDown(int.Parse(paramList[1]));
		}

		BattleUIModule module = ModuleManager.Instance.FindModule<BattleUIModule>();
		module.ShowTimer(System.Convert.ToBoolean(paramList[0]));

		return true;
	}

	public static bool TransInScene(List<string> paramList)
	{
		if (paramList == null || paramList.Count < 2 || paramList.Count > 3)
			return false;

		BaseScene scene = SceneManager.Instance.GetCurScene();
		if (scene == null)
			return false;

		BattleUnit unit = PlayerController.Instance.GetControlObj() as BattleUnit;
		if (unit == null)
			return false;

		float x = float.Parse(paramList[0]);
		float z = float.Parse(paramList[1]);
		unit.SetPosition(new Vector3(x, scene.GetHeight(x, z), z));

		if(paramList.Count == 3)
		{
			unit.SetDirection(float.Parse(paramList[2]));
		}

		return true;
	}

    public static bool GiveSkill(List<string> paramList)
    {
        if (paramList == null || paramList.Count < 1)
            return false;

        SkillModule module = ModuleManager.Instance.FindModule<SkillModule>();

        module.GiveSkills(paramList);

//         BaseScene scene = SceneManager.Instance.GetCurScene();
//         if (scene == null)
//             return false;
// 
//         BattleUnit unit = PlayerController.Instance.GetControlObj() as BattleUnit;
//         if (unit == null)
//             return false;
// 
//         float x = float.Parse(paramList[0]);
//         float z = float.Parse(paramList[1]);
//         unit.SetPosition(new Vector3(x, scene.GetHeight(x, z), z));
// 
//         if (paramList.Count == 3)
//         {
//             unit.SetDirection(float.Parse(paramList[2]));
//         }
        return true;
    }

	public static bool CheckFirstEnter(List<string> paramList)
	{
		BaseScene scene = SceneManager.Instance.GetCurScene();
		if (scene == null)
			return false;

		SceneTableItem res = scene.GetSceneRes();
		if (res == null)
			return false;

		PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
		return !PlayerPrefs.HasKey(module.getGUID().ToString() + "visited_" + res.resID.ToString());
	}

	public static bool SetPlayerLeague(List<string> paramList)
	{
		if (paramList == null || paramList.Count != 1)
			return false;

		BattleUnit unit = PlayerController.Instance.GetControlObj() as BattleUnit;
		if (unit == null)
			return false;

		unit.SetLeague((LeagueDef)(int.Parse(paramList[0])));

		return true;
	}

    public static bool UnlockWeaponSkill(List<string> paramList)
    {
        SkillModule module = ModuleManager.Instance.FindModule<SkillModule>();

        module.UnlockWeaponSkill();
        return true;
    }

	public static bool ShowProgress(List<string> paramList)
	{
		if (paramList == null || paramList.Count != 3)
			return false;

		BattleUIModule module = ModuleManager.Instance.FindModule<BattleUIModule>();
		module.ShowProgress(int.Parse(paramList[0]), paramList[1], float.Parse(paramList[2]));

		return true;
	}
}
