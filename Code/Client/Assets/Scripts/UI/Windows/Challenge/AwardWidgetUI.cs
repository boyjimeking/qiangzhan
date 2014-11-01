
using UnityEngine;


public class AwardWidgetUI 
{
    private GameObject mObj;
    public UIPlayTween mTween;

    //掉落物容器
    public UIGrid DropGrid;
    private ChallengeModule mModule;

    private AwardItemUI mFirstAwardItemUI = null;
    private AwardItemUI mSecondAwardItemUI = null;

    private ChallengeCompleteParam mParam = null;
    public GameObject backGround2;
    public AwardWidgetUI(GameObject obj)
    {
        mObj = obj;
        backGround2 = ObjectCommon.GetChild(mObj, "background2");
        mTween = ObjectCommon.GetChildComponent<UIPlayTween>(mObj, "Award");
        DropGrid = ObjectCommon.GetChildComponent<UIGrid>(mObj, "Award/DropGrid");
        UIEventListener.Get(backGround2).onClick = CloseUI;

    }

    public void setShow(bool isShow)
    {
        mObj.SetActive(isShow);
    }

    void CloseUI(GameObject target)
    {
        setShow(false);
    }

    public void PlayTween()
    {
        mTween.resetOnPlay = true;
        mTween.Play(true);
    }

    public void SetShowInfo(ChallengeCompleteParam param)
    {
        mParam = param;
        if (mParam == null)
            return;
        ObjectCommon.DestoryChildren(DropGrid.gameObject);

        for (int i = 0; i < param.mDrops.Count; i++)
        {
            AwardItemUI awardItemUI = new AwardItemUI(param.mDrops[i].mResId, param.mDrops[i].mNum);
            awardItemUI.gameObject.transform.parent = DropGrid.gameObject.transform;
            awardItemUI.gameObject.transform.localScale = Vector3.one;
        }

        DropGrid.repositionNow = true;
    }
}

