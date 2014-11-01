using UnityEngine;
using System.Collections;

public class MallItemUI
{
    protected UISprite effectSp;
    protected UILabel name;
    protected UISprite icon;
    protected UISprite discountSp;      //打折底图;
    protected UILabel discountLb;       //打几折;
    protected UISprite moneySp;         //金钱类型;
    protected UILabel moneyLb;          //金钱数量;
    protected UILabel orignalPriceLb;   //原价数;
    protected UISprite orignalPriceSp;  //原价背景框;
    protected UILabel limitTimesLb;     //限购次数;
    protected UISprite limitTimesSp;    //限购次数底图;
    protected UILabel countLb;          //物品个数;
    protected UISprite huiSp;           //灰色蒙版;
    protected UILabel nullLb;           //暂无商品;

    UIButton itemBtn;

    private uint totalTimes;
    private uint curPrice = 0;
    private bool mCanOpenBuyForm = true;

    private ProceedsType mPt = ProceedsType.Invalid;
    private uint mPrice = 0;

    protected GameObject mGameObj;

    public bool MallCanOpenBuyForm
    {
        get 
        {
            return mCanOpenBuyForm;
        }
    }

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


    public MallItemUI(GameObject go)
    {
        effectSp = ObjectCommon.GetChildComponent<UISprite>(go , "effectSp");
        name = ObjectCommon.GetChildComponent<UILabel>(go , "nameLb");
        icon = ObjectCommon.GetChildComponent<UISprite>(go, "itemIconSp");
        discountSp = ObjectCommon.GetChildComponent<UISprite>(go, "discountSp");
        discountLb = ObjectCommon.GetChildComponent<UILabel>(go, "discountLb");
        moneySp = ObjectCommon.GetChildComponent<UISprite>(go, "moneySp");
        moneyLb = ObjectCommon.GetChildComponent<UILabel>(go, "moneyLb");
        orignalPriceLb = ObjectCommon.GetChildComponent<UILabel>(go, "yuanjiaLb");
        orignalPriceSp = ObjectCommon.GetChildComponent<UISprite>(go, "yuanjiaSp");
        limitTimesLb = ObjectCommon.GetChildComponent<UILabel>(go, "limitTimesLb");
        limitTimesSp = ObjectCommon.GetChildComponent<UISprite>(go, "limitTimesSp");
        countLb = ObjectCommon.GetChildComponent<UILabel>(go, "countLb");
        huiSp = ObjectCommon.GetChildComponent<UISprite>(go, "huiSp");
        nullLb = ObjectCommon.GetChildComponent<UILabel>(go, "nullLb");

        mGameObj = go;
    }

    public bool IsShowDiscountSp
    {
        get
        {
            return discountSp.gameObject.activeSelf;
        }

        set
        {
            discountSp.gameObject.SetActive(value);
        }
    }

    public void SetData(MallTableItem node)
    {
        if (node == null)
        {
            SetName("");

            huiSp.gameObject.SetActive(true);
            nullLb.gameObject.SetActive(true);

            icon.gameObject.SetActive(false);
            moneyLb.gameObject.SetActive(false);
            moneySp.gameObject.SetActive(false);
            countLb.gameObject.SetActive(false);
            discountSp.gameObject.SetActive(false);
            discountLb.gameObject.SetActive(false);
            effectSp.gameObject.SetActive(false);
            orignalPriceLb.gameObject.SetActive(false);
            orignalPriceSp.gameObject.SetActive(false);
            limitTimesLb.gameObject.SetActive(false);
            limitTimesSp.gameObject.SetActive(false);

            return;
        }

        ItemTableItem itemItem = ItemManager.GetItemRes(node.itemId);
        if (itemItem == null)
        {
            Debug.LogError("找不到物品id" + node.itemId + "对应的物品");
            return;
        }
        //SetName(itemItem.name);
        SetName(ItemManager.getItemNameWithColor(node.itemId));
        SetIcon(itemItem.picname);
        SetMoneyType((ProceedsType)node.processType);

        uint priceOrig = node.mallItems[0].processOrignal;
        uint priceNow = node.mallItems[0].processNow;

        curPrice = priceNow;

        SetOrignalPrice(priceOrig);
        SetCurPrice(priceNow);

        totalTimes = node.limitTimes;
        switch ((MallLimitType)node.limitType)
        {
            case MallLimitType.NONE:
                limitTimesLb.gameObject.SetActive(false);
                limitTimesSp.gameObject.SetActive(false);
                break;
            case MallLimitType.DAY:
            case MallLimitType.FOREVER:
                limitTimesLb.gameObject.SetActive(true);
                limitTimesSp.gameObject.SetActive(true);
                break;
        }
        countLb.text = "x" + node.mallItems[0].count.ToString();
        int saleNum = (int)((float)priceNow / (float)priceOrig * 10f);
        discountLb.text = saleNum + "折";

        bool isShow = priceNow < priceOrig;

        discountSp.gameObject.SetActive(isShow);
        discountLb.gameObject.SetActive(isShow);
        effectSp.gameObject.SetActive(isShow);
        orignalPriceLb.gameObject.SetActive(false);
        orignalPriceSp.gameObject.SetActive(false);
        nullLb.gameObject.SetActive(false);
    }

    public void ShowHuiSprite(bool isShow)
    {
 
        huiSp.gameObject.SetActive(isShow);
    }

    protected void SetName(string text)
    {
        name.text = text;
    }
    protected void SetIcon(string spriteInfo)
    {
        UIAtlasHelper.SetSpriteImage(icon, spriteInfo, true);
    }

    protected void SetMoneyType(ProceedsType type)
    {
        mPt = type;
        UIAtlasHelper.SetSpriteByMoneyType(moneySp, type, true);
        moneySp.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
    }

    protected void SetCurPrice(uint price)
    {
        mPrice = price;
        UpdateMoneyShow();
    }

    protected void SetOrignalPrice(uint price)
    {
        orignalPriceLb.text = "原价：" + price.ToString();
    }

    /// <summary>
    /// 已经购买次数和总共次数;
    /// </summary>
    /// <param name="buyTimes"></param>
    /// <param name="totalTimes"></param>
    public void SetBuyTimes(uint buyTimes)
    {
        limitTimesLb.text = "购买次数" + buyTimes.ToString() + "/" + totalTimes.ToString();

        mCanOpenBuyForm = !(buyTimes >= totalTimes);

        SetBuyDone(buyTimes >= totalTimes);
    }

    public void UpdateMoneyShow()
    {
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (pdm == null)
            return;

        if (pdm.GetProceeds(mPt) >= mPrice)
            moneyLb.text = mPrice.ToString();
        else
            moneyLb.text = StringHelper.StringWithColor(FontColor.Red, mPrice.ToString());
    }

    private void SetBuyDone(bool isDone)
    {
        BoxCollider boc = gameObject.GetComponent<BoxCollider>();
        if (isDone)
        {
            moneyLb.text = "已售罄";
            //boc.enabled = false;
            huiSp.gameObject.SetActive(true);
        }
        else
        {
            SetCurPrice(curPrice);
            //boc.enabled = true;
            huiSp.gameObject.SetActive(false);
        }
    }
}
