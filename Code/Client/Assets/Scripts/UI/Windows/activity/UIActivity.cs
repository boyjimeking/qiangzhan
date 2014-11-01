using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class ActivityItem
{
    public GameObject mObj;

    private UILabel mActivityName;
    private UILabel mNeedLevel;
    private UILabel mOkLabel;
    private UILabel mActivityTime;
    private UILabel mLeftCount;
    private UISprite mCompleted;
    private UILabel mDesc;
    private UIButton mXiangQingBtn;
    private UIButton mOkBtn;
    private UISprite mback;
    private UISprite mSprite1;
    private UISprite mSprite2;

    private UILabel mLabel1;
    private UILabel mLabel2;
    private UILabel mLabel3;
    private UILabel mLabel4;
    private UILabel mLabel5;

    private int mType;
    private int mResid;

    private int mResId;

    public delegate void OnClickFuntion(ActivityItem item);

    public OnClickFuntion onOkClick = null;
    public OnClickFuntion onXiangQingClick = null;

    public ActivityItem(GameObject gameObj)
    {
        mObj = gameObj;

        mActivityName = ObjectCommon.GetChildComponent<UILabel>(mObj, "activityname");
        mNeedLevel = ObjectCommon.GetChildComponent<UILabel>(mObj, "level");
        mActivityTime = ObjectCommon.GetChildComponent<UILabel>(mObj, "time");
        mLeftCount = ObjectCommon.GetChildComponent<UILabel>(mObj, "leftcount");
        mCompleted = ObjectCommon.GetChildComponent<UISprite>(mObj, "completed");
        mDesc = ObjectCommon.GetChildComponent<UILabel>(mObj, "liangli_miaoshu");
        mOkLabel = ObjectCommon.GetChildComponent<UILabel>(mObj, "LabelOK");
        mback = ObjectCommon.GetChildComponent<UISprite>(mObj, "bg");
        mSprite1 = ObjectCommon.GetChildComponent<UISprite>(mObj, "Sprite1");
        mSprite2 = ObjectCommon.GetChildComponent<UISprite>(mObj, "Sprite2");
        mLabel1 = ObjectCommon.GetChildComponent<UILabel>(mObj, "Label1");
        mLabel2 = ObjectCommon.GetChildComponent<UILabel>(mObj, "Label2");
        mLabel3 = ObjectCommon.GetChildComponent<UILabel>(mObj, "Label3");
        mLabel4 = ObjectCommon.GetChildComponent<UILabel>(mObj, "Label4");
        mLabel5 = ObjectCommon.GetChildComponent<UILabel>(mObj, "Label5");

        mXiangQingBtn = ObjectCommon.GetChildComponent<UIButton>(mObj, "xiangqing");
        mOkBtn = ObjectCommon.GetChildComponent<UIButton>(mObj, "OkBtn");
        EventDelegate.Add(mXiangQingBtn.onClick, OnXiangQingClick);
        EventDelegate.Add(mOkBtn.onClick, OnOkClick);
    }

    public void updatezeroInfo(int type, int ResId, string name, string bg)
    {
        mType = type;
        mResid = ResId;
        mActivityName.text = name;
        UIAtlasHelper.SetSpriteImage(mback, bg, true);
        mOkBtn.isEnabled = false;
        mOkLabel.text = "[423e3e]" + "开始挑战";
        mCompleted.gameObject.SetActive(false);
        mNeedLevel.gameObject.SetActive(false);
        mActivityTime.gameObject.SetActive(false);
        mDesc.gameObject.SetActive(false);

        mLabel1.gameObject.SetActive(false);
        mLabel2.gameObject.SetActive(false);
        mLabel3.gameObject.SetActive(false);
        mLabel4.gameObject.SetActive(false);
        mLabel5.gameObject.SetActive(false);

        mSprite1.gameObject.SetActive(false);
        mSprite2.gameObject.SetActive(false);
        mLeftCount.gameObject.SetActive(false);
    }
    public void UpdateInfo(int type, int ResId, string name, int needLevel, int playerLevel, int openTime, int closeTime, int leftCount, bool completed, string desc, string scenebg)
    {
        bool enable = true;

        mType = type;
        mResId = ResId;

        mActivityName.text = name;
        if(needLevel > playerLevel)
        {
            enable = false;
            mNeedLevel.text = "[e92224]" + needLevel.ToString() + "级";
        }
        else
        {
            mNeedLevel.text = "[fed514]" + needLevel.ToString() + "级";
        }

        if(!isOpenTime(openTime, closeTime))
        {
            enable = false;
            mActivityTime.text = "[e92224]" + FormatTime(openTime, closeTime);
        }
        else
        {
            mActivityTime.text = "[fed514]" + FormatTime(openTime, closeTime);
        }

        if(leftCount <= 0)
        {
            enable = false;
            mLeftCount.text = "[ff0000]" + leftCount.ToString();
        }
        else
        {
            mLeftCount.text = "[81ffa5]" + leftCount.ToString();
        }

        mDesc.text = desc;
        mCompleted.gameObject.SetActive(completed);
        mOkBtn.isEnabled = enable && !completed;
        if (!mOkBtn.isEnabled)
            mOkLabel.text = "[423e3e]" + "开始挑战";
        else
            mOkLabel.text = "[5d230c]" + "开始挑战";

        UIAtlasHelper.SetSpriteImage(mback, scenebg, true);


    }

    private void OnXiangQingClick()
    {
        if(onXiangQingClick == null)
            return;

        onXiangQingClick(this);
    }

    private void OnOkClick()
    {
        if(onOkClick == null)
            return;

        onOkClick(this);
    }

    private string FormatTime(int openTime, int closeTime)
    {
        int t1 = openTime / 100;
        int t2 = openTime - t1 * 100;

        int t3 = closeTime / 100;
        int t4 = closeTime - t3 * 100;

        return string.Format("{0:D2}", t1) + ":" + string.Format("{0:D2}", t2) + " - " + string.Format("{0:D2}", t3) + ":" + string.Format("{0:D2}", t4);
    }

    private bool isOpenTime(int openTime, int closeTime)
    {
        System.DateTime nowTime = System.DateTime.Now;
        int t = nowTime.Hour * 100 + nowTime.Minute;

        if (t < openTime)
            return false;

        if (t >= closeTime)
            return false;

        return true;
    }

    public int GetActivityId()
    {
        return mResId;
    }

    public int GetType()
    {
        return mType;
    }
   
};
public class UIActivity : UIWindow
{
    private UIButton mLeftBtn;
    private UIButton mRightBtn;
    private UIGrid mGrid;
    private GameObject mActivityItemUnit;
    private UIScrollView mScrolV;

    private List<ActivityItem> mActivityItemList = new List<ActivityItem>();

    public UIActivity()
    {
    }
    
    protected override void OnLoad()
    {
        base.OnLoad();

        mLeftBtn = FindComponent<UIButton>("PageLeft");
        mRightBtn = FindComponent<UIButton>("PageRight");
        mActivityItemUnit = FindChild("ActivityItem");
        mGrid = FindComponent<UIGrid>("container/Scroll View/UIGrid");
        mScrolV = FindComponent<UIScrollView>("container/Scroll View");
    }
 
    protected override void OnOpen(object param = null)
    {
        RefreshUI();

        EventDelegate.Add(mLeftBtn.onClick, OnLeftClick);
        EventDelegate.Add(mRightBtn.onClick, OnRightClick);

        EventSystem.Instance.addEventListener(ActivityDataUpdateEvent.ACTIVITYDATA_UPDATE_EVENT, OnDataUpdate); 
    }

    protected override void OnClose()
    {
        EventDelegate.Remove(mLeftBtn.onClick, OnLeftClick);
        EventDelegate.Remove(mRightBtn.onClick, OnRightClick);

        EventSystem.Instance.removeEventListener(ActivityDataUpdateEvent.ACTIVITYDATA_UPDATE_EVENT, OnDataUpdate);

        WindowManager.Instance.CloseUI("activityinfo");
    }

    public override void Update(uint elapsed)
    {
       
    }

    private void OnDataUpdate(EventBase e)
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        PlayerDataModule ply = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (ply == null)
            return;
        IDictionaryEnumerator itr = DataManager.ActivityTypeTable.GetEnumerator();
        while (itr.MoveNext())
        {
            ActivityTypeTableItem item = itr.Value as ActivityTypeTableItem;
            if (item == null)
                continue;

            int resId = GetActivityByType((int)item.type);
            if (resId < 0)
                continue;

            ActivityTableItem activityItem = DataManager.ActivityTable[resId] as ActivityTableItem;

            ActivityItem activity = null;
            foreach (ActivityItem temp in mActivityItemList)
            {
                if (temp.GetType() == (int)item.type)
                {
                    activity = temp;
                }
            }

            if (activity == null)
            {
                GameObject gameObj = WindowManager.Instance.CloneGameObject(mActivityItemUnit);
                if (gameObj == null)
                {
                    GameDebug.LogError("instance activityitem error");
                    return;
                }

                gameObj.SetActive(true);
                gameObj.transform.parent = mGrid.gameObject.transform;
                gameObj.transform.localScale = Vector3.one;
                mGrid.Reposition();
                if (0 == item.isopen)
                {
                    activity = new ActivityItem(gameObj);
                    activity.updatezeroInfo((int)item.type, resId, item.name, item.scnenbg);

                    activity.onOkClick = OnJoinActivity;
                    activity.onXiangQingClick = OnActivityXiangQing;
                }
                else if (1 == item.isopen)
                {
                    activity = new ActivityItem(gameObj);
                    activity.UpdateInfo((int)item.type, resId, item.name, item.needlevel, ply.GetLevel(),
                        (int)activityItem.starttime, (int)activityItem.overtime,
                        item.max_time - ply.GetActivityTypeCompleteTime(activityItem.type), ply.IsActivityCompleted(resId),
                        item.award_desc, item.scnenbg);

                    activity.onOkClick = OnJoinActivity;
                    activity.onXiangQingClick = OnActivityXiangQing;
                }

                mActivityItemList.Add(activity);
            }
            else
            {
                if (0 == item.isopen)
                {
                    activity.updatezeroInfo((int)item.type, resId, item.name, item.scnenbg);
                }
                else if (1 == item.isopen)
                {
                    activity.UpdateInfo((int)item.type, resId, item.name, item.needlevel, ply.GetLevel(),
                    (int)activityItem.starttime, (int)activityItem.overtime,
                    item.max_time - ply.GetActivityTypeCompleteTime(activityItem.type), ply.IsActivityCompleted(resId),
                    item.award_desc, item.scnenbg);
                }
            }
        }
//         foreach (DictionaryEntry de in DataManager.ActivityTypeTable)
//         {
//            
//         }

        mGrid.repositionNow = true;
    }


    private void OnLeftClick()
    {
        mScrolV.Scroll(1.0f);
    }

    private void OnRightClick()
    {
        mScrolV.Scroll(-1.0f);
    }

    private void OnJoinActivity(ActivityItem act)
    {
        JoinActivityActionParam param = new JoinActivityActionParam();
        param.ActivityId = act.GetActivityId();

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_ACTIVITY_JOIN, param);
    }

    private void OnActivityXiangQing(ActivityItem act)
    {
        ActivityTypeTableItem activityTypeItem = DataManager.ActivityTypeTable[act.GetType()] as ActivityTypeTableItem;
        if (null != activityTypeItem)
        {
            UIWindow activity = WindowManager.Instance.GetUI("activityinfo");
            if (activity != null)
            {
                WindowManager.Instance.CloseUI("activityinfo");
            }

            WindowManager.Instance.OpenUI("activityinfo", activityTypeItem.activity_desc);
        }
    }

    private int GetActivityByType(int type)
    {
        System.DateTime nowTime = System.DateTime.Now;
        int time = nowTime.Hour * 100 + nowTime.Minute;

        //ArrayList skeys = new ArrayList(DataManager.ActivityTable.Keys);
        ArrayList skeys = DataManager.ActivityTable.GetKeys();

        skeys.Sort();

        int result = -1;

        for (int i = 0; i < skeys.Count; ++i )
        {
            ActivityTableItem item = DataManager.ActivityTable[skeys[i]] as ActivityTableItem;
            if (item == null)
                continue;

            if (item.type != type)
                continue;

            if (result < 0)
                result = item.id;

            if (time > item.overtime)
                continue;

            return item.id;
        }

// 
//             foreach (int key in skeys)
//             {
//                
//             }

        return result;
    }
}
