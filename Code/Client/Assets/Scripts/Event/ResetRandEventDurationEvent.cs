using UnityEngine;
using System.Collections;

/// <summary>
/// 随机事件被重置时间的事件通知.
/// </summary>
public class ResetRandEventDurationEvent : EventBase
{
	static public string RESET_RAND_EVENT_DURATION = "RESET_RAND_EVENT_DURATION";
	public uint OwnerInstanceID { get; private set; }
	public uint BuffID { get; private set; }
	public ResetRandEventDurationEvent(uint ownerID, uint buffID)
		: base(RESET_RAND_EVENT_DURATION)
	{
		OwnerInstanceID = ownerID;
		BuffID = buffID;
	}
}
