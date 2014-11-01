using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class PopTipNode
{
    public bool mUsed = false;
    public UILabel mLabel = null;
    private UIPanel mPanel = null;
    private static Vector3 pressed = new Vector3(0f, 200f);

    private GameObject mObj = null;

    
    private bool mWaittingMoved = false;
    public int mWaitingTime = 0;

    private bool mMoved = false;
    public int mMoveTime = 0;


    public Vector3 mTargetPos = Vector3.zero;

    public PopTipNode(GameObject obj)
    {
        mObj = obj;
        mLabel = ObjectCommon.GetChildComponent<UILabel>(mObj, "Sprite/Label");
        mPanel = mObj.GetComponent<UIPanel>();
    }
    public void SetPos(Vector2 pos)
    {
        if (mObj == null)
        {
            return;
        }
        Vector3 _pos = WindowManager.current2DCamera.ScreenToWorldPoint(pos);
        mObj.transform.position = _pos;
    }   
    public string GetText()
    {
        return mLabel.text;
    }
    public int GetHeight()
    {
        return mLabel.height;
    }

    public void Moved()
    {
        if (mMoved)
            return;
        mMoved = true;
        mWaittingMoved = false;
        mMoveTime = 1500;
        mTargetPos = mObj.transform.localPosition;

        PopTipManager.Instance.OnMoved(this);
    }
    public void Start(string txt,int time = 1500)
    {
        NGUITools.SetActive(mObj.gameObject, true);
        mUsed = true;
        mWaittingMoved = true;
        mWaitingTime = time;
        mPanel.alpha = 1.0f;
        mLabel.text = txt;
    }
    public void Update(uint elapsed)
    {
        if (!mUsed)
            return;

        if (mWaittingMoved)
        {
            mWaitingTime -= (int)elapsed;
            if (mWaitingTime <= 0)
            {
                Moved();
            }
        }

        if( mMoved )
        {
            mMoveTime -= (int)elapsed;

            mPanel.alpha = (float)mMoveTime / 1500.0f;

            mTargetPos.y += (float)elapsed * 200.0f / 1500.0f;
            mObj.transform.localPosition = mTargetPos;

            if( mMoveTime <= 0 )
            {
                OnEnd();
            }
        }
    }
    void OnEnd()
    {
        mUsed = false;
        mMoved = false;
        mWaittingMoved = false;
        NGUITools.SetActive(mObj.gameObject, false);

        PopTipManager.Instance.FreeUI(this);
    }
}

//提示文字
public class PopTipManager
{

    private List<PopTipNode> mAllList = new List<PopTipNode>();

    private List<PopTipNode> mCacheList = new List<PopTipNode>();
    //缓存列表
    private Queue mCacheQueue = new Queue();

    private static PopTipManager sInstance = null;

    private Vector2 mSrcPos = Vector2.zero;

   // private PopTipNode mLastPopTip = null;

    private static int MAX_CACHE_SIZE = 5;
    private float mOffsetY = 0.0f;

    public static PopTipManager Instance
    {
        get
        {
            return sInstance;
        }
    }

    public PopTipManager()
    {
        sInstance = this;

        UICamera.onScreenResize += onScreenResize;

        onScreenResize();
    }

    private void onScreenResize()
    {
        mSrcPos.x = Screen.width / 2.0f;
        mSrcPos.y = Screen.height / 2.0f + 200;
    }


    public void AddNewTip( string txt , float offsetY = 0.0f)
    {
        if( string.IsNullOrEmpty(txt) )
        {
            return;
        }

        PopTipNode node = null;
        if (mCacheQueue.Count > 0)
        {
            node = mCacheQueue.Dequeue() as PopTipNode;
        }
        else
        {
            //等待新机制修改
            GameObject clone = WindowManager.Instance.CloneCommonUI("PopTipUI");
            GameObject.DontDestroyOnLoad(clone);
            WindowManager.Instance.SetDepth(clone, 1000, true);
            node = new PopTipNode(clone);
            mAllList.Add(node);
        }

        if (mCacheList.Count >= MAX_CACHE_SIZE)
        {
            PopTipNode lastNode = mCacheList[0] as PopTipNode;
            if( lastNode != null && lastNode.mUsed )
            {
                lastNode.Moved();
            }
        }

        node.Start(txt);

        mCacheList.Add(node);

//         if( mLastPopTip != null && mLastPopTip.mUsed )
//         {
//             mLastPopTip.Moved();
//         }

//        node.SetPos(new Vector2(mSrcPos.x, offsetY + mSrcPos.y - (float)Screen.height / 4.0f));
//        node.Start(txt);

//        mLastPopTip = node;
        SortCaches();
    }

    /// <summary>
    /// 获取物品的提示;
    /// </summary>
    /// <param name="itemid"></param>
    /// <param name="count"></param>
    /// <param name="offsetY"></param>
    public void AddGetItemTip(int itemid, int count = 1, float offsetY = 0.0f)
    {
        string content = StringHelper.GetString("egg_get_item") + ItemManager.getItemNameWithColor(itemid) + " X " + count;
        PopTipManager.Instance.AddNewTip(content, offsetY);
    }

    public void SetOffsetY(float offset)
    {
        mOffsetY = offset;
        SortCaches();
    }

    private void SortCaches()
    {
        float y = mOffsetY + mSrcPos.y - (float)Screen.height / 3.0f;
        float offset = 40.0f;
        for( int i = mCacheList.Count - 1 ; i >=0  ; -- i )
        {
            PopTipNode node = mCacheList[i] as PopTipNode;
            node.SetPos(new Vector2(mSrcPos.x, y ));

            y += offset;
        }
    }

    public void OnMoved(PopTipNode node)
    {
        mCacheList.Remove(node);
        SortCaches();
    }
    public void FreeUI(PopTipNode node)
    {
        mCacheQueue.Enqueue(node);
    }

    public void Update(uint elapsed)
    {
        for (int i = 0; i < mAllList.Count; ++i)
        {
            PopTipNode _node = mAllList[i] as PopTipNode;
            _node.Update(elapsed);
        }
    }

}
