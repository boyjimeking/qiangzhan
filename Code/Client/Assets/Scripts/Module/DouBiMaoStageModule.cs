using System;
using UnityEngine;
using System.Collections.Generic;


class DouBiMaoStageModule : ModuleBase
{

    public bool IsDouBiMaoGame
    {
        get
        {
            StageScene bs = SceneManager.Instance.GetCurScene() as StageScene;
            if (bs == null)
            {
                return false;
            }

            return SceneManager.GetSceneType(bs.GetStageRes()) == SceneType.SceneType_Mao;
        }
    }
}
