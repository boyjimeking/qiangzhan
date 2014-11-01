using UnityEngine;
using System.Collections;

public class BloodUI
{
    private UISlider mSlider = null;

    private GameObject mObj = null;

    private bool mShow = false;

    public BloodUI( GameObject obj )
    {
        mObj = obj;
        mSlider = obj.GetComponent<UISlider>();
    }

    public GameObject gameObject
    {
        get
        {
            return mObj;
        }
    }

    public bool IsShow()
    {
        return mShow;
    }

    public void Show()
    {
        if (mShow)
            return;
        mShow = true;
        NGUITools.SetActive(gameObject, true);
    }

    public void Hide()
    {
        if (!mShow)
            return;
        mShow = false;
        NGUITools.SetActive(gameObject, false);
    }

    public void SetProgress(float progress)
    {
        if (mSlider == null)
            return;
        mSlider.value = progress;
    }
}
