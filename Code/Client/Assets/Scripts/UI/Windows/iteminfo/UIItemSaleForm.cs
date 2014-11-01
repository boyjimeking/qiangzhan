using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using Message;

public class ItemUIParam
{
    public int itemid = -1;
    public bool isSaleAll = true;
    public PackageType packtype = PackageType.Invalid;
    public int packpos = -1;
    public int mSaleNum = -1;
}

public class UIItemSaleForm : UIWindow
{
    //Button
    private UIButton mBtnCancle = null;
    private UIButton mBtnYes = null;
    private UIButton mBtnSub = null;
    private UIButton mBtnAdd = null;
    //~Btn
    private UILabel mInput = null;
    private UIInput mUIInput = null;

    private ItemUIParam mParam = null;

    private int mItemMax = 1;
    private int mItemMin = 1;

    private bool isSubDown = false;
    private bool isAddDown = false;
    private uint mTime = 0;
    private const uint mOnPressCd = 100;

    public UIItemSaleForm()
    {

    }
    protected override void OnLoad()
    {
        base.OnLoad();

        //btn
        mBtnCancle = this.FindComponent<UIButton>("btnCancle");
        mBtnYes = this.FindComponent<UIButton>("btnYes");
        mBtnSub = this.FindComponent<UIButton>("btnsub");
        mBtnAdd = this.FindComponent<UIButton>("btnadd");
        //~btn
        mInput = this.FindComponent<UILabel>("input/Label");
        mUIInput = this.FindComponent<UIInput>("input");
    }
    protected override void OnOpen(object param = null)
    {
        mParam = (ItemUIParam)param;

        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        uint num = module.GetItemNumByIDAndPos(mParam.itemid,mParam.packpos,mParam.packtype);
        mInput.text = num.ToString();
        mItemMax = (int)num;
        InitUI();
    }
    protected override void OnClose()
    {

    }
    public override void Update(uint elapsed)
    {
        mTime += elapsed;
        if (isSubDown && mTime >= mOnPressCd)
        {
            mTime = 0;
            OnBtnSubHandler();
        }
        if (isAddDown && mTime >= mOnPressCd)
        {
            mTime = 0;
            OnBtnAddHandler();
        }
    }

    private void InitUI()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        EventDelegate.Add(mBtnCancle.onClick, OnBtnCancleHandler);
        EventDelegate.Add(mBtnYes.onClick, OnBtnYesHandler);
        EventDelegate.Add(mBtnSub.onClick, OnBtnSubHandler);
        EventDelegate.Add(mBtnAdd.onClick, OnBtnAddHandler);
        UIEventListener.Get(mBtnSub.gameObject).onPress = OnBtnSubPress;
        UIEventListener.Get(mBtnAdd.gameObject).onPress = OnBtnAddPress;
        EventDelegate.Add(mUIInput.onChange, OnChange);

        mUIInput.onValidate = OnValidate;
    }

    private void OnBtnCancleHandler()
    {
        SoundManager.Instance.Play(15);
        WindowManager.Instance.CloseUI("itemsale");
    }

    private void OnBtnYesHandler()
    {
        SoundManager.Instance.Play(15);
        mParam.mSaleNum = System.Convert.ToInt32(mInput.text);
        if (mParam.mSaleNum < mItemMax)
            mParam.isSaleAll = false;
        else
            mParam.isSaleAll = true;
        WindowManager.Instance.OpenUI("saleagain", mParam);
    }

    private void OnBtnSubHandler()
    {
        SoundManager.Instance.Play(15);
        int citem = System.Convert.ToInt32(mInput.text);
        if (citem > mItemMin)
            mInput.text = (citem - 1).ToString();
    }

    private void OnBtnAddHandler()
    {
        SoundManager.Instance.Play(15);
        int citem = System.Convert.ToInt32(mInput.text);
        if(citem < mItemMax)
            mInput.text = (citem + 1).ToString();
    }

    public void OnBtnSubPress(GameObject target, bool isPressed)
    {
        isSubDown = isPressed;
    }

    public void OnBtnAddPress(GameObject target, bool isPressed)
    {
        isAddDown = isPressed;
    }

    public void OnChange()
    {
        //PopTipManager.Instance.AddNewTip(mUIInput.value);
    }

    public char OnValidate(string text, int charIndex, char addedChar)
    {
        if ('0' > addedChar || '9' < addedChar)
            return (char)0;
        if (System.Convert.ToInt32(text + addedChar) > mItemMax)
            return (char)0;
        return addedChar;
    }
}
