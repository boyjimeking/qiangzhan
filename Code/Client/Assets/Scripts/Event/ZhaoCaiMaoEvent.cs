using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZhaoCaiMaoUpdateRankListEvent : EventBase
{
	public static string ZHAOCAIMAO_UPDATE_RANKLIST_EVENT = "ZHAOCAIMAO_UPDATE_RANKLIST_EVENT";

	public List<Message.zhaocaimao_ranklist_role_info> sortInfo = new List<Message.zhaocaimao_ranklist_role_info>();

	public ZhaoCaiMaoUpdateRankListEvent()
		: base(ZHAOCAIMAO_UPDATE_RANKLIST_EVENT)
	{

	}
}

public class ZhaoCaiMaoUpdateDamageEvent : EventBase
{
	public static string ZHAOCAIMAO_UPDATE_DAMAGE_EVENT = "ZHAOCAIMAO_UPDATE_DAMAGE_EVENT";
	public int mDamage;

	public ZhaoCaiMaoUpdateDamageEvent(int damage)
		: base(ZHAOCAIMAO_UPDATE_DAMAGE_EVENT)
	{
		mDamage = damage;
	}
}
