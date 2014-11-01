using UnityEngine;
using System.Collections;

/// <summary>
/// 动态物体的动画.
/// </summary>
using System.Collections.Generic;


public class DynamicAnimation : MonoBehaviour
{
    public delegate void StopToTargetCallBack(string name);
    public StopToTargetCallBack TargetStopCallBack;
	public float movdirx = 1.0f;
	public float movdiry = 0.0f;
	public float movdirz = 0.0f;
	public float movspeed = 25.0f;
	public float offset = 25f;
	public float length = 69f;
	//private int howMuchToCopy = 2;

	private List<DynamicCell> celllist = new List<DynamicCell>();

	private List<DynamicCell> copylist = new List<DynamicCell>();

	private float dynamicbkMoveDistance = 0;
    private float runtimeCorrection = 0;

    private bool mInvalidToTarget = false;
    private bool mPause = false;
	// Use this for initialization
	void Start () 
	{

		DynamicCell[] cells = transform.GetComponentsInChildren<DynamicCell>();


		for(int i=0; i < 3; ++i)
		{
			if(i == 1)
			{
				foreach(DynamicCell cell in cells)
				{
					celllist.Add(cell);
				}
				continue;
			}
			foreach(DynamicCell cell in cells)
			{
				DynamicCell copyCell = GameObject.Instantiate(cell) as DynamicCell;
				copyCell.transform.parent = transform;
				copylist.Add(copyCell);
				celllist.Add(copyCell);
				Vector3 trans = new Vector3(movdirx,movdiry,movdirz);
				Vector3 voffset = trans * offset;
				trans *= length * (i-1);
				trans += voffset;

				copyCell.transform.localPosition += trans;
				copyCell.name = "group"+i.ToString();
			}

		}
	
	}

	void OnDestroy()
	{
		copylist.Clear();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
        if (mPause)
            return;
		Vector3 movDir = new Vector3(movdirx,movdiry,movdirz);
		movDir *= movspeed;
		movDir *= Time.fixedDeltaTime;

        movDir += new Vector3(movdirx,movdiry,movdirz)*runtimeCorrection;
		dynamicbkMoveDistance += movDir.magnitude;
        runtimeCorrection = 0;

		foreach(DynamicCell cell in celllist)
		{
			cell.transform.localPosition += movDir;

            if(mInvalidToTarget)
            {
                if(Mathf.Abs(cell.transform.localPosition.magnitude-cell.stopPoint.magnitude) < 0.1f)
                {
                    mInvalidToTarget = false;
                    mPause = true;
                    CameraController.Instance.ShakeCamera(8, 1);
                    if (TargetStopCallBack != null)
                        TargetStopCallBack(cell.pointname);
                }
            }
		}

		if(dynamicbkMoveDistance >= length)
		{
			if(celllist.Count > 1)
			{
				DynamicCell cell = celllist[celllist.Count-1];
				celllist.RemoveAt(celllist.Count -1);

				Vector3 trans = new Vector3(movdirx,movdiry,movdirz);
				trans *= length * -1;
				cell.transform.localPosition = celllist[0].transform.localPosition + trans;
				celllist.Insert(0,cell);
			}
			dynamicbkMoveDistance -= length;
		}
	}

    //找到合适的转移位置
    public void MakeRuntimeCorrection()
    {
        runtimeCorrection = 0;
        Vector3 v = Vector3.zero;
        Vector3 dir = new Vector3(movdirx,movdiry,movdirz);
        DynamicCell rCell = null;
        foreach (DynamicCell cell in celllist)
        {
            Vector3 tDir = cell.stopPoint - cell.transform.localPosition;
            if(tDir.normalized.Equals(dir.normalized))
            {
                if(rCell != null)
                {
                    if ((rCell.stopPoint - cell.transform.localPosition).magnitude > tDir.magnitude)
                        rCell = cell;
                }
                else
                {
                    rCell = cell;
                }
            }
        }

        if(rCell != null)
        {
            runtimeCorrection = (rCell.stopPoint - rCell.transform.localPosition).magnitude;
          if (runtimeCorrection > 10)
              runtimeCorrection -= 10;
        }
        
    }

    public void InvalidToTarget()
    {
        mInvalidToTarget = true;
        MakeRuntimeCorrection();
    }
    public void Play()
    {
        mPause = false;
    }
    public void Pause()
    {
        mPause = true;
    }
}
