using UnityEngine;
using System.Collections;

// 竞技场
public class ArenaRandomTableItem
{
	// ID
	public int mId;

	// 描述
	public string mDesc;

	// 最高排名
	public uint mHighestRank;

	// 最低排名
	public uint mLowestRank;

	// 强区间
	public int mHigherLeft;

	// 强区间
	public int mHigherRight;

	// 平区间
	public int mSameLeft;

	// 平区间
	public int mSameRight;

	// 弱区间
	public int mLowerLeft;

	// 弱区间
	public int mLowerRight;
}
