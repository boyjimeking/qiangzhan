using UnityEngine;
using System.Collections;
public static class MyJoystick
{
    public static Vector2 joystickAxis = Vector2.zero;
}
public class UIJoystick : UIWindow
{
    protected GameObject mBg = null;

    public GameObject Backgroud = null;
    public UISprite Button = null;
    public UISprite Sprite = null;

    //摇杆圆点
    private Vector2 mDotPos = Vector2.zero;

    private int mCurrentTouchID = -1;
    public float deadZone = 20;
    private float zoneRadius = 100f;

    private Vector2 joystickTouch = Vector2.zero;

    private bool mPress = false;

    private Vector2 mBackgroudSize = Vector2.zero;

    private UICamera.MouseOrTouch mCurrentTouch = null;

    private Vector2 mLastButtonPos = Vector2.zero;


    private Vector2 mButtonPos = Vector2.zero;

    public UIJoystick()
    {

    }
    protected override void OnLoad()
    {
        mBg = this.FindChild("bg");
        Backgroud = this.FindChild("Trigger");
        Button = this.FindComponent<UISprite>("Trigger/Button");
        Sprite = this.FindComponent<UISprite>("Trigger/Sprite");

        UIEventListener.Get(mBg).onClick = OnBlankClick;
    }
    //界面打开
    protected override void OnOpen(object param = null)
    {
        UICamera.onScreenResize += ResetButtonPos;
        UIEventListener.Get(Backgroud.gameObject).onPress = onJoystickPress;
    }
    //界面关闭
    protected override void OnClose()
    {
        UICamera.onScreenResize -= ResetButtonPos;

        ClearJoystick();
    }

    private void ClearJoystick()
    {
        MyJoystick.joystickAxis = Vector2.zero;
        mPress = false;
    }

    void OnBlankClick(GameObject go)
    {
        CloseSubForm();
    }

    void CloseSubForm()
    {
        UICityForm form = WindowManager.Instance.GetUI("city") as UICityForm;
        if (form != null)
            form.closeSubMenuForm();
    }

    void onJoystickPress(GameObject obj, bool isPress)
    {
        CloseSubForm();
        mPress = isPress;

        mCurrentTouchID = UICamera.currentTouchID;
        mCurrentTouch = UICamera.GetTouch(mCurrentTouchID);

        if (!mPress || mCurrentTouch == null)
        {
            MyJoystick.joystickAxis = Vector2.zero;
            ResetButtonPos();
            return;
        }

        mDotPos = mCurrentTouch.pos;

        Vector3 pos = WindowManager.current2DCamera.ScreenToWorldPoint(mDotPos);
        //更新新位置
        Button.transform.position = Sprite.transform.position = pos;
    }

    float ComputeDeadZone(Vector2 distance)
    {
        float dist = Mathf.Max(distance.magnitude, 0.1f);

        return Mathf.Max(dist - deadZone, 0) / (zoneRadius - deadZone) / dist;
    }

    Vector2 GetCurrentTouchPos()
    {
        if (mCurrentTouch == null)
        {
            return Vector2.zero;
        }
        return mCurrentTouch.pos;
    }
    Vector2 GetBackgroudSize()
    {
        float scale = (float)Screen.height / UIRootAdaptive.DesignHeight;

        float w = (float)Sprite.width * Backgroud.transform.localScale.x * scale;
        float h = (float)Sprite.height * Backgroud.transform.localScale.y * scale;
        if (mBackgroudSize.x != w || mBackgroudSize.y != h)
        {
            mBackgroudSize.x = w;
            mBackgroudSize.y = h;
        }
        return mBackgroudSize;
    }
    float GetTouchAngle(Vector2 p1, Vector2 p2)
    {
        float distanceX = (int)(p1.x - p2.x);
        float distanceY = (int)(p1.y - p2.y);
        return Mathf.Atan2(distanceY, distanceX);
    }

    void ResetButtonPos()
    {
        float w = (float)Sprite.width;
        float h = (float)Sprite.height;
        float scale = (float)Screen.height / UIRootAdaptive.DesignHeight;
        Vector2 pos = new Vector2(w * scale, h * scale);
        Button.transform.position = Sprite.transform.position = WindowManager.current2DCamera.ScreenToWorldPoint(pos);
    }
    public override void Update(uint elapsed)
    {
        if( mView == null )
        {
            return;
        }
        if( !mView.activeInHierarchy )
        {
            ClearJoystick();
        }

        if (mPress)
        {
            Vector2 pos = GetCurrentTouchPos();
            Vector2 size = GetBackgroudSize();
            float radius = size.x / 2.0f;
            float dis = Vector2.Distance(pos, mDotPos);
            //Vector2 buttonPos = new Vector2();
            //超出圆范围
            if (dis > radius)
            {
                float angle = GetTouchAngle(pos, mDotPos);
                mButtonPos.x = (mDotPos.x + radius * Mathf.Cos(angle/* * PI /180*/ ));
                mButtonPos.y = (mDotPos.y + radius * Mathf.Sin(angle /** PI /180*/ ));
            }
            else
            {
                mButtonPos = pos;
            }

            if (mLastButtonPos != mButtonPos)
            {
                mLastButtonPos = mButtonPos;
                Button.transform.position = WindowManager.current2DCamera.ScreenToWorldPoint(mButtonPos);

                Vector2 distance = mButtonPos - mDotPos;

                float deadCoef = ComputeDeadZone(distance);
                //MyJoystick.joystickAxis = new Vector2(distance.x * deadCoef, distance.y * deadCoef);
                MyJoystick.joystickAxis.x = distance.x * deadCoef;
                MyJoystick.joystickAxis.y = distance.y * deadCoef;
            }
        }
    }
}
