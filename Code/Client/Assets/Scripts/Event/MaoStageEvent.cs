using UnityEngine;
using System.Collections;


public class MaoStageSucceedEvent : EventBase
{
    public static string MAO_STAGE_SCENE_SUCCEED_EVENT = "MAO_STAGE_SCENE_SUCCEED_EVENT";
    public MaoStageSucceedEvent()
        : base(MAO_STAGE_SCENE_SUCCEED_EVENT)
	{

	}
}

public class MaoStageUpdateGoldEvent : EventBase
{
    public static string MAO_STAGE_UPDATE_GOLD_EVENT = "MAO_STAGE_UPDATE_GOLD_EVENT";

    public int CurrentGold = 0;
    public int TotalGold = 0;
    public Vector3 PickPos;

    public MaoStageUpdateGoldEvent()
        : base(MAO_STAGE_UPDATE_GOLD_EVENT)
    {
    }
}

// public class MaoStageUpdageAngerEvent : EventBase
// {
//     public static string MAO_STAGE_UPDAGE_ANGER_EVENT = "MAO_STAGE_UPDAGE_ANGER_EVENT";
// 
//     public float Value = 0.0f;
// 
//     public MaoStageUpdageAngerEvent()
//         :base(MAO_STAGE_UPDAGE_ANGER_EVENT)
//     {   
//     }
// }

// public class MaoStageCrazyEvent : EventBase
// {
//     public static string MAO_STAGE_CRAZY_EVENT = "MAO_STAGE_CRAZY_EVENT";
// 
//     public bool Crazy = true;
// 
//     public MaoStageCrazyEvent()
//         : base(MAO_STAGE_CRAZY_EVENT)
//     {
//     }
// }

public class MaoStageUpdateTargetPosEvent : EventBase
{
    public static string MAO_STAGE_UPDATE_TARGET_POS_EVENT = "MAO_STAGE_UPDATE_TARGET_POS_EVENT";

    public Vector3 CatPos;

    public MaoStageUpdateTargetPosEvent()
        : base(MAO_STAGE_UPDATE_TARGET_POS_EVENT)
    {

    }
}




