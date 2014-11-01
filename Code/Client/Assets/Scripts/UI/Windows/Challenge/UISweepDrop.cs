using System.Collections.Generic;
using UnityEngine;


public class UISweepDrop : UIWindow 
{
    private UILabel mLabel;
    private GameObject ScrolV;
    private GameObject Table;
    private int mScrolIndex = 0;
    private List<ChallengeSweepParam> mAwardItem;
    private List<UILabel> mLabels;
    private UISprite mMask;
    private UIButton mCloseBtn;

    public UISweepDrop()
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        mLabel = FindComponent<UILabel>("text");
        ScrolV = FindChild("Scroll View");
        Table = FindChild("Scroll View/GameObject");
        mMask = FindComponent<UISprite>("mask");
        mCloseBtn = FindComponent<UIButton>("noBt");

    }

    private void OnMaskClick(GameObject obj)
    {
       CloseUI();
    }

    private void CloseUI()
    {
        WindowManager.Instance.CloseUI("sweepDrop");
    }

    protected override void OnOpen(object param = null)
    {
        UIEventListener.Get(mMask.gameObject).onClick = OnMaskClick;
        EventDelegate.Add(mCloseBtn.onClick, CloseUI);
        mAwardItem = param as List<ChallengeSweepParam>;
        if (mAwardItem == null)
            return;
        ClearUI();
        mLabels = new List<UILabel>();
        mLabel.text = "";

        for (int i = 0; i < mAwardItem.Count; i++)
        {
            var obj = GameObject.Instantiate(mLabel.gameObject) as GameObject;
            obj.transform.parent = Table.transform;
            obj.transform.localPosition = new Vector3(0, 0, 0);
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.name = i.ToString();
            obj.SetActive(false);
            var lb = obj.GetComponent<UILabel>();
            lb.text = GetShowText(mAwardItem[i]);
            mLabels.Add(lb);

        }
        Repositon();

        if (mLabels.Count > 0)
        {
            mLabels[0].gameObject.SetActive(true);
            mLabels[0].gameObject.GetComponent<FTypewriterEffect>().enabled = true;
            mLabels[0].gameObject.GetComponent<FTypewriterEffect>().SetPText(mLabels[0].processedText);
            mLabels[0].gameObject.GetComponent<FTypewriterEffect>().CallBack = TypeWritterCallback;
        }
       
    }

    private void Repositon()
    {
        int height = 0;

        for (int i = 0; i < mLabels.Count; i++)
        {
            mLabels[i].gameObject.transform.localPosition = new Vector3(0, height, 0);
            height -= (mLabels[0].height + 30);
        }
    }

    protected override void OnClose()
    {
        base.OnClose();
        UIEventListener.Get(mMask.gameObject).onClick = null;
        EventDelegate.Remove(mCloseBtn.onClick, CloseUI);
    }
   
    public void TypeWritterCallback()
    {
        if (mAwardItem == null)
            return;

        if (mScrolIndex < mAwardItem.Count - 1)
        {

            iTween.MoveTo(Table.gameObject, iTween.Hash
                 ("y", Table.transform.localPosition.y + mLabels[mScrolIndex].height,
                 "islocal", true,
                 "time", 0.3f,
                 "easetype", iTween.EaseType.linear));
            mScrolIndex++;
            string str = mLabels[mScrolIndex].processedText;

            mLabels[mScrolIndex].text = "";

            mLabels[mScrolIndex].gameObject.SetActive(true);

            var obj = mLabels[mScrolIndex].gameObject;

            obj.GetComponent<FTypewriterEffect>().enabled = true;

            obj.GetComponent<FTypewriterEffect>().SetPText(str);

            obj.GetComponent<FTypewriterEffect>().CallBack = TypeWritterCallback;

        }

    }


    private void ClearUI()
    {
        if (mLabels != null)
        {
            for (int i = 0; i < mLabels.Count; i++)
            {
                Object.DestroyImmediate(mLabels[i].gameObject);
            }
            mLabels.Clear();

        }

        ScrolV.GetComponent<UIScrollView>().panel.clipOffset.Set(0,-80);
        ScrolV.GetComponent<UIScrollView>().ResetPosition();
        mScrolIndex = 0;
    }
  

    private string GetShowText(ChallengeSweepParam cps)
    {
        string re = "";
        re += string.Format(StringHelper.GetString("sweep_drop"), cps.mFloor);
        for (int i = 0; i < cps.mDrops.Count; i++)
        {
           re += ItemManager.Instance.getItemName(cps.mDrops[i].mResId) + "x" + cps.mDrops[i].mNum + "  ";
        }
        return re;
    }
}
