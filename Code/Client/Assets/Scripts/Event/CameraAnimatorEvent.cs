

/// <summary>
/// 摄像机动画事件
/// </summary>
public class CameraAnimatorEvent : EventBase
{
    // 摄像机动画完成
    public static string CAMERA_ANIMATOR_FINISH = "cameraanimatorfinish";

	// 摄像机事件点触发
	public static string CAMERA_EVENT_POINT_TRIGGER = "cameraeventpointtrigger";

	// 完成的摄像机路径名/触发的事件点名称
	public string mName = null;

    public CameraAnimatorEvent(string eventName, string name)
        : base(eventName)
    {
		mName = name;
    }
}

