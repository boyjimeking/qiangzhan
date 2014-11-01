using UnityEngine;
using System.Collections;

public class EggManager
{
    private static EggManager instance = null;

    private EggModule mModule = null;

    EggModule Module
    {
        get
        {
            if (mModule == null)
                mModule = ModuleManager.Instance.FindModule<EggModule>();

            return mModule;
        }
    }

    public EggManager()
	{
		instance = this;
	}
    public static EggManager Instance
	{
		get
		{
			return instance;
		}
	}

    public void Update(uint elapsed)
    {
        BaseFlow bf = GameApp.Instance.GetCurFlow();
       
        if(bf == null)
            return;

        if(bf.GetFlowEnum() == GAME_FLOW_ENUM.GAME_FLOW_MAIN)
            Module.Update(elapsed);
    }

}
