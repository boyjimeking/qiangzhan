using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DropBoxItem
{
	public int itemid;
	public int itemnum;
	public int itemweight;
	public bool isItemOrCondition;
}

public class DropManager
{
	private class DropGroup
	{
		public int id;
		public ArrayList items = null;
	}

	private class DropGroupItem
	{
		public int dropboxid;
		public int dropboxweight;
	}

	private class DropBox
	{
		public int id;
		public string desc;
		public ArrayList items = null;
	}

    private static DropManager instance;

	// 总权重10000
	public static int MAX_WEIGHT = 10000;

	// 掉落包
	private Dictionary<int, DropBox> mDropBoxes = null;

	// 掉落组
	private Dictionary<int, DropGroup> mDropGroups = null;

	public DropManager()
	{
		instance = this;
	}

	public static DropManager Instance
	{
		get
		{
			return instance;
		}
	}

	// 初始化
	public void InitDataStruct()
	{
		mDropGroups = new Dictionary<int, DropGroup>();
		System.Type grouptype = typeof(DropGroupTableItem);
        IDictionaryEnumerator itr = DataManager.DropGroupTable.GetEnumerator();
        while (itr.MoveNext())
        {
            DropGroupTableItem res = itr.Value as DropGroupTableItem;
            DropGroup dropgroup = new DropGroup();
            dropgroup.items = new ArrayList();
            dropgroup.id = res.id;

            for (int i = 0; i < DropGroupTableItem.MAX_ITEM_NUM; ++i)
            {
                System.Reflection.FieldInfo fieldid = grouptype.GetField("dropBoxId" + i.ToString());
                int dropboxid = (int)fieldid.GetValue(res);

                System.Reflection.FieldInfo fieldweight = grouptype.GetField("dropBoxWeight" + i.ToString());
                int dropboxweight = (int)fieldweight.GetValue(res);

                if (dropboxid < 0 || dropboxweight < 1)
                {
                    break;
                }

                DropGroupItem item = new DropGroupItem();
                item.dropboxid = dropboxid;
                item.dropboxweight = dropboxweight;

                dropgroup.items.Add(item);
            }

            mDropGroups.Add(res.id, dropgroup);
        }
// 		foreach (DropGroupTableItem res in DataManager.DropGroupTable.Values)
// 		{
// 			DropGroup dropgroup = new DropGroup();
// 			dropgroup.items = new ArrayList();
// 			dropgroup.id = res.id;
// 
// 			for (int i = 0; i < DropGroupTableItem.MAX_ITEM_NUM; ++i)
// 			{
// 				System.Reflection.FieldInfo fieldid = grouptype.GetField("dropBoxId" + i.ToString());
// 				int dropboxid = (int)fieldid.GetValue(res);
// 
// 				System.Reflection.FieldInfo fieldweight = grouptype.GetField("dropBoxWeight" + i.ToString());
// 				int dropboxweight = (int)fieldweight.GetValue(res);
// 
// 				if (dropboxid < 0 || dropboxweight < 1)
// 				{
// 					break;
// 				}
// 
// 				DropGroupItem item = new DropGroupItem();
// 				item.dropboxid = dropboxid;
// 				item.dropboxweight = dropboxweight;
// 
// 				dropgroup.items.Add(item);
// 			}
// 
// 			mDropGroups.Add(res.id, dropgroup);
// 		}

		mDropBoxes = new Dictionary<int, DropBox>();

		System.Type type = typeof(DropBoxTableItem);
        itr = DataManager.DropBoxTable.GetEnumerator();
        while (itr.MoveNext())
        {
            DropBoxTableItem res = itr.Value as DropBoxTableItem;
            DropBox dropbox = new DropBox();
            dropbox.items = new ArrayList();
            dropbox.id = res.id;
            dropbox.desc = res.desc;

            for (int i = 0; i < DropBoxTableItem.MAX_ITEM_NUM; ++i)
            {
                System.Reflection.FieldInfo fieldid = type.GetField("itemid" + i.ToString());
                int itemid = (int)fieldid.GetValue(res);

                System.Reflection.FieldInfo fieldnum = type.GetField("itemnum" + i.ToString());
                int itemnum = (int)fieldnum.GetValue(res);

                System.Reflection.FieldInfo fieldweight = type.GetField("itemweight" + i.ToString());
                int itemweight = (int)fieldweight.GetValue(res);

                if (itemid < 0 || itemnum < 1 || itemweight < 1)
                {
                    break;
                }

                DropBoxItem item = new DropBoxItem();
                item.itemid = itemid;
                item.itemnum = itemnum;
                item.itemweight = itemweight;
                item.isItemOrCondition = res.isItemId > 0;

                dropbox.items.Add(item);
            }

            mDropBoxes.Add(res.id, dropbox);
        }
// 		foreach(DropBoxTableItem res in DataManager.DropBoxTable.Values)
// 		{
// 			
// 		}
	}

	/// <summary>
	/// 随机生成掉落道具ID。从dropgroup开始随机
	/// </summary>
	/// <param name="dropgroupId">dropgroup.txt的Id</param>
	/// <param name="droplist">输出生成的DropItem列表</param>
	/// <param name="dropnum">需要生成几个道具，-1表示不限制个数，按表规则掉落</param>
	/// <returns></returns>
	public bool GenerateDropGroup(int dropgroupId, out ArrayList droplist, int dropnum = -1)
	{
		droplist = null;

		if (!mDropGroups.ContainsKey(dropgroupId) || !DataManager.DropGroupTable.ContainsKey(dropgroupId))
		{
			return false;
		}

		DropGroupTableItem res = DataManager.DropGroupTable[dropgroupId] as DropGroupTableItem;

		DropGroup group = mDropGroups[dropgroupId] as DropGroup;
		if (group == null)
		{
			return false;
		}

		droplist = new ArrayList();
		ArrayList dropboxlist = new ArrayList();

		if (dropnum > 0)
		{// 在dropnum次随机中 不允许重复掉落 在掉落组中整体随机

			while (dropboxlist.Count < dropnum)
			{
				int rand = Random.Range(0, MAX_WEIGHT);
				int weight = 0;
				for (int i = 0; i < group.items.Count; ++i)
				{
					DropGroupItem item = group.items[i] as DropGroupItem;

					if ((item.dropboxweight + weight) > rand)
					{
						if (!dropboxlist.Contains(item))
						{
							dropboxlist.Add(item);
						}

						break;
					}

					weight += item.dropboxweight;
				}
			}
		}
		else
		{// 未指定掉落个数 不允许重复掉落 每个道具单独随机

			int maxnum = res.maxnum;
			if (maxnum > group.items.Count)
			{
				maxnum = group.items.Count;
			}

			for (int i = 0; i < group.items.Count; ++i)
			{
				int rand = Random.Range(0, MAX_WEIGHT);
				DropGroupItem item = group.items[i] as DropGroupItem;
				if ((item.dropboxweight) > rand)
				{
					if (!dropboxlist.Contains(item))
					{
						dropboxlist.Add(item);
					}
				}

				if (dropboxlist.Count >= maxnum)
				{
					break;
				}
			}
		}

		ArrayList genlist = new ArrayList();
		foreach(DropGroupItem groupres in dropboxlist)
		{
			genlist.Clear();
			if(!GenerateDropBox(groupres.dropboxid, out genlist))
			{
				continue;
			}

			foreach(DropBoxItem item in genlist)
			{
				droplist.Add(item);
			}
		}

		return true;
	}

	/// <summary>
	/// 随机生成掉落道具ID。可以直接调该方法，在掉落包内随机掉落道具，不需要在掉落包间随机。
	/// </summary>
	/// <param name="dropboxId">dropbox.txt的Id</param>
	/// <param name="droplist">输出生成的DropItem列表</param>
	/// <param name="dropnum">需要生成几个道具，-1表示不限制个数，按表规则掉落</param>
	/// <returns></returns>
	public bool GenerateDropBox(int dropboxId, out ArrayList droplist, int dropnum = -1)
	{
		droplist = null;

		if(!mDropBoxes.ContainsKey(dropboxId) || !DataManager.DropBoxTable.ContainsKey(dropboxId))
		{
			return false;
		}

		DropBoxTableItem res = DataManager.DropBoxTable[dropboxId] as DropBoxTableItem;

		DropBox box = mDropBoxes[dropboxId] as DropBox;
		if(box == null)
		{
			return false;
		}

		droplist = new ArrayList();

		if (dropnum > 0)
		{// 在dropnum次随机中 不允许重复掉落 在掉落组中整体随机

			while (droplist.Count < dropnum)
			{
				int rand = Random.Range(0, MAX_WEIGHT);
				int weight = 0;
				for (int i = 0; i < box.items.Count; ++i)
				{
					DropBoxItem item = box.items[i] as DropBoxItem;

					if ((item.itemweight + weight) > rand)
					{
						if (!droplist.Contains(item))
						{
							droplist.Add(item);
						}

						break;
					}

					weight += item.itemweight;
				}
			}
		}
		else
		{// 未指定掉落个数 不允许重复掉落 每个道具单独随机

			int maxnum = res.maxnum;
			if (maxnum > box.items.Count)
			{
				maxnum = box.items.Count;
			}

			for (int i = 0; i < box.items.Count; ++i)
			{
				int rand = Random.Range(0, MAX_WEIGHT);
				DropBoxItem item = box.items[i] as DropBoxItem;
				if ((item.itemweight) > rand)
				{
					if (!droplist.Contains(item))
					{
						droplist.Add(item);
					}
				}

				if (droplist.Count >= maxnum)
				{
					break;
				}
			}
		}

		return true;
	}
}
