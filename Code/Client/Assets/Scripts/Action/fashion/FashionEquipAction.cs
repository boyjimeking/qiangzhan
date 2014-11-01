
 using System;
 using Message;

public class EquipActionParam
{
    public int fashionid;
    public int action;
}
public class FashionEquipAction:LogicAction<request_equip_fashion,respond_equip_fashion>
 {
    public FashionEquipAction() : base((int)MESSAGE_ID.ID_MSG_FASHION_EQUIP)
    {
    }

    protected override void OnRequest(request_equip_fashion request, object userdata)
    {
        EquipActionParam param = userdata as EquipActionParam;
        request.fashionid = param.fashionid;
        request.action = param.action;
        if (request.action == 1)
        {
            GameDebug.Log("请求装备时装" + request.fashionid);
        }
        else
        {
            GameDebug.Log("请求脱下时装" + request.fashionid);
        }
       
    }

    protected override void OnRespond(respond_equip_fashion respond, object userdata)
    {
        EquipActionParam param = userdata as EquipActionParam;
        if (respond.errorcode == (int) ERROR_CODE.ERR_FASHION_EQUIP_OK)
        {
            FashionTableItem res = DataManager.FashionTable[param.fashionid] as FashionTableItem;
            if (res == null) return;
            bool is_success = ModuleManager.Instance.FindModule<FashionModule>().EquipFashion(param.fashionid,param.action,res.bodypart);
            if (is_success)
            {
                EventSystem.Instance.PushEvent(new FashionEvent(FashionEvent.FASHION_EQUIP)
                {
                    mfashionid = param.fashionid,
                    actionid = param.action
                });
                GameDebug.Log("更换装备成功 id = " + param.fashionid);
            }
        }
    }
 }

