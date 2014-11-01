using UnityEngine;
using System.Collections;

public enum TriggerType : int
{
	Base = 0,
	Growth = 1,
	Function = 2,
}

public class BaseTrigger
{
    public string name = "";

    protected bool mRunning = false;

	protected BaseScene mScene = null;

	protected TriggerType type = TriggerType.Base; 

	public BaseTrigger(BaseScene scn, TriggerType t = TriggerType.Base)
	{
		mScene = scn;
		type = t;
	}

	virtual public void Update(uint elapsed)
    {
	
	}

    public void Start()
    {
        mRunning = true;

    }

    public void Stop()
    {
        mRunning = false;
    }

    public bool IsRunning()
    {
        return mRunning;
    }

	virtual public void Destroy()
	{

	}

    virtual public void Restart()
    {
        mRunning = true;
    }
   
	public bool IsTrigger(TriggerType t)
	{
		return t == type;
	}
}
