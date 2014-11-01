using UnityEngine;
using System.Collections;

// 排位赛奖励
public class QualifyingAwardTableItem
{
    // ID
    public int mId;

    // 描述
    public string mDesc;

	// 最高排名
	public uint mHighestRank;

	// 最低排名
	public uint mLowestRank;

	// 声望
	public uint mAwardPrestige;

	// 金币
	public uint mAwardGold;
}
