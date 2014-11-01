using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public enum PosDir : int
{
    up = 1,
    down = 2,
    left = 3,
    right = 4,
}

//控件查找类型
public enum CTRL_FIND_TYPE:int
{
    FIND_TYPE_BAG_ITEM = 0,
    FIND_TYPE_QUEST_SELECT_ID = 1,
    FIND_TYPE_STAGE_SELECT_ID = 2,
    FIND_TYPE_EQUIP_ITEM = 3,

    FIND_TYPE_CHECK_EQUIP_ITEM = 4,
    FIND_TYPE_CHECK_NO_SKILL = 5,
}

public class GuideRuntime 
{
    private bool mRuning = false;

    private int mCurStep = -1;

    private List<GuideStepTableItem> mSteps = new List<GuideStepTableItem>();

    private GameObject mControl = null;
    private bool mHide = false;
    private int mDepth = 1000;

    private int mResID = -1;
    public GuideRuntime(int resID , string script)
    {
        mResID = resID;
        InitScript(script);
    }

    public int GetResID()
    {
        return mResID;
    }
    public GuideTableItem GetRes()
    {
        if (!DataManager.GuideTable.ContainsKey(mResID))
            return null;

        return DataManager.GuideTable[mResID] as GuideTableItem;
    }

    private void InitScript(string script)
    {
        string[] steps = script.Split(new char[] { '|' });

        for( int i = 0 ; i < steps.Length ; ++i )
        {
            int id = System.Convert.ToInt32(steps[i]);
            if (!DataManager.GuideStepTable.ContainsKey(id))
            {
                GameDebug.LogError("guide表中对应的，guidestepid:" + id + "不存在");
                continue;
            }
            
            mSteps.Add(DataManager.GuideStepTable[id] as GuideStepTableItem);
        }
    }

    public Vector3 GetControlPos()
    {
        if (mControl == null)
            return Vector3.zero;
        //在场景里未显示
        if (!mControl.activeInHierarchy)
            return Vector3.zero;

        //UIWidget widget = mControl.GetComponent<UIWidget>();
        //Vector3 pos = mControl.transform.localPosition;

        //if (widget != null)
        //{
        //    switch (widget.pivot)
        //    {
        //        case UIWidget.Pivot.Bottom:
        //            pos.y += widget.height / 2f;
                    
        //            break;
        //        case UIWidget.Pivot.Center:
        //            break;
        //        default:
        //            Debug.LogError("现只支持bottom和center");
        //            break;
        //    }
        //}
        //mControl.transform.localPosition = pos;
        
        //WorldToScreenPoint 效率如何?  考虑不在Update里处理
        return WindowManager.current2DCamera.WorldToScreenPoint(mControl.transform.position);
    }

    public int GetDepth()
    {
        return mDepth;
    }

    public bool IsWeak()
    {
        if (mCurStep < 0 || mCurStep >= mSteps.Count)
        {
            return false;
        }
        GuideStepTableItem node = mSteps[mCurStep];

        return (node.isWeak > 0);
    }
    public bool IsShowMask()
    {
        if (mCurStep < 0 || mCurStep >= mSteps.Count)
        {
            return false;
        }
        GuideStepTableItem node = mSteps[mCurStep];

        return ( node.showMask > 0 );
    }

    public bool IsHelperReplace()
    {
        GuideTableItem item = GetRes();
        if (item == null)
            return false;
        return item.helperReplace > 0;
    }

    public bool IsHide()
    {
        return mHide;
    }

    public Vector2 GetControlPovitOffset()
    {
        if (mControl == null)
            return Vector2.zero;
        
        UIWidget widget = mControl.gameObject.GetComponent<UIWidget>();
        if (widget != null)
        {
            switch (widget.pivot)
            {
                case UIWidget.Pivot.Center:
                    return Vector2.zero;
                case UIWidget.Pivot.Bottom:
                    return new Vector2(0f, widget.height / 2f);
            }
        }

        return Vector2.zero;
    }

    public int GetControlWidth()
    {
        if (mControl == null)
            return 0;
        UIWidget widget = mControl.gameObject.GetComponent<UIWidget>();
        if (widget != null)
        {
            return widget.width;
        }
        BoxCollider box = mControl.gameObject.GetComponent<BoxCollider>();

        if(box != null)
        {
            return (int)box.size.x;
        }

        return 0;
    }
    public int GetControlHeight()
    {
        if (mControl == null)
            return 0;
        UIWidget widget = mControl.gameObject.GetComponent<UIWidget>();
        if (widget != null)
        {
            return widget.height;
        }
        BoxCollider box = mControl.gameObject.GetComponent<BoxCollider>();

        if (box != null)
        {
            return (int)box.size.y;
        }
        return 0;
    }

    public ArrowRot GetArrowRot()
    {
        if (mCurStep < 0 || mCurStep >= mSteps.Count)
        {
            return ArrowRot.ArrowRot_Invaild;
        }

        GuideStepTableItem node = mSteps[mCurStep];

        if (node.arrow <= (int)ArrowRot.ArrowRot_Invaild || node.arrow > (int)ArrowRot.ArrowRot_RIGHT)
            return ArrowRot.ArrowRot_Invaild;

        return (ArrowRot)node.arrow;
    }

    private void NextStep()
    {
        mControl = null; 

        ++ mCurStep;

        if( mCurStep < 0 || mCurStep >= mSteps.Count )
        {
            End();
            //GameDebug.LogError("引导操作完成 ID= " + mResID.ToString());
            return;
        }
    }

    public void Begin()
    {
        mRuning = true;
        NextStep();
    }

    public void End()
    {
        mRuning = false;
        GuideManager.Instance.OnGuideEnd();
    }
    public void Destroy()
    {
        End();
    }

    public bool IsRuning()
    {
        return mRuning;
    }

    public void Update(uint elapsed)
    {
        if (!mRuning)
        {
            return;
        }

        if( mCurStep < 0 || mCurStep >= mSteps.Count )
        {
            return;
        }

        GuideStepTableItem node = mSteps[mCurStep];

        if( mControl == null )
        {
            mControl = FindControl(node);

            if( mControl != null )
            {
                RegisterTalk(node);
                RegisterEvent(mControl);
            }
        }

        UpdateHideState();
    }

    private GameObject FindControl(GuideStepTableItem node)
    {
        GameObject obj = null;
        //如果填表错误 一直找不到对象  需要处理 
        UIWindow window = WindowManager.Instance.GetUI(node.window);
        if( window == null )
        {
            return null;
        }

        mDepth = window.GetDepth() /*+ 1*/;

        if( node.param1 >= 0 )
        {
            switch( node.param1 )
            {
                case (int)CTRL_FIND_TYPE.FIND_TYPE_BAG_ITEM:
                    {
                        if( window is UIBagForm )
                        {
                            UIBagForm bag = (UIBagForm)window;

                            return bag.FindBagItem(node.param2);
                        }
                    }
                    break;
                case (int)CTRL_FIND_TYPE.FIND_TYPE_EQUIP_ITEM:
                    {
                        if (window is UIBagForm)
                        {
                            UIBagForm bag = (UIBagForm)window;

                            return bag.FindEquipItem(node.param2);
                        }
                    }break;
                case (int)CTRL_FIND_TYPE.FIND_TYPE_QUEST_SELECT_ID:
                    {
                        if( window is UIQuestForm )
                        {
                            UIQuestForm quest = (UIQuestForm)window;

                            return quest.FindEnterBtn(node.param2);
                        }
                    }
                    break;
                case (int)CTRL_FIND_TYPE.FIND_TYPE_STAGE_SELECT_ID:
                    {
                        if (window is UIStageList)
                        {
                            UIStageList stage = (UIStageList)window;

                            return stage.FindEnterBtn(node.param2);
                        }
                    }
                    break;
                 case (int)CTRL_FIND_TYPE.FIND_TYPE_CHECK_EQUIP_ITEM:
                    {
                        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

                        PackageManager pack = module.GetPackManager();

                        Dictionary<int, ItemObj> dic = pack.getPackDic(PackageType.Pack_Equip);

                        bool find = false;
                        foreach (KeyValuePair<int, ItemObj> value in dic)
                        {
                            if( value.Value.GetResId() == node.param2 )
                            {
                               find = true;
                            }
                        }
                        if( !find )
                        {
                            End();
                            return null;
                        }

                    }break;
                 case (int)CTRL_FIND_TYPE.FIND_TYPE_CHECK_NO_SKILL:
                     {
                         SkillModule module = ModuleManager.Instance.FindModule<SkillModule>();

                        if( module.GetSkillLvBySkillID( node.param2 ) > 0 )
                        {
                            End();
                            return null;
                        }
                     } break;
            }
        }
        obj = window.FindChild(node.ctrl);
        return obj;
    }

    private void UpdateHideState()
    {
        if (mCurStep < 0 || mCurStep >= mSteps.Count)
        {
            return;
        }
        GuideStepTableItem node = mSteps[mCurStep];
        UIWindow window = WindowManager.Instance.GetUI(node.window);
        if (window == null)
        {
            return ;
        }
        if( (int)CTRL_FIND_TYPE.FIND_TYPE_QUEST_SELECT_ID == node.param1 )
        {
            UIQuestForm quest = (UIQuestForm)window;

            mHide = (quest.GetCurrentID() != node.param2);
        }else if( (int)CTRL_FIND_TYPE.FIND_TYPE_STAGE_SELECT_ID == node.param1 )
        {
            UIStageList stage = (UIStageList)window;

            mHide = (stage.GetCurrentID() != node.param2);
        }else
        {
            mHide = false;
        }
    }


    private void SaveCompleteState()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
        {
            return ;
        }
        string name = module.GetName();
        string key = name + "_guide_" + mResID.ToString();
        PlayerPrefs.SetInt(key, mCurStep);
    }

    private void OnControlComplete()
    {
        UnRegisterTalk();
        UnRegisterEvent(mControl);
        SaveCompleteState();
        //尝试进入下一步
        NextStep();
    }

    private void RegisterTalk(GuideStepTableItem node)
    {
        //MM对话
        if (!string.IsNullOrEmpty(node.talk))
        {
            GuideManager.Instance.RegisterTalk(node.talk, node.talkpos);
        }
    }

    private void UnRegisterTalk()
    {
        GuideManager.Instance.UnRegisterTalk();
    }

    private void RegisterEvent(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }
        if( mCurStep < 0 || mCurStep >= mSteps.Count )
        {
            return;
        }
        GuideStepTableItem node = mSteps[mCurStep];

        if (node.ctrlType == "button")
        {
            UIButton btn = obj.GetComponent<UIButton>();
            if (btn != null)
            {
                EventDelegate.Add(btn.onClick, OnControlComplete);
            }
        }

        if (node.ctrlType == "toggle")
        {
            UIToggle toggle = obj.GetComponent<UIToggle>();
            if (toggle != null)
            {
                EventDelegate.Add(toggle.onChange, OnControlComplete);
            }
        }
    }

    private void UnRegisterEvent(GameObject obj)
    {
        if( obj == null )
        {
            return;
        }
        if (mCurStep < 0 || mCurStep >= mSteps.Count)
        {
            return;
        }
        GuideStepTableItem node = mSteps[mCurStep];

        if (node.ctrlType == "button")
        {
            UIButton btn = obj.GetComponent<UIButton>();
            if (btn != null)
            {
                EventDelegate.Remove(btn.onClick, OnControlComplete);
            }
        }

        if (node.ctrlType == "toggle")
        {
            UIToggle toggle = obj.GetComponent<UIToggle>();
            if (toggle != null)
            {
                EventDelegate.Remove(toggle.onChange, OnControlComplete);
            }
        }
    }

}
