using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//每个怪物信息
public class GrowthTriggerInfo
{
	public string type = "";
	public int resId = -1;
    public string alias = null;
	public float x = 0.0f;
	public float z = 0.0f;
	public float dir = 0.0f;

    public int talkID = -1;
}

public class PickGrowthTriggerInfo : GrowthTriggerInfo
{
	public int picktype;
	public int content;
}

public class BuildGrowthTriggerInfo : GrowthTriggerInfo
{
	public int barrier;
}

//步骤
public class GrowthTriggerStep
{
	public bool killAll = false;
	public int time = 0;
	public int curTime = 0;

	// 循环刷怪次数
	public int repeat = 1;
	// 当前循环刷怪次数
	public int curRepeat = 0;
	// 循环间隔时间
	public int spacetime = 0;
	// 当前循环间隔时间
	public int curSpaceTime = 0;

	public ArrayList objs = new ArrayList();

	public void reset()
	{
		curTime = 0;
		curRepeat = 0;
		curSpaceTime = 0;
	}
}

//刷新器
public class GrowthTrigger : BaseTrigger
{
	public ArrayList steps = new ArrayList();

	private int mCurStep = -1;

	// 需要全部击杀的实例id
	private ArrayList cacheIdKillAll = new ArrayList();

	// 不需全部击杀的实例id
	private ArrayList cacheId = new ArrayList();

	// 等待创建的Obj
	private ArrayList slowCreateIds = new ArrayList();

	// 等待移除
	private ArrayList cacheIdRemove = new ArrayList();

	public GrowthTrigger(BaseScene scn)
		: base(scn, TriggerType.Growth)
	{
	}

	override public void Update(uint elapsed)
	{
        if(!IsRunning())
        {
            return;
        }

		if( mScene == null )
        {
            return;
        }

		if( mCurStep < 0 )
		{
			DoNext(++mCurStep);
			return ;
		}

		//步骤执行完
		if(  mCurStep >= steps.Count )
		{
			return ;
		} 

		GrowthTriggerStep step = steps[mCurStep] as GrowthTriggerStep;
		if( step == null )
        {
            return;
        }

		if (doSlowCreate(step.killAll))
		{
			return;
		}

		if( step.killAll && !CheckKillAll(step) )
		{
			return ;
		}

		int spacetime = (int)elapsed;

		if(step.curRepeat < step.repeat || step.repeat < 0)
		{
			step.curSpaceTime += spacetime;
			if(step.curSpaceTime > step.spacetime)
			{
				DoNext(mCurStep);
			}
			else
			{
				return;
			}
		}

		step.curTime += spacetime;

		if( step.curTime < step.time )
		{
			return ;
		}

		++mCurStep;
		if( mCurStep >= steps.Count )
        {
            Stop();
			mScene.OnTriggerFinish(this.name);
            mScene.OnGrowthTriggerFinish(this.name);
            return;
        }

		DoNext( mCurStep );
	}

	private bool CheckKillAll(GrowthTriggerStep step)
	{
		if( step == null )
        {
            return false;
        }

		bool allDie = true;
		for( int i = 0 ; i < cacheIdKillAll.Count ; ++i )
		{
			ObjectBase obj = mScene.FindObject((uint)cacheIdKillAll[i]);
			if( obj == null )
			{
				cacheIdRemove.Add(cacheIdKillAll[i]);
			}
			else
			{
				allDie = false;
			}
		}

		if(allDie)
		{
			cacheIdKillAll.Clear();
		}

		foreach(uint id in cacheIdRemove)
		{
			cacheIdKillAll.Remove(id);
		}
		cacheIdRemove.Clear();

		return allDie;
	}

	private void DoNext(int next)
	{
		if(mScene == null)
		{
			return;
		}

		GrowthTriggerStep step = steps[next] as GrowthTriggerStep;
		if( step == null || step.objs.Count <= 0 )
        {
            return;
        }

		slowCreateIds = (ArrayList)step.objs.Clone();

		step.curTime = 0;
		step.curSpaceTime = 0;
		step.curRepeat++;
	}

	private bool doSlowCreate(bool killAll)
	{
		if(slowCreateIds.Count < 1)
		{
			return false;
		}

		GrowthTriggerInfo info = slowCreateIds[0] as GrowthTriggerInfo;

		if (info.type == "NPC")
		{
			NpcInitParam npcParam = new NpcInitParam();
			npcParam.npc_res_id = info.resId;
			float y = mScene.GetHeight(info.x, info.z);
			npcParam.init_pos = new Vector3(info.x, y, info.z);
			npcParam.init_dir = info.dir;
			npcParam.alias = info.alias;
            npcParam.talk_id = info.talkID;

			ObjectBase obj = mScene.CreateSprite(npcParam);
			if(obj == null)
			{
				GameDebug.LogError("创建Npc失败。npcId:" + info.resId);
				return false;
			}

            if ("ghost" == info.alias)
            {
                mScene.GhostObjects().Add(info);
            }

			if (obj != null)
			{
				if (killAll)
				{
					cacheIdKillAll.Add(obj.InstanceID);
				}
				else
				{
					cacheId.Add(obj.InstanceID);
				}
			}
		}
		else if (info.type == "PICK")
		{
			PickGrowthTriggerInfo pickinfo = info as PickGrowthTriggerInfo;

			List<PickInitParam> paramList = new List<PickInitParam>();
			if (SceneObjManager.CreatePickInitParam((Pick.PickType)(System.Enum.Parse(typeof(Pick.PickType), pickinfo.picktype.ToString())),
				pickinfo.resId, pickinfo.content, new Vector3(info.x, mScene.GetHeight(info.x, info.z), info.z), info.dir, out paramList, false, 
				Pick.FlyType.FLY_OUT, pickinfo.picktype != (int)Pick.PickType.SUPER_WEAPON))
			{
				foreach (PickInitParam param in paramList)
				{
					param.init_pos.y = mScene.GetHeight(param.init_pos.x, param.init_pos.z);
					param.alias = info.alias;
					ObjectBase obj = mScene.CreateSprite(param);
					if (obj == null)
					{
						GameDebug.LogError("创建Pick失败。pickId:" + info.resId);
						return false;
					}

					if (obj != null)
					{
						if (killAll)
						{
							cacheIdKillAll.Add(obj.InstanceID);
						}
						else
						{
							cacheId.Add(obj.InstanceID);
						}
					}
				}
			}
		}
		else if (info.type == "BUILD")
		{
			BuildGrowthTriggerInfo buildinfo = info as BuildGrowthTriggerInfo;

			BuildInitParam buildParam = new BuildInitParam();
			buildParam.build_res_id = info.resId;
			float y = mScene.GetHeight(info.x, info.z);
			buildParam.init_pos = new Vector3(info.x, y, info.z);
			buildParam.init_dir = info.dir;
			buildParam.build_barrier = buildinfo.barrier > 0;
			buildParam.alias = info.alias;

			ObjectBase obj = mScene.CreateSprite(buildParam);
			if (obj == null)
			{
				GameDebug.LogError("创建Building失败。buildId:" + info.resId);
				return false;
			}

			if (obj != null)
			{
				if (killAll)
				{
					cacheIdKillAll.Add(obj.InstanceID);
				}
				else
				{
					cacheId.Add(obj.InstanceID);
				}
			}
		}
		else if (info.type == "PARTICLE")
		{
			Vector3 pos = new Vector3(info.x, mScene.GetHeight(info.x, info.z), info.z);
			mScene.CreateEffect(info.resId, Vector3.one, pos, info.dir, info.alias);
		}

		slowCreateIds.RemoveAt(0);

		if (slowCreateIds.Count < 1)
		{
			return false;
		}

		return true;
	}

	public void ClearCache()
	{
		for (int i = 0; i < cacheId.Count; ++i)
		{
			mScene.RemoveObject((uint)cacheId[i]);
		}

		for (int i = 0; i < cacheIdKillAll.Count; ++i)
		{
			mScene.RemoveObject((uint)cacheIdKillAll[i]);
		}  
	}

    public void Reset()
    {
        mCurStep = -1;
    }
	override public void Destroy()
	{
		ClearCache();
		Stop();
		mCurStep = steps.Count;
		slowCreateIds.Clear();
		cacheIdRemove.Clear();
	}

    public override void Restart()
    {
        base.Restart();
        mCurStep = -1;
        foreach (var step in steps)
        {
            var s = step as GrowthTriggerStep;
			s.reset();
        }
    }
}
