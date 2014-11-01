using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UITitle : UIWindow 
{
    protected UITable mGrid = null;
    protected UISprite mIconSp = null;
    protected BetterList<UILabel> mAttrisLb = new BetterList<UILabel>();
    protected UILabel mGradeLb = null;
    protected UILabel mConditionLb = null;
    protected UIButton mReturnBtn = null;
    protected UIButton mEquipBtn = null;
    protected UILabel mEquipBtnLb = null;
    protected UIScrollBar mScrollBar = null;

    protected GameObject mItemObj = null;
    protected GameObject mGroupObj = null;

    private Dictionary<int, TitleGroupUI> mGroupUIs = new Dictionary<int, TitleGroupUI>();
    private Dictionary<int, TitleItemUI> mItemUIs = new Dictionary<int, TitleItemUI>();

    private int mCurTitleId = -1;

    int CurTitleId
    {
        get
        {
            return mCurTitleId;
        }

        set
        {
            if (mCurTitleId != value)
            {
                mCurTitleId = value;
                setCurTitle(mCurTitleId);
            }
        }
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        mGrid = this.FindComponent<UITable>("Left/Grid");

        mIconSp = this.FindComponent<UISprite>("Right/icon");

        mAttrisLb.Add(this.FindComponent<UILabel>("Right/Attri/attr1"));
        mAttrisLb.Add(this.FindComponent<UILabel>("Right/Attri/attr2"));
        mAttrisLb.Add(this.FindComponent<UILabel>("Right/Attri/attr3"));
        mAttrisLb.Add(this.FindComponent<UILabel>("Right/Attri/attr4"));
        
        mGradeLb = this.FindComponent<UILabel>("Right/Zhanli/zhanLb");
        mConditionLb = this.FindComponent<UILabel>("Right/Tiaojian/tiaojianLb");
        mEquipBtn = this.FindComponent<UIButton>("Right/EquipBtn");
        mEquipBtnLb = this.FindComponent<UILabel>("Right/EquipBtn/Label");
        mReturnBtn = this.FindComponent<UIButton>("returnBtn");
        mScrollBar = this.FindComponent<UIScrollBar>("ScrollBar");

        mGroupObj = this.FindChild("item/titleGroup");
        mItemObj = this.FindChild("item/titleItem");

        Init();
    }

    protected override void OnOpen(object param = null)
    {
        base.OnOpen(param);

        EventSystem.Instance.addEventListener(TitleUIEvent.TITLE_CHANGED, onTitleChanged);

        EventDelegate.Add(mReturnBtn.onClick, onReturnBtnClick);
        EventDelegate.Add(mEquipBtn.onClick, onEquipBtnClick);

        updateTitleInfo();
    }

    protected override void OnClose()
    {
        base.OnClose();
        
        EventSystem.Instance.removeEventListener(TitleUIEvent.TITLE_CHANGED, onTitleChanged);

        EventDelegate.Remove(mReturnBtn.onClick, onReturnBtnClick);
        EventDelegate.Remove(mEquipBtn.onClick, onEquipBtnClick);
    }

    void updateTitleInfo(int titleItemId = -1)
    {
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (pdm == null)
            return;

        // 更新全部;
        if (titleItemId < 0)
        {
            IDictionaryEnumerator itr = DataManager.TitleItemTable.GetEnumerator();
            while (itr.MoveNext())
            {
                TitleItemTableItem item = itr.Value as TitleItemTableItem;

                if (item == null)
                    continue;

                if (!mItemUIs.ContainsKey(item.id))
                    continue;

                mItemUIs[item.id].SetIsHave(pdm.IsHasTitleByID(item.id));
            }
//             foreach (int key in DataManager.TitleItemTable.Keys)
//             {
//                 TitleItemTableItem item = DataManager.TitleItemTable[key] as TitleItemTableItem;
// 
//                 if (item == null)
//                     continue;
// 
//                 if (!mItemUIs.ContainsKey(item.id))
//                     continue;
// 
//                 mItemUIs[item.id].SetIsHave(pdm.IsHasTitleByID(item.id));
//             }
        }

        else
        {
            if (mItemUIs.ContainsKey(titleItemId))
                mItemUIs[titleItemId].SetIsHave(pdm.IsHasTitleByID(titleItemId));
        }
    }

    void Reset()
    {
        mScrollBar.value = 0f;
    }

    void Init()
    {
        CreateGroupTitles();
        CreateTitleItems();

        CurTitleId = 1;
    }

    void onTitleChanged(EventBase ev)
    {
        setCurTitle(mCurTitleId);
    }

    void onReturnBtnClick()
    {
        WindowManager.Instance.CloseUI("title");
    }

    void onEquipBtnClick()
    {
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (pdm == null)
            return;

        if (!pdm.IsHasTitleByID(CurTitleId))
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("title_no_have", FontColor.Red));
            return;
        }

        TitlePutAction param = new TitlePutAction();

        param.OpType = pdm.GetCurTitle() == CurTitleId ? (int)Message.TITLE_OP_TYPE.TITLE_PICK_OFF : (int)Message.TITLE_OP_TYPE.TITLE_PICK_UP;
        param.ResId = CurTitleId;

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_TITLE, param);
    }

    void onTitleGroupClick(GameObject go)
    {
        if (go == null)
            return;

        int groupId = System.Convert.ToInt32(go.name);

        if (!mGroupUIs.ContainsKey(groupId))
            return;

        TitleGroupUI ui = mGroupUIs[groupId];
        ui.OpenOrClose();
    }

    void onTitleItemClick(GameObject go)
    {
        if (go == null)
            return;

        int itemId = System.Convert.ToInt32(go.name);

        if (!mItemUIs.ContainsKey(itemId))
            return;

        foreach (int key in mItemUIs.Keys)
        {
            TitleItemUI item = mItemUIs[key];
            item.SetSelected(false);
        }

        TitleItemUI ui = mItemUIs[itemId];
        ui.SetSelected(true);

        CurTitleId = itemId;
    }

    void setCurTitle(int titleId)
    {
        TitleItemTableItem item = TitleModule.GetTitleItemById(titleId);
        
        if (item == null)
            return;

        UIAtlasHelper.SetSpriteImage(mIconSp, item.picName, true);
        
        mAttrisLb[0].text = item.detail1;
        mAttrisLb[1].text = item.detail2;
        mAttrisLb[2].text = item.detail3;
        mAttrisLb[3].text = item.detail4;

        mGradeLb.text = item.grade.ToString();

        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (pdm == null)
            return;

        bool ishave = pdm.IsHasTitleByID(titleId);

        string detail = "";
        switch(item.contentId)
        {
            case 0:
                FontColor fc = ishave ? FontColor.Green : FontColor.Red;
                string tmp = StringHelper.StringWithColor(fc, pdm.GetLevel() + "/" + item.conditionVal);
                detail = string.Format(item.contentDetail, tmp);
                break;
            default:
                detail = item.contentDetail;
                break;
        }

        mConditionLb.text = detail;

        UISprite btnBg = mEquipBtn.GetComponent<UISprite>();
        if(btnBg != null)
        {
            UIAtlasHelper.SetSpriteShaderGrey(btnBg, !ishave);
        }

        mEquipBtnLb.text = pdm.GetCurTitle() == titleId ? "卸下" : "穿戴";
    }

    void CreateGroupTitles()
    {
        BetterList<int> ids = TitleModule.GetAllTitleGroupId();

        if (ids == null || ids.size == 0)
            return;

        foreach (int id in ids)
        {
            CreateGroupTitle(id);
        }

        mGrid.repositionNow = true;
    }

    void CreateGroupTitle(int id)
    {
        GameObject go = WindowManager.Instance.CloneGameObject(mGroupObj);
        
        if (go == null)
            return;

        go.name = id.ToString();
        go.transform.parent = mGrid.transform;
        go.transform.localScale = Vector3.one;
        go.SetActive(true);

        TitleGroupUI ui = new TitleGroupUI(go);

        TitleGroupTableItem data = TitleModule.GetTitleGroupItemById(id);

        ui.SetData(data);

        mGroupUIs.Add(id, ui);

        UIEventListener.Get(go).onClick = onTitleGroupClick;
    }

    void CreateTitleItems()
    {
        IDictionaryEnumerator itr = DataManager.TitleItemTable.GetEnumerator();
        while (itr.MoveNext())
        {
            TitleItemTableItem item = itr.Value as TitleItemTableItem;

            if (item == null)
                continue;

            CreateTitleItem(item);
        }
//         foreach (int key in DataManager.TitleItemTable.Keys)
//         {
//             TitleItemTableItem item = DataManager.TitleItemTable[key] as TitleItemTableItem;
// 
//             if (item == null)
//                 continue;
// 
//             CreateTitleItem(item);
//            
    }

    void CreateTitleItem(TitleItemTableItem item)
    {
        if (item == null)
            return;

        TitleGroupUI groupUI = getGroupUIById(item.groupId);
        
        if (groupUI == null)
            return;

        GameObject go = WindowManager.Instance.CloneGameObject(mItemObj);

        if (go == null)
            return;

        go.name = item.id.ToString();
        go.SetActive(true);

        TitleItemUI ui = new TitleItemUI(go);
        
        ui.SetData(item);

        groupUI.AddChild(go);

        mItemUIs.Add(item.id, ui);

        UIEventListener.Get(go).onClick = onTitleItemClick;
    }

    TitleGroupUI getGroupUIById(int id)
    {
        if (!mGroupUIs.ContainsKey(id))
            return null;

        return mGroupUIs[id];
    }
}
