using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AnnounceItemNode
{
    public AnnounceItem mUI = null;
    public bool nextFlag = false;
    public bool isend = false;
    public AnnounceItemNode()
    {

    }

    public bool Init()
    {

        //等待新机制修改
        UIWindow AnnounceUI = WindowManager.Instance.GetUI("announcement");
        UISprite mContent = AnnounceUI.FindComponent<UISprite>("mContent");
        GameObject mClone = AnnounceUI.FindChild("AnnounceItem");
        GameObject clone = WindowManager.Instance.CloneGameObject(mClone);
        GameObject.DontDestroyOnLoad(clone);
        mUI = new AnnounceItem(clone);
        mUI.gameObject.transform.parent = mContent.transform;
        mUI.gameObject.transform.localPosition = new Vector3(480, 0, 0);
        mUI.gameObject.transform.localRotation = Quaternion.identity;
        mUI.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);

        return true;
    }
    public bool InitItem(string str)
    {
        UIWindow AnnounceUI = WindowManager.Instance.GetUI("announcement");
        UISprite mContent = AnnounceUI.FindComponent<UISprite>("mContent");
        mUI.IconWidth = mUI.mIcon.width;
        SetText(str);
        SetIcon("announcement:announcement_sys");
        mUI.mText.MakePixelPerfect();
        mUI.TextWidth = mUI.mText.width;
        Vector3 itempos = mContent.transform.localPosition;
        mUI.mText.transform.localPosition = new Vector3(mUI.IconWidth + mUI.TextWidth / 2, 0, 0);
        mUI.mIcon.transform.localPosition = new Vector3(mUI.IconWidth / 2, 0, 0);

        mUI.itemWidth = (int)(mUI.TextWidth + mUI.IconWidth);
        mUI.gameObject.transform.localPosition = new Vector3(480, 0, 0);

        return true;
    }
    public bool IsHide()
    {
        return !mUI.gameObject.activeSelf;
    }

    public void Show()
    {
        if (mUI != null && IsHide())
        {
            NGUITools.SetActive(mUI.gameObject, true);
        }
    }

    public void Hide()
    {
        if (mUI != null && !IsHide())
        {
            NGUITools.SetActive(mUI.gameObject, false);
        }
        // mLastProgress = 1.0f;
    }
    public void SetText(string text)
    {
        if (mUI == null)
            return;
        if (WindowManager.current2DCamera != null)
        {
            mUI.SetText(text);
        }
    }

    public void SetIcon(string Icon)
    {
        if (mUI == null)
            return;
        if (WindowManager.current2DCamera != null)
        {
            mUI.SetIcon(Icon);
        }
    }

    
}

//公告内容界面管理器
public class AnnounceItemManager
{
    public int passnum = 0;
    private bool mishide = true;
    public bool nextflag = false;
    private bool mSetHide = false;
    //显示列表
    public List<AnnounceItemNode> mShowList = new List<AnnounceItemNode>();
    //缓存列表
    private Queue mCacheQueue = new Queue();

    private static AnnounceItemManager instance = null;
    public static AnnounceItemManager Instance
    {
        get
        {
            return instance;
        }
    }

    public bool ishide
    {
        get { return mishide; }
        set { mishide = value; }
    }

    public bool SetHide
    {
        get { return mSetHide; }
        set { mSetHide = value; }
    }

    public AnnounceItemManager()
    {
        instance = this;
    }


    public AnnounceItemNode CreateAnnounceItem(string str)
    {
        AnnounceItemNode node = null;
        if (mCacheQueue.Count > 0)
        {
            node = mCacheQueue.Dequeue() as AnnounceItemNode;
            node.InitItem(str);
        }
        else
        {
            node = new AnnounceItemNode();

            node.Init();
            node.InitItem(str);

        }
        node.isend = false;
        AddNewAnnounceShow(node);
        return node;
    }

    public void ReleaseAnnounceItem(AnnounceItemNode node)
    {
        node.Hide();
        mCacheQueue.Enqueue(node);
    }
    public void AddNewAnnounceShow(AnnounceItemNode node)
    {
        mShowList.Add(node);
    }

    public void RemoveAnnounceShow(AnnounceItemNode node)
    {
        if (mShowList.Count > 0)
        {
            mShowList.Remove(node);
        }
    }


    public void HideList()
    {
        if (0 >= mShowList.Count)
            return ;

        for (int i = 0; i < mShowList.Count; ++i)
        {
            if (null != mShowList[i])
            {
                mShowList[i].Hide();
            }
        }

        NGUITools.SetActive(mShowList[0].mUI.gameObject.transform.parent.gameObject, false);
    }

    public void ShowList()
    {
        if (0 >= mShowList.Count)
            return;

        for (int i = 0; i < mShowList.Count; ++i)
        {
            if (null != mShowList[i])
            {
                mShowList[i].Show();
            }
        }

        NGUITools.SetActive(mShowList[0].mUI.gameObject.transform.parent.gameObject, true);
    }

    public void Update()
    {
        if (WindowManager.current2DCamera != null)
        {
            if (mShowList.Count == 0)
            {
                ishide = true;
                nextflag = true;
            }
            else
            {
                ishide = false;
                for (int i = 0; i < mShowList.Count; ++i)
                {
                    AnnounceItemNode _node = mShowList[i] as AnnounceItemNode;
                    Vector3 post = _node.mUI.gameObject.transform.localPosition;
                    if (post.x < 480 - _node.mUI.itemWidth && !_node.nextFlag)
                    {
                        _node.nextFlag = true;
                        passnum++;

                    }
                    if (post.x >= -(480 + _node.mUI.itemWidth))
                    {
                        _node.mUI.gameObject.transform.Translate(-Time.deltaTime * 0.2f, 0, 0, Space.Self);
                    }
                    else
                    {
                        _node.isend = true;
                        RemoveAnnounceShow(_node);
                        passnum--;
                        ReleaseAnnounceItem(_node);
                    }
                }
                if (passnum >= AnnounceItemManager.Instance.mShowList.Count)
                {
                    AnnounceItemManager.Instance.nextflag = true;
                }
            }
        }
    }
}