using UnityEngine;
using System.Collections;

public class UIAnnouncement : UIWindow
{
    private AnnounceItemNode mAnnounceItemNode = null;
    public UISprite mContent;
    private ulong lastUpdataTime = 0;

    protected override void OnLoad()
    {
        mContent = this.FindComponent<UISprite>("mContent");
    }
    protected override void OnClose()
    {

    }
    protected override void OnOpen(object param = null)
    {
        
    }
    public override void Update(uint elapsed)
    {
        if (TimeUtilities.GetNow() - lastUpdataTime > 1000)
        {
            if (!OnCreateItem() && AnnounceItemManager.Instance.ishide == true)
            {
                if (mContent.gameObject.activeSelf == true)
                {
                    NGUITools.SetActive(mContent.gameObject, false);
                }
            }
            lastUpdataTime = TimeUtilities.GetNow();
        }
        if (AnnounceItemManager.Instance.ishide == false)
        {
            AnnounceItemManager.Instance.Update();
        }
    }

    private bool OnCreateItem()
    {
        int maxnum = DataManager.AnnouncementTable.Count;
        for (int id = 1; id < maxnum+1; id++)
        {
            AnnouncementItem items = DataManager.AnnouncementTable[id] as AnnouncementItem;
            if(items == null)
            {
                return false;
            }
            ulong starTime = (ulong)ToSec(items.starttime);
            ulong endTime = (ulong)ToSec(items.endtime);
            ulong intervalTime = (ulong)items.interval;
            ulong now = TimeUtilities.GetNow();
            double nowsec = now / 1000;
            ulong nowTime = (ulong)nowsec % (24 * 60 * 60);
            double breakTime = (nowsec - starTime) % intervalTime;
            if ((0 < endTime && nowTime > starTime && nowTime < endTime) || (0 >= endTime && nowTime > starTime))
            {
                if (breakTime < 1)
                {
                    if (AnnounceItemManager.Instance.SetHide == false &&( AnnounceItemManager.Instance.nextflag == true || AnnounceItemManager.Instance.ishide == true))
                    {
                        if (mContent.gameObject.activeSelf == false)
                        {
                            NGUITools.SetActive(mContent.gameObject, true);
                        }
                        mAnnounceItemNode = AnnounceItemManager.Instance.CreateAnnounceItem(items.describe);
                        mAnnounceItemNode.nextFlag = false;
                        AnnounceItemManager.Instance.nextflag = false;
                        AnnounceItemManager.Instance.ishide = false;
                        mAnnounceItemNode.Show();

                        return true;
                    }
                }
            }
        }
        return false;
    }
    //把表里的时间转化成秒。
    private int ToSec(int time)
    {
        int hour = time / 10000;
        int min = (time - hour * 10000) / 100;
        int sec = time % 100;
        int timesec = hour * 60 * 60 + min * 60 + sec;
        return timesec;
    }
 
}
