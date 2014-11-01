
using UnityEngine;


public class UIQualifyingRole
{
	public UILabel mInfoText;
	public UILabel mBattleGradeText;
	public UILabel mRankingText;

	public UISprite mRoleIcon;

	public UIButton mBeginBtn;

	private int mIdx = 0;

    private GameObject mObj;

	private QualifyingModule mModule = ModuleManager.Instance.FindModule<QualifyingModule>();

    public GameObject gameObject
    {
        get
        {
            return mObj;
        }
    }

	public UIQualifyingRole(GameObject obj)
    {
        mObj = obj;
		mInfoText = ObjectCommon.GetChildComponent<UILabel>(obj, "mRoleInfoPanel/mInfoText");
		mBattleGradeText = ObjectCommon.GetChildComponent<UILabel>(obj, "mRoleInfoPanel/mBattleGradeText");
		mRankingText = ObjectCommon.GetChildComponent<UILabel>(obj, "mRoleInfoPanel/mRankingText");

		mRoleIcon = ObjectCommon.GetChildComponent<UISprite>(obj, "mRoleInfoPanel/mRoleIcon");

		mBeginBtn = ObjectCommon.GetChildComponent<UIButton>(obj, "mBeginBtn");
    }

    public void AddListener()
    {
		EventDelegate.Add(mBeginBtn.onClick, OnBeginBtnClicked);

		EventSystem.Instance.addEventListener(QualifyingEvent.RECEIVE_LIST_DATA, OnReceiveListData);
    }

    public void RemoveListener()
    {
		EventDelegate.Remove(mBeginBtn.onClick, OnBeginBtnClicked);

		EventSystem.Instance.removeEventListener(QualifyingEvent.RECEIVE_LIST_DATA, OnReceiveListData);
    }

	private void OnBeginBtnClicked()
	{
		mModule.RequestBegin(mIdx);
	}

	private void OnReceiveListData(EventBase ev)
	{
		qualifying_simple_s simple_s = mModule.GetFighterData(mIdx);
		if(simple_s == null)
		{
			ClearUI();
			return;
		}

		this.gameObject.SetActive(true);

		mInfoText.text = string.Format(StringHelper.GetString("level_name"), simple_s.level, simple_s.name);
		mBattleGradeText.text = simple_s.grade.ToString();
		mRankingText.text = (mModule.GetRankingByIdx(mIdx) + 1).ToString();
		UIAtlasHelper.SetSpriteImage(mRoleIcon, "touxiang:qhead_" + simple_s.job.ToString());
	}

	public void ClearUI()
	{
		mInfoText.text = null;
		mBattleGradeText.text = null;
		mRankingText.text = null;

		UIAtlasHelper.SetSpriteImage(mRoleIcon, null);

		this.gameObject.SetActive(false);
	}

	// 在列表中位置
	public int Idx
	{
		get
		{
			return mIdx;
		}

		set
		{
			mIdx = value;
		}
	}
}
