
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAssisterForm:UIWindow
{
    private UISprite mHeadIcon;
    private UILabel qqName;
    private UILabel charName;
    //战力
    private UILabel battlegrade;
    //推荐战力
    private UILabel recom_grade;
    private UILabel recom_grade2;
    //战力描述
    private UILabel gradeDesc;
    // S,A,B,C
    private UISprite gradeLevel;

    private UIGrid btnGrid;
    private GameObject mExamBtnItem;

    private UIGrid funcGrid;
    private GameObject mExamFuncItem;
    private List<FuncItemUI> funcBtnList;
    private List<AssisterItemUI> assisterItemList;
    private int mSelecteId;
    private bool IsDirty;
    private UIScrollView assisterItemScroll;
    protected override void OnLoad()
    {
        base.OnLoad();
        mHeadIcon = FindComponent<UISprite>("headIcon");
        qqName = FindComponent<UILabel>("qqname");
        charName = FindComponent<UILabel>("charname");
        battlegrade = FindComponent<UILabel>("zhannum");
        recom_grade = FindComponent<UILabel>("recomzhanli");
        recom_grade2 = FindComponent<UILabel>("recomzhanli2");
        gradeDesc = FindComponent<UILabel>("zhanlidesc");
        gradeLevel = FindComponent<UISprite>("grade");
        btnGrid = FindComponent<UIGrid>("btnScrollView/btngrid");
        funcGrid = FindComponent<UIGrid>("funcScrollView/funcgrid");
        assisterItemScroll = FindComponent<UIScrollView>("funcScrollView");
        mExamBtnItem = FindChild("btnItem");
        mExamBtnItem.gameObject.SetActive(false);
        mExamFuncItem = FindChild("functionItem");
        mExamFuncItem.gameObject.SetActive(false);
        funcBtnList = new List<FuncItemUI>();
        IDictionaryEnumerator iter = DataManager.AssisterTable.GetEnumerator();
        while (iter.MoveNext())
        {
            FuncItemUI item = new FuncItemUI(GameObject.Instantiate(mExamBtnItem) as GameObject);
            AssisterTableItem res = iter.Value as AssisterTableItem;
            item.mBtn.CustomData = res.id;
            item.label.text = res.desc;
            item.gameobject.transform.parent = btnGrid.gameObject.transform;
            item.gameobject.transform.localScale = Vector3.one;
            item.mClickCallBack = OnBtnClick;
            item.gameobject.SetActive(true);
            funcBtnList.Add(item);
        } 

        btnGrid.repositionNow = true;
        assisterItemList = new List<AssisterItemUI>();
        mSelecteId = -1;
    }

    public override void Update(uint elapsed)
    {
        base.Update(elapsed);
        if (IsDirty)
        {
            IsDirty = false;
            RefreshPanel();
        }
    }

    private void RefreshPanel()
    {
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();

        qqName.text = "QQ名字XXX";
        charName.text ="LV."+ pdm.GetLevel()+ " "+ pdm.GetName();
        battlegrade.text = pdm.GetGrade().ToString();
        LevelTableItem level_res = DataManager.LevelTable[pdm.GetLevel()] as LevelTableItem;
        if (level_res != null)
        {
            recom_grade.text = string.Format(StringHelper.GetString("recom_grade"), pdm.GetLevel());
            recom_grade2.text = level_res.recom_grade.ToString();
        }
        float percent = (float)pdm.GetGrade() / (float)level_res.recom_grade;
        string battleGradeLevel = Convert.ToString(ConfigManager.GetVal<string>(ConfigItemKey.BATTLE_GRADE_LEVEL));
        string[] gradeRange = battleGradeLevel.Split(new[] { '|' });
        if (gradeRange.Length == 3)
        {            
            if (percent >= Convert.ToSingle(gradeRange[2]))
            {
                gradeDesc.text = StringHelper.GetString("s_desc");
                UIAtlasHelper.SetSpriteImage(gradeLevel, "youxizhushou:youxizhushou_02",true);
            }
            else if (percent >= Convert.ToSingle(gradeRange[1]))
            {
                gradeDesc.text = StringHelper.GetString("a_desc");
                UIAtlasHelper.SetSpriteImage(gradeLevel, "youxizhushou:youxizhushou_01",true);
            }
            else if (percent >= Convert.ToSingle(gradeRange[0]))
            {
                gradeDesc.text = StringHelper.GetString("b_desc");
                UIAtlasHelper.SetSpriteImage(gradeLevel, "youxizhushou:youxizhushou_03",true);
            }
            else
            {
                gradeDesc.text = StringHelper.GetString("c_desc");
                UIAtlasHelper.SetSpriteImage(gradeLevel, "youxizhushou:youxizhushou_04",true);
            }
        }
       

        for (int i = 0; i < funcBtnList.Count; ++i)
        {
            funcBtnList[i].seletedSprite.gameObject.SetActive(Convert.ToInt32(funcBtnList[i].mBtn.CustomData) == mSelecteId);
        }

        AssisterTableItem res = DataManager.AssisterTable[mSelecteId] as AssisterTableItem;
        if (res == null) return;
        int[] funList = res.GetFunction();
        if (funList.Length >= assisterItemList.Count)
        {
            for (int i = 0; i < funList.Length; ++i)
            {
                if (i < assisterItemList.Count)
                {
                    assisterItemList[i].SetShowInfo(funList[i]);
                }
                else
                {
                    GreateNewAssiterItem(funList[i]);
                }
            }
        }
        else
        {
            for (int j = 0; j < assisterItemList.Count; ++j)
            {
                if (j < funList.Length)
                {
                    assisterItemList[j].SetShowInfo(funList[j]);
                }
                else
                {
                    assisterItemList[j].gameObject.SetActive(false);
                }
            }
        }

        funcGrid.repositionNow = true;
        assisterItemScroll.ResetPosition();

    }

    private void OnBtnClick(int id)
    {
        if (mSelecteId == id) return;
        mSelecteId = id;
        IsDirty = true;
    }

    protected override void OnOpen(object param = null)
    {
        base.OnOpen(param);
        EventSystem.Instance.addEventListener(FunctionEvent.FUNCTION_UNLOCKED,OnFunctionUnlock);
        mSelecteId = Convert.ToInt32(param ?? funcBtnList[0].mBtn.CustomData);
        IsDirty = true;
    }

    protected override void OnClose()
    {
        base.OnClose();
        EventSystem.Instance.removeEventListener(FunctionEvent.FUNCTION_UNLOCKED, OnFunctionUnlock);
    }

    private void OnFunctionUnlock(EventBase evt)
    {
        IsDirty = true;
    }

    private void GreateNewAssiterItem(int itemid)
    {
        AssisterItemUI itemui = new AssisterItemUI(GameObject.Instantiate(mExamFuncItem) as GameObject);
        itemui.gameObject.SetActive(true);
        itemui.mClickCallBack = OnAssisterItemClick;
        itemui.SetShowInfo(itemid);
        itemui.gameObject.transform.parent = funcGrid.gameObject.transform;
        itemui.gameObject.transform.localScale = Vector3.one;
        assisterItemList.Add(itemui);
    }

    private void OnAssisterItemClick(int assisterid)
    {
        AssisterItemTableItem item_res = DataManager.AssisterItemTable[assisterid] as AssisterItemTableItem;
        if (item_res == null) return;
        if (item_res.openui.Equals("stagelist"))
        {
            SceneType type = (SceneType) Convert.ToInt32(item_res.openParam);
            var module = ModuleManager.Instance.FindModule<StageListModule>();
            int stageid;
                
            if (module.FindLastStage(type, out stageid))
            {
                var scene_res = DataManager.Scene_StageSceneTable[stageid] as Scene_StageSceneTableItem;
                if (scene_res == null) return;
                ModuleManager.Instance.FindModule<StageListModule>().OpenStageListUI(type,scene_res.mZoneId,stageid);
            }
        }
        else
        {
            WindowManager.Instance.OpenUI(item_res.openui);
        }
           
        WindowManager.Instance.CloseUI("assister");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        for (int i = 0; i < funcBtnList.Count; ++i)
        {
            funcBtnList[i].Clear();
        }
        funcBtnList.Clear();
        funcBtnList = null;
        for (int i = 0; i < assisterItemList.Count ;++i)
        {
            assisterItemList[i].Clear();
        }

        assisterItemList.Clear();
        assisterItemList = null;

    }
}

