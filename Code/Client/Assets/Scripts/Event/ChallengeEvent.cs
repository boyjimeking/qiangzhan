using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ChallengeEvent : EventBase
{
    //扫荡道具数目变化
    public const string SWEEP_ITEM_NUM = "SWEEP_ITEM_NUM";

    //扫荡掉落
    public const string SWEEP_DROP = "SWEEP_DROP";


    public const string CHALLENGE_UI_UPDATE = "CHALLENGE_UI_UPDATE";

    public const string DROP_UI_UPDATE = "DROP_UI_UPDATE";

    public const string CHALLENGE_RANK_UPDATE = "CHALLENGE_RANK_UPDATE";
    public ChallengeEvent(string eventName) : base(eventName)
    {

    }

    public Object SweepAward
    {
        get;
        set;
    }
}

