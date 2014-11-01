using UnityEngine;
using System.Collections;

//UISprite 自适应处理器
public class UIRootAdaptive : MonoBehaviour
{
    public Camera m3DCamera = null;

    public static float DesignWidth = 960.0f;
    public static float DesignHeight = 640.0f;

    public static int manualHeight = 640;

    private static float manualFieldOfView = 48.0f;

    private UIRoot mRoot = null;
    void Start()
    {
        mRoot = gameObject.GetComponent<UIRoot>();
        ResetManualHeight();
    }

	void OnEnable()
	{
        UICamera.onScreenResize += ResetManualHeight;
	}

	void OnDisable()
	{
        UICamera.onScreenResize -= ResetManualHeight;
	}

    void ResetManualHeight()
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        //设计屏幕的尺寸
        float designScale = (DesignWidth / DesignHeight);
        float screenScale = (float)screenWidth / (float)screenHeight;

        //当前屏幕的尺寸比例 小于设计尺寸比例
        if(  screenScale < designScale  )
        {
            float scale = designScale - screenScale;
            mRoot.manualHeight = (int)DesignHeight + (int)(DesignHeight * scale);
        }else
        {
            mRoot.manualHeight = (int)DesignHeight;
        }

        manualHeight = mRoot.manualHeight;


        if( m3DCamera != null )
        {
            m3DCamera.fieldOfView = manualFieldOfView / (DesignHeight / (float)manualHeight);
        }
    }

    void Update()
    {
//        if( mScreenWidth != Screen.width || mScreenHeight != Screen.height )
//        {
//            ResetSprite();
//        }
    }
}
