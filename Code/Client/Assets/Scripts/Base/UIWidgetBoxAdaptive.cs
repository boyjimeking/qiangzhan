using UnityEngine;
using System.Collections;

//自动把 当前Widget的boxcollder 设置为全屏大小
public class UIWidgetBoxAdaptive : MonoBehaviour
{
    private UIWidget mWidget = null;

    private int mScreenWidth = 0;
    private int mScreenHeight = 0;

    private float aspectRatio = 1.0f;
    void Start()
    {
        mWidget = gameObject.GetComponent<UIWidget>();
        ResetWidget();
    }

	void OnEnable()
	{
        UICamera.onScreenResize += ResetWidget;
	}

	void OnDisable()
	{
        UICamera.onScreenResize -= ResetWidget;
	}

    void ResetWidget()
    {
        if (mWidget == null)
        {
            return;
        }

        mScreenWidth = Screen.width;
        mScreenHeight = Screen.height;

        aspectRatio = UIRootAdaptive.manualHeight / (float)mScreenHeight;

        if( mWidget.hasBoxCollider )
        {
            BoxCollider boxCollider = mWidget.collider as BoxCollider;

            if( boxCollider != null )
            {
                UnityEngine.Vector3 size = boxCollider.size;

                size.y = (int)UIRootAdaptive.manualHeight;
                size.x = (int)Mathf.Ceil(mScreenWidth * aspectRatio);

                boxCollider.size = size;
            }
        }

       // mSprite.height = (int)UIRootAdaptive.manualHeight;
       // mSprite.width = (int)Mathf.Ceil(mScreenWidth * aspectRatio);
    }

    void Update()
    {
//        if( mScreenWidth != Screen.width || mScreenHeight != Screen.height )
//        {
//            ResetSprite();
//        }
    }
}
