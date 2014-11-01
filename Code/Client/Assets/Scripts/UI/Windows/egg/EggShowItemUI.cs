using UnityEngine;
using System.Collections;

class EggShowItemUI
{
    protected UISprite itemicon;

    private CommonItemUI mItemUI;

    private GameObject mGo;

    public EggShowItemUI(GameObject go)
    {
        itemicon = ObjectCommon.GetChildComponent<UISprite>(go, "icon");

        mGo = go;
    }

    public GameObject go
    {
        get
        {
            return mGo;
        }
    }

    public void SetImg(string spriteinfo, bool pixePerfect = false)
    {
        UIAtlasHelper.SetSpriteImage(itemicon, spriteinfo, pixePerfect);
    }

    public void SetInfo(int itemId)
    {
        mItemUI = new CommonItemUI(itemId);
        
        mItemUI.gameObject.transform.parent = mGo.transform;
        mItemUI.gameObject.transform.localScale = Vector3.one;
    }
}