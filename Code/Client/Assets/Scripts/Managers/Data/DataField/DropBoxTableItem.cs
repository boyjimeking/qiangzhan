using UnityEngine;
using System.Collections;

public class DropBoxTableItem
{
	public static uint MAX_ITEM_NUM = 10;

	// 掉落包Id
    public int id;

	// 备注
	public string desc;

	// 道具ID(1)还是ConditionId(0)
	public int isItemId;

	// 随机类型(0:单独随机 1:加权随机)
	public int randomType;

	// 最多掉几个
	public int maxnum;

	// 奖励Id
	public int itemid0;
	public int itemid1;
	public int itemid2;
	public int itemid3;
	public int itemid4;
	public int itemid5;
	public int itemid6;
	public int itemid7;
	public int itemid8;
	public int itemid9;

	// 个数
	public int itemnum0;
	public int itemnum1;
	public int itemnum2;
	public int itemnum3;
	public int itemnum4;
	public int itemnum5;
	public int itemnum6;
	public int itemnum7;
	public int itemnum8;
	public int itemnum9;

	// 权重
	public int itemweight0;
	public int itemweight1;
	public int itemweight2;
	public int itemweight3;
	public int itemweight4;
	public int itemweight5;
	public int itemweight6;
	public int itemweight7;
	public int itemweight8;
	public int itemweight9;
}
