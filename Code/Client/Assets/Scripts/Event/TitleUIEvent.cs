using UnityEngine;
using System.Collections;

public class TitleUIEvent : EventBase {

    public static string GET_NEW_TITLE = "GET_NEW_TITLE";
    public static string TITLE_CHANGED = "TITLE_CHANGED";

    public int titleId = -1;

    public TitleUIEvent(string eventName):base(eventName)
    {
 
    }
}
