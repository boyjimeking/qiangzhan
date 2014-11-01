using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class QuestEvent:EventBase
{
    
    public const string QUEST_CONDITION_CHECK = "QUEST_CONDITION_CHECK";
    public const string QUEST_ACCEPT = "QUEST_ACCEPT";
    public const string QUEST_UPDATE = "QUEST_UPDATE";
	public const string Quest_SYNC_SERVER_EVENT = "Quest_SYNC_SERVER_EVENT";
    public int mQuestId;
    public QuestEvent(string eventName) : base(eventName)
    {
    }
}

