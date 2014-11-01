using UnityEngine;
using System.Collections;

public class EggGoodsUI
{
    protected UISprite iconsp;
    protected UILabel countlb;
    protected UISprite anisp;

    private GameObject mGo;

    public GameObject Obj
    {
        get
        {
            return mGo;
        }
    }

    public EggGoodsUI(GameObject go)
    {
        iconsp = ObjectCommon.GetChildComponent<UISprite>(go , "icon");
        countlb = ObjectCommon.GetChildComponent<UILabel>(go, "count");
        anisp = ObjectCommon.GetChildComponent<UISprite>(go, "ani");

        mGo = go;
    }

    public void SetData(int itemId, int count,bool isShowAni = false , bool isShowNum = false)
    {
        ItemTableItem item = ItemManager.GetItemRes(itemId) as ItemTableItem;

        if (item == null)
            return;

        UIAtlasHelper.SetSpriteImage(iconsp, item.picname);
        countlb.text = "X" + count;

        countlb.gameObject.SetActive(isShowNum);
        anisp.gameObject.SetActive(isShowAni);
    }

    public void DestroySelf()
    {
        GameObject.Destroy(mGo);
    }
}
