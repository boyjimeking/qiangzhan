using UnityEngine;
using System.Collections;

public class EggTableItem 
{
    public int resId;
    
    // 物品ID;
    public int itemId;

    // 是否显示在下面列表中;
    public int isShow;

    // 掉落最小数量;
    public int minNum;

    // 掉落最大数量;
    public int maxNum;

    // 在普通蛋中的掉落概率;
    public int prob1;

    // 在高级蛋中的掉落概率;
    public int prob2;

    // 在钻石蛋中的掉落概率;
    public int prob3;

    // 描述，策划用;
    public string detail;
}
