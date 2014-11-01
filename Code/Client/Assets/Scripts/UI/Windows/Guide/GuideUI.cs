using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ArrowRot
{
    ArrowRot_Invaild,
    ArrowRot_UP,
    ArrowRot_DOWN,
    ArrowRot_LEFT,
    ArrowRot_RIGHT,
}
public class GuideUI
{
    public UISprite mArrow = null;
    public UISprite mEffect = null;

    public List<UISprite> mMask = new List<UISprite>();

    private ArrowRot mArrowRot = ArrowRot.ArrowRot_Invaild;

    private GameObject mObj = null;
    private int mDepth = 1000;
    public GuideUI( GameObject obj )
    {
        mObj = obj;

        mArrow = ObjectCommon.GetChildComponent<UISprite>(mObj, "Arrow");
        mEffect = ObjectCommon.GetChildComponent<UISprite>(mObj,"Effect");

        mMask.Add(ObjectCommon.GetChildComponent<UISprite>(mObj, "Mask1"));
        mMask.Add(ObjectCommon.GetChildComponent<UISprite>(mObj, "Mask2"));
        mMask.Add(ObjectCommon.GetChildComponent<UISprite>(mObj, "Mask3"));
        mMask.Add(ObjectCommon.GetChildComponent<UISprite>(mObj, "Mask4"));

        mEffect.gameObject.SetActive(false);
    }
    public void Open()
    {
        if (mObj != null)
            mObj.SetActive(true);
    }

    public void UpdateDepth(int depth)
    {
        if (mDepth != depth)
        {
            mDepth = depth;
            WindowManager.Instance.SetDepth(mObj, mDepth,true ,10);
        }
    }

    public bool IsOpned()
    {
        if (mObj == null)
            return false;
        return mObj.activeSelf;
    }
    public void End()
    {
        if( mObj != null )
            mObj.SetActive(false);
    }

    public void SetEffectPos(Vector3 pos)
    {
        mEffect.transform.position = pos;
    }
    public void SetEffectSize(int width, int height)
    {
        //美术做的这个图 有16个像素边框
        mEffect.width = width + 16;
        mEffect.height = height + 16;
    }

    public void SetArrowPos(Vector3 pos , bool showMask , bool weak)
    {
        mArrow.transform.position = pos;
        ResetMask(showMask,weak);
    }

    public int GetArrowWidth()
    {
        if (mArrowRot == ArrowRot.ArrowRot_UP || mArrowRot == ArrowRot.ArrowRot_DOWN)
        {
            return mArrow.width;
        }
        return mArrow.height;
    }
    public int GetArrowHeight()
    {
        if (mArrowRot == ArrowRot.ArrowRot_UP || mArrowRot == ArrowRot.ArrowRot_DOWN)
        {
            return mArrow.height;
        }
        return mArrow.width;
    }

    public void SetArrowRot(ArrowRot rot)
    {
        if( mArrowRot == rot )
        {
            return;
        }
        mArrowRot = rot;
        float rotAngle = 0.0f;
        switch( rot )
        {
            case ArrowRot.ArrowRot_UP:
                rotAngle = 180.0f;
                break;
            case ArrowRot.ArrowRot_DOWN:
                rotAngle = 0.0f;
                break;
            case ArrowRot.ArrowRot_LEFT:
                rotAngle = 270.0f;
                break;
            case ArrowRot.ArrowRot_RIGHT:
                rotAngle = 90.0f;
                break;
        }
        if (Mathf.Abs((mArrow.transform.eulerAngles.z - rotAngle)) > 0.00001f)
        {
            Quaternion qua = Quaternion.Euler(new Vector3(0.0f, 0.0f, rotAngle));
            mArrow.transform.rotation = qua;
        }
    }


    //更新遮罩
    private void ResetMask(bool showMask,bool weak)
    {
        if( weak )
        {
            for (int i = 0; i < mMask.Count; ++i)
            {
                NGUITools.SetActive(mMask[i].gameObject, false);
            }
            return;
        }
        float spectRatio = WindowManager.GetSpectRatio();
        //转圈特效坐标
        Vector3 pos = WindowManager.current2DCamera.WorldToScreenPoint(mEffect.transform.position);
        int idx = 0;
        float left = pos.x - (mEffect.width / spectRatio / 2);
        float right = pos.x + (mEffect.width / spectRatio / 2);
        float top = pos.y + mEffect.height / spectRatio / 2;
        float bottom = pos.y - mEffect.height / spectRatio / 2;
        UISprite mask = null;
        if( left > 0.0f )
        {
            mask = GetMask(idx++);
            NGUITools.SetActive(mask.gameObject,true);
            mask.alpha = showMask ? 1.0f : 0.01f;
            UpateMask(mask, new Vector3(left / 2, Screen.height / 2, 0), left * spectRatio,  Screen.height * spectRatio);
        }
        if( bottom < Screen.height )
        {
            mask = GetMask(idx++);
            NGUITools.SetActive(mask.gameObject, true);
            mask.alpha = showMask ? 1.0f : 0.01f;
            UpateMask(mask, new Vector3(pos.x, bottom / 2, 0), mEffect.width, (bottom) * spectRatio);
        }

        if( right < Screen.width )
        {
            mask = GetMask(idx++);
            NGUITools.SetActive(mask.gameObject, true);
            mask.alpha = showMask ? 1.0f : 0.01f;
            UpateMask(mask, new Vector3(right + (Screen.width - right) / 2, Screen.height / 2, 0), (Screen.width - right) * spectRatio, Screen.height * spectRatio);
        }
        if( top > 0.0f )
        {
            mask = GetMask(idx++);
            NGUITools.SetActive(mask.gameObject, true);
            mask.alpha = showMask ? 1.0f : 0.01f;
            UpateMask(mask, new Vector3(pos.x, top + (Screen.height - top) / 2, 0), mEffect.width,(Screen.height - top) * spectRatio);
        }

        for( int i = idx ; i < mMask.Count ; ++i )
        {
           NGUITools.SetActive(mMask[i].gameObject, false);
        }
    }

    UISprite  GetMask(int idx)
    {
        if (idx < 0 || idx >= mMask.Count)
            return null;
        return mMask[idx];
    }

    void UpateMask(UISprite sprite, Vector3 pos, float width, float height)
    {
        sprite.width = (int)(width + 0.5);
        sprite.height = (int)(height + 0.5); ;
        sprite.transform.position = WindowManager.current2DCamera.ScreenToWorldPoint(pos);
        sprite.ResizeCollider();
    }

}
