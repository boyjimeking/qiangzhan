
  using System;
  using System.Collections.Generic;
  using UnityEngine;

public class AssisterItemUI
{
    public GameObject gameObject;
    public UISprite funcIcon;
    public UILabel funcName;
    public UILabel funcdesc;
    public List<UISprite> starList;
    public UIButton doBtn;
    public ItemClickCallBack mClickCallBack;
    public AssisterItemUI(GameObject obj)
    {
        gameObject = obj;
        funcIcon = ObjectCommon.GetChildComponent<UISprite>(obj, "funcIcon");
        funcName = ObjectCommon.GetChildComponent<UILabel>(obj, "funcName");
        funcdesc = ObjectCommon.GetChildComponent<UILabel>(obj, "desc");
        doBtn = ObjectCommon.GetChildComponent<UIButton>(obj, "doBtn");
        starList = new List<UISprite>(5);
        for (int i = 0; i < 5; i++)
        {
            starList.Add(ObjectCommon.GetChildComponent<UISprite>(obj, "stargrid/s" + i));
        }
        EventDelegate.Add(doBtn.onClick, OnBtnClick);
    }

    public void Clear()
    {
        EventDelegate.Remove(doBtn.onClick, OnBtnClick);
        gameObject = null;
        funcIcon = null;
        funcName = null;
        funcdesc = null;
        starList.Clear();
        starList = null;
        doBtn = null;
        mClickCallBack = null;
        
    }

    private void OnBtnClick()
    {
        mClickCallBack(Convert.ToInt32(doBtn.CustomData));
        SoundManager.Instance.Play(15);
    }

    public void SetShowInfo(int id)
    {
        AssisterItemTableItem item_res = DataManager.AssisterItemTable[id] as AssisterItemTableItem;
        if (item_res == null) return;
        
        UIAtlasHelper.SetSpriteImage(funcIcon, item_res.icon);
        funcName.text = item_res.funcName;
        funcdesc.text = item_res.desc;
        doBtn.CustomData = id;
        for (int i = 0; i < 5; i++)
        {
            starList[i].gameObject.SetActive(i < item_res.starNum);
        }
        gameObject.SetActive(true);

        //判断功能开启
        if (item_res.funcid != -1)
        {
            FunctionModule fmodule = ModuleManager.Instance.FindModule<FunctionModule>();
            MenuTableItem menu_res = DataManager.MenuTable[item_res.funcid] as MenuTableItem;
            if (menu_res == null) return;
            doBtn.isEnabled = fmodule.IsFunctionUnlock(menu_res.mId);
        }
        else
        {
            if (item_res.openui.Equals("stagelist"))
            {
                int tempId;
                SceneType type = (SceneType) (Convert.ToInt32(item_res.openParam));
                doBtn.isEnabled = ModuleManager.Instance.FindModule<StageListModule>().FindLastStage(type, out tempId);
            }
            else
            {
                doBtn.isEnabled = true;
            }
            
        }
       


    }
}

