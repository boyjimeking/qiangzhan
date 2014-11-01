using UnityEngine;
using System.Collections;

public class ReloadEvent : EventBase
{
    public static string RELOAD_EVENT = "RELOAD_EVENT";

    public static string BULLET_CHANGE_EVENT = "BULLET_CHANGE_EVENT";


    public static string WEAPON_UP_EVENT = "WEAPON_UP_EVENT";
    public static string WEAPON_UP_REMOVE_EVENT = "WEAPON_UP_REMOVE_EVENT";

    public static string SUPER_WEAPON_EVENT = "SUPER_WEAPON_EVENT";


    public int reload_time = 0;
    public int super_weapon = -1;
    public ReloadEvent(string eventName)
        : base(eventName)
    {

    }
}
