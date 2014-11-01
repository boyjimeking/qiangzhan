using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Linq;
using Message;


public enum PackExtendNum:int
{
    MAX_PACK_EXTEND_NUM = 9,
}

public class PackInfo
{
    public int max_vaild_number;        //当前有效格子数
    public int max_number;              //最大格子数 ..  

    public PackInfo()
    {
        max_vaild_number = 0;
        max_number = 0;
    }
}

//包裹非单例类
public class PackageManager
{
    //背包道具数据
    private ArrayList mPacks = new ArrayList();

    //背包属性
    private ArrayList mPackInfos = new ArrayList();

    public PackageManager()
    {
        Clear();
    }

    public void Clear()
    {
        mPacks.Clear();
        mPackInfos.Clear();

        //初始化缓存池子
        for (int i = 0; i < (int)PackageType.Pack_Max; ++i)
        {
            mPacks.Add(new Dictionary<int, ItemObj>());

            mPackInfos.Add(new PackInfo());
        }
    }

    public void SyncPackBag(PackageType bagType , bag_info baginfo)
    {
        for (int i = 0; i < baginfo.items.Count; i++)
        {
            bag_item_info slot = baginfo.items[i];

            ItemObj item = ItemManager.Instance.CreateItem(slot.item);
            if (item == null)
                continue;

            PutItem(item, bagType, slot.pos);
        }

        SyncPackInfo(bagType, baginfo.max_valid_size, baginfo.max_size);
    }

    public void SyncPackInfo(PackageType packageType , int maxValidNumber , int maxNumber)
    {
        if (packageType <= PackageType.Invalid || packageType >= PackageType.Pack_Max)
            return;
        PackInfo info = mPackInfos[(int)packageType] as PackInfo;
        info.max_vaild_number = maxValidNumber;
        info.max_number = maxNumber;

        ItemEvent evt = new ItemEvent(ItemEvent.UPDATE_CHANGE);
        evt.bagType = packageType;
        EventSystem.Instance.PushEvent(evt);
    }

    public void SyncDiffItem(role_bag_diff_info diff_info)
    {
        if (diff_info.bagType <= (int)PackageType.Invalid || diff_info.bagType >= (int)PackageType.Pack_Max)
        {
            return;
        }
        ItemObj itemObj = ItemManager.Instance.CreateItem(diff_info.item.item);
        if (itemObj != null)
        {
            PutItem(itemObj, (PackageType)diff_info.bagType, diff_info.item.pos);
        }
        else
        {
            RemoveItem((PackageType)diff_info.bagType, diff_info.item.pos);
        }

        ItemEvent evt = new ItemEvent(ItemEvent.UPDATE_CHANGE);
        evt.bagType = (PackageType)diff_info.bagType;
        evt.itemId = (itemObj != null) ? itemObj.GetResId() : -1;
        evt.pos = diff_info.item.pos;
        EventSystem.Instance.PushEvent(evt);
    }

    private void PutItem(ItemObj item, PackageType packageType, int pos/*, bool hint*/)
    {
        if (packageType <= PackageType.Invalid || packageType >= PackageType.Pack_Max)
            return;

        item.PackType = packageType;
        item.PackPos = pos;

        //if(hint)
        //{
        //    string content = StringHelper.GetString("egg_get_item") + ItemManager.getItemNameWithColor(item.GetResId()) + " X " + item.GetCount();
        //    PopTipManager.Instance.AddNewTip(content);
        //}

        Dictionary<int, ItemObj> pack = mPacks[(int)packageType] as Dictionary<int, ItemObj>;
        
        if( !pack.ContainsKey(pos) )
        {
            pack.Add(pos, item);
        }
        else
        {
            pack[pos] = item;
        }
    }

    private void RemoveItem(PackageType pack , int pos)
    {
        if (pack <= PackageType.Invalid || pack >= PackageType.Pack_Max)
            return;

        Dictionary<int, ItemObj> package = mPacks[(int)pack] as Dictionary<int, ItemObj>;

        if (package.ContainsKey(pos))
        {
            package.Remove(pos);
        }
    }

    public ItemObj GetItemByIDAndPos(int resId, int pos, PackageType packageType = PackageType.Pack_Bag)
    {
        if (packageType <= PackageType.Invalid || packageType >= PackageType.Pack_Max)
            return null;
        Dictionary<int, ItemObj> pack = mPacks[(int)packageType] as Dictionary<int, ItemObj>;
        if (null == pack)
            return null;

        foreach (KeyValuePair<int, ItemObj> item in pack)
        {
            if (item.Value != null && item.Value.GetResId() == resId && item.Value.PackPos == pos)
                return item.Value;
        }
        return null;
    }

    public uint GetItemNumByIDAndPos(int resId, int pos, PackageType packageType = PackageType.Pack_Bag)
    {
        if (packageType <= PackageType.Invalid || packageType >= PackageType.Pack_Max)
            return 0;
        Dictionary<int, ItemObj> pack = mPacks[(int)packageType] as Dictionary<int, ItemObj>;
        if (null == pack)
            return 0;

        foreach (KeyValuePair<int, ItemObj> item in pack)
        {
            if (item.Value != null && item.Value.GetResId() == resId && item.Value.PackPos == pos)
                return item.Value.GetCount();
        }

        return 0;
    }

    public ItemObj GetItemByPos(int pos, PackageType packageType = PackageType.Pack_Bag)
    {
        if (packageType <= PackageType.Invalid || packageType >= PackageType.Pack_Max)
            return null;
        Dictionary<int, ItemObj> pack = mPacks[(int)packageType] as Dictionary<int, ItemObj>;
        if (null == pack)
            return null;

        foreach (KeyValuePair<int, ItemObj> item in pack)
        {
            if (item.Value != null && item.Value.PackPos == pos)
                return item.Value;
        }
        return null;
    }

    public ItemObj GetItemByID(int resId, PackageType pack = PackageType.Pack_Bag)
    {
        ItemObj item = null;
        if (pack == PackageType.Invalid)
        {
            item = GetItemByIDSinglePackage(resId, PackageType.Pack_Bag);
            if (item != null)
                return item;
            item = GetItemByIDSinglePackage(resId, PackageType.Pack_Equip);
            if (item != null)
                return item;
            item = GetItemByIDSinglePackage(resId, PackageType.Pack_Weapon);
            if (item != null)
                return item;
            item = GetItemByIDSinglePackage(resId, PackageType.Pack_Gem);
            if (item != null)
                return item;
        }
        else
        {
            item = GetItemByIDSinglePackage(resId, pack);
        }
        return item;
    }

    public uint GetNumByID(int resId, PackageType pack = PackageType.Invalid)
    {
        uint cnt = 0;
        if (pack == PackageType.Invalid)
        {
            cnt += GetNumByIDSinglePackage(resId, PackageType.Pack_Bag);
            cnt += GetNumByIDSinglePackage(resId, PackageType.Pack_Equip);
            cnt += GetNumByIDSinglePackage(resId, PackageType.Pack_Weapon);
            cnt += GetNumByIDSinglePackage(resId, PackageType.Pack_Gem);
        }
        else
        {
            cnt = GetNumByIDSinglePackage(resId, pack);
        }

        return cnt;
    }

    private ItemObj GetItemByIDSinglePackage(int resId, PackageType pack)
    {
        if (pack <= PackageType.Invalid || pack >= PackageType.Pack_Max)
            return null;

        Dictionary<int, ItemObj> package = mPacks[(int)pack] as Dictionary<int, ItemObj>;
        foreach (KeyValuePair<int, ItemObj> item in package)
        {
            if (item.Value != null && item.Value.GetResId() == resId)
                return item.Value;
        }
        return null;
    }

    private uint GetNumByIDSinglePackage(int resId, PackageType pack)
    {
        if (pack <= PackageType.Invalid || pack >= PackageType.Pack_Max)
            return 0;
        Dictionary<int, ItemObj> package = mPacks[(int)pack] as Dictionary<int, ItemObj>;

        uint cnt = 0;
        foreach (KeyValuePair<int, ItemObj> item in package)
        {
            if (item.Value != null && item.Value.GetResId() == resId)
                cnt += item.Value.GetCount();
        }
        return cnt;
    }

    public Dictionary<int, ItemObj> getPackDic(PackageType pack)
    {
        if (pack <= PackageType.Invalid || pack >= PackageType.Pack_Max)
            return null;
        return mPacks[(int)pack] as Dictionary<int, ItemObj>;
    }


    public int GetPackMaxSize(PackageType pack)
    {
        if (pack <= PackageType.Invalid || pack >= PackageType.Pack_Max)
            return 0;
        if (mPackInfos.Contains(pack))
            return 0;
        PackInfo info = mPackInfos[(int)pack] as PackInfo;
        return info.max_number;
    }
    public int GetPackMaxVaildSize(PackageType pack)
    {
        if (pack <= PackageType.Invalid || pack >= PackageType.Pack_Max)
            return 0;
        if (mPackInfos.Contains(pack))
            return 0;
        PackInfo info = mPackInfos[(int)pack] as PackInfo;
        return info.max_vaild_number;
    }
}
