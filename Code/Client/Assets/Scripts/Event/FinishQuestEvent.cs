using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Message;


public class FinishQuestEvent:EventBase
{
    public const string QUEST_FINISHED = "QUEST_FINISHED";
    public const string QUEST_FINISHED_ALL = "QUEST_FINISHED_ALL";

    public int mQuestId;

    public msg_quest_award mAwardInfo;
    public FinishQuestEvent(string eventName) : base(eventName)
    {
    }
}
