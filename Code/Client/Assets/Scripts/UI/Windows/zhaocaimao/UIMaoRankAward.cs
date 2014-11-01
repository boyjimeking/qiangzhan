using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMaoRankAwardParam
{
	public int item_id;
	public int item_count;
	public uint rank;

	public UIMaoRankAwardParam(uint r, int id, int count = 1)
	{
		item_id = id;
		item_count = count;
		rank = r;
	}
}

public class UIMaoRankAward : UIWindow
{
	private GameObject mItemGrid;

	private UILabel mRankText;

	private UILabel mCountText;

	private int mTimer = 5000;

	protected override void OnLoad()
	{
		mItemGrid = FindChild("mItemGrid");
		mRankText = FindComponent<UILabel>("mRankText");
		mCountText = FindComponent<UILabel>("mCountText");
	}

	//界面打开
	protected override void OnOpen(object param = null)
	{
		UIMaoRankAwardParam uiParam = param as UIMaoRankAwardParam;
		if (uiParam == null)
			return;

		base.OnOpen(param);

		UpdateUI(uiParam);
	}

	//界面关闭
	protected override void OnClose()
	{
		mTimer = 5000;
		mRankText.text = null;
		mCountText.text = null;
		ObjectCommon.DestoryChildren(mItemGrid);

		base.OnClose();
	}

	public override void Update(uint elapsed)
	{
		base.Update(elapsed);

		if(mTimer > 0)
		{
			mTimer -= (int)elapsed;
			if(mTimer <= 0)
			{
				SceneManager.Instance.RequestEnterLastCity();
			}
		}
	}

	private void UpdateUI(UIMaoRankAwardParam param)
	{
		ObjectCommon.DestoryChildren(mItemGrid);

		if(param.rank == uint.MaxValue)
		{
			mRankText.text = StringHelper.GetString("zcm_outofrank");
		}
		else
		{
			mRankText.text = string.Format(StringHelper.GetString("zcm_rank"), (param.rank + 1));
		}

		MoneyItemTableItem moneyres = ItemManager.GetItemRes(param.item_id) as MoneyItemTableItem;
		if (moneyres != null && moneyres.value > 0)
		{
			mCountText.text = "X" + moneyres.value.ToString();
		}

		AwardItemUI awardItemUI = new AwardItemUI(param.item_id, param.item_count);
		awardItemUI.gameObject.transform.parent = mItemGrid.transform;
		awardItemUI.gameObject.transform.localPosition = Vector3.zero;
		awardItemUI.gameObject.transform.localScale = Vector3.one;
	}
}
