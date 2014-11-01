using UnityEngine;
using System.Collections;

/// <summary>
/// 技能类型，是在技能列表中的技能、还是在技能槽中的技能;
/// </summary>
public enum SkillLockType
{
	/// 默认状态;
	None,
	/// 已经解锁了;
	UnLocked,
	/// 可以解锁,但是目前正锁定着呢;
	Opened,
	/// 不可以解锁，并锁定着呢;
	Locked,
}

public class SkillItemUI  
{
	public UISprite skillIcon;
	public UISprite selectSp;
	public UISprite lockSp;
	public UISprite unLockSp;
	public UILabel mLevel;
	public UILabel hintLb;//可解锁，26级解锁 提示文字;
	public GameObject equipObj;//该技能装备在技能槽中显示的obj;
	public UILabel equipSlotNum;//该技能装备在第几个技能槽中;

	private SkillLockType mSlt = SkillLockType.None;

    //public delegate void OnDragScrollView(Vector2 delta);
    //public OnDragScrollView onDragScrollView;

    private GameObject mView = null;

    public SkillItemUI( GameObject view )
    {
        mView = view;

        skillIcon = ObjectCommon.GetChildComponent<UISprite>(view,"skillIcon");
        selectSp = ObjectCommon.GetChildComponent<UISprite>(view, "selectSp");
        lockSp = ObjectCommon.GetChildComponent<UISprite>(view, "lockSp");
        unLockSp = ObjectCommon.GetChildComponent<UISprite>(view, "unlockSp");
        mLevel = ObjectCommon.GetChildComponent<UILabel>(view, "level");
        hintLb = ObjectCommon.GetChildComponent<UILabel>(view, "hintLb");

        equipObj = ObjectCommon.GetChild(view,"SkillNum");
        equipSlotNum = ObjectCommon.GetChildComponent<UILabel>(view, "SkillNum/numLb");
    }
	public UIButton Bt
	{
		get
		{
            return mView.GetComponent<UIButton>();
		}
	}
	
	/// <summary>
	/// 设置是否为选中状态;
	/// </summary>
	public bool IsSelected
	{
		get
		{
			if(selectSp == null)
			{
				Debug.LogError("Select Sprite is Null");
				return false;
			}

			return selectSp.gameObject.activeSelf;
		}
		set
		{
			if(selectSp == null)
			{
				Debug.LogError("Select Sprite is Null");
				return;
			}

			selectSp.gameObject.SetActive(value);
		}
	}

	/// <summary>
	/// 是否响应点击事件;	/// </summary>
	/// <value><c>true</c> if this instance is trigger; otherwise, <c>false</c>.</value>
	public bool IsTrigger
	{
		set
		{
			Bt.isEnabled = value;
		}
	}

	public bool IsShowIcon
	{
		set
		{
			skillIcon.gameObject.SetActive(value);
		}
	}

	public bool IsShowEquiSlotNum
	{
		set
		{
			equipObj.SetActive(value);
		}
	}

	public bool IsShowLv
	{
		set
		{
			mLevel.gameObject.SetActive(value);
		}
	}

	public bool IsShowHint
	{
		set
		{
			hintLb.gameObject.SetActive(value);
		}
	}

	/// <summary>
	/// 技能锁定状态;
	/// </summary>
	/// <value>The type of the S lock.</value>
	public SkillLockType SLockType
	{
		get
		{
			return mSlt;
		}

		set
		{
			if(value == mSlt)
				return;

			mSlt = value;

			switch(mSlt)
			{
			case SkillLockType.Locked://锁定状态;
				lockSp.gameObject.SetActive(true);
				unLockSp.gameObject.SetActive(false);
				break;
			case SkillLockType.Opened://可解锁状态;
				lockSp.gameObject.SetActive(false);
				unLockSp.gameObject.SetActive(true);
				break;
			case SkillLockType.UnLocked://已经解锁了;
				lockSp.gameObject.SetActive(false);
				unLockSp.gameObject.SetActive(false);
				break;
			}
		}
	}


//    void OnPress(bool isPress)
//    {
//        if(isPress)
//        {

//        }
//        else
//        {
////			shakeScale();
//        }
//    }

    //void OnDrag(Vector2 delta)
    //{
    //    //if (NGUITools.GetActive(this))
    //    //{
    //        if (onDragScrollView != null)
    //           onDragScrollView(delta);
    //    //}
    //}

	void shakeScale()
	{
		iTween.ShakeScale(mView.gameObject , new Vector3(0.08f , 0.08f , 0f) , 0.5f);
	}

	/// <summary>
	/// 设置是否显示提示文字信息;
	/// </summary>
	/// <value><c>true</c> if show level; otherwise, <c>false</c>.</value>
	public void SetShowHint(string hint , bool isShow = true)
	{
		if(hintLb == null)
		{
			Debug.LogError("hint Label is Null");
			return;
		}

		hintLb.text = "[27D5CF]" + hint + "[-]";
		hintLb.gameObject.SetActive(isShow);
	}

	/// <summary>
	/// 设置是否显示等级字样;
	/// </summary>
	/// <value><c>true</c> if show level; otherwise, <c>false</c>.</value>
	public void SetShowLevel(int lv , bool isShow = true)
	{
		if(mLevel == null)
		{
			Debug.LogError("Level Label is Null");
			return;
		}
		
		mLevel.text = "[i]Lv." + lv.ToString() + "[-]";
		mLevel.gameObject.SetActive(isShow);
	}

	/// <summary>
	/// 显示当前技能装备在技能槽中的索引，有效范围[1..4];
	/// </summary>
	public void SetShowEquipNum(int slotIdx)
	{
		if(mSlt != SkillLockType.UnLocked)
		{
			equipObj.SetActive(false);
			Debug.LogError("没解锁呢，不可能出现在装备栏中");
			return;
		}

		if(slotIdx < 1 || slotIdx > 4)
			return;

		equipSlotNum.text = slotIdx.ToString();
		equipObj.SetActive(true);
	}

	public void SetSkillIcon(string spriteInfo)
	{
		UIAtlasHelper.SetSpriteImage(skillIcon , spriteInfo);
	}
}
