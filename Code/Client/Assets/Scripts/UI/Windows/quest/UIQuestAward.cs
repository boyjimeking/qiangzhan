
using System.Collections.Generic;
using UnityEngine;

public class UIQuestAward : UIWindow
{
    public enum UIStep
    {
        // 原始状态
        STEP_ORIGINAL = 0,

        STEP_0,

        STEP_Stop,
    }

    // Use this for initialization
    public UILabel QuestName;
    public UIButton OkBtn;
    public UIPlayTween IconTween;
    public UIGrid DropGrid;
    private float mTimer = 0.0f;
    private UIStep mCurStep = UIStep.STEP_ORIGINAL;
    private List<AwardItemUI> mAwardUIList; 
    private QuestModule mModule;

    public UIQuestAward()
    {
        mModule = ModuleManager.Instance.FindModule<QuestModule>();
    }

    protected override void OnLoad()
    {
        QuestName = this.FindComponent<UILabel>("Sprite/QuestName");
        OkBtn = this.FindComponent<UIButton>("Ok");
        IconTween = this.FindComponent<UIPlayTween>("Icon");
        DropGrid = this.FindComponent<UIGrid>("AwardGrid");
    }

    //界面打开
    protected override void OnOpen(object param = null)
    {
        EventDelegate.Add(OkBtn.onClick, OnClickClose);
       
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        QuestAwardData mCurAwardInfo = PlayerDataPool.Instance.MainData.mQuestData.GetAward();
        QuestTableItem  qti =DataManager.QuestTable[mCurAwardInfo.mQuestId] as QuestTableItem;
        QuestName.text = qti.questName;
        ObjectCommon.DestoryChildren(DropGrid.gameObject);
        if (mAwardUIList != null)
        {
            mAwardUIList.Clear();
        }
        else
        {
            mAwardUIList = new List<AwardItemUI>();
        }

        for (int i = 0; i < mCurAwardInfo.mAwardList.Count; i++)
        {
            AwardItemUI temp = new AwardItemUI(mCurAwardInfo.mAwardList[i].mResId,
                (int) mCurAwardInfo.mAwardList[i].mNum);
            temp.gameObject.transform.parent = DropGrid.gameObject.transform;
            temp.gameObject.transform.localScale = Vector3.one;

        }

        DropGrid.repositionNow = true;

        mCurStep = UIStep.STEP_ORIGINAL;

        //强制隐藏引导
        GuideModule module = ModuleManager.Instance.FindModule<GuideModule>();
        module.ForceHideGuide(true);
    }

    //界面关闭
    protected override void OnClose()
    {
        EventDelegate.Remove(OkBtn.onClick, OnClickClose);

        GuideModule module = ModuleManager.Instance.FindModule<GuideModule>();
        module.ForceHideGuide(false);
    }

    private void OnClickClose()
    {
        WindowManager.Instance.CloseUI("questAward");
        if( !mModule.ShowNextAward() )
        {
            FunctionEvent ev = new FunctionEvent(FunctionEvent.FUNCTION_CHECK_EVENT);
            EventSystem.Instance.PushEvent(ev);

            //检测助手
            ZhushouManager.Instance.Begin();
        }

        SoundManager.Instance.Play(15);
    }

    public override void Update(uint elapsed)
    {
        mTimer += Time.deltaTime;
        switch (mCurStep)
        {
            case UIStep.STEP_ORIGINAL:
            {
                IconTween.resetOnPlay = true;
                mTimer = 0.0f;
                mCurStep = UIStep.STEP_0;
            }
                break;
            case UIStep.STEP_0:
            {
                IconTween.Play(true);
                mTimer = 0.0f;
                mCurStep = UIStep.STEP_Stop;
            }
                break;
            case UIStep.STEP_Stop:
            {

            }
                break;
        }
    }

}
