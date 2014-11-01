using UnityEngine;
using System.Collections.Generic;

public class PlayerSkillTransformEvent : EventBase
{
	public static string PLAYER_TRANSFORM_EVENT = "PLAYER_TRANSFORM_EVENT";

	public List<Pair<uint, string>> mNewSkillList = null;

	public PlayerSkillTransformEvent(IEnumerable<Pair<uint, string>> newSkills)
		: base(PLAYER_TRANSFORM_EVENT)
	{
		if (newSkills != null)
		{
			mNewSkillList = new List<Pair<uint, string>>(newSkills);
		}
	}
}
