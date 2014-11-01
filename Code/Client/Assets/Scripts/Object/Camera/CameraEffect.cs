using System;
using UnityEngine;

/// <summary>
/// 摄像机的效果
/// </summary>
public class CameraEffect
{
    public float start = 2f;
    public float end = 10f;
    public float time = 2f;

    public float mElapsed = 2f;

    public float Distance = 0;

    public bool mDirty = false;

    public void Initialize(float aStart = 2f,float aEnd = 2f,float fTime=0.8f)
    {
        start = aStart;
        end = aEnd;
        time = fTime;
        mElapsed = 0;
        Distance = 0;
        mDirty = true;
    }

    public void Update(float elapsed)
    {
        if(time <= 0.001f || !mDirty)
            return;
        if (mElapsed >= time)
        {
            mDirty = false;
            return;
        }

        mElapsed += elapsed;


        if (mElapsed / time < 0.8)
            Distance = start;
        else
            Distance = easeOutExpo(start, end, mElapsed / time);

    }
    public bool InUse
    {
        get
        {
            return mDirty && mElapsed < time;
        }
    }

    /// <summary>
    /// 摘自 Itween
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private float easeInOutQuart(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * value * value * value * value + start;
        value -= 2;
        return -end / 2 * (value * value * value * value - 2) + start;
    }

    private float easeInOutCirc(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return -end / 2 * (Mathf.Sqrt(1 - value * value) - 1) + start;
        value -= 2;
        return end / 2 * (Mathf.Sqrt(1 - value * value) + 1) + start;
    }

    private float easeInOutCubic(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * value * value * value + start;
        value -= 2;
        return end / 2 * (value * value * value + 2) + start;
    }

    private float easeOutExpo(float start, float end, float value)
    {
        end -= start;
        return end * (-Mathf.Pow(2, -10 * value / 1) + 1) + start;
    }
}

