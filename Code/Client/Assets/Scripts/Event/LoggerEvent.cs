using UnityEngine;
using System.Collections;

public class LoggerEvent : EventBase
{
    public static string LOGGER_PUSH_LOG = "LOGGER_PUSH_LOG";

    public string msg = "";
    public LoggerEvent(string eventName)
        : base(eventName)
    {

    }
}
