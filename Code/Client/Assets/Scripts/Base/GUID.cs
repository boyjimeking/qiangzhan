using UnityEngine;
using System.Collections;
using Message;
public class GUID
{
    public uint low = uint.MaxValue;
    public uint high = uint.MaxValue;

    public static implicit operator GUID(MSGGuild msgGuid)
    {
        return new GUID(msgGuid.low, msgGuid.high);
    }

    public MSGGuild ToMSGGuid()
    {
        MSGGuild msgGuid = new MSGGuild();
        msgGuid.low = this.low;
        msgGuid.high = this.high;
        return msgGuid;
    }
    public GUID()
    {

    }

    public GUID(uint _low , uint _high)
    {
        this.low = _low;
        this.high = _high;
    }

    public bool IsValid()
    {
        return (low != uint.MaxValue && high != uint.MaxValue);
    }

    public ulong ToULong()
    {
        ulong temp = high;
        return (temp << 32) | low;
    }

	public string ToString()
	{
		return high.ToString() + low.ToString();
	}
}
