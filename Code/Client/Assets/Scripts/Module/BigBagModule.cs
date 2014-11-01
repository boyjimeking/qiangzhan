using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Message;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class BigBagModle : ModuleBase
{
    public const int MAX_DATA_NUM = 7;
    public const int MIN_DATA_NUM = 1;
    public enum BUTTON_STATE : int
    {
        Invalid = 0,
        Have_yes = 1,//已领取
        Have_no = 2,//未领取
        Replenish_yes = 3,//已补签
        Replenish_no = 4,//未补签
    }

    public Dictionary<int, BigItemUnit> mItemDic = new Dictionary<int, BigItemUnit>();

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable(); 
    }

    public string getText(BigBagModle.BUTTON_STATE state, UIButton Btn)
    {
        switch (state)
        {
            case BigBagModle.BUTTON_STATE.Have_no:
                UIAtlasHelper.SetButtonImage(Btn, "common:btn_blue_4word");
                return "领取";

            case BigBagModle.BUTTON_STATE.Replenish_no:
                UIAtlasHelper.SetButtonImage(Btn, "common:btn_yellow_4word");
                return "补领";
        }

        return "";
    }

    public ArrayList getBigBagList(int week)
    {
        SevenTableItem seven = DataManager.SevenTable[week] as SevenTableItem;
        if (null == seven)
        {
            GameDebug.LogError("seven_award.txt 配置错误 Id为：" + week);
            return null;
        }

        ArrayList itemList = new ArrayList();
        if (DropManager.Instance.GenerateDropBox(seven.droupId, out itemList))
        {
            return itemList;
        }

        return null;
    }
}
