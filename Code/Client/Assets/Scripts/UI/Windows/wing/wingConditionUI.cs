using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class wingConditionUI
{
    public UILabel content1;
    public UISprite biaoji1;
	public UISprite num1;
    private GameObject mView;
    public wingConditionUI(GameObject view)
    {
        mView = view;

        content1 = ObjectCommon.GetChildComponent<UILabel>(view,"content1");
        biaoji1 = ObjectCommon.GetChildComponent<UISprite>(view,"biaoji1");
		num1 = ObjectCommon.GetChildComponent<UISprite>(view,"num1");
    }
}

