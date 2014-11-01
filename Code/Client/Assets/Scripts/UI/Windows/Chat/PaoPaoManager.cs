using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PaoPaoNode
{
    private PaoPaoUI mView = null;

    private int mLeftTime = 0;
    private bool mLeftStart = false;
    TweenAlpha alphaCom = null;

    bool mTop = false;

    private Vector3 mOldPos = Vector3.zero;

    public PaoPaoNode()
    {
        GameObject clone = WindowManager.Instance.CloneCommonUI("PaoPaoUI");

        GameObject.DontDestroyOnLoad(clone);

        mView = new PaoPaoUI(clone);

        NGUITools.SetActive(mView.gameObject, false);
    }

    public void SetOffset(float offset)
    {
        mView.SetLableOffset(offset);
    }
    public void SetLayer(int layer)
    {
        WindowManager.Instance.SetLayer(mView.gameObject, layer);
    }
    public void SetTop(bool top)
    {
        mTop = top;
        UISpriteAnimation ani = mView.mSprite.gameObject.GetComponent<UISpriteAnimation>();
        if (ani == null) ani= mView.mSprite.gameObject.AddComponent<UISpriteAnimation>();
        ani.PixelPerfect = false;

        if( top )
        {
            mView.mSprite.centerType = UISprite.AdvancedType.Tiled;
            mView.mSprite.leftType = UISprite.AdvancedType.Sliced;
            mView.mSprite.rightType = UISprite.AdvancedType.Sliced;
            mView.mSprite.bottomType = UISprite.AdvancedType.Sliced;
            mView.mSprite.topType = UISprite.AdvancedType.Sliced;
            UIAtlasHelper.SetSpriteImage(mView.mSprite, "zhushouduihua:01_000");
            ani.enabled = true;
            ani.RebuildSpriteList();
            ani.Reset();
            
        }else
        {
            mView.mSprite.centerType = UISprite.AdvancedType.Sliced;
            mView.mSprite.leftType = UISprite.AdvancedType.Sliced;
            mView.mSprite.rightType = UISprite.AdvancedType.Sliced;
            mView.mSprite.bottomType = UISprite.AdvancedType.Sliced;
            mView.mSprite.topType = UISprite.AdvancedType.Sliced;
            UIAtlasHelper.SetSpriteImage(mView.mSprite, "Chat:paopao");
            ani.enabled = false;
        }

        mView.mLabel.gameObject.transform.localPosition = new Vector3(0.0f, 5.0f, 0.0f);
    }
    public void SetMinSize(int minWidth , int minHeight)
    {
        mView.MinWidth = minWidth;
        mView.MinHeight = minHeight;
    }
    public void SetGap(int w, int h)
    {
        mView.SetGap(w, h);
    }

    public void SetColor(uint color)
    {
        mView.SetColor(color);
    }

    public void Talk(string txt, int time , int depth = 1)
    {
        if (mView == null)
            return;
        if( time < 0 )
        {
            mLeftTime = int.MaxValue;
        }else
        {
            mLeftTime = time;
        }
        mLeftStart = true;
        mView.SetAlpha(1.0f);
        mView.SetText(txt);

        WindowManager.Instance.SetDepth(mView.gameObject, depth, true);
    }

    public void Update(Vector3 _pos, uint elapsed)
    {
        if( mView == null )
        {
            return;
        }

        if (mLeftStart)
        {
            mLeftTime -= (int)elapsed;
            if( mLeftTime <= 0 )
            {
                End();
                mLeftStart = false;
            }

            if( IsHide() )
            {
                Show();
            }
        }

        if( mOldPos == _pos )
        {
            return;
        }

        mOldPos.x = _pos.x;
        mOldPos.y = _pos.y;
        mOldPos.z = _pos.z;

        if (mTop)
        {
            mOldPos.x -= ((float)mView.GetWidth() / 2.0f);
            mOldPos.y -= ((float)mView.GetHeight() / 2.0f);
        }else
        {
            mOldPos.x += ((float)mView.GetWidth() / 2.0f - 20.0f);
            mOldPos.y += ((float)mView.GetHeight() / 2.0f);
        }


        Vector3 pos2 = WindowManager.current2DCamera.ScreenToWorldPoint(mOldPos);
        mView.gameObject.transform.position = pos2;
    }
    public void End()
    {
        alphaCom = TweenAlpha.Begin(mView.gameObject, 1.0f, 0.0f);
        alphaCom.method = UITweener.Method.EaseInOut;

        EventDelegate.Add(alphaCom.onFinished, onFinished);
    }
    void onFinished()
    {
        EventDelegate.Remove(alphaCom.onFinished, onFinished);
        Hide();
    }
    public void Show()
    {
        if( !mView.gameObject.activeSelf )
        {
            NGUITools.SetActive(mView.gameObject, true);
        }
    }
    public void Hide()
    {
        mLeftStart = false;
        if( mView.gameObject.activeSelf )
        {
            NGUITools.SetActive(mView.gameObject, false);
        }
    }

    public bool IsHide()
    {
        return !mView.gameObject.activeSelf;
    }

}

//聊天泡泡管理
public class PaoPaoManager
{
    //缓存列表
    private Queue mCacheQueue = new Queue();

    private static PaoPaoManager instance = null;
    public static PaoPaoManager Instance
    {
        get
        {
            return instance;
        }
    }
    public PaoPaoManager()
    {
        instance = this;
    }

    public PaoPaoNode CreatePaoPaoUI(bool top = false, int minWidth = 210, int minHeight = 120 , int layer = (int)UI_LAYER_TYPE.UI_LAYER_ITEMS)
    {
        PaoPaoNode node = null;
        if (mCacheQueue.Count > 0)
        {
            node = mCacheQueue.Dequeue() as PaoPaoNode;
        }
        else
        {
            node = new PaoPaoNode();
        }
        node.SetLayer(layer);
        node.SetTop(top);
        node.SetMinSize(minWidth, minHeight);
        return node;
    }

    public void ReleasePaoPaoUI(PaoPaoNode node)
    {
        node.Hide();
        mCacheQueue.Enqueue(node);
    }
}