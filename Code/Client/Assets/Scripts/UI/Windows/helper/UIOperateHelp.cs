using UnityEngine;
using System.Collections;

public class UIOperateHelp : UIWindow
{
    public UIButton playBtn;

    protected override void OnLoad()
    {
        playBtn = this.FindComponent<UIButton>("playBtn");
    }
    //界面打开
    protected override void OnOpen(object param = null)
    {
        EventDelegate.Add(playBtn.onClick, Play);
        SceneManager.Instance.LogicPause();
    }
    //界面关闭
    protected override void OnClose()
    {
        EventDelegate.Remove(playBtn.onClick, Play);
    }

    void Play()
    {
        WindowManager.Instance.CloseUI("op_help");
        SceneManager.Instance.LogicResume();
    }
}
