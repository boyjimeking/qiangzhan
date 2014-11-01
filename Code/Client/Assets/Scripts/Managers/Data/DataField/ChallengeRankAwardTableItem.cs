using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AwardRange
{
    public int itemid;
}
public class ChallengeRankAwardTableItem
{
   public static readonly uint RANK_COUNT = 10;
    public int id;
    public string desc;
    public uint max_score;
    public uint min_score;
    public AwardRange[] rank_list = new AwardRange[RANK_COUNT];

}

