using UnityEngine;
using System;
using System.Collections;

//Coded by HuRuiFeng.

public class MyTimer{

    //If the Timer is running 
    private bool b_Tricking;

    //Current time
    private float f_CurTime;

    //Time to reach
    private float f_TriggerTime;

    //Use delegate to hold the methods
    public delegate void EventHandler();

    //The trigger event list
    public event EventHandler tick;

	public EventHandler oneTick;

    /// <summary>
    /// Init
    /// </summary>
    /// <param name="second">Trigger Time</param>
    public MyTimer(float second)
    {
        f_CurTime = 0.0f;
        f_TriggerTime = second;
    }
    
	/// <summary>
	/// Gets a value indicating whether this <see cref="MyTimer"/> is tricking.
	/// 判断是否正在计时;
	/// </summary>
	/// <value><c>true</c> if is tricking; otherwise, <c>false</c>.</value>
	public bool isTricking{
		get{
			return b_Tricking;
		}
	}

	public float TriggerTime
	{
		get
		{
			return f_TriggerTime;
		}
	}

    /// <summary>
    /// Start Timer
    /// </summary>
	public void Start()
    {
        b_Tricking = true;
    }
    
    /// <summary>
    /// Update Time
    /// </summary>
    public void Update()
    {
        if (b_Tricking)
        {
            f_CurTime += Time.deltaTime;

            if (f_CurTime > f_TriggerTime)
            {
                //b_Tricking must set false before tick() , cause if u want to restart in the tick() , b_Tricking would be reset to fasle .
                b_Tricking = false;
				if(tick != null)
                	tick();
				if(oneTick != null)
					oneTick();
            }
        }
    }

    /// <summary>
    /// Stop the Timer
    /// </summary>
    public void Stop()
    {
        b_Tricking = false;
    }

    /// <summary>
    /// Continue the Timer
    /// </summary>
    public void Continue()
    {
        b_Tricking = true;
    }

    /// <summary>
    /// Restart the this Timer
    /// </summary>
    public void Restart()
    {
        b_Tricking = true;
        f_CurTime = 0.0f;
    }

    public void Restop()
    {
        b_Tricking = false;
        f_CurTime = 0.0f;
    }

    /// <summary>
    /// Change the trigger time in runtime
    /// </summary>
    /// <param name="second">Trigger Time</param>
    public void ResetTriggerTime(float second)
    {
        f_TriggerTime = second;
    }
}