using UnityEngine;
using System.Collections;

public class SceneFactory
{
    virtual public SceneType GetSceneType()
    {
        return SceneType.SceneType_Invaild;
    }

    virtual public BaseScene CreateScene(int resId)
    {
        return null;
    }

    virtual public bool InitScene(BaseScene scene, int resId)
    {
        return false;
    }

/*    void DestroyScene(BaseScene scene);*/
}

// 主城
public class CitySceneFactory : SceneFactory
{
    override public SceneType GetSceneType()
    {
        return SceneType.SceneType_City;
    }

    override public BaseScene CreateScene(int resId)
    {
        BaseScene scene = new CityScene();

        if (!InitScene(scene, resId))
        {
            return null;
        }

        return scene;
    }

    override public bool InitScene(BaseScene scene, int resId)
    {
        CitySceneInitParam param = new CitySceneInitParam();
        param.res_id = resId;

        return scene.Init(param);
    }
}

public class QiangLinDanYuSceneFactory : SceneFactory
{
    override public SceneType GetSceneType()
    {
        return SceneType.SceneType_QiangLinDanYu;
    }

    override public BaseScene CreateScene(int resId)
    {
        BaseScene scene = new QiangLinDanYuScene();

        if (!InitScene(scene, resId))
        {
            return null;
        }

        return scene;
    }

    override public bool InitScene(BaseScene scene, int resId)
    {
        QiangLinDanYuSceneInitParam param = new QiangLinDanYuSceneInitParam();
        param.res_id = resId;

        return scene.Init(param);
    }
}

public class ArenaSceneFactory : SceneFactory
{
	override public SceneType GetSceneType()
	{
		return SceneType.SceneType_Arena;
	}

	override public BaseScene CreateScene(int resId)
	{
		BaseScene scene = new ArenaScene();

		if (!InitScene(scene, resId))
		{
			return null;
		}

		return scene;
	}

	override public bool InitScene(BaseScene scene, int resId)
	{
		ArenaSceneInitParam param = new ArenaSceneInitParam();
		param.res_id = resId;

		return scene.Init(param);
	}
}

public class QualifyingSceneFactory : SceneFactory
{
	override public SceneType GetSceneType()
	{
		return SceneType.SceneType_Qualifying;
	}

	override public BaseScene CreateScene(int resId)
	{
		BaseScene scene = new QualifyingScene();

		if (!InitScene(scene, resId))
		{
			return null;
		}

		return scene;
	}

	override public bool InitScene(BaseScene scene, int resId)
	{
		QualifyingSceneInitParam param = new QualifyingSceneInitParam();
		param.res_id = resId;

		return scene.Init(param);
	}
}

// 关卡
public class StageSceneFactory : SceneFactory
{
    override public SceneType GetSceneType()
    {
        return SceneType.SceneType_Stage;
    }

    override public BaseScene CreateScene(int resId)
    {
        BaseScene scene = new StageScene();

        if (!InitScene(scene, resId))
        {
            return null;
        }

        return scene;
    }

    override public bool InitScene(BaseScene scene, int resId)
    {
        StageSceneInitParam param = new StageSceneInitParam();
		param.res_id = resId;

        return scene.Init(param);
    }
}

// 战场
public class BattleSceneFactory : SceneFactory
{
    override public SceneType GetSceneType()
    {
        return SceneType.SceneType_Battle;
    }

    override public BaseScene CreateScene(int resId)
    {
        BaseScene scene = new BattleScene();

        if (!InitScene(scene, resId))
        {
            return null;
        }

        return scene;
    }

    override public bool InitScene(BaseScene scene, int resId)
    {
        BattleSceneInitParam param = new BattleSceneInitParam();
        param.res_id = resId;

        return scene.Init(param);
    }
}

public class TowerStageSceneFactory : SceneFactory
{
    override public SceneType GetSceneType()
    {
        return SceneType.SceneType_Tower;
    }

    override public BaseScene CreateScene(int resId)
    {
        BaseScene scene = new TowerStageScene();

        if (!InitScene(scene, resId))
        {
            return null;
        }

        return scene;
    }

    override public bool InitScene(BaseScene scene, int resId)
    {
        TowerStageSceneInitParam param = new TowerStageSceneInitParam();
        param.res_id = resId;

        return scene.Init(param);
    }
}

public class ZombiesStageSceneFactory : SceneFactory
{
    override public SceneType GetSceneType()
    {
        return SceneType.SceneType_Zombies;
    }

    override public BaseScene CreateScene(int resId)
    {
        BaseScene scene = new ZombiesStageScene();

        if (!InitScene(scene, resId))
        {
            return null;
        }

        return scene;
    }

    override public bool InitScene(BaseScene scene, int resId)
    {
        ZombiesStageSceneInitParam param = new ZombiesStageSceneInitParam();
        param.res_id = resId;

        return scene.Init(param);
    }
}

public class MonsterFloodStageSceneFactory : SceneFactory
{
    override public SceneType GetSceneType()
    {
        return SceneType.SceneType_MonsterFlood;
    }

    override public BaseScene CreateScene(int resId)
    {
        BaseScene scene = new MonsterFloodStageScene();

        if (!InitScene(scene, resId))
        {
            return null;
        }

        return scene;
    }

    override public bool InitScene(BaseScene scene, int resId)
    {
        MonsterFloodStageSceneInitParam param = new MonsterFloodStageSceneInitParam();
        param.res_id = resId;

        return scene.Init(param);
    }
}

public class MaoStageSceneFactory : SceneFactory
{
    override public SceneType GetSceneType()
    {
        return SceneType.SceneType_Mao;
    }

    override public BaseScene CreateScene(int resId)
    {
        BaseScene scene = new MaoStageScene();

        if (!InitScene(scene, resId))
        {
            return null;
        }

        return scene;
    }

    override public bool InitScene(BaseScene scene, int resId)
    {
        MaoStageSceneInitParam param = new MaoStageSceneInitParam();
        param.res_id = resId;

        return scene.Init(param);
    }
}

public class WantedStageSceneFactory : SceneFactory
{
	override public SceneType GetSceneType()
	{
		return SceneType.SceneType_Wanted;
	}

	override public BaseScene CreateScene(int resId)
	{
		BaseScene scene = new WantedStageScene();

		if (!InitScene(scene, resId))
		{
			return null;
		}

		return scene;
	}

	override public bool InitScene(BaseScene scene, int resId)
	{
		WantedStageSceneInitParam param = new WantedStageSceneInitParam();
		param.res_id = resId;

		return scene.Init(param);
	}
}

public class TDSceneFactory : SceneFactory
{
	override public SceneType GetSceneType()
	{
		return SceneType.SceneType_TD;
	}

	override public BaseScene CreateScene(int resId)
	{
		BaseScene scene = new TDScene();

		if (!InitScene(scene, resId))
		{
			return null;
		}

		return scene;
	}

	override public bool InitScene(BaseScene scene, int resId)
	{
		TDSceneInitParam param = new TDSceneInitParam();
		param.res_id = resId;

		return scene.Init(param);
	}
}

public class YaZhiXieESceneFactory : SceneFactory
{
	override public SceneType GetSceneType()
	{
		return SceneType.SceneType_YaZhiXieE;
	}

	override public BaseScene CreateScene(int resId)
	{
		BaseScene scene = new YaZhiXieEScene();

		if (!InitScene(scene, resId))
		{
			return null;
		}

		return scene;
	}

	override public bool InitScene(BaseScene scene, int resId)
	{
		YaZhiXieESceneInitParam param = new YaZhiXieESceneInitParam();
		param.res_id = resId;

		return scene.Init(param);
	}
}

public class ZhaoCaiMaoSceneFactory : SceneFactory
{
	override public SceneType GetSceneType()
	{
		return SceneType.SceneType_ZhaoCaiMao;
	}

	override public BaseScene CreateScene(int resId)
	{
		BaseScene scene = new ZhaoCaiMaoScene();

		if (!InitScene(scene, resId))
		{
			return null;
		}

		return scene;
	}

	override public bool InitScene(BaseScene scene, int resId)
	{
		ZhaoCaiMaoSceneInitParam param = new ZhaoCaiMaoSceneInitParam();
		param.res_id = resId;

		return scene.Init(param);
	}
}