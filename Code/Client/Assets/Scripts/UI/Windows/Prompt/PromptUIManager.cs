using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class PromptNode
{
    public bool mUsed = false;
    public bool mOverUpdate = false;
    public int mOverTime = 0;
    public UILabel mLabel = null;
    private static Vector3 pressed = new Vector3(0f, 200f);

    private TweenPosition posCom = null;
    private TweenAlpha alphaCom = null;

    private GameObject mObj = null;
    public PromptNode(GameObject obj)
    {
        mObj = obj;
        mLabel = mObj.GetComponent<UILabel>();
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
    public void Start(string txt,int time = 1500)
    {
        NGUITools.SetActive(mObj.gameObject, true);
        mUsed = true;
        mOverUpdate = true;
        mOverTime = time;
        mLabel.alpha = 1.0f;
        mLabel.text = txt;
    }
    public void Update(uint elapsed)
    {
        if (!mUsed)
            return;

        if (mOverUpdate)
        {
            mOverTime -= (int)elapsed;

            if (mOverTime <= 0)
            {
                End();
            }
        }
    }

    public void End()
    {
        mOverUpdate = false;

        PromptUIManager.Instance.RemoveAndSort(this);

        Vector3 targetPos = mObj.transform.localPosition + pressed;

        posCom = TweenPosition.Begin(mObj.gameObject, 1.0f, targetPos);
        posCom.method = UITweener.Method.EaseInOut;

        alphaCom = TweenAlpha.Begin(mObj.gameObject, 1.0f, 0.1f);
        alphaCom.method = UITweener.Method.EaseInOut;

        EventDelegate.Add(posCom.onFinished, onFinished);
    }
    void onFinished()
    {
        EventDelegate.Remove(posCom.onFinished, onFinished);
        mUsed = false;
        NGUITools.SetActive(mObj.gameObject, false);

        PromptUIManager.Instance.FreeUI(this);

        alphaCom.enabled = false;
    }
}

//提示文字
public class PromptUIManager
{
    private static int MAX_SHOW = 2;

    //显示列表
    private List<PromptNode> mShowList = new List<PromptNode>();

    //缓存列表
    private Queue mCacheQueue = new Queue();

    private static PromptUIManager sInstance = null;

    private Vector2 mSrcPos = Vector2.zero;

    public static PromptUIManager Instance
    {
        get
        {
            return sInstance;
        }
    }

    public PromptUIManager()
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


    public void AddNewPrompt( string txt )
    {
        if( string.IsNullOrEmpty(txt) )
        {
            return;
        }

        if( mShowList.Count > 0 )
        {
            PromptNode tmp = mShowList[mShowList.Count - 1] as PromptNode;
            if( tmp != null && txt.Equals(tmp.GetText()) )
            {
                tmp.Start(txt);
                return;
            }
        }

        if( mShowList.Count >= MAX_SHOW )
        {
            mShowList[0].End();
        }

        PromptNode node = null;
        if (mCacheQueue.Count > 0)
        {
            node = mCacheQueue.Dequeue() as PromptNode;
        }
        else
        {
            //等待新机制修改
            GameObject clone = WindowManager.Instance.CloneCommonUI("PromptUI");
            GameObject.DontDestroyOnLoad(clone);
            WindowManager.Instance.SetDepth(clone, 1000, true);
            node = new PromptNode(clone);
        }

        float y = mSrcPos.y;
        for (int i = 0; i < mShowList.Count; ++i )
        {
            PromptNode _node = mShowList[i] as PromptNode;
            y -= _node.GetHeight();
        }
        node.SetPos(new Vector2(mSrcPos.x, y));
        node.Start(txt);

        mShowList.Add(node);
    }

    public void RemoveAndSort(PromptNode node)
    {
        if( mShowList.Count > 0 )
        {
            mShowList.Remove(node);
        }

        for (int i = 0; i < mShowList.Count; ++i)
        {
            PromptNode _node = mShowList[i] as PromptNode;
            _node.SetPos(new Vector2(mSrcPos.x, mSrcPos.y - (i * _node.GetHeight() )));
        }
    }
    public void FreeUI(PromptNode node)
    {
        mCacheQueue.Enqueue(node);
    }
    public void Destory()
    {

    }

    public void Update(uint elapsed)
    {
        for (int i = 0; i < mShowList.Count; ++i)
        {
            PromptNode _node = mShowList[i] as PromptNode;
            _node.Update(elapsed);
        }
    }

}
