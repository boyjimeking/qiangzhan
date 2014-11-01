using System;
using UnityEngine;
using System.Collections;

public class AchieveItemUI
{
    private GameObject mObj;

    public UILabel mNum;
    public UILabel mAdd;
    public UISprite mIcon;
    private int curShowNum = 0;
    public UISpriteAnimation mAni;
    [HideInInspector] public int mMaxNum;

    public AchieveItemUI(GameObject gObj)
    {
        mObj = gObj;
        mNum = ObjectCommon.GetChildComponent<UILabel>(mObj, "Label1");
        mAdd = ObjectCommon.GetChildComponent<UILabel>(mObj, "Label2");
        mAni = ObjectCommon.GetChildComponent<UISpriteAnimation>(mObj, "AchieveAni");
        mAni.loop = false;
        mAni.Stop();
        mAni.gameObject.SetActive(false);
        mAdd.gameObject.SetActive(true);
        mIcon = mObj.GetComponent<UISprite>();
    }

    public bool ScrollNum()
    {
        float increment = mMaxNum/25f;
        curShowNum = (int) Math.Floor(increment + curShowNum);
        if (curShowNum >= mMaxNum)
        {
            curShowNum = mMaxNum;
            mNum.text = curShowNum.ToString();

            return false;
        }
        mNum.text = curShowNum.ToString();
        return true;
    }

    public void Reset()
    {
        curShowNum = 0;
        mAdd.text = "";
        mAdd.alpha = 0;
        mAni.Stop();
        mAni.gameObject.SetActive(false);
    }

    public void PlayTween()
    {
        var tween = mAdd.gameObject.GetComponent<UIPlayTween>();
        mAdd.text = "+" + mMaxNum;
        tween.resetOnPlay = true;
        mAdd.transform.localPosition = new Vector3(0, -54, 0);
        mAdd.alpha = 1;
        tween.Play(true);
        mAni.gameObject.SetActive(true);
        mAni.Reset();
    }

}
