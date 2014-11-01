using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BloodNode
{
    private BloodUI mUI = null;

    //private float mLastProgress = 1.0f;

    private static float MAX_HIDE_TIME = 3.0f;
    private float mHideTime = 0.0f;
    public BloodNode()
    {
        
    }

    public bool Init()
    {
        //等待新机制修改

        GameObject clone = WindowManager.Instance.CloneCommonUI("BloodUI" , (int)UI_LAYER_TYPE.UI_LAYER_ITEMS);

        GameObject.DontDestroyOnLoad(clone);

        mUI = new BloodUI(clone);

        mUI.gameObject.transform.position = WindowManager.current2DCamera.ScreenToWorldPoint(new Vector3(-100.0f, -100.0f, 0.0f));
        //默认隐藏
        NGUITools.SetActive(clone, false);

        return true;
    }

    public bool IsHide()
    {
        return !mUI.IsShow();
    }

    public void Show()
    {
        if (mUI != null && !mUI.IsShow())
        {
            mUI.Show();
            mHideTime = MAX_HIDE_TIME;
        }
    }


    public void Hide()
    {
        if (mUI != null && mUI.IsShow())
        {
            mUI.Hide();
        }
    }

    public void Update(Vector3 pos , int hp , int maxHp)
    {
		if (hp == 0)
			GameDebug.Log(0 + ": " + maxHp);
        if( WindowManager.current2DCamera != null )
        {
            if( maxHp <= 0 )
            {
                return;
            }

            float progress = (float)hp / (float)maxHp;

            mHideTime -= Time.unscaledDeltaTime;
            if (mHideTime <= 0)
            {
                Hide();
                return;
            }

            Vector3 pos2 = WindowManager.current2DCamera.ScreenToWorldPoint(pos);
            mUI.gameObject.transform.position = pos2;
            mUI.SetProgress(progress);

           // mLastProgress = progress;
        }
    }
}

//头顶血槽界面管理器
public class BloodUIManager
{
    //缓存列表
    private Queue mCacheQueue = new Queue();

    private static BloodUIManager instance = null;
	public static BloodUIManager Instance
    {
        get
        {
            return instance;
        }
    }
    public BloodUIManager()
    {
        instance = this;
    }


    public BloodNode CreateBloodUI()
    {
        BloodNode node = null;
        if (mCacheQueue.Count > 0)
        {
            node = mCacheQueue.Dequeue() as BloodNode;
        }
        else
        {
            node = new BloodNode();

            node.Init();
        }

        return node;
    }

    public void ReleaseBloodUI(BloodNode node)
    {
        node.Hide();
        mCacheQueue.Enqueue(node);
    }
}