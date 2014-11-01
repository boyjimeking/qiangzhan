using System;
using UnityEngine;
class LoadingManager
{
    private static LoadingManager msInstance = null;

    private GameObject mView = null;

    private UIVerify mVerify = null;

    public LoadingManager()
    {
        msInstance = this;
    }

    public static LoadingManager Instance
    {
        get
        {
            return msInstance;
        }
    }

    public void ShowLoading()
    {
        if (mView)
        {
            mView.gameObject.SetActive(true);
            return;
        }
        mView = (GameObject)GameObject.Instantiate(Resources.Load("UI/UIVerify"));
        GameObject parent = WindowManager.Instance.GetLayer((int)UI_LAYER_TYPE.UI_LAYER_WINDOWS, false);
        mView.transform.parent = parent.transform;
        mView.transform.localPosition = Vector3.zero;
        mView.transform.localRotation = Quaternion.identity;
        mView.transform.localScale = Vector3.one;
        mView.layer = parent.layer;

        mVerify = mView.GetComponent<UIVerify>();
    }

    public void UpdateProgress(float progress)
    {
        if( mView == null || mVerify == null )
        {
            return;
        }
        mVerify.SetProgress(progress);
    }

    public void CloseLoading()
    {
        if (!mView)
            return;
        mView.gameObject.SetActive(false);
        GameObject.Destroy(mView);
        mView = null;
        mVerify = null;
    }
}
