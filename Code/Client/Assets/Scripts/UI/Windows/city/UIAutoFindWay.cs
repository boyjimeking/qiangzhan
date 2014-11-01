using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class UIAutoFindWay:UIWindow
{
   
    private int mTimer = 1000;
    private UISpriteAnimation ani;
    protected override void OnLoad()
    {
        base.OnLoad();
        ani = FindComponent<UISpriteAnimation>("Container/zheng");
    }

    protected override void OnOpen(object param = null)
    {
        base.OnOpen(param);
        ani.framesPerSecond = 15;
        ani.Reset();
    }


    public override void Update(uint elapsed)
    {
        base.Update(elapsed);
        if (WindowManager.Instance.IsOpen("stagelist") || WindowManager.Instance.IsOpen("mainmap")|| StoryManager.Instance.IsRunning())
        {
            WindowManager.Instance.CloseUI("autofindway");   
        }

    }
}

