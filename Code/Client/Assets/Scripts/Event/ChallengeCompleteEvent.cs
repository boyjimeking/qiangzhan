
using System.Collections.Generic;


public class ChallengeCompleteParam
{
    //层数
    public int mFloor;
    /// <summary>
    /// 掉落列表
    /// </summary>
    public List<DropItemParam> mDrops = new List<DropItemParam>();
    //分数
    public int mScore;
    public bool mAchieveOne;
    public bool mAchieveTwo;
    public bool mAchieveThree;
    public int mHistortyScore;
    public bool isFirstTime = false;
    //是否激活配件
    public bool isHasFitting = false;
}
public class ChallengeCompleteEvent:EventBase
{
    public const string CHALLENGE_COMPLETE = "CHALLENGE_COMPLETE";
    public ChallengeCompleteParam mParam;
    public ChallengeCompleteEvent(string eventName,ChallengeCompleteParam param) : base(eventName)
    {
        mParam = param;
    }
}
    

