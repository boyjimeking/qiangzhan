
using System;
using UnityEngine;
using FantasyEngine;
public class UIWingPreview : UIPreviewBase
{
	private int mResid;

	private MeshVisual mWing;

	GameObject mWingAttach;

	public UIWingPreview ():base()
	{
	}

	public override void Update()
	{
		base.Update();

		if(mWing != null && mWing.Visual != null)
		{
			mWingAttach.transform.localEulerAngles = new Vector3(-90, RotationY, 0);

			Renderer renderer = mWing.Visual.GetComponentInChildren<Renderer>();
			if(renderer != null)
			{
				mWing.Visual.transform.localPosition = new Vector3(0, -renderer.bounds.extents.y,0);
			}
		}
	}

	public void SetupWing(int resid)
	{
		if(!DataManager.WingCommonTable.ContainsKey(resid)) return;

		WingCommonTableItem wingtable = DataManager.WingCommonTable[resid] as WingCommonTableItem;

		if(wingtable == null)
		{
			GameDebug.LogError("WingTable 没有找到 id = " + resid.ToString());
			return ;
		}

		if(resid != mResid)
		{
			if(mWing != null)
			{
				mWing.Destroy();
			}

			mResid = resid;
			mWing = new MeshVisual();
			WingItemData itemData = PlayerDataPool.Instance.MainData.mWingData.GetWingItemDataById(mResid);
		    string modleName = WingModule.GetModelName(itemData.id, itemData.level);
			//mWing.CreateWithConfig(AssetConfig.WeaponPath + modleName,
		}

	}

	private void OnVisualSuccess()
	{
		if(mWingAttach == null)
		{
			mWingAttach = new GameObject();
			mWingAttach.transform.parent = mPreviewRoot.transform;
			mWingAttach.transform.localPosition = Vector3.zero;
			mWingAttach.transform.localEulerAngles = new Vector3(-90,0,0);
		}

		//设置显示层次
		ObjectCommon.SetObjectAndChildrenLayer(mWing.Visual,layermask);
		mWing.Visual.transform.parent = mWingAttach.transform;
		mWing.Visual.transform.localPosition = Vector3.zero;
		mWing.Visual.transform.localEulerAngles = Vector3.zero;
	}

	private void OnVisualFailed()
	{

	}

	public override void Destroy()
	{
		if(mWing != null)
			mWing.Destroy();
		mWing = null;
		base.Destroy();
	}	
}
		


