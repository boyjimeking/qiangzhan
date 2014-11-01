using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMultiProgressBar : UIProgressBar 
{

    public List<UISprite> fgList = new List<UISprite>();
    
    public List<UISprite> bgList = new List<UISprite>();

    private bool isLastBgShow = true;

    protected override void OnStart()
    {
        if (fgList.Count != bgList.Count)
        {
            Debug.LogError("前景条和背景条数不同！;");
        }
    }

    public void SetLastBgIsShow(bool isShow)
    {
        isLastBgShow = isShow;
    }

    public override void ForceUpdate()
    {
        mIsDirty = false;

        if (mFG != null)
        {
            int idx = getIndexByValue(value);

            if (idx < 0)
            {
                for (int i = 0, j = fgList.Count; i < j; i++)
                {
                    if (fgList[i].gameObject.activeSelf != true)
                        fgList[i].gameObject.SetActive(true);

                    if (bgList[i].gameObject.activeSelf != true)
                        bgList[i].gameObject.SetActive(true);
                }

                mFG = fgList[0];
                mBG = bgList[0];
            }
            else
            {
                for (int i = 0, j = fgList.Count; i < j; i++)
                {
                    //bool tmp = i <= idx;
                    bool tmp = i == idx;
                
                    if (fgList[i].gameObject.activeSelf != tmp)
                        fgList[i].gameObject.SetActive(tmp);

                    if (bgList[i].gameObject.activeSelf != tmp)
                    { 
                        bgList[i].gameObject.SetActive(tmp);
                    }

                    if ((i == 0) && tmp && !isLastBgShow)
                        bgList[0].gameObject.SetActive(false);                        
                }

                mFG = fgList[idx];
                mBG = bgList[idx];
                //mBG = (isLastBgShow && (idx > 0)) ? bgList[idx] : null;
            }


            UISprite sprite = fgList[idx];

            if (isHorizontal)
            {
                if (sprite != null && sprite.type == UISprite.Type.Filled)
                {
                    sprite.fillDirection = UISprite.FillDirection.Horizontal;
                    //sprite.invert = isInverted;
                    sprite.fillAmount = value * fgList.Count - (int)(value * fgList.Count);
                }
                else
                {
                    mFG.drawRegion = isInverted ?
                        new Vector4(1f - value, 0f, 1f, 1f) :
                        new Vector4(0f, 0f, value, 1f);
                }
            }
            else if (sprite != null && sprite.type == UISprite.Type.Filled)
            {
                sprite.fillDirection = UISprite.FillDirection.Vertical;
                //sprite.invert = isInverted;
                sprite.fillAmount = value * fgList.Count - (int)(value * fgList.Count);
            }
            else
            {
                mFG.drawRegion = isInverted ?
                    new Vector4(0f, 1f - value, 1f, 1f) :
                    new Vector4(0f, 0f, 1f, value);
            }
        }

        if (thumb != null && (mFG != null || mBG != null))
        {
            Vector3[] corners = (mFG != null) ? mFG.localCorners : mBG.localCorners;

            Vector4 br = (mFG != null) ? mFG.border : mBG.border;
            corners[0].x += br.x;
            corners[1].x += br.x;
            corners[2].x -= br.z;
            corners[3].x -= br.z;

            corners[0].y += br.y;
            corners[1].y -= br.w;
            corners[2].y -= br.w;
            corners[3].y += br.y;

            Transform t = (mFG != null) ? mFG.cachedTransform : mBG.cachedTransform;
            for (int i = 0; i < 4; ++i) corners[i] = t.TransformPoint(corners[i]);

            if (isHorizontal)
            {
                Vector3 v0 = Vector3.Lerp(corners[0], corners[1], 0.5f);
                Vector3 v1 = Vector3.Lerp(corners[2], corners[3], 0.5f);
                SetThumbPosition(Vector3.Lerp(v0, v1, isInverted ? 1f - value : value));
            }
            else
            {
                Vector3 v0 = Vector3.Lerp(corners[0], corners[3], 0.5f);
                Vector3 v1 = Vector3.Lerp(corners[1], corners[2], 0.5f);
                SetThumbPosition(Vector3.Lerp(v0, v1, isInverted ? 1f - value : value));
            }
        }
    }

    /// <summary>
    /// 根据value的值来判断当前显示的是第几组前景图和背景图;
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    int getIndexByValue(float val)
    {
        float per = 1f / fgList.Count;

        for (int i = 0, j = fgList.Count; i < j; i++)
        {
            if ((i + 1) * per > val)
                return i;
        }

        return -1;
    }
}
