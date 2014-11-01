using UnityEngine;
using System.Collections;

//UISprite 自适应处理器
public class UISpriteAdaptive : MonoBehaviour
{
    private UISprite mSprite = null;

    private int mScreenWidth = 0;
    private int mScreenHeight = 0;

    private float aspectRatio = 1.0f;
    void Start()
    {
        mSprite = gameObject.GetComponent<UISprite>();
        ResetSprite();
    }

	void OnEnable()
	{
		UICamera.onScreenResize += ResetSprite;
	}

	void OnDisable()
	{
		UICamera.onScreenResize -= ResetSprite;
	}

    void ResetSprite()
    {
        if( mSprite == null )
        {
            return;
        }

        mScreenWidth = Screen.width;
        mScreenHeight = Screen.height;

        aspectRatio = UIRootAdaptive.manualHeight / (float)mScreenHeight;

        mSprite.height = (int)UIRootAdaptive.manualHeight;
        mSprite.width = (int)Mathf.Ceil(mScreenWidth * aspectRatio);
    }

    void Update()
    {
//        if( mScreenWidth != Screen.width || mScreenHeight != Screen.height )
//        {
//            ResetSprite();
//        }
    }
}
