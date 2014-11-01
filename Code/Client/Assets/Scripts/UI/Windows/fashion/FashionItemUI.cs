
using System;
using UnityEngine;

public class FashionItemUI
{
    public UISprite bg;
    public UISprite cloth;
    public UILabel name;
    public UISprite flag;
    public UILabel flagTxt;
    public UIButton mBtn;
    public UISprite mLock;
    public GameObject mObject;
    public ItemClickCallBack clickCallBack;
   public FashionItemUI(GameObject obj)
    {
        mBtn = obj.GetComponent<UIButton>();
        bg = obj.GetComponent<UISprite>();
        cloth = ObjectCommon.GetChildComponent<UISprite>(obj, "Icon");
        name = ObjectCommon.GetChildComponent<UILabel>(obj, "name");
        flag = ObjectCommon.GetChildComponent<UISprite>(obj, "flag");
        flagTxt = ObjectCommon.GetChildComponent<UILabel>(obj, "flag/Label");
        mLock = ObjectCommon.GetChildComponent<UISprite>(obj, "lock");
        mObject = obj;
        EventDelegate.Add(mBtn.onClick,OnClick);
    }

  

    public GameObject gameObject
    {
        get
        {
            return mObject;
        }
    }

    public void OnClick()
    {
        if (clickCallBack != null)
            clickCallBack(Convert.ToInt32(mBtn.CustomData));
        SoundManager.Instance.Play(15);
    }

    public void Clear()
    {
        EventDelegate.Remove(mBtn.onClick, OnClick);
        clickCallBack = null;
    }


}

