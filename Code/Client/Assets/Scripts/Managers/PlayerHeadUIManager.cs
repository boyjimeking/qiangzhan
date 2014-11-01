using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HeadNode
{
    private PlayerHeadUI mUI = null;

    public HeadNode()
    {
        //等待新机制修改
//         GameObject obj = WindowManager.Instance.CreateUI("UI/player/PlayerHeadUI");
//         if (obj == null)
//         {
//             GameDebug.LogError("未找到PlayerHeadUI");
//             return;
//         }
//         mUI = obj.GetComponent<PlayerHeadUI>();
//         mUI.transform.position = WindowManager.current2DCamera.ScreenToWorldPoint(new Vector3(-100.0f, -120.0f, 0.0f)); ;

    }
    public bool Init()
    {
        //等待新机制修改;

        GameObject clone = WindowManager.Instance.CloneCommonUI("PlayerHeadUI",(int)UI_LAYER_TYPE.UI_LAYER_ITEMS);

        GameObject.DontDestroyOnLoad(clone);

        mUI = new PlayerHeadUI(clone);

        mUI.gameObject.transform.position = WindowManager.current2DCamera.ScreenToWorldPoint(new Vector3(-100.0f, -120.0f, 0.0f));

        return true;
    }

    public bool IsHide()
    {
        return !mUI.gameObject.activeSelf;
    }

    public void Show()
    {
        if (mUI != null && IsHide() && SceneManager.Instance.IsCurSceneType(SceneType.SceneType_City))
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

    public void SetName(string name)
    {
        if (mUI == null)
            return;
        if (WindowManager.current2DCamera != null)
        {
            mUI.SetName(name);
        }
    }

    public void SetTitle(string imgInfo)
    {
        if (mUI == null)
            return;

        if (WindowManager.current2DCamera != null)
        {
            mUI.SetTitle(imgInfo);
        }
    }

    public void Update(Vector3 pos)
    {
        if (mUI == null)
            return;
        if (WindowManager.current2DCamera != null)
        {
            Vector3 pos2 = WindowManager.current2DCamera.ScreenToWorldPoint(pos);
            mUI.gameObject.transform.position = pos2;
        }
    }
}

//头顶血槽界面管理器
public class PlayerHeadUIManager
{
    //缓存列表
    private Queue mCacheQueue = new Queue();

    private static PlayerHeadUIManager instance = null;
    public static PlayerHeadUIManager Instance
    {
        get
        {
            return instance;
        }
    }
    public PlayerHeadUIManager()
    {
        instance = this;
    }


    public HeadNode CreatePlayerHeadUI()
    {
        HeadNode node = null;
        if (mCacheQueue.Count > 0)
        {
            node = mCacheQueue.Dequeue() as HeadNode;
        }
        else
        {
            node = new HeadNode();
            node.Init();
        }

        return node;
    }

    public void ReleasePlayerHeadUI(HeadNode node)
    {
        node.Hide();
        mCacheQueue.Enqueue(node);
    }
}
