using UnityEngine;
using System.Collections;

public class UILoading : UIWindow
{
    public UISlider slider;
    public UISlider slider2;

    public UILabel bossNameLb;
    public UILabel bossDetailLb;
    public UISprite bossSp;
    public UILoading()
    {
        
    }

    protected override void OnLoad()
    {
        slider = this.FindComponent<UISlider>("cityScene/Progress1");
        slider2 = this.FindComponent<UISlider>("fightScene/Progress2");
        bossNameLb = this.FindComponent<UILabel>("fightScene/details/title");
        bossDetailLb = this.FindComponent<UILabel>("fightScene/details/detail");
        bossSp = this.FindComponent<UISprite>("fightScene/boss/bossSp");

        EventSystem.Instance.addEventListener(LoadingEvent.LOADING_PROGRESS, onProgress);

    }
    protected override void OnOpen(object param = null)
    {
        slider.value = 0.0f;
        slider2.value = 0.0f;

        UpdateInfo();
    }
    protected override void OnClose()
    {
        FightGradeManager.Instance.CheckAndPlayGradeChangeEff();

        if(TitleModule.Instance != null)
            TitleModule.Instance.CheckAndPlayTitleUnlock();
    }

    private void onProgress(EventBase evt)
    {
        if (!slider.gameObject.activeSelf)
            return;
        LoadingEvent ev = (LoadingEvent)evt;
        float val = 0.01f * ev.progress;
        slider.value = val;
        slider2.value = val;
    }
    void UpdateInfo()
    {
        BaseScene bs = SceneManager.Instance.GetCurScene();
        if (bs == null)
        {
            slider.transform.parent.gameObject.SetActive(true);
            slider2.transform.parent.gameObject.SetActive(false);
        }
        else
        {

            //int resId = bs.GetSceneResId();
            //if(!DataManager.SceneTable.ContainsKey(resId))
            //    return;

            //SceneTableItem sti = DataManager.SceneTable[resId] as SceneTableItem;
            SceneTableItem sti = bs.GetSceneRes();
            if (sti == null)
                return;

            switch (SceneManager.GetSceneType(sti))
            {
                case SceneType.SceneType_City://主城;
                    slider.transform.parent.gameObject.SetActive(true);
                    slider2.transform.parent.gameObject.SetActive(false);
                    break;
                case SceneType.SceneType_Stage: //关卡场景;
                case SceneType.SceneType_Tower:
                case SceneType.SceneType_Zombies:
                case SceneType.SceneType_MonsterFlood:
                case SceneType.SceneType_QiangLinDanYu:
                case SceneType.SceneType_Mao:
                case SceneType.SceneType_HunNeng:
                    slider.transform.parent.gameObject.SetActive(false);
                    slider2.transform.parent.gameObject.SetActive(true);

                    bossNameLb.text = sti.bossName;
                    bossDetailLb.text = sti.bossDetail;
                    UIAtlasHelper.SetSpriteImage(bossSp, sti.bossSprite, true);
                    break;
                default:

                    break;
            }
        }
    }
}
