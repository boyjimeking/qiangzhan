using UnityEngine;
using System.Collections;

public class ChargeItemInfo
{
    public string title;
    public int itemId;
    public int count;

    public ChargeItemInfo(int _itemId, int _count ,string _title = "")
    {
        itemId = _itemId;
        count = _count;
        title = _title;
    }
}

public class ChargeItemUI
{
    protected UILabel TitleLabel = null;
    protected UILabel NameLabel = null;
    protected UILabel CountLabel = null;
    protected UISprite IconSprite = null;
    protected UISprite GetDoneSprite = null;

    private GameObject mGo = null;
    private UIButton mBtn = null;
    private ChargeItemInfo mInfo = null;

    public GameObject gameObject
    {
        get
        {
            return mGo;
        }
    }

    public UIButton button
    {
        get
        {
            if(mBtn == null)
                mBtn = mGo.GetComponent<UIButton>();

            return mBtn;
        }
    }

    public ChargeItemUI(ChargeItemInfo info)
    {
        mInfo = info;
        if(mGo == null)
        {
            mGo = WindowManager.Instance.CloneCommonUI("ChargeItem");
        }

        if (mGo == null)
            return;

        TitleLabel = ObjectCommon.GetChildComponent<UILabel>(mGo, "title");
        NameLabel = ObjectCommon.GetChildComponent<UILabel>(mGo, "name");
        CountLabel = ObjectCommon.GetChildComponent<UILabel>(mGo, "number");
        IconSprite = ObjectCommon.GetChildComponent<UISprite>(mGo, "icon");
        GetDoneSprite = ObjectCommon.GetChildComponent<UISprite>(mGo, "getdone");

        EventDelegate.Add(button.onClick, onItemClick);

        SetData(info);
    }

    void onItemClick()
    {
        openItemInfoUI(mInfo.itemId);
    }

    void openItemInfoUI(int itemId)
    {
        ItemInfoParam param = new ItemInfoParam();
        param.itemid = itemId;
        WindowManager.Instance.OpenUI("iteminfo", param);
    }

    void SetData(ChargeItemInfo info)
    {
        if (info == null)
        {
            IconSprite.spriteName = "";
            CountLabel.text = "";
            TitleLabel.text = "";
 
            return;
        }

        ItemTableItem item = ItemManager.GetItemRes(info.itemId);
        if (item == null)
        {
            Debug.LogError("物品表中不存在的物品id=" + info.itemId);
            return;
        }

        TitleLabel.text = info.title;
        CountLabel.text = info.count.ToString();

        NameLabel.text = ItemManager.getItemNameWithColor(info.itemId);
        UIAtlasHelper.SetSpriteImage(IconSprite, item.picname, true);
    }

    public void IsGetDone(bool getDone)
    {
        GetDoneSprite.gameObject.SetActive(getDone);
    }

    public void UpdateData(ChargeItemInfo info)
    {
        SetData(info);
    }
}
