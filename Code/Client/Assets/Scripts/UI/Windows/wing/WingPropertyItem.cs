using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum WingPropertyType
{
    Life,
    Attack,
    Defence,
    Critical,
    Energy,
}
public class WingPropertyItem
{
    private GameObject mView;
    public WingPropertyType mType = WingPropertyType.Life;
    public UISlider propProcessBar;
    public UILabel propname;
	public UISprite foreGround;
    public WingPropertyItem(GameObject view)
    {
        mView = view;
        propProcessBar = ObjectCommon.GetChildComponent<UISlider>(view, "propProcessBar");
        propname = ObjectCommon.GetChildComponent<UILabel>(view, "propname");
		foreGround = ObjectCommon.GetChildComponent<UISprite>(view,"propProcessBar/fg");
    }
}

