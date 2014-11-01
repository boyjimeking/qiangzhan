using UnityEngine;
using System.Collections;


public class AwardItemUI
{
    private GameObject  mObj = null;
    private UILabel     mLabel = null;
    private CommonItemUI mItemUI = null;
    private UISprite mOverlap = null;//遮盖层

    private int mResId;
    private int mCnt;

    public CommonItemUI ItemUI
    {
        get {
            if (null != mItemUI)
                return mItemUI;
            else
                return null;
            }
    }
    public AwardItemUI(int itemResId, int cnt)
    {
        mResId = itemResId;
        mCnt = cnt;

        mObj = WindowManager.Instance.CloneCommonUI("AwardItemUI");
  
        mLabel = ObjectCommon.GetChildComponent<UILabel>(mObj, "text");
        mOverlap = ObjectCommon.GetChildComponent<UISprite>(mObj, "overlap");

        mItemUI = new CommonItemUI(mResId);

        mItemUI.gameObject.transform.parent = mObj.gameObject.transform;
        mItemUI.gameObject.transform.localPosition = new Vector3(0.0f, 10.0f, 0.0f);
        mItemUI.gameObject.transform.localScale = Vector3.one;

        mLabel.text = mItemUI.GetItemName() + " x " + cnt.ToString();
        setShowImage();
    }

   public void setShowText(bool value)
    {
        mLabel.gameObject.SetActive(value);
    }

   public void setShowImage(bool value = false)
   {
       mOverlap.gameObject.SetActive(value);
   }

    public GameObject gameObject
    {
        get
        {
            return mObj;
        }
    }
}
