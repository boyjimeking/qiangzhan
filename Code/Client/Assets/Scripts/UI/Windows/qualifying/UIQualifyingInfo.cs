using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIQualifyingInfo : UIWindow
{
	// 返回按钮
	public UIButton mReturnBtn;

	// 最高排名
	public UILabel mBestRankText;
	// 未开始排名
	public UILabel mNoBestRankText;

	// 奖励
	public UILabel mAwardText0;
	// 奖励
	public UILabel mAwardText1;

	// 排位赛规则
	public UILabel mRuleText;

	// 滑动条
	public UIScrollBar mScrollBar;
	// 滑动视图
	public UIScrollView mScrollView;

	// 声望文本
	private UILabel mReputationText;
	// 武器图片
	private UISprite mWeaponPic;
    // 武器Panel
    private GameObject mWeaponPanel;
	// 武器名称
	private UILabel mWeaponNameText;

	private QualifyingModule mModule = ModuleManager.Instance.FindModule<QualifyingModule>();

    protected override void OnLoad()
    {
		mReturnBtn = this.FindComponent<UIButton>("mReturnBtn");

		mBestRankText = this.FindComponent<UILabel>("mScrollView/mScrollSprite/mBestRankText");
		mNoBestRankText = this.FindComponent<UILabel>("mScrollView/mScrollSprite/mNoBestRankText");
		mRuleText = this.FindComponent<UILabel>("mScrollView/mScrollSprite/mRuleText");
		mAwardText0 = this.FindComponent<UILabel>("mScrollView/mScrollSprite/mAwardLabel0");
		mAwardText1 = this.FindComponent<UILabel>("mScrollView/mScrollSprite/mAwardLabel1");
		mReputationText = this.FindComponent<UILabel>("mScrollView/mScrollSprite/mReputationText");
		mWeaponPic = this.FindComponent<UISprite>("mScrollView/mScrollSprite/mWeapon/mWeaponPic");
		mWeaponNameText = this.FindComponent<UILabel>("mScrollView/mScrollSprite/mWeapon/mWeaponNameText");
        mWeaponPanel = this.FindChild("mScrollView/mScrollSprite/mWeapon");

		mRuleText.text = StringHelper.GetString("qualifying_rule");

		mScrollBar = this.FindComponent<UIScrollBar>("mScrollBar");
		mScrollView = this.FindComponent<UIScrollView>("mScrollView");

		mScrollBar.gameObject.SetActive(true);
		mScrollBar.foregroundWidget.gameObject.SetActive(false);
		mScrollBar.backgroundWidget.gameObject.SetActive(false);
    }

    //界面打开
    protected override void OnOpen(object param = null)
    {
		EventDelegate.Add(mReturnBtn.onClick, OnReturnBtnClicked);

		PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

		if (module.GetQualifyingBestRank() == uint.MaxValue)
		{
			mNoBestRankText.text = StringHelper.GetString("arena_outofrank");
			mNoBestRankText.alpha = 1.0f;
			mBestRankText.alpha = 0.0f;
		}
		else
		{
			mBestRankText.text = (module.GetQualifyingBestRank() + 1).ToString();
			mBestRankText.alpha = 1.0f;
			mNoBestRankText.alpha = 0.0f;
		}

		mAwardText0.text = "X0";
		mAwardText1.text = "X0";

		QualifyingAwardTableItem res = mModule.GetCurAwardRes();
		if(res != null)
		{
			MoneyItemTableItem prestige = ItemManager.GetItemRes((int)res.mAwardPrestige) as MoneyItemTableItem;
			if(prestige != null && prestige.value > 0)
			{
				mAwardText1.text = "X" + prestige.value.ToString();
			}

			MoneyItemTableItem gold = ItemManager.GetItemRes((int)res.mAwardGold) as MoneyItemTableItem;
			if(gold != null && gold.value > 0)
			{
				mAwardText0.text = "X" + gold.value.ToString();
			}
		}

        uint money_prestige = module.GetProceeds(ProceedsType.Money_Prestige);
        PrestigeTableItem mNextWeapon = new PrestigeTableItem();
        mNextWeapon.value = uint.MaxValue;

        IDictionaryEnumerator itr = DataManager.PrestigeTable.GetEnumerator();
        while (itr.MoveNext())
        {
            PrestigeTableItem item = itr.Value as PrestigeTableItem;

            if (item.value > money_prestige && item.value <= mNextWeapon.value)
            {
                if (item.value == mNextWeapon.value && item.sortid.CompareTo(mNextWeapon.sortid) > 0)
                    continue;
                mNextWeapon = item;
            }
        }

        if (mNextWeapon.value == uint.MaxValue)
        {
			mReputationText.text = StringHelper.GetString("qualifying_reputation2");
            mWeaponPanel.SetActive(false);
        }
        else
        {
			mReputationText.text = string.Format(StringHelper.GetString("qualifying_reputation"), mNextWeapon.value - money_prestige);
            mWeaponPanel.SetActive(true);
        }
        
        if (!DataManager.WeaponTable.ContainsKey(mNextWeapon.weaponid))
        {
            return;
        }

        WeaponTableItem weapon = DataManager.WeaponTable[mNextWeapon.weaponid] as WeaponTableItem;
        if (null == weapon)
            return;

        mWeaponNameText.text = weapon.name;
        UIAtlasHelper.SetSpriteImage(mWeaponPic, weapon.picname);
		mScrollBar.value = 0.0f;
    }

    //界面关闭
    protected override void OnClose()
    {
		EventDelegate.Remove(mReturnBtn.onClick, OnReturnBtnClicked);
    }

	private void OnReturnBtnClicked()
    {
		WindowManager.Instance.CloseUI("qualifyinginfo");
    }
}
