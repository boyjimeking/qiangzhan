using UnityEngine;
using System.Collections;

public class TowerStageSceneInitParam : StageSceneInitParam
{
}

public class TowerStageScene : StageScene
{
    public TowerStageScene()
    {

    }

    override public bool Init(BaseSceneInitParam param)
	{
		if (!base.Init(param))	
		   return false;

        mBalanceComponent = new TowerSceneBlanceComponent(this);
		mShowPickGuide = false;
     
		return true;
	}

	protected override void OnSceneInited ()
	{
		base.OnSceneInited();

        WindowManager.Instance.OpenUI("challengecountdown");

		mBattleUIModule.ShowTimer(true);
	}

    public override SceneType getType()
    {
        return SceneType.SceneType_Tower;
    }

	protected override void OnStateChangeToClosing()
	{
		base.OnStateChangeToClosing();

		mBattleUIModule.ShowTimer(false);
	}
}
