using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 场景摄像机动画路径管理器
/// </summary>
public class SceneCameraPathManager
{
    private Dictionary<string, CameraPathAnimator> mCameraPaths = new Dictionary<string, CameraPathAnimator>();

    public void InitPath(CameraPathAnimator[] paths)
    {
        //foreach (CameraPathAnimator path in paths)
        //{
        //    //添加进来的摄像机动画路径需要把路径上的事件指定到此类里以便于此类进行事件的派发
        //    path.AnimationCustomEvent += OnCustomEvent;
        //    path.AnimationFinishedEvent += OnFinish;

        //    mCameraPaths.Add(path.name, path);
        //}
    }

    public void OnCustomEvent(string eventname)
    {
        GameDebug.Log(eventname);
		EventSystem.Instance.PushEvent(new CameraAnimatorEvent(CameraAnimatorEvent.CAMERA_EVENT_POINT_TRIGGER, eventname));
    }
    public void OnFinish(string pathname)
    {
		EventSystem.Instance.PushEvent(new CameraAnimatorEvent(CameraAnimatorEvent.CAMERA_ANIMATOR_FINISH, pathname));
    }
    public void AddPath(string name,CameraPathAnimator path)
    {
        if (mCameraPaths.ContainsKey(name))
            return;
        mCameraPaths.Add(name, path);

    }

    public CameraPathAnimator GetPath(string name)
    {
        if (!mCameraPaths.ContainsKey(name))
            return null;
        return mCameraPaths[name];
    }

}

