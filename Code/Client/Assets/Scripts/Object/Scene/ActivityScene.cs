using UnityEngine;
using System.Collections;

public class ActivitySceneInitParam : GameSceneInitParam
{
}

public class ActivityScene : GameScene
{
	protected uint mReportInterval = uint.MaxValue;

	protected uint mReportTimer = 0;

	protected bool mCompleted = false;

	public ActivityScene()
    {

    }

	override public bool Init( BaseSceneInitParam param )
	{
		if( !base.Init(param) )	
		   return false;

		if (!InitActivityParam())
			return false;
		
		return true;
	}

	override public bool LogicUpdate(uint elapsed)
	{
		if (!base.LogicUpdate(elapsed))
			return false;

		if (mReportTimer < elapsed)
		{
			if(!mCompleted)
			{
				mReportTimer = mReportInterval * 10;

				Report();
			}
		}
		else
		{
			mReportTimer -= elapsed;
		}

		return true;
	}

	public void ResetReportTime()
	{
		mReportTimer = 0;
	}

	virtual protected void Report()
	{

	}

	virtual protected bool InitActivityParam()
	{
		return true;
	}

	override protected bool MayDisplayLianJi()
	{
		return false;
	}
}
