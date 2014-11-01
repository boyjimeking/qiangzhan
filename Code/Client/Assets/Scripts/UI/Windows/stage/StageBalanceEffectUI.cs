
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class StageBalanceEffectUI : UIWindowChild
{
	public GameObject mEffect0;
	public GameObject mEffect1;
	public GameObject mEffect2;
	public GameObject mEffect3;

	private bool mEnable = false;

	private float mTimer = 0.0f;

	private float mDistanceShort = 117.5f;
	private float mDistanceLong = 352.5f;

    public delegate void FinishCall(StageBalanceEffectUI effect);

    public FinishCall onFinish = null;

    private GameObject mObj = null;
	public StageBalanceEffectUI( GameObject obj )
    {
        mObj = obj;
        mEffect0 = ObjectCommon.GetChild(mObj,"Effect0");
        mEffect1 = ObjectCommon.GetChild(mObj, "Effect1");
        mEffect2 = ObjectCommon.GetChild(mObj, "Effect2");
        mEffect3 = ObjectCommon.GetChild(mObj, "Effect3");
    }

    public void Open()
    {
        mEnable = true;
        mObj.SetActive(true);
    }

    public void Stop()
    {
        mObj.SetActive(false);
        mEnable = false;
        mEffect0.transform.localPosition = Vector3.zero;
        mEffect1.transform.localPosition = Vector3.zero;
        mEffect2.transform.localPosition = Vector3.zero;
        mEffect3.transform.localPosition = Vector3.zero;
    }

    public void Update(uint elapsed)
	{
		if(!mEnable)
			return;

		mTimer += ((float)elapsed / 1000.0f);
		if(mTimer > 1.0f)
		{
			mEnable = false;
			mTimer = 0.0f;

			mEffect0.transform.localPosition = new Vector3(-mDistanceLong, 0.0f, 0.0f);
			mEffect1.transform.localPosition = new Vector3(-mDistanceShort, 0.0f, 0.0f);
			mEffect2.transform.localPosition = new Vector3(mDistanceShort, 0.0f, 0.0f);
			mEffect3.transform.localPosition = new Vector3(mDistanceLong, 0.0f, 0.0f);

            if (onFinish != null)
            {
                onFinish(this);
                Stop();
                return;
            }
		}
		else
		{
			mEffect0.transform.localPosition = new Vector3((-mDistanceLong) * mTimer, 0.0f, 0.0f);
			mEffect1.transform.localPosition = new Vector3((-mDistanceShort) * mTimer, 0.0f, 0.0f);
			mEffect2.transform.localPosition = new Vector3(mDistanceShort * mTimer, 0.0f, 0.0f);
			mEffect3.transform.localPosition = new Vector3(mDistanceLong * mTimer, 0.0f, 0.0f);
		}
	}
}
