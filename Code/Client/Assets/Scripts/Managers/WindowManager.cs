using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System;

public enum UI_LAYER_TYPE:int
{
    UI_LAYER_DEFAULT = 0,
    UI_LAYER_WINDOWS,
	UI_LAYER_JOYSTICK,
    UI_LAYER_WIDGET,
    UI_LAYER_ITEMS,
    UI_LAYER_COUNT,
}

public enum UI_FLOW_TYPE:int
{
    UI_FLOW_INVAID = -1,
    UI_FLOW_LOGIN = 0,
    UI_FLOW_CITY = 1,
    UI_FLOW_BATTLE = 2,
}

public class OpenUIParam
{
    public string uiName = null;
    public object param = null;
    public object preOpenParam = null;
    public string returnUI = null;
}
public class WindowManager
{
    public static int MAX_DEPTH = 1001;

   // private Dictionary<string, UIWindow> mCacheWindow = new Dictionary<string, UIWindow>();

    private UIWindow[] mCacheWindows = null;


    private Dictionary<string, int> mNameToID = new Dictionary<string, int>();

    private Dictionary<string, Type> mRegisters = new Dictionary<string, Type>();

    private GameObject m2DRoot = null;
    private GameObject m3DRoot = null;

	private static WindowManager instance = null;

    public const int DepthMultiply = 20;  //每个界面预留20个深度缓冲

    private GameObject[] m2DLayers = new GameObject[(int)UI_LAYER_TYPE.UI_LAYER_COUNT];

    private GameObject[] m3DLayers = new GameObject[(int)UI_LAYER_TYPE.UI_LAYER_COUNT];

    private Queue<OpenUIParam> mOpenQueue = new Queue<OpenUIParam>();

    private string mQueueUI = null;

    private UI_FLOW_TYPE mFlowType = UI_FLOW_TYPE.UI_FLOW_INVAID;


    private static Camera mCurrent2DCamera = null;
    private static Camera mCurrent3DCamera = null;

	public static WindowManager Instance
    {
        get
        {
            return instance;
        }
    }
	public WindowManager()
    {
        instance = this;
    }

    //计算二维坐标的时候 必须加入SpectRatio 做屏幕适应
    public static float GetSpectRatio()
    {
        return UIRootAdaptive.manualHeight / (float)Screen.height;
    }
    public bool Initialize()
    {
        Init2DRoot();
        Init3DRoot();

        RegisterAllWindow();

        return true;
    }

    public bool InitCaches(DataTable table)
    {
       IDictionaryEnumerator itr =  table.GetEnumerator();
        while( itr.MoveNext() )
        {
            UITableItem item = itr.Value as UITableItem;
            if( item != null )
            {
                mNameToID.Add(item.name, item.resID);
            }
        }
        int uiNumber = mNameToID.Count;

        mCacheWindows = new UIWindow[uiNumber];

        return true;
    }

    private void Init2DRoot()
    {
        //初始化2D
        m2DRoot = ResourceManager.Instance.LoadUI("UI/UIRoot");

        if (m2DRoot == null)
            return ;

        UICamera ca = m2DRoot.GetComponentInChildren<UICamera>();
        if (ca != null)
        {
            mCurrent2DCamera = ca.camera;
        }

        GameObject.DontDestroyOnLoad(m2DRoot);

        //创建层
        for (int i = 0; i < (int)UI_LAYER_TYPE.UI_LAYER_COUNT; ++i)
        {
            GameObject layer_obj = new GameObject("Layer " + i.ToString());
            layer_obj.transform.parent = m2DRoot.transform;
            layer_obj.transform.localPosition = Vector3.zero;
            layer_obj.transform.localRotation = Quaternion.identity;
            layer_obj.transform.localScale = Vector3.one;
            layer_obj.layer = m2DRoot.layer;

            m2DLayers[i] = layer_obj;
        }

        ca.gameObject.AddComponent<GuideClickObjTool>();
    }

    private void Init3DRoot()
    {
        //初始化3D
        m3DRoot = ResourceManager.Instance.LoadUI("UI/3DRoot");

        if (m3DRoot == null)
            return ;

        UICamera ca = m3DRoot.GetComponentInChildren<UICamera>();
        if (ca != null)
        {
            mCurrent3DCamera = ca.camera;
        }

        GameObject.DontDestroyOnLoad(m3DRoot);

        //创建层
        for (int i = 0; i < (int)UI_LAYER_TYPE.UI_LAYER_COUNT; ++i)
        {
            GameObject layer_obj = new GameObject("Layer " + i.ToString());
            layer_obj.transform.parent = m3DRoot.transform;
            layer_obj.transform.localPosition = Vector3.zero;
            layer_obj.transform.localRotation = Quaternion.identity;
            layer_obj.transform.localScale = Vector3.one;
            layer_obj.layer = m3DRoot.layer;

            m3DLayers[i] = layer_obj;
        }
    }

    public static Camera current2DCamera
    {
        get
        {
            return mCurrent2DCamera;
        }
    }
    public static Camera current3DCamera
    {
        get
        {
            return mCurrent3DCamera;
        }
    }

    public void SetLayer(GameObject clone, int layer = -1)
    {
        GameObject layer_obj = m2DRoot;
        if (layer >= (int)UI_LAYER_TYPE.UI_LAYER_DEFAULT && layer < (int)UI_LAYER_TYPE.UI_LAYER_COUNT)
        {
            layer_obj = m2DLayers[layer];
        }

        clone.transform.parent = layer_obj.transform;
        clone.transform.localPosition = Vector3.zero;
        clone.transform.localRotation = Quaternion.identity;
        clone.transform.localScale = Vector3.one;
        clone.layer = layer_obj.layer;
    }

    public GameObject CloneCommonUI( string src ,int layer = -1 )
    {
        UIWindow battleWin = WindowManager.Instance.GetUI("common");
        if (battleWin == null)
        {
            return null;
        }
        GameObject obj = battleWin.FindChild(src);
        if( obj == null )
        {
            return null;
        }
        GameObject clone = UIResourceManager.Instance.CloneGameObject(obj);

        SetLayer(clone, layer);

        return clone;
    }

    public GameObject CloneGameObject(GameObject obj)
    {
        return UIResourceManager.Instance.CloneGameObject(obj);
    }

    public GameObject GetRoot()
    {
        return m2DRoot;
    }

    private void RegisterAllWindow()
    {
        RegisterWindow<UILogin>("login");
        RegisterWindow<UILoading>("loading");
        RegisterWindow<UICityForm>("city");
        RegisterWindow<UIMainMap>("mainmap");
        RegisterWindow<UIChat>("chat");
        RegisterWindow<UIStory>("story");
        RegisterWindow<UIBattleForm>("battle");
        RegisterWindow<UIQuestForm>("quest");
        RegisterWindow<UIWeapon>("weapon");
        RegisterWindow<UISkill>("skill");
        RegisterWindow<UINetWaiting>("waiting");
        RegisterWindow<UIBloodStains>("bloodstains");
        RegisterWindow<UIOperateHelp>("op_help");
        RegisterWindow<UIStageRelive>("stagerelive");
        RegisterWindow<UIStageFailed>("stagefailed");
        RegisterWindow<UIBossAlert>("bossalert");
        RegisterWindow<UIQuestAward>("questAward");
        RegisterWindow<UIJoystick>("joystick");
        RegisterWindow<UIStageEnd>("stageend");
        RegisterWindow<UIStageBalance>("stagebalance");
        RegisterWindow<UIStageList>("stagelist");
        RegisterWindow<UIQueRen>("queren");
        RegisterWindow<UICreateRole>("createrole");
        RegisterWindow<UIChallenge>("challenge");
        RegisterWindow<UIChallengeDrop>("challengeDrop");
        RegisterWindow<UIChallengeFail>("challengeFail");
        RegisterWindow<UIChallengeCountdown>("challengecountdown");
        RegisterWindow<UIQuickChallenge>("quickChallenge");
        RegisterWindow<UISweepDrop>("sweepDrop");
        RegisterWindow<UIMallForm>("mall");
        RegisterWindow<UIBagForm>("bag");
        RegisterWindow<UIDefenceForm>("defence");
        RegisterWindow<UIWing>("wing");
		RegisterWindow<UIArena>("arena");
		RegisterWindow<UIArenaInfo>("arenainfo");
        RegisterWindow<UIShopForm>("shop");
        RegisterWindow<UIQiangLingDanYu>("qianglindanyu");
        RegisterWindow<UIQiangLinDanYuOver>("qianglindanyuover");
        RegisterWindow<UIActivity>("activity");
        RegisterWindow<UIDefencePromoteHint>("defencepromotehint");
        RegisterWindow<UIRecord>("record");
        RegisterWindow<UIRanking>("ranking");
		RegisterWindow<UIQualifying>("qualifying");
		RegisterWindow<UIQualifyingInfo>("qualifyinginfo");
 		RegisterWindow<UIAnnouncement>("announcement");
        RegisterWindow<UIActivityInfo>("activityinfo");
        RegisterWindow<UIStageMaoBattle>("maostagebattle");
        RegisterWindow<UIMail>("mail");
        RegisterWindow<UIStoneCombForm>("stonecomb");      
		RegisterWindow<UIPvpBalance>("pvpbalance");
		RegisterWindow<UIMessageBox>("msgbox");
		RegisterWindow<UIWanted>("wanted");
        RegisterWindow<UIFuncOpen>("funcopen");
        RegisterWindow<UILevelUp>("levelup");
        RegisterWindow<UIEgg>("egg");
		RegisterWindow<UIFight321>("fight321");
        RegisterWindow<UIItemInfoForm>("iteminfo");
        RegisterWindow<UIItemSaleForm>("itemsale");
        RegisterWindow<UISaleAgainUIForm>("saleagain");
        RegisterWindow<UIsystemset>("systemset");
        RegisterWindow<UIsystemInfo>("systeminfo");
        RegisterWindow<UIPause>("pause");
        RegisterWindow<UITitle>("title");
        RegisterWindow<UIStageReward>("stagereward");
		RegisterWindow<UITDInfo>("tdinfo");
        RegisterWindow<UIFashionForm>("fashion");
        RegisterWindow<UIFashionAddstar>("fashionaddstar");
		RegisterWindow<UIYaZhiXieERankInfo>("yzxerankinfo");
        RegisterWindow<UIBigBag>("bigbag");  
		RegisterWindow<UIZhaoCaiMaoRankInfo>("zcmrankinfo");
        RegisterWindow<UIFundForm>("fund");
        RegisterWindow<UICrops>("crops");
        RegisterWindow<UIOtherForm>("viewplayer");
		RegisterWindow<UIMaoAlert>("maoalert");
		RegisterWindow<UIMaoRankAward>("maorankaward");
		RegisterWindow<UIMaoDamageAward>("maodamageaward");
        RegisterWindow<UIAutoFindWay>("autofindway");
        RegisterWindow<UIChallengeInfo>("challengeinfo");
        RegisterWindow<UIAssisterForm>("assister");
		RegisterWindow<UIYZXEBalance>("yzxebalance");
        RegisterWindow<UIFirstCharge>("firstcharge");
        RegisterWindow<UIplayerplan>("playerplan");
        RegisterWindow<UITotalCharge>("totalcharge");
        RegisterWindow<UICharge>("charge");
        RegisterWindow<UITitleOpen>("titleopen");
    }

    private void RegisterWindow<T>( string name )
    {
        if( mRegisters.ContainsKey(name) )
        {
            return;
        }

        mRegisters.Add(name, typeof(T));
    }

	/// <summary>
	/// 判断当前是否有输入框是激活状态;
	/// </summary>
	/// <returns><c>true</c> if this instance has input active; otherwise, <c>false</c>.</returns>
    static Type type = typeof(UIInput);
    /// 
	public bool HasInputActive()
	{
		bool result = false;

		if(UICamera.inputHasFocus)
		{

// 			UIInput input = (UIInput)UICamera.selectedObject.GetComponent(type);
// 			if(input != null && input.isSelected && NGUITools.GetActive(input))
// 			{
				result = true;
//			}
		}

		return result;
	}


    public int GetUIResID(string uiName)
    {
        if (!mNameToID.ContainsKey(uiName))
        {
            return -1;
        }
        return mNameToID[uiName];
    }

    public UIWindow GetUI(string uiName)
    {
        int id = GetUIResID(uiName);

        if( id < 0 )
        {
            return null;
        }

        if (mCacheWindows[id] == null)
        {
            return null;
        }
        return mCacheWindows[id];
    }

    public bool QueueOpenUI(string uiName , object param = null, object preOpenParam = null, string returnUI = null)
    {
        //GameDebug.LogError("QueueOpenUI uiName = " + uiName);
        //if (mQueueUI == null)
        //{
        //    mQueueUI = uiName;
        //    return OpenUI(uiName, param, preOpenParam, returnUI);
        //}

        OpenUIParam item = new OpenUIParam();
        item.uiName = uiName;
        item.param = param;
        item.preOpenParam = preOpenParam;
        item.returnUI = returnUI;

        mOpenQueue.Enqueue(item);

        return true;
    }

    public bool HasQueueOpenUI(string uiName)
    {
        OpenUIParam[] arr = mOpenQueue.ToArray();

        for( int i = 0 ; i < arr.Length ; ++i )
        {
            if (arr[i].uiName == uiName)
                return true;
        }
        return false;
    }

    public void CheckQueueUI(string uiName)
    {
        if( mQueueUI == uiName )
        {
            mQueueUI = null;
        }

        //if (mOpenQueue.Count > 0)
        //{
        //    OpenUIParam item = mOpenQueue.Dequeue();
        //    mQueueUI = item.uiName;
        //    OpenUI(item.uiName, item.param, item.preOpenParam, item.returnUI);
        //}
        ShowQueueUI();
    }

    void ShowQueueUI()
    {
        if (mOpenQueue.Count > 0)
        {
            OpenUIParam item = mOpenQueue.Dequeue();
            mQueueUI = item.uiName;
            OpenUI(item.uiName, item.param, item.preOpenParam, item.returnUI);
        }
    }

	public bool OpenUI(string uiName , object param = null, object preOpenParam = null, string returnUI = null)
	{
		if( !DataManager.UITable.ContainsKey(uiName) )
		{
			GameDebug.Log("没有找到UI :" + uiName);
			return false;
		}
		UITableItem item = DataManager.UITable[uiName] as UITableItem;
	    if (item.openSound != -1)
	    {
	        SoundManager.Instance.Play(item.openSound);
	    }

		if(!string.IsNullOrEmpty(item.hideLayer))
		{
			string[] layers = item.hideLayer.Split(new char[] {'|'});
			foreach(string layer in layers)
			{
				HideLayer(System.Convert.ToUInt32(layer));
			}
		}

		UIWindow window = null;
		//常驻内存的界面
        if (mCacheWindows[item.resID] != null)
		{
            window = mCacheWindows[item.resID];
			
			if( !window.IsOpened() )
			{
                window.SetPreOpenParam(preOpenParam);
                window.SetParam(param);
                window.Open();
			}
		}else{
            window = CreateUI(uiName);
            window.SetPreOpenParam(preOpenParam);
            window.SetParam(param);
            window.SetName(uiName);

            if( !window.Load(item.prefab) )
            {
                return false;
            }
            window.SetLayer(item.layer , item.is3D);
            //设置层级
            SetDepth(window, item.depth);


            mCacheWindows[item.resID] = window;

            window.Open();
		}

		window.SetReturnWinName(returnUI);

		return true;
	}

    public void Update(uint elapsed)
    {
        if( mCacheWindows == null )
        {
            return;
        }

        for( int i = 0 ; i < mCacheWindows.Length ; ++i )
        {
            UIWindow window = mCacheWindows[i];
            if (window != null && window.IsLoaded() && window.IsOpened())
            {
                window.Update(elapsed);
            }
        }
// 
//         var buffer = new List<string>(mCacheWindow.Keys);
//         foreach (var key in buffer)
//         {
//             UIWindow window = mCacheWindow[key] as UIWindow;
//             if (window != null && window.IsLoaded() && window.IsOpened())
//             {
//                 window.Update(elapsed);
//             }
//         }

        if(mQueueUI == null)
        {
            ShowQueueUI();
        }

        if (mQueueUI != null && !IsOpen(mQueueUI))
        {
            CheckQueueUI(mQueueUI);
        }
    }

    public void HideLayer(uint layer)
    {
        if (layer != uint.MaxValue && layer >= 0 && layer < (int)UI_LAYER_TYPE.UI_LAYER_COUNT)
        {
            GameObject layer_obj = m2DLayers[layer];
            layer_obj.SetActive(false);

            layer_obj = m3DLayers[layer];
            layer_obj.SetActive(false);
        }
    }

    public void ShowLayer(uint layer)
    {
        if (layer != uint.MaxValue && layer >= 0 && layer < (int)UI_LAYER_TYPE.UI_LAYER_COUNT)
        {
            GameObject layer_obj = m2DLayers[layer];
            layer_obj.SetActive(true);

            layer_obj = m3DLayers[layer];
            layer_obj.SetActive(true);
        }
    }

    public GameObject GetLayer(uint layer , bool is3D)
    {
        GameObject layer_obj = null;

        if (layer != uint.MaxValue && layer >= 0 && layer < (int)UI_LAYER_TYPE.UI_LAYER_COUNT)
        {
            if (is3D)
            {
                layer_obj = m3DLayers[layer];
            }else
            {
                layer_obj = m2DLayers[layer];
            }
        }

        if (layer_obj == null)
        {
            if (is3D)
            {
                layer_obj = m3DRoot;
            }else
            {
                layer_obj = m2DRoot;
            }
        }
        return layer_obj;
    }

    public UIWindow CreateUI(string uiName)
    {
        UIWindow window = null;
        if( string.IsNullOrEmpty(uiName) || !mRegisters.ContainsKey( uiName )  )
        {
            window = new UIWindow();
        }else
        {
            window = (UIWindow)Activator.CreateInstance(mRegisters[uiName]);
        }
        return window;
    }

    public void SetDepth(GameObject obj , int depth , bool autoAddPanel = false , int offset = 0)
    {
        if (depth <= 0 || depth >= MAX_DEPTH)
        {
            return;
        }
        if (obj != null)
        {
            UIPanel panel = null;
            if (autoAddPanel)
                panel = obj.AddMissingComponent<UIPanel>();
            else
                panel = obj.GetComponent<UIPanel>();

            if (panel != null)
            {
                panel.depth = depth * WindowManager.DepthMultiply + offset;
            }

            Transform transform = obj.transform;

            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (child != null)
                {
                    SetChildDepth(child, (depth * WindowManager.DepthMultiply + offset));
                }
            }
        }
    }
    public void SetChildDepth(GameObject obj, int baseDepth)
    {
        if (obj == null || obj.transform == null)
            return;
        UIPanel panel = obj.GetComponent<UIPanel>();
        if (panel != null)
        {
            panel.depth = baseDepth + panel.depth;
        }

        Transform transform = obj.transform;

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child != null)
            {
                SetChildDepth(child, baseDepth);
            }
        }
    }
    public void SetDepth(UIWindow window,int depth , bool autoAddPanel = false)
    {
        if (depth <= 0 || depth >= MAX_DEPTH)
        {
            GameDebug.LogError("界面层级填写错误 uiName = " + window.GetName());
            return;
        }
        window.SetDepth(depth,autoAddPanel);
    }

	public void CloseUI(string uiName)
	{
		if (!DataManager.UITable.ContainsKey(uiName))
		{
			GameDebug.Log("没有找到UI :" + uiName);
			return;
		}

		UITableItem item = DataManager.UITable[uiName] as UITableItem;
		if (!string.IsNullOrEmpty(item.hideLayer))
		{
			string[] layers = item.hideLayer.Split(new char[] { '|' });
			foreach (string layer in layers)
			{
				ShowLayer(System.Convert.ToUInt32(layer));
			}
		}

		if( mCacheWindows[item.resID] != null )
		{
            UIWindow obj = mCacheWindows[item.resID];

            if (obj != null && obj.IsOpened())
			{
                obj.Close();
			}
		}

       // CheckQueueUI(uiName);
	}

	public void CloseAllUI()
	{
        for (int i = 0; i < mCacheWindows.Length; ++i )
        {
            UIWindow ui = mCacheWindows[i];
            if (ui != null && ui.IsOpened())
                ui.Close();
        }
	}

    /// <summary>
    /// 判断界面是否打开
    /// </summary>
    /// <param name="uiname"></param>
    /// <returns></returns>
    public bool IsOpen(string uiname)
    {
        if (!DataManager.UITable.ContainsKey(uiname))
        {
            return false;
        }
        UITableItem item = DataManager.UITable[uiname] as UITableItem;

        if (mCacheWindows[item.resID] == null) return false;

        UIWindow uiObj = mCacheWindows[item.resID];

        return uiObj.IsOpened();

    }
	/// <summary>
	/// 根据预设名判断是否是UI界面;
	/// uiName 表示预设名字对应的界面名;
	/// </summary>
	public bool IsUIByPrefabName(string prefabName , ref string uiName)
	{
		bool res = false;
		string name = "";
		string[] sep = new string[]{"/"};
		string[] temp = null;
        IDictionaryEnumerator itr = DataManager.UITable.GetEnumerator();
        while (itr.MoveNext())
        {
            UITableItem item = itr.Value as UITableItem;
            if (item == null)
                continue;

            //			if(string.Equals(prefabName , item.prefab))
            temp = item.prefab.Split(sep, System.StringSplitOptions.None);
            if ((temp == null) || (temp.Length < 1))
            {
                break;
            }

            name = temp[temp.Length - 1];
            if (string.Equals(name, prefabName))
            {
                uiName = item.name;
                res = true;
                break;
            }
        }
// 		foreach(UITableItem item in DataManager.UITable.Values)
// 		{
// 			if(item == null)
// 				continue;
// 
// //			if(string.Equals(prefabName , item.prefab))
// 			temp = item.prefab.Split(sep , System.StringSplitOptions.None);
// 			if((temp == null) || (temp.Length < 1))
// 			{
// 				break;
// 			}
// 
// 			name = temp[temp.Length - 1];
// 			if(string.Equals(name , prefabName))
// 			{
// 				uiName = item.name;
// 				res = true;
// 				break;
// 			}
// 		}

		return res;
	}

    public void EnterFlow(UI_FLOW_TYPE flowType)
    {
        if( mFlowType != flowType )
        {
            mFlowType = flowType;
        }
        GameDebug.Log("UI进入流程  " + flowType.ToString());
    }
    //离开流程
    public void LeaveFlow(UI_FLOW_TYPE flowType)
    {
        GameDebug.Log("UI离开流程  " + flowType.ToString());

        DestroyFlowWindow(flowType);
        
        mFlowType = UI_FLOW_TYPE.UI_FLOW_INVAID;
    }

    private void DestroyFlowWindow(UI_FLOW_TYPE flowType)
    {
        for( int i = 0 ; i < mCacheWindows.Length ; ++i )
        {
            UIWindow win = mCacheWindows[i];
            if (win == null)
                continue;
            UITableItem item = DataManager.UITable[win.GetName()] as UITableItem;
            //全局 无销毁
            if (item ==null || item.flow <= (int)UI_FLOW_TYPE.UI_FLOW_INVAID)
            {
                continue;
            }
            if (item.flow == (int)mFlowType)
            {
                win.Destroy();

                mCacheWindows[i] = null;
            }
        }
    }

    public UITableItem GetUIRes(string uiName)
    {
        if (!DataManager.UITable.ContainsKey(uiName))
        {
            GameDebug.Log("没有找到UI :" + uiName);
            return null;
        }
        return  DataManager.UITable[uiName] as UITableItem;
    }

}
