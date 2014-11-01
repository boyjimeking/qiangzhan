using UnityEngine;
using System.Collections;

public class BattleSceneInitParam : GameSceneInitParam
{
	public int battle_res_id = -1;
}

public class BattleScene : GameScene
{
	// BattleScene子表
	private Scene_BattleSceneTableItem mSubRes = null;

	public BattleScene()
	{

	}

    override public bool Init(BaseSceneInitParam param)
	{
		BattleSceneInitParam battleParam = param as BattleSceneInitParam;

		if (!DataManager.Scene_BattleSceneTable.ContainsKey(battleParam.battle_res_id))
		{
			return false;
		}

		mSubRes = DataManager.Scene_BattleSceneTable[battleParam.battle_res_id] as Scene_BattleSceneTableItem;

		if (!base.Init(battleParam))
			return false;

		return true;
	}

	public Scene_BattleSceneTableItem GetBattleRes()
	{
		return mSubRes;
	}

	public override SceneType getType()
	{
		return SceneType.SceneType_Battle;
	}
}
