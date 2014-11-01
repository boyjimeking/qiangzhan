using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class MailItemList
{
    public int resid;
    public int num;
    public MailItemList()
    {
        clear();
    }

    public void clear()
    {
        resid = 0;
        num = 0;
    }
}
public class MailsData
{
    public GUID mailid;
    public string sendname;
    public string title;
    public string content;
    public uint state;// 0删除,1正常，2已读，3已领取道具
    public uint itemcnt;
    public List<MailItemList> mItemsList = new List<MailItemList>(); //道具
    public ulong start_time;		//创建时间
    public MailsData()
    {
        Clear();
    }

    public void Clear()
    {
        sendname = "";
        title = "";
        content = "";
        state = 1;
        itemcnt = 0;
        mItemsList.Clear();
        start_time = ulong.MaxValue;		//创建时间
    }

    public bool IsRead()
    {
        return state == 2;
    }

    public bool IsEmpty()
    {
        return state == 3;
    }
}
public class MailModule : ModuleBase
{
    public List<MailsData> mMailsList = new List<MailsData>();
    public MailItem SelectedMail;
    public MailModule()
    {
        mMailsList.Clear();
        SelectedMail = null;
    }
}