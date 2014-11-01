using UnityEngine;
using System.Collections;

public class CreateRoleEvent : EventBase 
{
    public static string LOGIN_EVENT_CREATE_ROLE = "LOGIN_EVENT_CREATE_ROLE";
    public static string LOGIN_EVENT_CREATE_ROLE_RST = "LOGIN_EVENT_CREATE_ROLE_RST";
    public static string LOGIN_EVENT_GET_RANDOM_NAME_RST = "LOGIN_EVENT_GET_RANDOM_NAME_RST";

    public CreateRoleEvent(string eventName)
        : base(eventName)
    {
        
    }

    public string UserName
    {
        get;
        set;
    }
    public uint id
    {
        get;
        set;
    }

    public string Message
    {
        get;
        set;
    }
}
