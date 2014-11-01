using UnityEngine;
using System.Collections;

public class PropertyItem 
{
    public float baseValue = 0.0f;	//基础值
    public float addValue = 0.0f;	//附加值

    public float GetValue()
	{
		return this.baseValue + this.addValue;
	}
}
