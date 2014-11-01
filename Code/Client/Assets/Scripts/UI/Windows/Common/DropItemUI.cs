using UnityEngine;
using System.Collections;

/// <summary>
/// 掉落道具渲染类
/// </summary>
public class DropItemUI
{
    private GameObject mView = null;
    private UIButton mButton;
    private UILabel mLabel;
    public DropItemUI(GameObject dropItem)
    {
        mView = dropItem;
        mButton = dropItem.GetComponent<UIButton>();
        mLabel = ObjectCommon.GetChildComponent<UILabel>(dropItem, "Label");
    }

    public void SetShowInfo(string icon)
    {
        UIAtlasHelper.SetButtonImage(mButton, icon);
    }
    public void SetShowInfo(int itemId)
    {
        if (itemId == -1)
        {
            SetShowInfo("common:itembg");
            return;
        }
        string iconName = ItemManager.Instance.getItemBmp(itemId);

        SetShowInfo(iconName);
    }

    public void SetEnable(bool e)
    {
        mButton.gameObject.SetActive(e);
    }

    public void SetText(string txt)
    {
        mLabel.text = txt;
    }

    public void SetActive(bool active)
    {
        if (mView != null && mView.activeSelf != active)
        {
            mView.SetActive(active);
        }
    }
    public void SetPosition(Vector3 pos)
    {
        if (mView != null)
        {
            mView.transform.localPosition = pos;
        }
    }
    public void SetGrey(bool isGrey)
    {
       UIAtlasHelper.SetSpriteGrey(mButton.GetComponent<UISprite>(),isGrey);
    }
}
