using System;
using System.Collections.Generic;
using Message;
public class MailLoadAction : LogicAction<request_load_mail, respond_load_mail>
{
    public MailLoadAction()
        : base((int)MESSAGE_ID.ID_MSG_MAIL_LOAD)
    {

    }

    protected override void OnRequest(request_load_mail request, object userdata)
    {
    }

    protected override void OnRespond(respond_load_mail respond, object userdata)
    {
        MailModule module = ModuleManager.Instance.FindModule<MailModule>();
        module.mMailsList.Clear();
        if (respond.mail_lists == null)
            return;
        if(respond.mail_lists.MailList == null)
            return;
        if(respond.mail_lists.MailList.Count<=0)
            return;
        List<role_mail_s> rolemails = respond.mail_lists.MailList;
        for (int i = 0; i < respond.mail_lists.MailList.Count; ++i)
        {
            MailsData mail = new MailsData();
            mail.mailid = respond.mail_lists.MailList[i].mailid;
            mail.content = respond.mail_lists.MailList[i].content;
            mail.itemcnt = respond.mail_lists.MailList[i].itemcnt;
            mail.sendname = respond.mail_lists.MailList[i].sendname;
            mail.start_time = respond.mail_lists.MailList[i].start_time;
            mail.state = respond.mail_lists.MailList[i].state;
            mail.title = respond.mail_lists.MailList[i].title;
            if (respond.mail_lists.MailList[i].itemlist != null)
            {
                if (respond.mail_lists.MailList[i].itemlist.ItemList.Count > 0)
                {
                    for (int j = 0; j < respond.mail_lists.MailList[i].itemlist.ItemList.Count; ++j)
                    {
                        MailItemList mailItem = new MailItemList();
                        mailItem.resid = respond.mail_lists.MailList[i].itemlist.ItemList[j].resid;
                        mailItem.num = respond.mail_lists.MailList[i].itemlist.ItemList[j].num;
                        mail.mItemsList.Add(mailItem);
                    }
                }
            }
            module.mMailsList.Add(mail);
        }
        WindowManager.Instance.OpenUI("mail");
    }

}
 public class MailActionParam
{
     public GUID mailid; 
}

 public class MailOpenedAction : LogicAction<request_mail_state, respond_mail_state>
 {
     public MailOpenedAction()
         : base((int)MESSAGE_ID.ID_MSG_MAIL_STATE_OPENED)
     {

     }

     protected override void OnRequest(request_mail_state request, object userdata)
     {
         MailActionParam param = userdata as MailActionParam;
         request.mailguid = param.mailid.ToMSGGuid();
     }

     protected override void OnRespond(respond_mail_state respond, object userdata)
     {

     }
 }
 public class MailDeleteAction : LogicAction<request_mail_state, respond_mail_state>
 {
     public MailDeleteAction()
         : base((int)MESSAGE_ID.ID_MSG_MAIL_STATE_DELETE)
     {

     }

     protected override void OnRequest(request_mail_state request, object userdata)
     {
         MailActionParam param = userdata as MailActionParam;
         request.mailguid = param.mailid.ToMSGGuid();
     }

     protected override void OnRespond(respond_mail_state respond, object userdata)
     {

     }
 }
 public class MailPickAction : LogicAction<request_mail_state, respond_mail_state>
 {
     public MailPickAction()
         : base((int)MESSAGE_ID.ID_MSG_MAIL_STATE_PICKED)
     {

     }

     protected override void OnRequest(request_mail_state request, object userdata)
     {
         MailActionParam param = userdata as MailActionParam;
         request.mailguid = param.mailid.ToMSGGuid();
     }

     protected override void OnRespond(respond_mail_state respond, object userdata)
     {
         if (respond.result == (int)ERROR_CODE.ERR_MAIL_PICK_OK)
         {
             MailModule module = ModuleManager.Instance.FindModule<MailModule>();
             if(module.SelectedMail != null)
             {
                 module.SelectedMail.isPicked = true;
                 UIWindow MailUI = WindowManager.Instance.GetUI("mail");
                 UIButton mPickBtn = MailUI.FindComponent<UIButton>("background/mGetBtn");
                 UIGrid mItemGrid = MailUI.FindComponent<UIGrid>("Open/ItemGridBK/Scroll View/UIGrid");

                 UIAtlasHelper.SetButtonImage(mPickBtn, "common:anniuhui", true);
                 ObjectCommon.DestoryChildren(mItemGrid.gameObject);
                 for (int i = 0; i < module.SelectedMail.mMaildata.mItemsList.Count; i++)
                 {
                     PopTipManager.Instance.AddGetItemTip(module.SelectedMail.mMaildata.mItemsList[i].resid, module.SelectedMail.mMaildata.mItemsList[i].num);
                 }
             }
         }
     }
 }
