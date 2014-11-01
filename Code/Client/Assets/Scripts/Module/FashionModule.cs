
using System;
using System.Runtime.Remoting.Messaging;
using Message;

public class FashionModule:ModuleBase
 {
     protected override void OnEnable()
     {
         base.OnEnable();
         EventSystem.Instance.addEventListener(FashionEvent.FASHION_ACTIVE, OnRespondActive);
         EventSystem.Instance.addEventListener(FashionEvent.FASHION_ADDSTAR,OnRespondAddstar);
         EventSystem.Instance.addEventListener(FashionEvent.FASHION_EQUIP, OnRespondEquip);
     }

     protected override void OnDisable()
     {
         base.OnDisable();
         EventSystem.Instance.removeEventListener(FashionEvent.FASHION_ACTIVE, OnRespondActive);
         EventSystem.Instance.removeEventListener(FashionEvent.FASHION_ADDSTAR, OnRespondAddstar);
         EventSystem.Instance.removeEventListener(FashionEvent.FASHION_EQUIP, OnRespondEquip);
     }

    private void OnRespondActive(EventBase evt)
    {
        FashionEvent fevt = evt as FashionEvent;
    }

    private void OnRespondAddstar(EventBase evt)
    {
        FashionEvent fevt = evt as FashionEvent;
    }

    private void OnRespondEquip(EventBase evt)
    {
        FashionEvent fevt = evt as FashionEvent;
        //调用player接口
       Player ply = PlayerController.Instance.GetControlObj() as Player;
       ply.ChangeFashion(fevt.mfashionid,fevt.actionid);
       

    }
         
     public override void Update(uint elapsed)
     {
         base.Update(elapsed);
     }

    public void RequestActiveFashion(int fashionid)
    {
        if (CheckCostEnough(fashionid, false))
        {
            Net.Instance.DoAction((int)MESSAGE_ID.ID_MSG_FASHION_ACTIVE, fashionid);
        }
    }

    public bool RequestAddStar(int fashionid)
    {
        if (CheckCostEnough(fashionid, true))
        {
            Net.Instance.DoAction((int)MESSAGE_ID.ID_MSG_FASHION_ADDSTAR, fashionid);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fashionid"></param>
    /// <param name="action"> 1 是装备 2 是脱下</param>
    public void RequestEquip(int fashionid,int action)
    {
        EquipActionParam param = new EquipActionParam();
        param.fashionid = fashionid;
        param.action = action;
        Net.Instance.DoAction((int) MESSAGE_ID.ID_MSG_FASHION_EQUIP, param);
    }

    public bool GetFashionByID(int id, out FashionItemData itemdata)
    {
        FashionItemData[] data = PlayerDataPool.Instance.MainData.mFashion.items;
        for (int i = 0; i < PlayerDataPool.Instance.MainData.mFashion.unlock_count; ++i)
        {
            if (data[i].id == id)
            {
                itemdata = data[i];
                return true;
            }
        }

        itemdata = null;
        return false;
    }

    public int GetEquipId(int bodypart)
    {
        switch (bodypart)
        {
            case 1:
                 return PlayerDataPool.Instance.MainData.mFashion.head_id;
            case 2:
                return PlayerDataPool.Instance.MainData.mFashion.upper_body_id;
            case 3:
                return PlayerDataPool.Instance.MainData.mFashion.lower_body_id;
        }
        return -1;
    }

    public bool ActiveFashion(int id)
    {
        FashionItemData itemdata =
            PlayerDataPool.Instance.MainData.mFashion.items[PlayerDataPool.Instance.MainData.mFashion.unlock_count];
        itemdata.id = id;
        FashionTableItem res = DataManager.FashionTable[id] as FashionTableItem;
        if (res == null || res.min_stars < 0)
            return false;
        itemdata.starnum = (uint)res.min_stars;
        PlayerDataPool.Instance.MainData.mFashion.unlock_count ++;
        return true;
    }

    public bool AddStarFashion(int id)
    {
        FashionItemData itemdata;
        if (GetFashionByID(id, out itemdata))
        {
              FashionTableItem res = DataManager.FashionTable[id] as FashionTableItem;
            if (itemdata.starnum == res.max_stars)
                return false;
            itemdata.starnum++;
            return true;
        }
        return false;
    }

    public bool EquipFashion(int id,int action,int bodypart)
    {
        FashionItemData itemdata;
        if(GetFashionByID(id,out itemdata))
        {
            switch (bodypart)
            {
                case 1:
                    PlayerDataPool.Instance.MainData.mFashion.head_id = (action == 1) ? id : -1;
                    return true;
                case 2:
                    PlayerDataPool.Instance.MainData.mFashion.upper_body_id = (action == 1) ? id : -1;
                    return true;
                case 3:
                    PlayerDataPool.Instance.MainData.mFashion.lower_body_id = (action == 1) ? id : -1;
                    return true;

            }

        }
        return false;
    }

    private bool CheckCostEnough(int fashionid, bool is_addstar)
    {
        FashionTableItem res = DataManager.FashionTable[fashionid] as FashionTableItem;
        if (res == null)
        {
            GameDebug.LogError("无效的fashionid id= " + fashionid);
            return false;
        }

        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();

        uint itemNum;
        if (res.costtype == 1)
        {
            itemNum = pdm.GetProceeds(ProceedsType.Money_Game);
        }
        else if (res.costtype == 2)
        {
            itemNum = pdm.GetProceeds(ProceedsType.Money_RMB);
        }
        else
        {
            itemNum = pdm.GetItemNumByID(res.costid);
        }
        
        FashionPropTableItem propRes = null;
        if (is_addstar)
        {
            FashionItemData itemdata;
            ModuleManager.Instance.FindModule<FashionModule>().GetFashionByID(fashionid, out itemdata);
            if (itemdata == null) return false;
            propRes = DataManager.FashionPropTable[res.propid + itemdata.starnum] as FashionPropTableItem;
        }
        else
        {
            propRes = DataManager.FashionPropTable[res.propid] as FashionPropTableItem;
        }

        if (propRes == null) return false;
        if (itemNum >= propRes.costnum)
        {
            return true;
        }

        string itemName = "";
        if (res.costtype == 1)
        {
            itemName = StringHelper.GetString("money_game");
        }
        else if (res.costtype == 2)
        {
            itemName = StringHelper.GetString("money_rmb");
        }
        else
        {
            itemName = ItemManager.GetItemRes(res.costid).name;
        }

        PromptUIManager.Instance.AddNewPrompt(String.Format(StringHelper.GetString("not_much_money"), itemName));

        return false;
    }

 }

