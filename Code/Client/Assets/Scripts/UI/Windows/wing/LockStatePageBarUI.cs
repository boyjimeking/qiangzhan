using System.Collections.Generic;
using  UnityEngine;

public class LockStatePageBarUI
{
    public GameObject mView = null;

    private int mMaxPageNum;
    private int mCurPage = 1;
    private int mLockPage = 0;
    private List<UISprite> mPageList;
    private int mCurPageNum;
    private GameObject mPageItem;
    private UIGrid mGrid;
    public LockStatePageBarUI(GameObject view,int maxPageNum,GameObject pageItem)
    {
        mView = view;
        mMaxPageNum = maxPageNum;
        mPageItem = pageItem;
        pageItem.SetActive(false);
        mPageList= new List<UISprite>();
        mGrid = ObjectCommon.GetChildComponent<UIGrid>(mView, "Grid");
        for (int i = 1; i <= mMaxPageNum; ++i)
        {
            GameObject obj = Object.Instantiate(pageItem) as GameObject;
            obj.transform.parent = mGrid.gameObject.transform;
            obj.transform.localScale = Vector3.one;
            mPageList.Add(obj.GetComponent<UISprite>());
            obj.SetActive(false);
        }

        mGrid.repositionNow = true;

    }

    public int CurPageNum
    {
        get { return mCurPageNum; }
        set
        {
            if (mCurPageNum == value) return;
            mCurPageNum = value;
            for (int i = 0; i < mPageList.Count; i++)
            {
                mPageList[i].gameObject.SetActive(i < mCurPageNum);
            }
            mGrid.repositionNow = true;
          
            UpdateUI();
        }
    }

    public int CurPage
    {
        get { return mCurPage; }
        set
        {
            mCurPage = value;
            UpdateUI();
        }
    }

    public int LockPage
    {
        get { return mLockPage; }
        set
        {
            if (mLockPage == value) return;
            mLockPage = value;
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        for (int i = 0; i < mCurPageNum; ++i)
        {
            int pageIndex = i + 1;
            if (pageIndex < mLockPage && mCurPage != pageIndex) //已解锁不是当前页
            {
                UIAtlasHelper.SetSpriteImage(mPageList[i],"wing0:wing0-011");
            }else if (pageIndex < mLockPage && mCurPage == pageIndex)//已解锁是当前页
            {
                UIAtlasHelper.SetSpriteImage(mPageList[i], "wing0:wing0-012");
            }else if (pageIndex >= mLockPage && mCurPage != pageIndex)
            {
                UIAtlasHelper.SetSpriteImage(mPageList[i], "wing0:wing0-014");
            }else if (pageIndex >= mLockPage && mCurPage == pageIndex)
            {
                UIAtlasHelper.SetSpriteImage(mPageList[i], "wing0:wing0-013");
            }
            else
            {
                GameDebug.LogError("设置LockStatePageBarUI图片异常");
            }
        }
    }
}

