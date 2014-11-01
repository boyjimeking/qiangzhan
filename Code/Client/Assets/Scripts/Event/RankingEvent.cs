using System;

class RankingEvent : EventBase
{
    public const string RANKING_UPDATE = "RANKING_UPDATE";

    public RankingEvent(string eventName)
        : base(eventName)
    {
    }
}
