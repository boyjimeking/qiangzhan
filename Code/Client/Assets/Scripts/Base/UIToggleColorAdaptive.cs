using System;
using UnityEngine;

class UIToggleColorAdaptive : MonoBehaviour
{
    public UILabel mLable = null;
    private UIToggle mToggle = null;
    void Start()
    {
    }

    void OnEnable()
    {
        mToggle = gameObject.GetComponent<UIToggle>();
        if (mToggle != null)
        {
            EventDelegate.Add(mToggle.onChange, OnToggleChange);
        }
        UpdateColor();
    }

    void OnDisable()
    {
        if (mToggle != null)
        {
            EventDelegate.Remove(mToggle.onChange, OnToggleChange);
        }
    }
    void OnToggleChange()
    {
        UpdateColor();
    }
    void UpdateColor()
    {
        if( mLable == null )
        {
            return ;
        }
        if( mToggle == null )
        {
            return;
        }
        mLable.applyGradient = mToggle.value;
    }
}
