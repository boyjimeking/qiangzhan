using UnityEngine;
using System.Collections;

public class DropEffect : MonoBehaviour {

    private Vector3 mFrom = Vector3.zero;
    private Vector3 mTo = Vector3.zero;
    //private float mRateDuration = 0.3f;  //圆形运动占总时间比例;
    private float mDurationSec = 0f;     //自由落体时间;
    //private float mDurationSecRota = 0f; //圆形运动时间;
    private bool mAutoDestroy = true;

    private float mTimet = 0f;
    private bool mStart = false;

    public delegate void OnFinish();
    public OnFinish onFinish;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (!mStart)
            return;

        mTimet += Time.deltaTime;

        if (mTimet > mDurationSec)
        {
            if (onFinish != null)
                onFinish();

            if (mAutoDestroy)
                DestroySelf();

            this.enabled = false;
        }
        else
        { 
            transform.localPosition = getPosByTime(mTimet);
        }
	}

    public void Play(Vector3 from, Vector3 to, float duration, bool autoDestroy = true)
    {
        mFrom = from;
        mTo = to;
        mDurationSec = duration;
        mAutoDestroy = autoDestroy;

        mStart = true;
    }

    #region 平抛自由落体运动;
    float getSpeedX()
    {
        return (mTo.x - mFrom.x) / mDurationSec;
    }


    float getAccelerationY()
    {
        float height = mTo.y - mFrom.y;

        return (2 * height) / (mDurationSec * mDurationSec);
    }

    Vector3 getPosByTime(float time)
    {
        Vector3 res = new Vector3();

        res.x = mFrom.x + getSpeedX() * time;
        float acc = getAccelerationY();
        res.y = mFrom.y + acc * time * time / 2f;
        res.z = 0f;

        return res;
    }
    #endregion

    #region 圆形轨迹运动;
    ////获得圆的半径;
    //float getCircleRadio()
    //{
    //    return getSpeedX() * mDurationSecRota / 2f;
    //}

    //float getDirPerTime()
    //{
    //    return Mathf.PI / mDurationSecRota;
    //}

    //Vector3 getCirclePosByTime(float time)
    //{
    //    float angle = time * getDirPerTime();

    //    Vector3 res = new Vector3();

    //    res.x = 

    //    return Vector3.zero;
    //}

    #endregion
    void DestroySelf()
    {
        GameObject.Destroy(this.gameObject);
    }
}
