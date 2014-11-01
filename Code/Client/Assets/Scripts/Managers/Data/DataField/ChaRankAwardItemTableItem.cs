using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ChallengeAwardItem
{
    public int itemid;
    public int itemnum;

}
public class ChaRankAwardItemTableItem
{
    public static readonly uint AWARD_COUNT = 4;
    public int id;
    public ChallengeAwardItem[] awardItems = new ChallengeAwardItem[AWARD_COUNT];
}

