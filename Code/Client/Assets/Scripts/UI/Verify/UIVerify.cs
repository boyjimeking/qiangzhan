using System;
using UnityEngine;
class UIVerify : MonoBehaviour
{
    public UISlider mSlider = null;
    public UILabel mLoadingContent = null;

    void Awake()
    {
        
    }
    void Start()
    {
        
    }
    void OnEnable()
    {
        EventSystem.Instance.addEventListener(LoadingEvent.LOADING_PROGRESS, onProgress);
    }

    void OnDisable()
    {
        EventSystem.Instance.removeEventListener(LoadingEvent.LOADING_PROGRESS, onProgress);

    }

    public void onProgress(EventBase evt)
    {
        LoadingEvent levt = evt as LoadingEvent;
        SetProgress(levt.progress);

        mLoadingContent.text = levt.showname;
    }

    public void SetProgress(float pro)
    {
        mSlider.value = (float)pro/100;
    }
}

