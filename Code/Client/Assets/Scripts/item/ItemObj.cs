using UnityEngine;
using System.Collections;
using System;

public class ItemObjInit
{
    public ulong mItemGuid;
    public int mResId;
    public ushort mCount;
    public long mCreateTime;
};

public class ItemObj
{
    //这2个数据用于索引道具位置 
    private PackageType mPackType = PackageType.Invalid;
    private int mPackPos = -1;

    protected ulong mGuid;
    protected int mResId;
    protected ushort mCount  = 1;
    protected long mCreateTime;

    public ItemObj()
    {
        Dispose();
    }

    public PackageType PackType
    {
        get
        {
            return mPackType;
        }
        set
        {
            mPackType = value;
        }
    }
    public int PackPos
    {
        get
        {
            return mPackPos;
        }
        set
        {
            mPackPos = value;
        }
    }

    //服务器数据信息,临时数据
    virtual public bool Init(ItemObjInit initData)
    {
        if (initData == null)
            return false;

        mGuid = initData.mItemGuid;
        mResId = initData.mResId;
        mCount = initData.mCount;
        mCreateTime = initData.mCreateTime;


        return true;
    }

    virtual public bool BuildProperty(PropertyOperation operation)
    {
        return false;
    }

    public void Dispose()
    {
        mGuid = ulong.MaxValue;
        mResId = -1;
        mCount = 0;
        mCreateTime = 0;
    }

    public int GetResId()
    {
        return mResId;
    }

    public ulong GetGuid()
    {
        return mGuid;
    }

    public virtual ItemType GetType()
    {
        return ItemType.Invalid;
    }

    public uint GetCount()
    {
        return mCount;
    }

    public long GetCreateTime()
    {
        return mCreateTime;
    }

    public virtual ItemTableItem GetRes()
    {
        return null;
    }
}