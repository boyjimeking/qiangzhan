
using System;
using System.Collections.Generic;
using System.Text;
using Message;
using System.Security.Cryptography;



public class RandomNameAction : BaseAction<request_random_name, respond_random_name>
{
    public RandomNameAction()
        : base((int)MESSAGE_ID.ID_MSG_RANDOM_NAME)
    {
    }

    protected override void OnRequest(request_random_name request, object userdata)
    {
    }

    protected override void OnRespond(respond_random_name respond, object userdata)
    {
        CreateRoleEvent e = new CreateRoleEvent(CreateRoleEvent.LOGIN_EVENT_GET_RANDOM_NAME_RST);
        e.UserName = respond.name;

        EventSystem.Instance.PushEvent(e);
    }
}
