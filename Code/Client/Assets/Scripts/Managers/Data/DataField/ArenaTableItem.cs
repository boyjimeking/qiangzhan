using UnityEngine;
using System.Collections;

// 竞技场
public class ArenaTableItem
{
    // ID
    public int mId;

    // 描述
    public string mDesc;

	// 最高排名
	public uint mHighestRank;

	// 最低排名
	public uint mLowestRank;

	// 分段
	public uint mRankLevel;

	// 胜高档积分
	public int mScoreWinHigher;

	// 胜同档积分
	public int mScoreWinSame;

	// 胜低档积分
	public int mScoreWinLower;

	// 败高档积分
	public int mScoreLoseHigher;

	// 败同等积分
	public int mScoreLoseSame;

	// 败低档积分
	public int mScoreLoseLower;

	// 胜高档竞技币
	public int mMoneyWinHigher;

	// 胜同档竞技币
	public int mMoneyWinSame;

	// 胜低档竞技币
	public int mMoneyWinLower;

	// 败高档竞技币
	public int mMoneyLoseHigher;

	// 败同档竞技币
	public int mMoneyLoseSame;

	// 败低档竞技币
	public int mMoneyLoseLower;
}
