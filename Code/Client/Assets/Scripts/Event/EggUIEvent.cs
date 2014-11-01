using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Message;

public class EggUIEvent : EventBase
{
    public static string EGG_OPEN_SUCESS = "EGG_OPEN_SUCESS";

    public EggType eggType;
    public List<role_egg_item_items> items = new List<role_egg_item_items>();

	public EggUIEvent(string eventName) :base(eventName)
	{
		
	}
}
