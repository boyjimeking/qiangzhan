using UnityEngine;
using System.Collections;

public class MallGroupItemUI
{
    //UISprite effectSp;
    //UILabel name;
    UISprite icon;
    //UISprite discountSp;
    UISprite moneySp;//金钱类型;
    UILabel moneyLb;//金钱数量;
    //UILabel orignalPriceLb;//原价数;
    //UILabel limitTimesLb;//限购次数;
    //UISprite limitTimesSp;//限购次数底图;
    UILabel discountLb;
    UILabel countLb;

    UIButton itemBtn;

    private ProceedsType mPt = ProceedsType.Invalid;
    private uint mPrice = 0;

    private uint totalTimes;
    private GameObject mGameObj;

    public UIButton ItemBtn
    {
        get
        {
            return itemBtn;
        }
    }

    public GameObject gameObject
    {
        get
        {
            return mGameObj;
        }
    }


    public MallGroupItemUI(GameObject go)
    {
        //effectSp = ObjectCommon.GetChildComponent<UISprite>(go, "effectSp");
        //name = ObjectCommon.GetChildComponent<UILabel>(go, "nameLb");
        icon = ObjectCommon.GetChildComponent<UISprite>(go, "iconSp");
        //discountSp = ObjectCommon.GetChildComponent<UISprite>(go, "discountSp");
        moneySp = ObjectCommon.GetChildComponent<UISprite>(go, "moneySp");
        moneyLb = ObjectCommon.GetChildComponent<UILabel>(go, "moneyLb");
        //orignalPriceLb = ObjectCommon.GetChildComponent<UILabel>(go, "yuanjiaLb");
        //limitTimesLb = ObjectCommon.GetChildComponent<UILabel>(go, "limitTimesLb");
        //limitTimesSp = ObjectCommon.GetChildComponent<UISprite>(go, "limitTimesSp");
        discountLb = ObjectCommon.GetChildComponent<UILabel>(go , "discountLb");
        countLb = ObjectCommon.GetChildComponent<UILabel>(go, "countLb");
        mGameObj = go;
    }

    public void SetData(MallTableItem node , uint subId)
    {
        ItemTableItem itemItem = ItemManager.GetItemRes(node.itemId);
        if (itemItem == null)
        {
            Debug.LogError("找不到物品id" + node.itemId + "对应的物品");
            return;
        }
        SetIcon(itemItem.picname);
        SetMoneyType((ProceedsType)node.processType);

        if (subId > node.mallItems.Length)
            return;

        MallItemInfo mii = node.mallItems[subId];

        if (mii == null)
            return;

        SetCurPrice(mii.processNow);
        SetDiscount(mii.processOrignal, mii.processNow);
        countLb.text = mii.count.ToString();

        totalTimes = node.limitTimes;
    }

    void SetDiscount(uint yuanjia, uint discount)
    {
        if (discount >= yuanjia)
            discountLb.text = "原价";

        else
        {
            int dis = Mathf.FloorToInt((float)discount / (float)yuanjia * 10f);
            discountLb.text = dis.ToString() + "折";
        }
    }

    void SetIcon(string spriteInfo)
    {
        UIAtlasHelper.SetSpriteImage(icon, spriteInfo);
    }

    void SetMoneyType(ProceedsType type)
    {
        mPt = type;
        UIAtlasHelper.SetSpriteByMoneyType(moneySp, type, true);
        moneySp.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
    }

    void SetCurPrice(uint price)
    {
        mPrice = price;
        
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (pdm == null)
            return;

        if (pdm.GetProceeds(mPt) >= mPrice)
            moneyLb.text = price.ToString();
        else
            moneyLb.text = StringHelper.StringWithColor(FontColor.Red , price.ToString());
    }
}
