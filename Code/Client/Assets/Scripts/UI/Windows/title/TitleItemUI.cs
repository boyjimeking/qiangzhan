using UnityEngine;
using System.Collections;

public class TitleItemUI 
{
    protected UILabel mName;
    protected UISprite mGet;
    protected UISprite mBg;

    private GameObject mGo;

    public TitleItemUI(GameObject go)
    {
        mName = ObjectCommon.GetChildComponent<UILabel>(go, "name");
        mGet = ObjectCommon.GetChildComponent<UISprite>(go, "getSp");
        mBg = ObjectCommon.GetChildComponent<UISprite>(go, "selectSp"); ;

        mGo = go;
    }

    public void SetData(TitleItemTableItem item)
    {
        if (item == null)
            return;

        mName.text = item.name;
    }

    public void SetSelected(bool isSelected)
    {
        mBg.gameObject.SetActive(isSelected);
    }

    public void SetIsHave(bool isHave)
    {
        mGet.gameObject.SetActive(isHave);
    }
}
