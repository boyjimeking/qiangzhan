using UnityEngine;
using System.Collections;

//所有显示窗口的基类
public class UIWindow 
{
    protected GameObject mView = null;

    private bool mInRam = false;

    private uint mLayer = uint.MaxValue;

    private bool mIs3D = false;

    private string mWinName = "";

    protected int mDepth = -1;
    private bool mAutoPanel = false;

    private bool mLoaded = false;

    private bool mOpened = false;

    private MoneyBarUI mMoneyBar = null;
    //private EnergyBarUI mEnergyBar = null;

    private object mParam = null;
    private object mPreOpenParam = null;

	private string mReturnWinName = "";

    private string mPrefabName = "";

    public UIWindow()
    {

    }

    //界面加载完成
    protected virtual void OnLoad()
    {
        GameObject go = this.FindChild("MoneyBar");
        if (go != null)
            go.SetActive(false);

        if(mMoneyBar == null)
        {
            mMoneyBar = new MoneyBarUI(this);   
        }

        GameObject energyBar = this.FindChild("energybar");
        if (energyBar != null)
            energyBar.SetActive(false);
        //GameObject energyBar = this.FindChild("energybar");
        //if (energyBar != null)
        //{
        //    mEnergyBar = new EnergyBarUI(energyBar, this);
        //}
    }

    protected virtual void OnPreOpen(object param = null)
    {
        
    }

    //界面打开
    protected virtual void OnOpen(object param = null)
    {
        //if (mEnergyBar != null && mWinName.Equals("stagelist"))
        //    mEnergyBar.UpdateInfo();
        

    }
    //界面关闭
    protected virtual void OnClose()
    {

    }
    //界面销毁
    protected virtual void OnDestroy()
    {

    }

    public virtual void Update(uint elapsed)
    {

    }
    public bool IsOpened()
    {
        return mOpened;
    }

    public bool IsLoaded()
    {
        return mLoaded;
    }
    //打开界面
    public virtual bool Open()
    {
        mOpened = true;
        if( mView == null && !mLoaded )
        {
            return false;
        }
        OnPreOpen(mPreOpenParam);
        mView.SetActive(true);
        OnOpen(mParam);

        if(mMoneyBar != null)
        {
            mMoneyBar.OnEnable();
        }

        {
            if( IsLockMove() )
            {
                InputSystem.Instance.SetLockMove(true);
            }
        }
        //引导
        {
            GuideManager.Instance.OnOpenUI(GetName());
        }
        return true;
    }


    //关闭界面  (只能由WindowManager调用)
    public virtual bool Close()
    {
        mOpened = false;
        if (mView == null && !mLoaded)
        {
            return false;
        }
		OnClose();

        {
            if (IsLockMove())
            {
                InputSystem.Instance.SetLockMove(false);
            }
        }

        mView.SetActive(false);

		if(!string.IsNullOrEmpty(mReturnWinName))
		{
			WindowManager.Instance.OpenUI(mReturnWinName);
			mReturnWinName = null;
		}
       
        return true;
    }
    //开始加载界面
    public bool Load(string prefab)
    {
        GameDebug.Log("开始加载界面" + prefab);
        mPrefabName = prefab;
        return UIResourceManager.Instance.LoadUI(mPrefabName, OnLoadComplete);
    }

    private void OnLoadComplete(GameObject obj)
    {
        GameDebug.Log("界面加载结束" + mPrefabName);

        mView = obj;
        SetLayer(mLayer, mIs3D);
        SetDepth(mDepth,mAutoPanel);
        SetInRam(mInRam);

        OnLoad();

        if( mOpened )
        {
            this.Open();
        }
        mLoaded = true;
    }


    public T FindComponent<T>(string path) where T : Component
    {
        if (mView == null)
        {
            return null;
        }
        return ObjectCommon.GetChildComponent<T>(mView, path);
    }
    public T GetComponent<T>() where T : Component
    {
        if (mView == null)
        {
            return null;
        }
        return mView.GetComponent<T>();
    }

    public GameObject FindChild(string path)
    {
        if( mView == null )
        {
            return null;
        }
        return ObjectCommon.GetChild(mView, path);
    }

    public void SetInRam(bool inRam)
    {
        mInRam = inRam;

        if (mInRam && mView != null)
        {
            GameObject.DontDestroyOnLoad(mView);
        }
    }


    //设置显示层
    public void SetLayer(uint layer, bool is3D)
    {
        mLayer = layer;
        mIs3D = is3D;
        if( mView != null )
        {
            GameObject layer_obj = WindowManager.Instance.GetLayer((uint)layer, mIs3D);
            if( layer_obj == null )
            {
                GameDebug.LogError("没有找到UI层 layer = " + layer.ToString());
                return;
            }
            Transform t = mView.transform;
            t.parent = layer_obj.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            mView.layer = layer_obj.layer;
        }
    }

    public int GetDepth()
    {
        return mDepth;
    }
    //设置显示深度
    public void SetDepth(int depth , bool autoPanel)
    {
        mDepth = depth;
        mAutoPanel = autoPanel;

        if (mView != null)
        {
            UIPanel panel = null;
            if (mAutoPanel)
                panel = mView.AddMissingComponent<UIPanel>();
            else
                panel = mView.GetComponent<UIPanel>();

            if (panel != null)
            {
                panel.depth = depth * WindowManager.DepthMultiply;
            }

            Transform transform = mView.transform;

            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (child != null)
                {
                    WindowManager.Instance.SetChildDepth(child, (depth * WindowManager.DepthMultiply));
                }
            }
        }
    }
    public void SetPreOpenParam(object param)
    {
        mPreOpenParam = param;
    }

    public void SetParam(object param)
    {
        mParam = param;
    }

    public void SetName(string name)
    {
        mWinName = name;
    }

    public string GetName()
    {
        return mWinName;
    }

	public string GetReturnWinName()
	{
		return mReturnWinName;
	}

	public void SetReturnWinName(string name)
	{
		mReturnWinName = name;
	}

    private bool IsLockMove()
    {
       UITableItem item = WindowManager.Instance.GetUIRes(GetName());
       if (item != null)
           return item.lockMove;
       return false;
    }

    /// <summary>
    /// 获取当前界面在uiconfig表中的id;
    /// </summary>
    /// <returns></returns>
    public int GetResId()
    {
        UITableItem item = WindowManager.Instance.GetUIRes(GetName());
        if (item != null)
            return -1;
        return item.resID;
    }

    //public void SetEnergyBarShowType(EnergyBarUI.EnergyBarShowType type)
    //{
    //    if (mEnergyBar != null)
    //        mEnergyBar.SetShowType(type);
    //}

    public void SetMoneyBarActive(bool active)
    {
        if(mMoneyBar == null)
            return;

        mMoneyBar.gameObject.SetActive(active);
    }

    public void SetMoneyBarShowType(MoneyBarType type)
    {
        if (mMoneyBar != null)
            mMoneyBar.SetShowType(type);
    }

    public void SetMoneyBarShowType(BetterList<MoneyBarType> types)
    {
        if (mMoneyBar != null)
            mMoneyBar.SetShowType(types);
    }

    //销毁当前窗口 (只能由WindowManager调用)
    public void Destroy()
    {

        if (mMoneyBar != null)
        {
            mMoneyBar.Destroy();
            mMoneyBar = null;
        }

        //if( mEnergyBar != null )
        //{
        //    mEnergyBar.Destroy();
        //    mEnergyBar = null;
        //}

        if( mView != null )
        {
            OnClose();

            mView.SetActive(false);

            GameObject.Destroy(mView);

            mView = null;

            UIResourceManager.Instance.UnLoadUI(mPrefabName);
        }

        mLoaded = false;
        mOpened = false;
        OnDestroy();


    }

    public GameObject gameObject
    {
        get
        {
            return mView;
        }
    }
}
