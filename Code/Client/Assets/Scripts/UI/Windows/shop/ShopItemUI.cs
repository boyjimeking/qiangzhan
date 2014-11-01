using UnityEngine;
using System.Collections;

public class ShopItemUI : MallItemUI
{
    private int resId = -1;
    private ShopTableItem mitem;
    private ShopItemInfo mInfo = null;

    private ProceedsType mPt = ProceedsType.Invalid;
    private uint mCost = 0;

    private bool isScretItemBuyDone = false;
    public bool IsScretItemBuyDone
    {
        get
        {
            return isScretItemBuyDone;
        }
    }

    public ShopItemUI(GameObject go):base(go)
    {

    }


    public void SetData(ShopTableItem node)
    {
        if (node == null)
        {
            resId = -1;
            mitem = null;

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

        resId = node.resId;
        mitem = node;

        SetName(ItemManager.getItemNameWithColor(node.itemId));
        SetIcon(itemItem.picname);

        if (node.subTable != (int)ShopSubTable.Sceret)
        {
            uint count = node.minCount * node.multiple;
            
            mPt = (ProceedsType)(node.proceedType0);
            mCost = (uint)node.perPrice0 * count;

            //UIAtlasHelper.SetSpriteByMoneyType(moneySp, mPt);
            SetMoneyType(mPt);
            SetCurPrice(mCost);
            countLb.text = "x" + count;

            mInfo = new ShopItemInfo();
            mInfo.proceedsTypeIdx = 0;
            mInfo.isBuyDone = false;
            mInfo.count = (int)node.minCount;

        }
        nullLb.gameObject.SetActive(false);
        huiSp.gameObject.SetActive(false);
        discountSp.gameObject.SetActive(false);
        discountLb.gameObject.SetActive(false);
        effectSp.gameObject.SetActive(false);
        orignalPriceLb.gameObject.SetActive(false);
        orignalPriceSp.gameObject.SetActive(false);
        limitTimesLb.gameObject.SetActive(false);
        limitTimesSp.gameObject.SetActive(false);
    }

    /// <summary>
    /// ProceedsTypeIdx [0-2]对应到shop表格中的货币类型索引下标;
    /// </summary>
    /// <param name="type"></param>
    /// <param name="count"></param>
    /// <param name="isBuy"></param>
    public void UpdateData(ShopItemInfo info)
    {
        if (info == null)
            return;

        mInfo = info;
        UpdateData(info.proceedsTypeIdx, info.count, info.isBuyDone);
    }

    void UpdateData(int ProceedsTypeIdx, int count , bool isBuy)
    {
        if(mitem == null)
        {
            GameDebug.LogError("请先初始化在进行刷新操作");
            return;
        }

        if (ProceedsTypeIdx < 0 || ProceedsTypeIdx > 2)
        {
            GameDebug.LogError("商店数据错误");
            return;
        }

        int proceedsType = ShopModule.GetShopItemProceedsType(mitem , ProceedsTypeIdx);
        int perPrice = ShopModule.GetShopItemPerPrice(mitem , ProceedsTypeIdx);

        mPt = (ProceedsType)proceedsType;

        SetMoneyType(mPt);
        int itemNumber = (int)(count * mitem.multiple);
        countLb.text = "x" + itemNumber;

        mCost = (uint)(perPrice * itemNumber);

        if (isBuy)
        {
            moneyLb.text = "已售罄";
        }
        else
        {
            PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
            if (pdm == null)
                return;

            if (pdm.GetProceeds(mPt) >= mCost)
                moneyLb.text = mCost + "";
            else
                moneyLb.text = StringHelper.StringWithColor(FontColor.Red, mCost.ToString());
        }
        huiSp.gameObject.SetActive(isBuy);
        isScretItemBuyDone = isBuy;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pt"></param>
    /// <param name="cost"></param>
    /// <returns></returns>
    public bool GetProcTypeAndCost(ref ProceedsType pt, ref int cost)
    {
        pt = mPt;
        cost = (int)mCost;

        //ShopTableItem item = DataManager.ShopTable[this.resId] as ShopTableItem;
        //if (item == null)
        //    return false;

        //ShopModule module = ModuleManager.Instance.FindModule<ShopModule>();
        //if (module == null)
        //    return false;

        //if (item.subTable == (int)ShopSubTable.Sceret)
        //{
        //    pt = (ProceedsType)ShopModule.GetShopItemProceedsType(this.resId, mInfo.proceedsTypeIdx);
        //    int perPrice = ShopModule.GetShopItemPerPrice(this.resId, mInfo.proceedsTypeIdx);
        //    cost = (int)(mInfo.count * item.multiple * perPrice);
        //}
        //else
        //{
        //    pt = (ProceedsType)ShopModule.GetShopItemProceedsType(this.resId, 0);
        //    int perPrice = ShopModule.GetShopItemPerPrice(this.resId, 0);
        //    cost = (int)(mInfo.count * item.multiple * perPrice);
        //}

        return true;
    }
}
