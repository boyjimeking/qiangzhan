using UnityEngine;
using System.Collections;

public class ChargeEvent : EventBase
{
    public static string CHARGE_RMB_SUCESS = "CHARGE_RMB_SUCESS";

    public object msg = null;

    public ChargeEvent(string name):base(name)
    {

    }
}
