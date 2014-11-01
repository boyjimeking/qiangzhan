
using System;
using System.Collections;
using System.Collections.Generic;

public class PoolSlot
{
    public int Index = -1;
    public bool Used = false;

    public PoolSlot PreSlot = null;
    public PoolSlot NextSlot = null;
    public object Content = null;
}

public class Pool
{
    private PoolSlot[] mSlot = null;
    private int mMaxCount = 0;
    private int mCount = 0;

    private PoolSlot mFreeList = null;
    private PoolSlot mUsedList = null;

    public Pool(int maxCount = 1000)
    {
        if (maxCount < 0)
            maxCount = 1000;
 
        mMaxCount = maxCount;

        mSlot = new PoolSlot[mMaxCount];
        for(int i = 0; i < mMaxCount; i++)
        {
            mSlot[i] = new PoolSlot();
            mSlot[i].Index = i;
            mSlot[i].PreSlot = null;
            mSlot[i].NextSlot = null;
            mSlot[i].Content = null;
            mSlot[i].Used = false;

            if(mFreeList != null)
            {
                mFreeList.PreSlot = mSlot[i];
                mSlot[i].NextSlot = mFreeList;
                
                mFreeList = mSlot[i];
            }
            else
            {
                mFreeList = mSlot[i];
            }
        }
    }

    public int Put(object obj)
    {
        if (obj == null)
            return -1;

        if (mFreeList == null)
            return -1;

        PoolSlot useSlot = mFreeList;

        mFreeList = useSlot.NextSlot;
        if(mFreeList != null)
        {
            mFreeList.PreSlot = null;
        }

        useSlot.Used = true;
        useSlot.NextSlot = null;
        useSlot.PreSlot = null;
        useSlot.Content = obj;

        if(mUsedList != null)
        {
            mUsedList.PreSlot = useSlot;
            useSlot.NextSlot = mUsedList;

            mUsedList = useSlot;
        }
        else
        {
            mUsedList = useSlot;
        }

        mCount++;
        return useSlot.Index;
    }

    public bool Remove(int index)
    {
        if (index < 0 || index >= mMaxCount)
            return false;

        if (mSlot[index] == null)
            return false;

        PoolSlot slot = mSlot[index];

        if (!slot.Used)
            return false;

        PoolSlot preSlot = slot.PreSlot;
        PoolSlot nextSlot = slot.NextSlot;

        if(preSlot != null)
        {
            preSlot.NextSlot = nextSlot;
        }

        if(nextSlot != null)
        {
            nextSlot.PreSlot = preSlot;
        }

        if(mUsedList == slot)
        {
            mUsedList = slot.NextSlot;
        }

        slot.Used = false;
        slot.NextSlot = null;
        slot.PreSlot = null;
        slot.Content = null;

        if (mFreeList != null)
        {
            mFreeList.PreSlot = slot;
            slot.NextSlot = mFreeList;

            mFreeList = slot;
        }
        else
        {
            mFreeList = slot;
        }

        mCount--;
        return true;
    }

    public object Get(int index)
    {
        if (index < 0 || index >= mMaxCount)
            return null;

        return mSlot[index].Content; 
    }

    public int GetMaxCount()
    {
        return mMaxCount;
    }

    public int GetCount()
    {
        return mCount;
    }

    public void Clear()
    {
        mFreeList = null;
        mUsedList = null;

        for(int i = 0; i < mMaxCount; i++)
        {
            mSlot[i].Index = i;
            mSlot[i].PreSlot = null;
            mSlot[i].NextSlot = null;
            mSlot[i].Content = null;
            mSlot[i].Used = false;

            if (mFreeList != null)
            {
                mFreeList.PreSlot = mSlot[i];
                mSlot[i].NextSlot = mFreeList;

                mFreeList = mSlot[i];
            }
            else
            {
                mFreeList = mSlot[i];
            }
        }
    }

    public PoolSlot UsedList()
    {
        return mUsedList;
    }
};