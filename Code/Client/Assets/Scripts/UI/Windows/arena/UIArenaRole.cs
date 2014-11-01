
using UnityEngine;


public class UIArenaRole
{
	public UILabel mInfoText;
	public UILabel mBattleGradeText;
	public UILabel mAwardText;

	public UISprite mRoleModel;

	public UIButton mBeginBtn;

	private int mIdx = 0;

    private GameObject mObj;

	private UICharacterPreview mPreview = new UICharacterPreview();

	private ArenaModule mModule = ModuleManager.Instance.FindModule<ArenaModule>();

    public GameObject gameObject
    {
        get
        {
            return mObj;
        }
    }

	public UIArenaRole(GameObject obj)
    {
        mObj = obj;
		mInfoText = ObjectCommon.GetChildComponent<UILabel>(obj, "mRoleTopPanel/mInfoText");
		mBattleGradeText = ObjectCommon.GetChildComponent<UILabel>(obj, "mRoleTopPanel/mBattleGradeText");
		mAwardText = ObjectCommon.GetChildComponent<UILabel>(obj, "mRoleBottomPanel/mAwardText");

		mRoleModel = ObjectCommon.GetChildComponent<UISprite>(obj, "mModelPanel");

		mBeginBtn = ObjectCommon.GetChildComponent<UIButton>(obj, "mBeginBtn");

		mPreview.SetTargetSprite(mRoleModel);
		mPreview.RotationY = 180;
    }

    public void AddListener()
    {
		EventDelegate.Add(mBeginBtn.onClick, OnBeginBtnClicked);

		EventSystem.Instance.addEventListener(ArenaEvent.RECEIVE_REFRESH_DATA, OnReceiveRefreshData);
    }

    public void RemoveListener()
    {
		EventDelegate.Remove(mBeginBtn.onClick, OnBeginBtnClicked);

		EventSystem.Instance.removeEventListener(ArenaEvent.RECEIVE_REFRESH_DATA, OnReceiveRefreshData);
    }

	private void OnBeginBtnClicked()
	{
		mModule.RequestBegin(mIdx);
	}

	private void OnReceiveRefreshData(EventBase ev)
	{
		arena_simple_s simple_s = mModule.GetFighterData(mIdx);
		if(simple_s == null)
		{
			ClearUI();
			return;
		}

		this.gameObject.SetActive(true);

		mInfoText.text = string.Format(StringHelper.GetString("level_name"), simple_s.level, simple_s.name);
		mBattleGradeText.text = simple_s.grade.ToString();
		mAwardText.text = mModule.GetAwardByIdx(mIdx).ToString();

		PlayerTableItem res = DataManager.PlayerTable[simple_s.job] as PlayerTableItem;
		if(res != null)
		{
			mPreview.SetupCharacter(res.model, null,-1,uint.MaxValue);
			mPreview.ChangeWeapon((int)simple_s.weaponid);
			GameDebug.LogError("武器Id:"+simple_s.weaponid.ToString());
		}
	}

	public void ClearUI()
	{
		mInfoText.text = null;
		mBattleGradeText.text = null;
		mAwardText.text = null;

		//todo:mRoleModel

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

	public void Update()
	{
		if(this.gameObject.active)
			mPreview.Update();
	}
}
