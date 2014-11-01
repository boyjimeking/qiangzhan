using UnityEngine;
using System.Collections;

public class UINetWaiting : UIWindow
{
    public UISprite mSprite = null;
    public float mMaxRotTime = 0.1f;
    public float mMaxRotAngle = 10.0f;

    private float mRotAngle = 360.0f;

    private float mRotTime = 0.0f;
    public UINetWaiting()
    {

    }
    protected override void OnLoad()
    {
        mSprite = this.FindComponent<UISprite>("Container/quan");
    }
    protected override void OnOpen(object param = null)
    {

    }

    public override void Update(uint elapsed)
    {
        mRotTime += Time.deltaTime;
        if (mRotTime >= mMaxRotTime)
        {
            mRotAngle -= 10.0f;
            if (mRotAngle < 0.0f)
            {
                mRotAngle = 360.0f;
            }
            mSprite.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, mRotAngle));

            mRotTime = 0.0f;
        }
    }
    protected override void OnClose()
    {

    }
}
