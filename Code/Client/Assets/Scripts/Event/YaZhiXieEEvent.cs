using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class YaZhiXieEUpdateRankListEvent : EventBase
{
	public static string YAZHIXIEE_UPDATE_RANKLIST_EVENT = "YAZHIXIEE_UPDATE_RANKLIST_EVENT";

	public List<Message.yazhixiee_ranklist_role_info> sortInfo = new List<Message.yazhixiee_ranklist_role_info>();

	public YaZhiXieEUpdateRankListEvent()
		: base(YAZHIXIEE_UPDATE_RANKLIST_EVENT)
	{

	}
}

public class YaZhiXieEUpdateScoreEvent : EventBase
{
	public static string YAZHIXIEE_UPDATE_SCORE_EVENT = "YAZHIXIEE_UPDATE_SCORE_EVENT";
	public uint mScore;
	public uint mCount;

	public YaZhiXieEUpdateScoreEvent(uint score, uint count)
		: base(YAZHIXIEE_UPDATE_SCORE_EVENT)
	{
		mScore = score;
		mCount = count;
	}
}
