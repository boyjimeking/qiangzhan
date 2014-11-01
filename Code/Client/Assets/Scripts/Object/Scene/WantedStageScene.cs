using UnityEngine;
using System.Collections;

// 通缉令
public class WantedStageSceneInitParam : StageSceneInitParam
{
}

public class WantedStageScene : StageScene
{

	public WantedStageScene()
    {

    }

    override public bool Init(BaseSceneInitParam param)
	{
		if (!base.Init(param))
		   return false;


		return true;
	}

	protected override void OnSceneInited ()
	{
		base.OnSceneInited();

	}

    public override SceneType getType()
    {
        return SceneType.SceneType_Wanted;
    }

	protected override void OnStateChangeToClosing()
	{
		mBattleUIModule.ShowTimer(false);

		base.OnStateChangeToClosing();
	}
}
