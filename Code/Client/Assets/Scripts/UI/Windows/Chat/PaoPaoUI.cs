using UnityEngine;
using System.Collections;

public class PaoPaoUI  {

    public UISprite mSprite = null;
    public UILabel mLabel = null;

    private int mMinWidth = 210;
    private int mMinHeight = 120;

    private int GapWidth = 50;
    private int GapHeight = 60;

    private float LableOffset = 11.0f;


    private static int msMaxWidth = 150;



    private GameObject mObj = null;
	public PaoPaoUI(GameObject obj )
    {
        mObj = obj;

        mSprite = mObj.GetComponent<UISprite>();
        mLabel = ObjectCommon.GetChildComponent<UILabel>(obj, "Label");

//         UIDynamicFace dface = new UIDynamicFace();
// 
//         dface.SetAtlas(UIResourceManager.Instance.GetAtlas("Face"));
//         for (int i = 0; i < 60; ++i)
//         {
//             string key = (i + 1).ToString();
//             dface.AddSymbol("#" + key, key);
//         }
//         mLabel.SetDynamicFace(dface);
    }
    public GameObject gameObject
    {
        get
        {
            return mObj;
        }
    }

    public int MinWidth
    {
        get
        {
            return mMinWidth;
        }
        set
        {
            mMinWidth = value;
        }
    }

    public int MinHeight
    {
        get
        {
            return mMinHeight;
        }
        set
        {
            mMinHeight = value;
        }
    }

    public void SetGap(int w , int h)
    {
        GapWidth = w;
        GapHeight = h;
    }
    public void SetColor(uint color)
    {
        mLabel.color = NGUIMath.HexToColor(color);
    }

    public void SetLableOffset(float offset)
    {
        LableOffset = offset;
    }

    public void SetText(string txt)
    {
        mLabel.text = txt;

        mLabel.MakePixelPerfect();
        int width = mLabel.width;

        if( width > msMaxWidth )
        {
            int value = width / msMaxWidth;
            value = txt.Length / value;
            int n = 1;
            string tmp = "";
            for( int i = 0 ; i < txt.Length ;++i )
            {
                if (n * value == i)
                {
                    tmp += '\n';
                    n++;
                }
                tmp += txt[i];
            }

            mLabel.text = tmp;
            mLabel.MakePixelPerfect();
        }

        width = mLabel.width;
        int height = mLabel.height;

        int spr_width = width + GapWidth;
        int spr_height = height + GapHeight ;

        if( spr_width < mMinWidth )
            spr_width = mMinWidth;
        if( spr_height < mMinHeight )
            spr_height = mMinHeight;

        Vector3 pos = mLabel.gameObject.transform.localPosition;
        pos.y = LableOffset;
        mLabel.gameObject.transform.localPosition = pos;

        mSprite.width = spr_width;
        mSprite.height = spr_height;
    }

    public void SetAlpha(float alpha)
    {
        mSprite.alpha = alpha;
    }
	
    public int GetWidth()
    {
        return mSprite.width;
    }

    public int GetHeight()
    {
        return mSprite.height;
    }
}
