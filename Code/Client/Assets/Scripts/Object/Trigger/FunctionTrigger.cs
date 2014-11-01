using System;
using UnityEngine;
using System.Collections;

// 步骤中的功能
public class FunctionTriggerInfo
{
	public string funcname = "";
}

public class TimeTriggerInfo
{
    //总时间，毫秒为单位
    public int mTotalTime;
}

// 步骤
public class FunctionTriggerStep
{
	public int	time = 0;
	public int	curTime = 0;
	public ArrayList functions = new ArrayList();
    public ArrayList times= new ArrayList();
}

// 功能触发器
public class FunctionTrigger : BaseTrigger
{
	public ArrayList steps = new ArrayList();

	private int mCurStep = -1;

	public FunctionTrigger(BaseScene scn)
		: base(scn, TriggerType.Function)
	{
	}

    public override void Restart()
    {
        mCurStep = -1;
        base.Restart();
        foreach (var step in steps)
        {
           var s= step as FunctionTriggerStep;
            s.curTime = 0;
        }
    }

	override public void Update(uint elapsed)
	{
        if (!IsRunning())
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

        FunctionTriggerStep step = steps[mCurStep] as FunctionTriggerStep;
		if( step == null )
        {
            return;
        }
		step.curTime += (int)elapsed;

		if( step.curTime < step.time )
		{
			return ;
		}
	    StepStop(mCurStep);
		++mCurStep;
		if( mCurStep >= steps.Count )
        {
            Stop();
            mScene.OnTriggerFinish(this.name);
            return;
        }

		DoNext( mCurStep );
	}

    private void DoNext(int next)
    {

        FunctionTriggerStep step = steps[next] as FunctionTriggerStep;
        if (step == null || (step.functions.Count < 1 && step.times.Count < 1))
        {
            return;
        }

        for (int i = 0; i < step.functions.Count; ++i)
        {            
            FunctionTriggerInfo info = step.functions[i] as FunctionTriggerInfo;
            SceneManager.Instance.InvokeFunction(info.funcname);
        }

        for (int i = 0; i < step.times.Count; ++i)
        {
             TimeTriggerInfo info = step.times[i] as TimeTriggerInfo;
          
             SceneManager.Instance.SetCountDown(info.mTotalTime);
        }

        step.curTime = 0;
    }

    private void StepStop(int index)
    {
        FunctionTriggerStep step = steps[index] as FunctionTriggerStep;
        if (step == null || (step.functions.Count < 1 && step.times.Count < 1))
        {
            return;
        }

        if (step.times.Count > 0)
        {
            BattleUIEvent ev = new BattleUIEvent(BattleUIEvent.BATTLE_UI_SHOW_TIMER);
            ev.msg = 0;
            EventSystem.Instance.PushEvent(ev);
        }

    }

	override public void Destroy()
	{
		Stop();
		mCurStep = steps.Count;
	}
}
