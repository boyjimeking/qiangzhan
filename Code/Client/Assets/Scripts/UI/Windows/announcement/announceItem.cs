using UnityEngine;
using System.Collections;
public class AnnounceItem
{
    public int itemWidth = 0;
    public int IconWidth = 0;
    public int TextWidth = 0;
    public UILabel mText;
    private GameObject mObj;
    public UISprite mIcon;
    public AnnounceItem(GameObject go)
    {
        mObj = go;
        if (go != null)
        {
            mText = ObjectCommon.GetChildComponent<UILabel>(go, "mText");
            mIcon = ObjectCommon.GetChildComponent<UISprite>(go, "mIcon");
        }
    }

    public void SetText(string text)
    {
        if (mText)
        {
            mText.text = text;
        }
    }

    public void SetIcon(string Icon = "")
    {
        if (mIcon)
        {
            UIAtlasHelper.SetSpriteImage(mIcon, Icon);
        }
    }
    public GameObject gameObject
    {
        get { return mObj; }
    }
}