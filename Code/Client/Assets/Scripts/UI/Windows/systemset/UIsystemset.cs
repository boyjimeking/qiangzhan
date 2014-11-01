using UnityEngine;
using System.Collections;

public class UIsystemset : UIWindow
{
#region button按钮
    private GameObject mBkmusic;
    private GameObject mGamesound;
    private GameObject mShield;
    private GameObject mRoll;
    private GameObject mServer;
    private GameObject mExit;
    private GameObject mNotice;
    private GameObject mActivity;
    private GameObject mClose;
#endregion
    private UILabel mServerText;
    private UILabel mQQText;
    public UIsystemset()
    {

    }
    protected override void OnLoad()
    {
        mBkmusic = this.FindChild("bkmusic");
        mGamesound = this.FindChild("gamesound");
        mShield = this.FindChild("shield");
        mRoll = this.FindChild("roll");
        mServer = this.FindChild("serverbutton");
        mExit = this.FindChild("exit");
        mNotice = this.FindChild("notice");
        mActivity = this.FindChild("activity");
        mClose = this.FindChild("close");

        UIEventListener.Get(mBkmusic).onClick = onBkmusicClick;
        UIEventListener.Get(this.FindChild("bk/bkmusic/Scroll Bar/Foreground")).onClick = onBkmusicClick;
        UIEventListener.Get(this.FindChild("bk/bkmusic/Scroll Bar/Foreground")).onDrag = onBkmusicDrag;

        UIEventListener.Get(mGamesound).onClick = onGamesoundClick;
        UIEventListener.Get(this.FindChild("bk/gamesound/Scroll Bar/Foreground")).onClick = onGamesoundClick;
        UIEventListener.Get(this.FindChild("bk/gamesound/Scroll Bar/Foreground")).onDrag = onGamesoundDrag;

        UIEventListener.Get(mShield).onClick = onShieldClick;
        UIEventListener.Get(this.FindChild("bk/shield/Scroll Bar/Foreground")).onClick = onShieldClick;
        UIEventListener.Get(this.FindChild("bk/shield/Scroll Bar/Foreground")).onDrag = onShieldDrag;

        UIEventListener.Get(mRoll).onClick = onRollClick;
        UIEventListener.Get(this.FindChild("bk/roll/Scroll Bar/Foreground")).onClick = onRollClick;
        UIEventListener.Get(this.FindChild("bk/roll/Scroll Bar/Foreground")).onDrag = onRollDrag;

        UIEventListener.Get(mServer).onClick = onServerClick;
        UIEventListener.Get(mExit).onClick = onExitClick;
        UIEventListener.Get(mNotice).onClick = onNoticeClick;
        UIEventListener.Get(mActivity).onClick = onActivityClick;
         UIEventListener.Get(mClose).onClick = onCloseClick;

        mServerText = this.FindComponent<UILabel>("serverinfo"); ;
        mQQText = this.FindComponent<UILabel>("qqinfo"); ;
    }


    private void onGamesoundDrag(GameObject go, Vector2 delta)
    {
        UIScrollBar Bar = go.GetComponentInChildren<UIScrollBar>() ? go.GetComponentInChildren<UIScrollBar>() : go.GetComponentInParent<UIScrollBar>();
        if (!Bar)
            return;

        if (0.5 > Bar.value)
            SoundManager.Instance.IsSound = true;
        else
            SoundManager.Instance.IsSound = false;

        SoundManager.Instance.SetBkSound(SoundManager.Instance.IsSound);
    }

    private void onBkmusicDrag(GameObject go, Vector2 delta)
    {
        UIScrollBar Bar = go.GetComponentInChildren<UIScrollBar>() ? go.GetComponentInChildren<UIScrollBar>() : go.GetComponentInParent<UIScrollBar>();
        if (!Bar)
            return;

        if (0.5 > Bar.value)
            SoundManager.Instance.IsBkSound = true;
        else
            SoundManager.Instance.IsBkSound = false;

        SoundManager.Instance.SetBkSound(SoundManager.Instance.IsBkSound);
    }

 

    private void onCloseClick(GameObject go)
    {
        WindowManager.Instance.CloseUI("systemset");
    }

    private void onActivityClick(GameObject go)
    {
        //UIWindow systeminfo = WindowManager.Instance.GetUI("systeminfo");
        //if (systeminfo != null)
        //{
        //    WindowManager.Instance.CloseUI("systeminfo");
        //}

        //UIWindow system = WindowManager.Instance.GetUI("systemset");
        //if (system != null)
        //{
        //    WindowManager.Instance.CloseUI("systemset");
        //}

        //WindowManager.Instance.OpenUI("systeminfo", "活动");
        PopTipManager.Instance.AddNewTip(StringHelper.GetString("is_open"));
    }

    private void onNoticeClick(GameObject go)
    {
        UIWindow systeminfo = WindowManager.Instance.GetUI("systeminfo");
        //if (systeminfo != null)
        //{
        //    WindowManager.Instance.CloseUI("systeminfo");
        //}

        //UIWindow system = WindowManager.Instance.GetUI("systemset");
        //if (system != null)
        //{
        //    WindowManager.Instance.CloseUI("systemset");
        //}

        //WindowManager.Instance.OpenUI("systeminfo", "活动2222");

        PopTipManager.Instance.AddNewTip(StringHelper.GetString("is_open"));
    }

    private void onExitClick(GameObject go)
    {
        MainFlow flow = GameApp.Instance.GetCurFlow() as MainFlow;
        flow.BackToLogin();
        WindowManager.Instance.OpenUI("login");
    }

    private void onServerClick(GameObject go)
    {
        throw new System.NotImplementedException();
    }

    private void onRollDrag(GameObject go, Vector2 delta)
    {
        UIScrollBar Bar = go.GetComponentInChildren<UIScrollBar>() ? go.GetComponentInChildren<UIScrollBar>() : go.GetComponentInParent<UIScrollBar>();
        if (!Bar)
            return;

        if (0.5 > Bar.value)
        {
            AnnounceItemManager.Instance.SetHide = false;
            AnnounceItemManager.Instance.ShowList();
        }
           
        else
        {
            AnnounceItemManager.Instance.SetHide = true;
            AnnounceItemManager.Instance.HideList();
        }  
    }

    private void onRollClick(GameObject go)
    {
        UIScrollBar Bar = go.GetComponentInChildren<UIScrollBar>() ? go.GetComponentInChildren<UIScrollBar>() : go.GetComponentInParent<UIScrollBar>();
        if (!Bar)
            return;

        if (!AnnounceItemManager.Instance.SetHide)
        {
            Bar.value = 1;
            AnnounceItemManager.Instance.SetHide = true;
            AnnounceItemManager.Instance.HideList();
        }
        else
        {
            Bar.value = 0;
            AnnounceItemManager.Instance.SetHide = false;
            AnnounceItemManager.Instance.ShowList();
        }

    }

    private void onShieldDrag(GameObject go, Vector2 delta)
    {
        BaseScene Base = SceneManager.Instance.GetCurScene();
        SceneObjManager objMng = Base.GetSceneObjManager();
        UIScrollBar Bar = go.GetComponentInChildren<UIScrollBar>() ? go.GetComponentInChildren<UIScrollBar>() : go.GetComponentInParent<UIScrollBar>();
        if (null == Bar || null == objMng)
            return;

        if (0.5 > Bar.value)
        {
            objMng.IsDelete = true;
            Base.RemoveAllGhost();
        }
        else
        {
            objMng.IsDelete = false;
            if(!objMng.HasObjectByAlias(Base.GhostObjects()[0].alias))
                Base.AddAllGhost();
        }
    }

    private void onShieldClick(GameObject go)
    {
        SceneObjManager objMng = SceneManager.Instance.GetCurScene().GetSceneObjManager();
        UIScrollBar Bar = go.GetComponentInChildren<UIScrollBar>() ? go.GetComponentInChildren<UIScrollBar>() : go.GetComponentInParent<UIScrollBar>();
        if (null == Bar || null == objMng)
            return;

        if (objMng.IsDelete)
        {
            Bar.value = 1;
            objMng.IsDelete = false;
            SceneManager.Instance.GetCurScene().AddAllGhost();
        }
        else
        {
            Bar.value = 0;
            objMng.IsDelete = true;
            SceneManager.Instance.GetCurScene().RemoveAllGhost();
        }
    }
           

    private void onGamesoundClick(GameObject go)
    {
        UIScrollBar Bar = go.GetComponentInChildren<UIScrollBar>() ? go.GetComponentInChildren<UIScrollBar>() : go.GetComponentInParent<UIScrollBar>();
        if (!Bar)
            return;

        if (!SoundManager.Instance.IsSound)
        {
 
            Bar.value = 0;
            SoundManager.Instance.IsSound = true;
        }
        else
        {
            Bar.value = 1;
            SoundManager.Instance.IsSound = false;
        }

        SoundManager.Instance.SetSound(SoundManager.Instance.IsSound);
    }

    void onBkmusicClick(GameObject go)
    {
        UIScrollBar Bar = go.GetComponentInChildren<UIScrollBar>() ? go.GetComponentInChildren<UIScrollBar>() : go.GetComponentInParent<UIScrollBar>();
        if (!Bar)
            return;

        if (!SoundManager.Instance.IsBkSound)
        {         
            Bar.value = 0;
            SoundManager.Instance.IsBkSound = true;
        }
        else
        {
            Bar.value = 1;
            SoundManager.Instance.IsBkSound = false;
        }

        SoundManager.Instance.SetBkSound(SoundManager.Instance.IsBkSound);
    }

    protected override void OnOpen(object param = null)
    {

    }
    protected override void OnClose()
    {

    }


}
