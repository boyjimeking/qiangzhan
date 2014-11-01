
 using System;
 using UnityEngine;

public class FuncItemUI
{
    public GameObject gameobject;
    public UILabel label;
    public UISprite seletedSprite;
    public UIButton mBtn;
    public ItemClickCallBack mClickCallBack;
    public FuncItemUI(GameObject obj)
    {
        gameobject = obj;
        label = ObjectCommon.GetChildComponent<UILabel>(obj, "Label");
        seletedSprite = ObjectCommon.GetChildComponent<UISprite>(obj, "selected");
        mBtn = obj.GetComponent<UIButton>();
        EventDelegate.Add(mBtn.onClick, OnBtnClick);
    }

    public void Clear()
    {
        EventDelegate.Remove(mBtn.onClick, OnBtnClick);
        gameobject = null;
        label = null;
        seletedSprite = null;
        mClickCallBack = null;

    }

    private void OnBtnClick()
    {
        if (mClickCallBack != null)
        {
            SoundManager.Instance.Play(15);
            mClickCallBack(Convert.ToInt32(mBtn.CustomData));
        }
       
    }
 }

