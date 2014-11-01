using System;

using Message;

public class FashionAddStarAction:LogicAction<request_addstar_fashion,respond_addstar_fashion>
{
    public FashionAddStarAction() : base((int) MESSAGE_ID.ID_MSG_FASHION_ADDSTAR)
    {

    }

    protected override void OnRequest(request_addstar_fashion request, object userdata)
    {
        int fashionid = Convert.ToInt32(userdata);      
        request.fashionid = fashionid;
        GameDebug.Log("请求升星时装" + request.fashionid);
    }

    protected override void OnRespond(respond_addstar_fashion respond, object userdata)
    {
        int fashionid = Convert.ToInt32(userdata);
        if (respond.errorcode == (int) ERROR_CODE.ERR_FASHION_ADDSTAR_OK)
        {
            bool is_success = ModuleManager.Instance.FindModule<FashionModule>().AddStarFashion(fashionid);
            EventSystem.Instance.PushEvent(new FashionEvent(FashionEvent.FASHION_ADDSTAR){mfashionid =  fashionid});
            EventSystem.Instance.PushEvent(new FashionEvent(FashionEvent.FASHION_UPDATE));
            if (is_success)
            {
                GameDebug.Log("升星成功 id = " +fashionid);
            }       
        }

       
    }
}

