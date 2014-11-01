using UnityEngine;
using System.Collections;
using System.IO;
//主游戏入口
public class GameInitialize : MonoBehaviour
{
	private GameApp	mGameApp = null;
	private bool hasOnLoad = false;

    private double mLastTime = System.DateTime.Now.TimeOfDay.TotalMilliseconds;


    private static GameInitialize mOnlyInstance = null;

	void Awake()
	{
#if !UNITY_EDITOR && UNITY_ANDROID
		ApplicationUtility.instance.GetAppName();
#endif
        if (mOnlyInstance == null) 
		{
            mOnlyInstance = this;

           //初始化系统信息
           AppSystemInfo.InitSysInfos();
			OnLoad();
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
	}

	void Start () 
    {

	}

	void OnLoad()
	{
        Application.targetFrameRate = 60;

		mGameApp = new GameApp();

		DontDestroyOnLoad(this);
		
		mGameApp.Init();

        //加载登陆场景
	}

    void Update()
    {
        double nowTime = System.DateTime.Now.TimeOfDay.TotalMilliseconds;
        uint elapsed = (uint)(nowTime - mLastTime);
        mLastTime = nowTime;

		if( mGameApp != null )
            mGameApp.Update(elapsed);
    }
}
