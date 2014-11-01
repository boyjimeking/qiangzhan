using UnityEngine;
using System.Collections;

public class CitySceneInitParam : BaseSceneInitParam
{

}

public class CityScene : BaseScene
{

    override public bool Init(BaseSceneInitParam param)
	{
		if( !base.Init(param) )	
		   return false;
		
		return true;
	}

	override public bool InitializeUpdate(uint elapsed)
	{
		if(!base.InitializeUpdate(elapsed))
		{
			return false;
		}

		SceneManager.Instance.SetLastCityResId(mSceneRes.resID);

		return true;
	}

	override public bool isSafeScene()
	{
		return true;
	}

    public override SceneType getType()
    {
        return SceneType.SceneType_City;
    }

    protected override void OnSceneInited()
    {
        base.OnSceneInited();

        WindowManager.Instance.EnterFlow(UI_FLOW_TYPE.UI_FLOW_CITY);

        WindowManager.Instance.OpenUI("city");
        WindowManager.Instance.OpenUI("announcement");

		PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
		PlayerPrefs.SetInt(module.getGUID().ToString() + "visited_" + mSceneRes.resID.ToString(), 1);

        //FightGradeManager.Instance.CheckAndPlayGradeChangeEff();
    }

    protected override void OnSceneDestroy()
    {
        base.OnSceneDestroy();
        WindowManager.Instance.LeaveFlow(UI_FLOW_TYPE.UI_FLOW_CITY);

        WindowManager.Instance.CloseUI("city");
        WindowManager.Instance.CloseUI("announcement");
    }
}
