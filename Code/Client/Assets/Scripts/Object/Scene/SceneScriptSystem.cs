using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using Object = UnityEngine.Object;

/// <summary>
/// 区域
/// </summary>
public class Zone
{

    // 区域类型
    public enum ZoneType : int
    {
        ZoneType_Invalid = -1,          // 无效
        ZoneType_Event = 0,             // 事件区域
    }

    // 类型
    public ZoneType type;

    // 形状
    public SceneShape shape;

    // 名称
    public string name;

    public Zone()
    {

    }

    // 包含
    public bool contains(Vector2 pt)
    {
        return shape.contains(pt);
    }

    // 相交
    public bool intersect(SceneShapeRect rect)
    {
        return shape.intersect(rect);
    }
}

/// <summary>
/// 格子
/// </summary>
public class Cell
{
    public int row;
    public int col;

    public ArrayList zones = new ArrayList();
    public ArrayList sprites = new ArrayList();

    public Cell(int r, int c)
    {
        row = r;
        col = c;
    }

    public static bool operator ==(Cell lhs, Cell rhs)
    {
        return Object.Equals(lhs, rhs);
    }

    public static bool operator !=(Cell lhs, Cell rhs)
    {
        return !Object.Equals(lhs, rhs);
    }
    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        if (GetType() != obj.GetType())
            return false;
        Cell other = obj as Cell;
        return ((other.row == this.row) && (other.col == this.col));
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public ArrayList GetZoneList()
    {
        return zones;
    }

    public bool AddZone(Zone zone)
    {
        if(zones.Contains(zone))
        {
            return false;
        }

        zones.Add(zone);

        return true;
    }

    public bool AddSprite(ObjectBase sprite)
    {
        if(sprites.Contains(sprite))
        {
            return false;
        }

        if(sprite.GetCell() == this)
        {
            return false;
        }

        sprites.Add(sprite);

        sprite.SetCell(this);

        return true;
    }

    public bool RemoveSprite(ObjectBase sprite)
    {
        if(sprite.GetCell() != this)
        {
            return false;
        }

        if(!sprites.Contains(sprite))
        {
            return false;
        }

        sprites.Remove(sprite);

        sprite.SetCell(null);

        return true;
    }
}

// xml出生点
public class XMLInitPosStruct
{
	// 名称
	public string mName;

	// 坐标
	public Vector3 mInitPos = Vector3.zero;

	// 方向
	public float mInitDir = 0.0f;
}

// xml事件结构
public class XMLEventStruct
{
    // 事件类型
    public string mType;

    // 事件名
    public string mName;

    // 事件参数
	public List<string> mParams = new List<string>();

    // 功能列表
    public ArrayList mFunctions = new ArrayList();

    public XMLEventStruct()
    {
    }


}

// xml条件结构
public class XMLConditionStruct
{
    // 条件类型
    public string mType;

    // 条件名
    public string mName;

    // 条件参数
	public List<string> mParams = new List<string>();

    public XMLConditionStruct()
    {

    }
}

// xml功能结构
public class XMLFunctionStruct
{
    // 功能类型
    public string mType;

    // 功能名
    public string mName;

    // 功能参数
	public List<string> mParams = new List<string>();

    // 条件列表
    public ArrayList mConditions = new ArrayList();

    public XMLFunctionStruct()
    {

    }
}


/// <summary>
/// 脚本.xml
/// </summary>
public class SceneScriptSystem
{
    // 出生点
	private List<XMLInitPosStruct> mInitPosList = new List<XMLInitPosStruct>();

    public Vector3 mCameraInfo = new Vector3(-45, -45, 5);

	//刷怪器
	//private GrowthTrigger mGrowthTrigger = null;

    // 刷怪器
    private Hashtable mTriggers = new Hashtable();

    // 区域
    private Hashtable mZones = new Hashtable();

    // 是否初始化
    private static bool mInited = false;

    // 全部场景xml接口函数映射
    private static Dictionary<string, SceneFunctionInterface> mSceneInterfaceMap = null;

    // xml事件回调函数
    public delegate bool SceneCallback(XMLEventStruct e, List<string> param);
    private static Dictionary<string, SceneCallback> mSceneCallbackMap = null;

    // XML事件是否初始化完成
    private bool mEventsInited = false;
    // XML事件列表
    //private ArrayList mEventsList = new ArrayList();
    // XML事件名称映射
    private Dictionary<string, XMLEventStruct> mEventsNameMap = new Dictionary<string, XMLEventStruct>();
    // XML事件类型映射
    private Dictionary<string, ArrayList> mEventsTypeMap = new Dictionary<string, ArrayList>();

    // XML条件列表
    //private ArrayList mConditionsList = new ArrayList();
    // XML条件名称映射
    private Dictionary<string, XMLConditionStruct> mConditionsNameMap = new Dictionary<string, XMLConditionStruct>();

    // XML功能是否初始化完成
    private bool mFunctionsInited = false;
    // XML功能列表
    //private ArrayList mFunctionsList = new ArrayList();
    // XML功能名称映射
    private Dictionary<string, XMLFunctionStruct> mFunctionsNameMap = new Dictionary<string, XMLFunctionStruct>();

    // 当前场景
	private BaseScene mScene = null;

    public SceneScriptSystem(BaseScene scn)
	{
		mScene = scn;
	}

	// 进入点
	public Vector3 GetInitPos(int idx = 0)
	{
		if (mInitPosList == null || idx < 0 || idx >= mInitPosList.Count)
			return Vector3.zero;

		return mInitPosList[idx].mInitPos;
	}

	// 进入点
	public Vector3 GetInitPosByName(string name)
	{
		if (mInitPosList == null)
			return Vector3.zero;

		foreach(XMLInitPosStruct initStruct in mInitPosList)
		{
			if(string.Equals(initStruct.mName, name))
				return initStruct.mInitPos;
		}

		return Vector3.zero;
	}

	// 进入点角色方向
	public float GetInitDir(int idx = 0)
	{
		if (mInitPosList == null || idx < 0 || idx >= mInitPosList.Count)
			return 0.0f;

		return mInitPosList[idx].mInitDir;
	}

	// 进入点角色方向
	public float GetInitDirByName(string name)
	{
		if (mInitPosList == null)
			return 0.0f;

		foreach (XMLInitPosStruct initStruct in mInitPosList)
		{
			if (string.Equals(initStruct.mName, name))
				return initStruct.mInitDir;
		}

		return 0.0f;
	}

    // 区域
    public Hashtable Zones
    {
        get
        {
            return mZones;
        }
    }

    // 是否初始化
    public static bool Inited
    {
        get
        {
            return mInited;
        }
    }

    // 读取xml数据
    private bool InitXMLData(string txt)
    {
        if( string.IsNullOrEmpty(txt) )
			return false;
		XmlDocument xDoc = new XmlDocument();
		xDoc.LoadXml( txt );
		XmlNode node = xDoc.FirstChild;
		node = node.NextSibling;
		if( node.Name != "Root" )
		{
			return false;
		}
		XmlNodeList nodeList = node.ChildNodes;
		for( int i = 0 ; i < nodeList.Count ; ++i )
		{
			XmlNode childNode = nodeList[i];
			if( childNode.Name == "Head" )
			{
				ParseHead( childNode );
				continue;
			}

			if( childNode.Name == "Triggers" )
			{
				ParseTriggers( childNode );
				continue;
			}

            if( childNode.Name == "Zones" )
            {
                ParseZones(childNode);
                continue;
            }

            if( childNode.Name == "Events" )
            {
                ParseEvents(childNode);
                continue;
            }

            if( childNode.Name == "Functions" )
            {
                ParseFunctions(childNode);
                continue;
            }

            if( childNode.Name == "Conditions" )
            {
                ParseConditions(childNode);
                continue;
            }
		}
		return true;
    }

    // 注册xml接口函数映射
    public static void InitSceneInterfaceMap()
    {
		mSceneInterfaceMap = new Dictionary<string, SceneFunctionInterface>();
		mSceneInterfaceMap.Add("StartTrigger", SceneInterface.StartTrigger);
		mSceneInterfaceMap.Add("StopTrigger", SceneInterface.StopTrigger);
		mSceneInterfaceMap.Add("SetIntParam", SceneInterface.SetIntParam);
		mSceneInterfaceMap.Add("SetFloatParam", SceneInterface.SetFloatParam);
		mSceneInterfaceMap.Add("CheckIntParam", SceneInterface.CheckIntParam);
		mSceneInterfaceMap.Add("CheckFloatParam", SceneInterface.CheckFloatParam);
		mSceneInterfaceMap.Add("SetResult", SceneInterface.SetResult);
        mSceneInterfaceMap.Add("onHpDamageAward", SceneInterface.onHpDamageAward);
		mSceneInterfaceMap.Add("PassStage", SceneInterface.PassStage);
		mSceneInterfaceMap.Add("TalkPop", SceneInterface.TalkPop);
		mSceneInterfaceMap.Add("IncIntParam", SceneInterface.IncIntParam);
		mSceneInterfaceMap.Add("IncFloatParam", SceneInterface.IncFloatParam);
		mSceneInterfaceMap.Add("OpenUI", SceneInterface.OpenUI);
		mSceneInterfaceMap.Add("CloseUI", SceneInterface.CloseUI);
		mSceneInterfaceMap.Add("StartCurTrigger", SceneInterface.StartCurTrigger);
		mSceneInterfaceMap.Add("StartCameraPath", SceneInterface.StartCameraPath);
		mSceneInterfaceMap.Add("StopCameraPath", SceneInterface.StopCameraPath);
		mSceneInterfaceMap.Add("PauseCameraPath", SceneInterface.PauseCameraPath);
		mSceneInterfaceMap.Add("BossAlert", SceneInterface.BossAlert);
        mSceneInterfaceMap.Add("ZombieCrazy", SceneInterface.ZombieCrazy);
        mSceneInterfaceMap.Add("ZombieTenSecond", SceneInterface.ZombieTenSecond);
        mSceneInterfaceMap.Add("ZombieEnermyNum", SceneInterface.ZombieSetEnermyNum);
        mSceneInterfaceMap.Add("SaySomething", SceneInterface.SaySomething);
		mSceneInterfaceMap.Add("LogicPause", SceneInterface.LogicPause);
		mSceneInterfaceMap.Add("LogicResume", SceneInterface.LogicResume);
		mSceneInterfaceMap.Add("StartStory", SceneInterface.StartStory);
		mSceneInterfaceMap.Add("RemoveObjectByAlias", SceneInterface.RemoveObjectByAlias);
		mSceneInterfaceMap.Add("RemoveParticleByAlias", SceneInterface.RemoveParticleByAlias);
		mSceneInterfaceMap.Add("SetCamera", SceneInterface.SetCamera);
		mSceneInterfaceMap.Add("KillObjectByAlias", SceneInterface.KillObjectByAlias);
		mSceneInterfaceMap.Add("LockCamera", SceneInterface.LockCamera);
        mSceneInterfaceMap.Add("SetChallengeTotalNum", SceneInterface.SetChallengeTotalNum);
        mSceneInterfaceMap.Add("SetChallengeNum", SceneInterface.SetChallengeNum);
        mSceneInterfaceMap.Add("CheckPickable", SceneInterface.CheckPickable);
        mSceneInterfaceMap.Add("RestartTrigger", SceneInterface.RestartTrigger);
		mSceneInterfaceMap.Add("AddBuff", SceneInterface.AddBuff);
		mSceneInterfaceMap.Add("PlaySceneAni", SceneInterface.PlaySceneAni);
		mSceneInterfaceMap.Add("ShowGuide", SceneInterface.ShowGuide);
		mSceneInterfaceMap.Add("DarkScene", SceneInterface.DarkScene);
		mSceneInterfaceMap.Add("DeDarkScene", SceneInterface.DeDarkScene);
		mSceneInterfaceMap.Add("StopDynamicOBJToTarget", SceneInterface.StopDynamicOBJToTarget);
		mSceneInterfaceMap.Add("PauseDynamicOBJ", SceneInterface.PauseDynamicOBJ);
		mSceneInterfaceMap.Add("PlayDynamicOBJ", SceneInterface.PlayDynamicOBJ);
		mSceneInterfaceMap.Add("ShowTimer", SceneInterface.ShowTimer);
		mSceneInterfaceMap.Add("TransInScene", SceneInterface.TransInScene);
        mSceneInterfaceMap.Add("GiveSkill", SceneInterface.GiveSkill);
		mSceneInterfaceMap.Add("CheckFirstEnter", SceneInterface.CheckFirstEnter);
        mSceneInterfaceMap.Add("SetPlayerLeague", SceneInterface.SetPlayerLeague);
        mSceneInterfaceMap.Add("UnlockWeaponSkill", SceneInterface.UnlockWeaponSkill);
		mSceneInterfaceMap.Add("ShowProgress", SceneInterface.ShowProgress);
		
    }

    // 注册xml事件响应函数
    public static void InitSceneCallbackMap()
    {
		mSceneCallbackMap = new Dictionary<string,SceneCallback>();
		mSceneCallbackMap.Add("onGameStart", onGameStart);
		mSceneCallbackMap.Add("onTriggerFinish", onTriggerFinish);
		mSceneCallbackMap.Add("onGrowthTriggerFinish", onGrowthTriggerFinish);
		mSceneCallbackMap.Add("onTriggerStart", onTriggerStart);
		mSceneCallbackMap.Add("onPlayerEnterZone", onPlayerEnterZone);
		mSceneCallbackMap.Add("onPlayerLeaveZone", onPlayerLeaveZone);
		mSceneCallbackMap.Add("onKillEnemy", onKillEnemy);
        mSceneCallbackMap.Add("onHpDamage", onHpDamage);
		mSceneCallbackMap.Add("onCameraPathFinish", onCameraPathFinish);
		mSceneCallbackMap.Add("onCameraPathEvent", onCameraPathEvent);
		mSceneCallbackMap.Add("onSceneInited", onSceneInited);
		mSceneCallbackMap.Add("onPick", onPick);
		mSceneCallbackMap.Add("onStoryEnd", onStoryEnd);
		mSceneCallbackMap.Add("onSceneAniFinished", onSceneAniFinished);
       
		//mSceneCallbackMap.Add("onStoryStep", onStoryStep);不提供了，在story.txt中有两列功能trigger
    }

    // 初始化
	public bool Init (string txt) 
	{
		if(!Inited)
        {
            InitSceneInterfaceMap();
            InitSceneCallbackMap();

			mInited = true;
        }

		EventSystem.Instance.addEventListener(StoryEvent.STORY_END, onStoryEndEvent);
		EventSystem.Instance.addEventListener(CameraAnimatorEvent.CAMERA_ANIMATOR_FINISH, onCameraAnimatorFinish);
		EventSystem.Instance.addEventListener(CameraAnimatorEvent.CAMERA_EVENT_POINT_TRIGGER, onCameraEventPointTrigger);

        return InitXMLData(txt);
	}

	// 销毁
	public void Destroy()
	{
		EventSystem.Instance.removeEventListener(StoryEvent.STORY_END, onStoryEndEvent);
		EventSystem.Instance.removeEventListener(CameraAnimatorEvent.CAMERA_ANIMATOR_FINISH, onCameraAnimatorFinish);
		EventSystem.Instance.removeEventListener(CameraAnimatorEvent.CAMERA_EVENT_POINT_TRIGGER, onCameraEventPointTrigger);
	}

	// 摄像机路径完成
	void onCameraAnimatorFinish(EventBase evt)
	{
		CameraAnimatorEvent e = evt as CameraAnimatorEvent;

		List<string> arr = new List<string>();
		arr.Add(e.mName);
		TriggerEvent("onCameraPathFinish", arr);

		//temp
		CameraController.Instance.StopCameraAniamtion();
	}

	// 摄像机事件点到达
	void onCameraEventPointTrigger(EventBase evt)
	{
		CameraAnimatorEvent e = evt as CameraAnimatorEvent;

		List<string> arr = new List<string>();
		arr.Add(e.mName);
		TriggerEvent("onCameraPathEvent", arr);
	}

	// 情节结束
	void onStoryEndEvent(EventBase evt)
	{
		StoryEvent e = evt as StoryEvent;

		List<string> arr = new List<string>();
		arr.Add(e.mStoryId.ToString());
		TriggerEvent("onStoryEnd", arr);
	}

    // 事件立即触发
	public void TriggerEvent(string type, List<string> param)
    {
        if(!mEventsTypeMap.ContainsKey(type))
        {
            return;
        }

        // xml监听的该事件类型的事件列表
        ArrayList list = mEventsTypeMap[type];
        if(list == null)
        {
            return;
        }

        foreach(XMLEventStruct e in list)
        {
            if(!mSceneCallbackMap.ContainsKey(e.mType))
            {
                continue;
            }

            // 通过事件参数判断是否为我感兴趣的事件
            SceneCallback func = mSceneCallbackMap[e.mType];
            if (!func(e, param))
            {
                continue;
            }

            if (e.mFunctions == null)
            {
                continue;
            }

            // 遍历该事件触发后调用的功能
            foreach(XMLFunctionStruct f in e.mFunctions)
            {
                if(!doFunction(f))
                {
                    continue;
                }
            }
        }
    }

    // 外部调用功能
    public bool InvokeFunction(string name)
    {
        if(string.IsNullOrEmpty(name))
        {
            return false;
        }

        if(!mFunctionsNameMap.ContainsKey(name))
        {
            return false;
        }

        XMLFunctionStruct xmlfunc = mFunctionsNameMap[name];
        return doFunction(xmlfunc);
    }

    // 调用功能
    protected bool doFunction(XMLFunctionStruct f)
    {
        if (f == null)
        {
            return false;
        }

        // 遍历该功能绑定的条件检查
        foreach (XMLConditionStruct c in f.mConditions)
        {
            if (mSceneInterfaceMap.ContainsKey(c.mType))
            {
                // 调用场景提供的功能接口
                SceneFunctionInterface funcCondition = mSceneInterfaceMap[c.mType];
                if (!funcCondition(c.mParams))
                {
                    return false;
                }
            }
        }

        // 检查成功 调用功能函数
        if (mSceneInterfaceMap.ContainsKey(f.mType))
        {
            SceneFunctionInterface funcInterface = mSceneInterfaceMap[f.mType];
            funcInterface(f.mParams);
        }

        return true;
    }

    // 解析Head
	private void ParseHead(XmlNode node)
	{
		XmlNodeList nodeList = node.ChildNodes;

        for(int i=0; i<nodeList.Count; ++i)
        {
            XmlNode childNode = nodeList[i];

            		    //InitPos
		    if( childNode != null && childNode.Name == "InitPos" )
		    {
				XMLInitPosStruct initStruct = new XMLInitPosStruct();
				initStruct.mName = childNode.Attributes["name"].Value;
				initStruct.mInitPos.x = System.Convert.ToSingle(childNode.Attributes["x"].Value);
				initStruct.mInitPos.z = System.Convert.ToSingle(childNode.Attributes["z"].Value);
				initStruct.mInitDir = System.Convert.ToSingle(childNode.Attributes["dir"].Value) * Mathf.Deg2Rad;
				mInitPosList.Add(initStruct);
		    }

            if( childNode != null && childNode.Name == "Camerainfo" )
		    {
			    mCameraInfo.x = System.Convert.ToSingle(childNode.Attributes["anglex"].Value);
			    mCameraInfo.y = System.Convert.ToSingle(childNode.Attributes["angley"].Value);
			    mCameraInfo.z = System.Convert.ToSingle(childNode.Attributes["distance"].Value);
		    }
        }
	}

    // 解析Triggers
	private void ParseTriggers(XmlNode node)
	{
		XmlNodeList nodeList = node.ChildNodes;
		for( int i = 0 ; i < nodeList.Count ; ++i )
		{
			XmlNode childNode = nodeList[i];
			if( childNode != null && childNode.Name == "Trigger" )
			{
				string type = childNode.Attributes["type"].Value;
				if( type == "growth" )
				{
					ParseNpcGrowth(childNode);
                    continue;
				}

                if(type == "function")
                {
                    ParseFunctionTrigger(childNode);
                    continue;
                }
			}
		}
	}

    // 解析NpcGrowth
	private void ParseNpcGrowth(XmlNode node)
	{
		GrowthTrigger trigger = new GrowthTrigger(mScene);
		trigger.name = node.Attributes["name"].Value;
		XmlNodeList nodeList = node.ChildNodes;
		for( int i = 0 ; i < nodeList.Count ; ++i )
		{
			XmlNode childNode = nodeList[i];
			if( childNode != null && childNode.Name == "Step" )
			{
				GrowthTriggerStep step = new GrowthTriggerStep();
				int ka =  System.Convert.ToInt32(childNode.Attributes["killAll"].Value);
				step.killAll = (ka != 0);
				step.time = System.Convert.ToInt32(childNode.Attributes["time"].Value);

				if(childNode.Attributes["repeat"] != null && childNode.Attributes["spacetime"] != null)
				{
					step.repeat = System.Convert.ToInt32(childNode.Attributes["repeat"].Value);
					step.spacetime = System.Convert.ToInt32(childNode.Attributes["spacetime"].Value);
				}

				XmlNodeList stepList = childNode.ChildNodes;
                for (int j = 0; j < stepList.Count; ++j)
				{
					XmlNode stepNode = stepList[j];

					if( stepNode != null && stepNode.Name == "Growth" )
					{
						GrowthTriggerInfo info = null;
						string growthtype = stepNode.Attributes["type"].Value;
						if(growthtype.Equals("PICK"))
						{
							info = new PickGrowthTriggerInfo();
							(info as PickGrowthTriggerInfo).picktype = System.Convert.ToInt32(stepNode.Attributes["picktype"].Value);
							(info as PickGrowthTriggerInfo).content = System.Convert.ToInt32(stepNode.Attributes["content"].Value);
						}
						else if(growthtype.Equals("BUILD"))
						{
							info = new BuildGrowthTriggerInfo();
							(info as BuildGrowthTriggerInfo).barrier = System.Convert.ToInt32(stepNode.Attributes["barrier"].Value);
						}
						else
						{
							info = new GrowthTriggerInfo();
						}

						info.type = growthtype;
						info.resId = System.Convert.ToInt32(stepNode.Attributes["resId"].Value);
						info.x = System.Convert.ToSingle(stepNode.Attributes["x"].Value);
						info.z = System.Convert.ToSingle(stepNode.Attributes["z"].Value);
						info.dir = System.Convert.ToSingle(stepNode.Attributes["dir"].Value) * Mathf.Deg2Rad;

                        if (stepNode.Attributes["talkid"] != null)
                        {
                            info.talkID = System.Convert.ToInt32(stepNode.Attributes["talkid"].Value);
                        }

						if (stepNode.Attributes["name"] != null)
						{
							info.alias = stepNode.Attributes["name"].Value;
						}

						step.objs.Add( info );
					}
				}
				trigger.steps.Add( step );
			}
		}

		mTriggers.Add( trigger.name, trigger );
	}

    // 解析功能触发器
    private void ParseFunctionTrigger(XmlNode node)
    {
        FunctionTrigger trigger = new FunctionTrigger(mScene);
        trigger.name = node.Attributes["name"].Value;
        XmlNodeList nodeList = node.ChildNodes;
        for (int i = 0; i < nodeList.Count; ++i)
        {
            XmlNode childNode = nodeList[i];
            if (childNode != null && childNode.Name == "Step")
            {
                FunctionTriggerStep step = new FunctionTriggerStep();
                step.time = System.Convert.ToInt32(childNode.Attributes["time"].Value);

                XmlNodeList stepList = childNode.ChildNodes;
                for (int j = 0; j < nodeList.Count; ++j)
                {
                    XmlNode stepNode = stepList[j];
                    if (stepNode != null && stepNode.Name == "FuncTrigger")
                    {
                        FunctionTriggerInfo info = new FunctionTriggerInfo();
                        info.funcname = stepNode.Attributes["function"].Value;

                        step.functions.Add(info);
                    }else if (stepNode != null && stepNode.Name == "TimeTrigger")
                    {
                        TimeTriggerInfo info= new TimeTriggerInfo();
                        info.mTotalTime = step.time;
                        step.times.Add(info);
                    }

                    
                }
                trigger.steps.Add(step);
            }
        }
     
        mTriggers.Add(trigger.name, trigger);
    }

    // 解析Zones
    private void ParseZones(XmlNode node)
    {
        XmlNodeList nodeList = node.ChildNodes;
        for (int i = 0; i < nodeList.Count; ++i)
        {
            XmlNode childNode = nodeList[i];
            if (childNode != null && childNode.Name == "Zone")
            {
                int type = System.Convert.ToInt32(childNode.Attributes["type"].Value);
                if((Zone.ZoneType)type == Zone.ZoneType.ZoneType_Invalid || !System.Enum.IsDefined(typeof(Zone.ZoneType), type))
                {
                    continue;
                }

                Zone zone = new Zone();
                zone.type = (Zone.ZoneType)type;
                zone.name = childNode.Attributes["name"].Value;
                
                ShapeType shapetype = (ShapeType)(System.Convert.ToInt32(childNode.Attributes["shapeType"].Value));
                if(shapetype == ShapeType.ShapeType_Round)
                {
                    zone.shape = new SceneShapeRound();
                    SceneShapeRound round = zone.shape as SceneShapeRound;
                    round.mCenter.x = (float)System.Convert.ToDouble(childNode.Attributes["x"].Value);
                    round.mCenter.y = (float)System.Convert.ToDouble(childNode.Attributes["y"].Value);
                    round.mRadius = (float)System.Convert.ToDouble(childNode.Attributes["r"].Value);
                }
                else if(shapetype == ShapeType.ShapeType_Rect)
                {
                    zone.shape = new SceneShapeRect();
                    SceneShapeRect rect = zone.shape as SceneShapeRect;
                    rect.mLeft = (float)System.Convert.ToDouble(childNode.Attributes["x1"].Value);
                    rect.mTop = (float)System.Convert.ToDouble(childNode.Attributes["y1"].Value);
                    rect.mRight = (float)System.Convert.ToDouble(childNode.Attributes["x2"].Value);
                    rect.mBottom = (float)System.Convert.ToDouble(childNode.Attributes["y2"].Value);
                }
                else if(shapetype == ShapeType.ShapeType_Polygon)
                {
                    zone.shape = new SceneShapePolygon();
                    SceneShapePolygon polygon = zone.shape as SceneShapePolygon;
                    
                    int index = 1;
                    XmlAttribute attribX = null;
                    XmlAttribute attribY = null;

                    while((attribX = childNode.Attributes["x"+index.ToString()]) != null && (attribY = childNode.Attributes["y"+index.ToString()]) != null)
                    {
                        polygon.PushbackVector2(new Vector2((float)System.Convert.ToDouble(attribX.Value), (float)System.Convert.ToDouble(attribY.Value)));
                        index++;
                    }
                }

                mZones.Add(zone.name, zone);
            }
        }
    }

    // 解析Events
    private void ParseEvents(XmlNode node)
    {
        XmlNodeList nodeList = node.ChildNodes;
        for (int i = 0; i < nodeList.Count; ++i)
        {
            XmlNode childNode = nodeList[i];
            if (childNode != null && childNode.Name == "Event")
            {
                string type = childNode.Attributes["type"].Value;
                if(type == null || type.Length < 1)
                {
                    continue;
                }

                XMLEventStruct xmlEvent = new XMLEventStruct();
                xmlEvent.mName = childNode.Attributes["name"].Value;
                xmlEvent.mType = childNode.Attributes["type"].Value;

                int index = 1;
                XmlAttribute attrib = null;
                while((attrib = childNode.Attributes["param"+index.ToString()]) != null)
                {
                    xmlEvent.mParams.Add(attrib.Value);
                    index++;
                }

                //mEventsList.Add(xmlEvent);
				if(mEventsNameMap.ContainsKey(xmlEvent.mName))
				{
					GameDebug.LogError("ScriptXML Error : Event自命名重复！name:" + xmlEvent.mName);
				}

                mEventsNameMap.Add(xmlEvent.mName, xmlEvent);

                ArrayList arr = null;
                if(mEventsTypeMap.ContainsKey(xmlEvent.mType))
                {
                    arr = mEventsTypeMap[xmlEvent.mType];
                }

                if(arr == null)
                {
                    arr = new ArrayList();
                    mEventsTypeMap.Add(xmlEvent.mType, arr);
                }
                
                arr.Add(xmlEvent);
            }
        }

        mEventsInited = true;
    }

    // 解析Functions
    private void ParseFunctions(XmlNode node)
    {
        if(!mEventsInited)
        {
			GameDebug.LogError("ScriptXML Error : Events 需要位于 Functions 之前!");
            return;
        }

        XmlNodeList nodeList = node.ChildNodes;
        for (int i = 0; i < nodeList.Count; ++i)
        {
            XmlNode childNode = nodeList[i];
            if (childNode != null && childNode.Name == "Function")
            {
                string type = childNode.Attributes["type"].Value;
                if (type == null || type.Length < 1)
                {
                    continue;
                }

                XMLFunctionStruct xmlFunction = new XMLFunctionStruct();
                xmlFunction.mName = childNode.Attributes["name"].Value;
                xmlFunction.mType = childNode.Attributes["type"].Value;

                int index = 1;
                XmlAttribute attrib = null;
                while ((attrib = childNode.Attributes["param" + index.ToString()]) != null)
                {
                    xmlFunction.mParams.Add(attrib.Value);
                    index++;
                }

                //mFunctionsList.Add(xmlFunction);
				if(mFunctionsNameMap.ContainsKey(xmlFunction.mName))
				{
					GameDebug.LogError("ScriptXML Error : Function自命名重复！name:" + xmlFunction.mName);
				}

                mFunctionsNameMap.Add(xmlFunction.mName, xmlFunction);

                XmlAttribute evtattrib = childNode.Attributes["event"];
                if(evtattrib != null)
                {
                    string eventName = evtattrib.Value;
                    if (mEventsNameMap.ContainsKey(eventName))
                    {
                        mEventsNameMap[eventName].mFunctions.Add(xmlFunction);
                    }

//                    GameDebug.Log("mEventsNameMap.mFunctions.Count:" + mEventsNameMap[eventName].mFunctions.Count.ToString() +
//                    "         mEventTypeMap.mFunctions.Count:" + (mEventsTypeMap[mEventsNameMap[eventName].mType][0] as XMLEventStruct).mFunctions.Count.ToString());
                }
            }
        }

        mFunctionsInited = true;
    }

    // 解析Condition
    private void ParseConditions(XmlNode node)
    {
        if(!mFunctionsInited)
        {
            GameDebug.LogError("ScriptXML Error : Functions 需要位于 Conditions 之前!");
            return;
        }

        XmlNodeList nodeList = node.ChildNodes;
        for (int i = 0; i < nodeList.Count; ++i)
        {
            XmlNode childNode = nodeList[i];
            if (childNode != null && childNode.Name == "Condition")
            {
                string type = childNode.Attributes["type"].Value;
                if (type == null || type.Length < 1)
                {
                    continue;
                }

                XMLConditionStruct xmlCondition = new XMLConditionStruct();
                xmlCondition.mName = childNode.Attributes["name"].Value;
                xmlCondition.mType = childNode.Attributes["type"].Value;

                int index = 1;
                XmlAttribute attrib = null;
                while ((attrib = childNode.Attributes["param" + index.ToString()]) != null)
                {
                    xmlCondition.mParams.Add(attrib.Value);
                    index++;
                }

                //mConditionsList.Add(xmlFunction);
				if(mConditionsNameMap.ContainsKey(xmlCondition.mName))
				{
					GameDebug.LogError("ScriptXML Error : Condition自命名重复！name:" + xmlCondition.mName);
				}

                mConditionsNameMap.Add(xmlCondition.mName, xmlCondition);

                string functionName = childNode.Attributes["function"].Value;
                if(mFunctionsNameMap.ContainsKey(functionName))
                {
                    mFunctionsNameMap[functionName].mConditions.Add(xmlCondition);
                }
            }
        }
    }

    // onGameStart回调
    public static bool onGameStart(XMLEventStruct e, List<string> param)
    {
        return true;
    }

	// onSceneInited回调
	public static bool onSceneInited(XMLEventStruct e, List<string> param)
	{
		return true;
	}

    // onTriggerFinish回调
	public static bool onTriggerFinish(XMLEventStruct e, List<string> param)
    {
        if(param.Count != 1)
        {
            return false;
        }

        if(e.mParams.Count != 1)
        {
            return false;
        }

        string strParam = param[0];
        if(strParam == null || strParam.Length < 1)
        {
            return false;
        }

        string strEvent = e.mParams[0];
        if(strEvent == null || strEvent.Length < 1)
        {
            return false;
        }

        return strParam.Equals(strEvent);
    }

	public static bool onSceneAniFinished(XMLEventStruct e, List<string> param)
	{
		if (param.Count != 1)
		{
			return false;
		}

		if (e.mParams.Count != 1)
		{
			return false;
		}

		string strParam = param[0];
		if (strParam == null || strParam.Length < 1)
		{
			return false;
		}

		string strEvent = e.mParams[0];
		if (strEvent == null || strEvent.Length < 1)
		{
			return false;
		}

		return strParam.Equals(strEvent);
	}

	public static bool onGrowthTriggerFinish(XMLEventStruct e, List<string> param)
    {
        return true;
    }

    // onTriggerStart回调
	public static bool onTriggerStart(XMLEventStruct e, List<string> param)
    {
        if (param.Count != 1)
        {
            return false;
        }

        if (e.mParams.Count != 1)
        {
            return false;
        }

        string strParam = param[0];
        if (strParam == null || strParam.Length < 1)
        {
            return false;
        }

        string strEvent = e.mParams[0];
        if (strEvent == null || strEvent.Length < 1)
        {
            return false;
        }

        return strParam.Equals(strEvent);
    }

	// onPlayerEnterZone回调
	public static bool onPlayerEnterZone(XMLEventStruct e, List<string> param)
	{
		if (param.Count != 1)
		{
			return false;
		}

		if (e.mParams.Count != 1)
		{
			return false;
		}

		string strParam = param[0];
		if (strParam == null || strParam.Length < 1)
		{
			return false;
		}

		string strEvent = e.mParams[0];
		if (strEvent == null || strEvent.Length < 1)
		{
			return false;
		}

		return strParam.Equals(strEvent);
	}

	// onPlayerLeaveZone回调
	public static bool onPlayerLeaveZone(XMLEventStruct e, List<string> param)
	{
		if (param.Count != 1)
		{
			return false;
		}

		if (e.mParams.Count != 1)
		{
			return false;
		}

		string strParam = param[0];
		if (strParam == null || strParam.Length < 1)
		{
			return false;
		}

		string strEvent = e.mParams[0];
		if (strEvent == null || strEvent.Length < 1)
		{
			return false;
		}

		return strParam.Equals(strEvent);
	}

    // onKillEnemy回调
	public static bool onKillEnemy(XMLEventStruct e, List<string> param)
    {
        if (param.Count != 1)
        {
            return false;
        }

        if (e.mParams.Count != 1)
        {
            return false;
        }

        string strParam = param[0];
        if (strParam == null || strParam.Length < 1)
        {
            return false;
        }

        string strEvent = e.mParams[0];
        if (strEvent == null || strEvent.Length < 1)
        {
            return false;
        }

        return strParam.Equals(strEvent);
    }

    public static bool onHpDamage(XMLEventStruct e, List<string> param)
    {
        if (param.Count != 1)
        {
            return false;
        }

        if (e.mParams.Count != 1)
        {
            return false;
        }

        string strParam = param[0];
        if (strParam == null || strParam.Length < 1)
        {
            return false;
        }

        string strEvent = e.mParams[0];
        if (strEvent == null || strEvent.Length < 1)
        {
            return false;
        }

        return strParam.Equals(strEvent);
    }


    

	// 摄像机路径结束
	public static bool onCameraPathFinish(XMLEventStruct e, List<string> param)
	{
		if (param.Count != 1)
		{
			return false;
		}

		if (e.mParams.Count != 1)
		{
			return false;
		}

		string strParam = param[0];
		if (strParam == null || strParam.Length < 1)
		{
			return false;
		}

		string strEvent = e.mParams[0];
		if (strEvent == null || strEvent.Length < 1)
		{
			return false;
		}

		return strParam.Equals(strEvent);
	}
	
	// 摄像机事件触发
	public static bool onCameraPathEvent(XMLEventStruct e, List<string> param)
	{
		if (param.Count != 1)
		{
			return false;
		}

		if (e.mParams.Count != 1)
		{
			return false;
		}

		string strParam = param[0];
		if (strParam == null || strParam.Length < 1)
		{
			return false;
		}

		string strEvent = e.mParams[0];
		if (strEvent == null || strEvent.Length < 1)
		{
			return false;
		}

		return strParam.Equals(strEvent);
	}

    public bool StartTrigger(string name)
    {
      
        if(!mTriggers.ContainsKey(name))
        {
            return false;
        }

        BaseTrigger trigger = mTriggers[name] as BaseTrigger;
        if (trigger != null)
        {
            trigger.Start();
        }

        return true;
    }

    public bool RestartTrigger(string name)
    {
        if (!mTriggers.ContainsKey(name))
        {
            return false;
        }

        BaseTrigger trigger = mTriggers[name] as BaseTrigger;
        if (trigger != null)
        {
            trigger.Restart();
        }

        return true;
    }

    public bool StopTrigger(string name)
    {
        if (!mTriggers.ContainsKey(name))
        {
            return false;
        }

        BaseTrigger trigger = mTriggers[name] as BaseTrigger;
        if(trigger != null)
        {
            trigger.Stop();
        }

        return true;
    }

	public void Update (uint elapsed)
    {
        IDictionaryEnumerator enumerator = mTriggers.GetEnumerator(); 
        while (enumerator.MoveNext()) 
        {
            BaseTrigger trigger = enumerator.Value as BaseTrigger;
            if (trigger != null)
            {
                trigger.Update(elapsed);
            }
        }
	}

	// 清理当前刷怪器 只是清理掉当前Step的怪物 刷怪会继续刷下一波 想直接停掉请用下面那个DestroyCurGrowthTrigger
	public void ClearCurGrowthTrigger()
	{
		foreach (BaseTrigger trigger in mTriggers.Values)
		{
			if(!trigger.IsRunning())
			{
				continue;
			}

			if (trigger.IsTrigger(TriggerType.Growth))
			{
				GrowthTrigger growthtrigger = trigger as GrowthTrigger;
				growthtrigger.ClearCache();
			}
		}
	}

	// 清理当前刷怪器刷的怪物 并终止
	public void DestroyCurGrowthTrigger()
	{
		foreach (BaseTrigger trigger in mTriggers.Values)
		{
			if (!trigger.IsRunning())
			{
				continue;
			}

			if (trigger.IsTrigger(TriggerType.Growth))
			{
				GrowthTrigger growthtrigger = trigger as GrowthTrigger;
				growthtrigger.Destroy();
                growthtrigger.Reset();
			}
		}
	}

	// onPick回调
	public static bool onPick(XMLEventStruct e, List<string> param)
	{
		if(!(e.mParams.Count>0 && param.Count>0)) return false;

		string strParam = param[0];
		if (strParam == null || strParam.Length < 1)
		{
			return false;
		}
		
		string strEvent = e.mParams[0];
		if (strEvent == null || strEvent.Length < 1)
		{
			return false;
		}

	    if(e.mParams.Count == 1)
		{ 
			return strParam.Equals(strEvent);

		}else if(e.mParams.Count == 2)
		{
			return strParam.Equals(strEvent) && 
				(Convert.ToInt32(param[1]) == Convert.ToInt32(e.mParams[1]));
		}

		return false;






	}

	// onStoryEnd回调
	public static bool onStoryEnd(XMLEventStruct e, List<string> param)
	{
		if (param.Count != 1)
		{
			return false;
		}

		if (e.mParams.Count != 1)
		{
			return false;
		}

		int intParam = int.Parse(param[0]);
		int intEvent = int.Parse(e.mParams[0]);

		return intParam == intEvent;
	}

    //获取跨图传送点
    public Vector3 GetTransPort()
    {
         ArrayList list = mEventsTypeMap["onPlayerEnterZone"];
        if (list == null)
        {
            return Vector3.zero;
        }

        string zoneName = "";
        foreach (XMLEventStruct e in list)
        {
            foreach (XMLFunctionStruct f in e.mFunctions)
            {
                if (f.mType == "OpenUI" && f.mParams[0] == "mainmap")
                {
                    zoneName = e.mParams[0];
                }
            }
        }

        if (zoneName != "")
        {
            Zone zone = mZones[zoneName] as Zone;
            if (zone != null)
            {
                 if (zone.shape is SceneShapeRound)
                    {
                        SceneShapeRound ssr = zone.shape as SceneShapeRound;
                        GameDebug.Log("获取传送点 " + new Vector3(ssr.mCenter.x, 0, ssr.mCenter.y));
                        return new Vector3(ssr.mCenter.x,0,ssr.mCenter.y);
                    }else if (zone.shape is SceneShapeRect)
                    {
                        SceneShapeRect sst = zone.shape as SceneShapeRect;
                        GameDebug.Log("获取传送点 " + new Vector3((sst.mLeft + sst.mRight) / 2, 0, (sst.mTop + sst.mBottom) / 2));
                        return new Vector3((sst.mLeft + sst.mRight)/2, 0, (sst.mTop + sst.mBottom)/2);                       
                    }
            }
            
        }

        return Vector3.zero;
    }
}
