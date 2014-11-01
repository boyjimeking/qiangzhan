using System;
using Message;

public class FashionActiveAction:LogicAction<request_active_fashion,respond_active_fashion>

{
    public FashionActiveAction() 
        : base((int)MESSAGE_ID.ID_MSG_FASHION_ACTIVE)
    {
    }

    protected override void OnRequest(request_active_fashion request, object userdata)
    {
        int fashionid = Convert.ToInt32(userdata);
        request.fashionid = fashionid;
        GameDebug.Log("请求购买时装" + request.fashionid);
    }

    protected override void OnRespond(respond_active_fashion respond, object userdata)
    {
        int fashionid = Convert.ToInt32(userdata);
        if (respond.errorcode == (int)ERROR_CODE.ERR_FASHION_ACTIVE_OK)
        {
            ModuleManager.Instance.FindModule<FashionModule>().ActiveFashion(fashionid);
            EventSystem.Instance.PushEvent(new FashionEvent(FashionEvent.FASHION_ACTIVE){ mfashionid =  fashionid});
            GameDebug.Log("购买时装成功 id=" + fashionid);
            EventSystem.Instance.PushEvent(new FashionEvent(FashionEvent.FASHION_UPDATE));
        }else if (respond.errorcode == (int) ERROR_CODE.ERR_FASHION_ITEM_NO_ENOUGH)
        {
            PopTipManager.Instance.AddNewTip("道具不足");
        }
    }
}

