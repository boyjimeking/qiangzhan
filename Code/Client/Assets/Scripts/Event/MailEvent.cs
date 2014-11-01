using System;

public class MailEvent : EventBase
{
    //邮件数量更新了
    public static string MAIL_NUMBER_UPDATE = "MAIL_NUMBER_UPDATE";
    public static string MAIL_UNREAD_NUMBER_UPDATE = "MAIL_UNREAD_NUMBER_UPDATE";

    public uint number = 0;
    public uint unreadnumber = 0;
    public MailEvent(string eventName)
        : base(eventName)
    {

    }
}