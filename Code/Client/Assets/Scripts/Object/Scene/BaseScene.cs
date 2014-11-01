using Assets.Scripts.Managers.Data.DataField;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using FantasyEngine;


public enum BlockType
{
    /* 无阻挡 */
    BLOCK_NONE = 0,

    /* 低阻挡，阻止(海陆) */
    BLOCK_LOW,

    /* 高阻挡，阻止(海陆空) */
    BLOCK_HIGH,

    /* 水域，阻止(陆) */
    BLOCK_WATER,

    BLOCK_TYPE_NUMBER
};

public class BlockWall : IElement
{
	public BlockType blockType;
    public LineSegf lineSeg = new LineSegf();

	public bool TestContain( float fMinX, float fMaxX, float fMinZ, float fMaxZ )
	{
		return Geometryalgorithm2d.lineseg_in_rectangle(lineSeg, new Rectanglef(new Vector2f(fMinX, fMinZ), new Vector2f(fMaxX, fMaxZ)));
	}
};

public class LineBlockWallIntersect : ITestIntersect
{
    public  BlockType blockType;
    public LineSegf line = new LineSegf();

	public bool TestIntersect( float fMinX, float fMaxX, float fMinZ, float fMaxZ )
	{
		return Geometryalgorithm2d.lineseg_intersect_rectangle(line, new Rectanglef(new Vector2f(fMinX, fMinZ), new Vector2f(fMaxX, fMaxZ)));
	}

	public bool TestIntersect(IElement element)
	{
		BlockWall wall = (BlockWall)element;
        return Geometryalgorithm2d.lineseg_intersect_lineseg(line, wall.lineSeg);
	}
};

public class PointInWalkableTriangle : ITestIntersect
{
	public Vector2f point = new Vector2f();

	public bool TestIntersect( float fMinX, float fMaxX, float fMinZ, float fMaxZ )
	{
		return Geometryalgorithm2d.point_in_rectangle(point, new Rectanglef(new Vector2f(fMinX, fMinZ), new Vector2f(fMaxX, fMaxZ)));
	}

	public bool TestIntersect(IElement element)
	{
		WalkableTriangle walkable = (WalkableTriangle)element;
		return Geometryalgorithm2d.point_in_triangle(point, walkable.tri);
	}
};

public class WalkableTriangle : IElement
{
	public Trianglef tri  = new Trianglef();
    public float[] height = new float[3];

    public  Vector2f center = new Vector2f();
    public  int index;
    public  List<uint> adjances = new List<uint>();

    public  uint areaid;

	public bool TestContain( float fMinX, float fMaxX, float fMinZ, float fMaxZ )
	{
		Rectanglef rect = new Rectanglef(new Vector2f(fMinX, fMinZ), new Vector2f(fMaxX, fMaxZ));

        return Geometryalgorithm2d.point_in_rectangle(tri.v0, rect) &&
            Geometryalgorithm2d.point_in_rectangle(tri.v1, rect) &&
            Geometryalgorithm2d.point_in_rectangle(tri.v2, rect);
	}
};

//场景初始化参数
public class BaseSceneInitParam
{
	public uint scene_inst_id = uint.MaxValue;
	public int res_id = -1;
}

public class BaseScene : IAStarGraph
 {
	public uint	mSceneID = uint.MaxValue;

    // scene.txt
	protected SceneTableItem mSceneRes = null;

    // 状态
    private SceneState mState = SceneState.SceneState_Invalid;

    // 加载进度
	private int mLoadProgress = 0;

	// 单元格长宽
	private float mMetaCellLength = 3.0f;
	private float mMetaCellWidth = 3.0f;

    // 长宽
    private float mHeight = 0.0f;
    private float mWidth = 0.0f;
    
    // 横纵格子数
    private uint mCellNumX = 0;
    private uint mCellNumY = 0;

    // 格子[][]
    private ArrayList mCells = new ArrayList();

    // 场景UI
    protected Hashtable mUIs = new Hashtable();

    // 运行时间
    private uint mRunTime = 0;

	// 玩家
	private ObjectBase mOwner = null;

    // 场景脚本
    protected SceneScriptSystem mScriptSystem = null;

    // 场景Obj管理
    protected SceneObjManager mSceneObjManager = null;

	// 摄像机路径
    private SceneCameraPathManager mSceneCameraPath;

    //场景中所有特效管理器
    private SceneParticleManager mParticleManager = null;

    //场景效果管理器
    private SceneEffectManager mEffectManager = null;

    // 场景变量
    private const int SceneParam_Count = 10;
    protected int[] mSceneIntParams = new int[SceneParam_Count];
    protected float[] mSceneFloatParams = new float[SceneParam_Count];

    private QuadTree mWalkableRegion = new QuadTree();
    private List<WalkableTriangle> mWalkableTriangles = new List<WalkableTriangle>();
    private ArrayList[,] mWalkableCells = null;

    private pathFinder mPathFinder = null;

    private List<BlockWall> mBlockWalls = new List<BlockWall>();
    private QuadTree mQuadtreeHighBlockWalls = new QuadTree();
    private QuadTree mQuadtreeLowBlockWalls  = new QuadTree();
    private const float WALKABLE_CELL_WIDTH = 0.5f;
    private int mWalkableCellRowCnt = 0;
    private int mWalkableCellColCnt = 0;

    //背景音乐实例id
    private int mBgSoundInstanceId = -1;

    private DynamicAnimation[] mDynamicAnimations = null;

    private DelayBehaviourManager mDelayBehaviourManager = null;

    private List<GrowthTriggerInfo> mGhostObjects = null;        //虚拟角色
	// 行为权限
	private int mActionFlag = (int)SceneActionFlag.SceneActionFlag_All;

    public List<GrowthTriggerInfo> GhostObjects()
    {
        return mGhostObjects;
    }
	public BaseScene()
	{
        mScriptSystem = new SceneScriptSystem(this);
        mParticleManager = new SceneParticleManager();
        mSceneCameraPath = new SceneCameraPathManager();
        mEffectManager = new SceneEffectManager();

        mDelayBehaviourManager = new DelayBehaviourManager(this);

		mMetaCellLength = GameConfig.SceneCellSizeX;
		mMetaCellWidth = GameConfig.SceneCellSizeY;
        mGhostObjects = new List<GrowthTriggerInfo>();
	}

    public void RemoveAllGhost()
    {
        if (0 < mGhostObjects.Count)
            RemoveObjectByAlias(mGhostObjects[0].alias);
    }

    public void AddAllGhost()
    {
        for (int i = 0; i < mGhostObjects.Count; ++i)
        {
            GrowthTriggerInfo info = mGhostObjects[i];
            if (null == info)
                continue;

            NpcInitParam npcParam = new NpcInitParam();
            npcParam.npc_res_id = info.resId;
            float y = GetHeight(info.x, info.z);
            npcParam.init_pos = new Vector3(info.x, y, info.z);
            npcParam.init_dir = info.dir;
			npcParam.alias = info.alias;
            npcParam.talk_id = info.talkID;

            ObjectBase obj = CreateSprite(npcParam);
            if (obj == null)
            {
                GameDebug.LogError("创建Npc失败。npcId:" + info.resId);
                return ;
            }
        }
    }

	virtual public bool Init( BaseSceneInitParam param )
	{
		mSceneID = param.scene_inst_id;
		if( !DataManager.SceneTable.ContainsKey(param.res_id) )
		{
			return false;
		}
		mSceneRes = DataManager.SceneTable[param.res_id] as SceneTableItem;

        mSceneObjManager = new SceneObjManager(this);

		return true;
	}

    public DelayBehaviourManager GetDelayBehaviourManager()
    {
        return mDelayBehaviourManager;
    }

    public void PlayNormalBgSound()
    {

        if (mBgSoundInstanceId != -1)
        {
            SoundManager.Instance.RemoveSoundByID(mBgSoundInstanceId);
        }
       
        if (mSceneRes.normalSound == -1)
        {
            GameDebug.Log("scene表里场景音乐字段为-1，所以没有音乐");
            return;
        }
        mBgSoundInstanceId = SoundManager.Instance.Play(mSceneRes.normalSound, null, true);
    }

    public void PlayBossBgSound()
    {
        if (mBgSoundInstanceId != -1)
        {
            SoundManager.Instance.RemoveSoundByID(mBgSoundInstanceId);
        }
        if(mSceneRes.bossSound==-1) return ;
        mBgSoundInstanceId = SoundManager.Instance.Play(mSceneRes.bossSound, null, true);
    }

    /// <summary>
    /// 暂停当前播放的背景音乐
    /// </summary>
    public void PauseBgSound()
    {
        if (mBgSoundInstanceId != -1)
        {
            SoundManager.Instance.Pause(mBgSoundInstanceId);
        }
    }

    public void StopBgSound()
    {
        if (mBgSoundInstanceId != -1)
        {
            SoundManager.Instance.Stop(mBgSoundInstanceId);
        }
    }

    public void PlayBgSound()
    {
        if (mBgSoundInstanceId != -1)
        {
            SoundManager.Instance.ContinuePlay(mBgSoundInstanceId);
        }
    }
	public int GetSceneResId()
	{
		return mSceneRes.resID;
	}

	public SceneTableItem GetSceneRes()
	{
		return mSceneRes;
	}

    public SceneObjManager GetSceneObjManager()
    {
        return mSceneObjManager;
    }

    public SceneParticleManager GetParticleManager()
    {
        return mParticleManager;
    }

    public SceneCameraPathManager GetCameraPathManager()
    {
        return mSceneCameraPath;
    }
    public SceneEffectManager GetEffectManager()
    {
        return mEffectManager;
    }

	virtual public SceneType getType()
	{
		return SceneType.SceneType_Invaild;
	}

	public Vector3 GetInitPos(int idx = 0)
	{
        return mScriptSystem.GetInitPos(idx);
	}

	public Vector3 GetInitPosByName(string name)
	{
		return mScriptSystem.GetInitPosByName(name);
	}

    public Vector3 GetCameraInfo()
    {
        return mScriptSystem.mCameraInfo;
    }

	public float GetInitDir(int idx = 0)
	{
        return mScriptSystem.GetInitDir(idx);
	}

	public float GetInitDirByName(string name)
	{
		return mScriptSystem.GetInitDirByName(name);
	}

    public SceneState GetState()
    {
        return mState;
    }

	virtual public bool IsSafeScene()
	{
		return true;
	}

	//添加一个Object
	public uint	AddSprte( ObjectBase sprite )
	{
        if(mSceneObjManager == null)
        {
            return uint.MaxValue;
        }

        return mSceneObjManager.AddSprte(sprite);
	}

    public bool StartTrigger(string name)
    {
        return mScriptSystem.StartTrigger(name);
    }

    public bool RestartTrigger(string name)
    {
        return mScriptSystem.RestartTrigger(name);
    }
    public bool StopTrigger(string name)
    {
        return mScriptSystem.StopTrigger(name);
    }

    virtual public void OnTriggerFinish(string name)
    {
		List<string> arr = new List<string>();
        arr.Add(name);
        mScriptSystem.TriggerEvent("onTriggerFinish", arr);
    }

    virtual public void OnGrowthTriggerFinish(string name)
    {
		List<string> arr = new List<string>();
        arr.Add(name);
        mScriptSystem.TriggerEvent("onGrowthTriggerFinish", arr);
    }

    virtual public void OnKillEnemy(ObjectBase sprite, uint killId)
    {
		List<string> arr = new List<string>();
        arr.Add(sprite.GetAlias());
        mScriptSystem.TriggerEvent("onKillEnemy", arr);
		
// 		if(PlayerController.Instance.GetControl() == sprite.InstanceID)
// 		{
// 			OnMainPlayerDie(sprite);
// 		}
    }

    virtual public void onHpDamage(ObjectBase sprite, uint killId)
    {
		List<string> arr = new List<string>();
        arr.Add(sprite.GetAlias());
        mScriptSystem.TriggerEvent("onHpDamage", arr);
    }
    

	virtual public void OnPick(ObjectBase pick, ObjectBase picker)
	{
		List<string> arr = new List<string>();
        arr.Add(pick.GetAlias());
		Pick p = pick as Pick;
		arr.Add(p.GetCurPickTableItem().resID.ToString());		
		mScriptSystem.TriggerEvent("onPick", arr);
	}

	virtual public void OnMainPlayerDie()
	{

	}

    virtual public void OnCropsDie()
    { 
    
    }

    virtual public void OnSpriteEnterScene(ObjectBase sprite)
    {

    }

    virtual public void OnSpriteLeaveScene(ObjectBase sprite)
    {

    }

	virtual public void OnSpriteModelLoaded(uint instanceid)
	{
	    PlayerController.Instance.QuestMoveCheck();
	}

    virtual public void OnSpriteEnterZone(ObjectBase sprite, Zone zone)
    {
		if(PlayerController.Instance.GetControl() == sprite.InstanceID)
		{
			List<string> arr = new List<string>();
			arr.Add(zone.name);
			mScriptSystem.TriggerEvent("onPlayerEnterZone", arr);
		}
    }

    virtual public void OnSpriteLeaveZone(ObjectBase sprite, Zone zone)
    {
		if (PlayerController.Instance.GetControl() == sprite.InstanceID)
		{
			List<string> arr = new List<string>();
			arr.Add(zone.name);
			mScriptSystem.TriggerEvent("onPlayerLeaveZone", arr);
		}
    }

	virtual public void OnSceneAniFinished(string name)
	{
		List<string> arr = new List<string>();
		arr.Add(name);
		mScriptSystem.TriggerEvent("onSceneAniFinished", arr);
	}

	//找一个Object
	public ObjectBase FindObject(uint instid)
	{
        if(mSceneObjManager == null)
        {
            return null;
        }

        return mSceneObjManager.FindObject(instid);
	}

	public bool RemoveObject(uint instid)
	{
		if(mSceneObjManager == null)
		{
			return false;
		}

		return mSceneObjManager.RemoveObject(instid);
	}

     //搜索一个自动瞄准的目标
    public BattleUnit SearchAutoAimEnemy(BattleUnit owner, float distance)
    {
        if(mSceneObjManager == null)
        {
            return null;
        }

        return mSceneObjManager.SearchAutoAimEnemy(owner, distance);
    }

    //搜索区域内的 BattleUnit
    public ArrayList SearchObject(SceneShapeRect shape, int searchType, bool ignoreDead = true)
    {
        if(mSceneObjManager == null)
        {
            return null;
        }

        return mSceneObjManager.SearchObject(shape, searchType, ignoreDead);
    }

    public ArrayList SearchBattleUnit(Vector2f center, float radius)
    {
        if (mSceneObjManager == null)
        {
            return null;
        }

        return mSceneObjManager.SearchBattleUnit(center, radius);
    }

	//加载当前场景
	public bool LoadScene()
	{
		if( mState != SceneState.SceneState_Invalid || mSceneRes == null )
		{
			GameDebug.Log("LoadScene 失败 mSceneRes = null");
			return false;
		}

        mState = SceneState.SceneState_Loading;

		mLoadProgress = 0;

		return true;
	}

	// 脚本下载成功
	private void onScriptLoadCallback (string path, byte[] bytes)
	{
		string txt = System.Text.Encoding.UTF8.GetString (bytes);
		if( string.IsNullOrEmpty(txt) )
			return ;

        mScriptSystem.Init(txt);
	}

    private void onBlockLoadCallback(string path, byte[] bytes)
    {
        string txt = System.Text.Encoding.UTF8.GetString(bytes);
        if (string.IsNullOrEmpty(txt))
            return;

        loadBlock(txt);
    }

	virtual public void Update(uint elapsed)
	{
        mRunTime += elapsed;

        bool ret = true;

        switch(mState)
        {
            case SceneState.SceneState_Invalid:
                {

                }
                break;

            case SceneState.SceneState_Loading:
                {
                    ret = LoadingUpdate(elapsed);
                }
                break;
            case SceneState.SceneState_Initialize:
                {
                    ret = InitializeUpdate(elapsed);
                }
                break;
            case SceneState.SceneState_LogicRun:
                {
                    ret = LogicUpdate(elapsed);
                }
                break;
            case SceneState.SceneState_Destroy:
                {

                }
                break;
            default:
                break;
        }

        if(!ret)
        {
            Destroy();
        }
	}

    // 加载
    virtual public bool LoadingUpdate(uint elapsed)
    {
        if(mLoadProgress >= 100)
        {
            return true;
        }

        if (mLoadProgress == 30)
        {
            if (!string.IsNullOrEmpty(mSceneRes.sceneScript))
            {
                ResourceManager.Instance.LoadBytes(mSceneRes.sceneScript, onScriptLoadCallback);
            }
        }

        if (mLoadProgress == 40)
        {
            ResourceManager.Instance.LoadBytes(mSceneRes.sceneBlock, onBlockLoadCallback);
        }
         
        if (mLoadProgress == 60)
        {
            EngineApplication.LoadLevel(mSceneRes.sceneFile, onLoadComplete);
        }
        //			if( mLoadProgress == 80 )
        //			{
        //				GameApp.Instance.StartCoroutine(AsyncLoadLevel(mSceneRes.sceneName));
        //			}

        onLoadProgress((++mLoadProgress));

        return true;
    }

    // 初始化
    virtual public bool InitializeUpdate(uint elapsed)
    {
        InitCell();

        //OpenAllSceneUI();

		OnSceneInited();

        mState = SceneState.SceneState_LogicRun;

        return true;
    }

    // 游戏逻辑
    virtual public bool LogicUpdate(uint elapsed)
    {
        if (mSceneObjManager != null)
        {
            //注释掉场景的此UPdate
            mSceneObjManager.Update(elapsed);
        }

        if (mScriptSystem != null)
        {
            mScriptSystem.Update(elapsed);
        }

        if( mDelayBehaviourManager != null )
        {
            mDelayBehaviourManager.Update(elapsed);
        }

		mParticleManager.Update();

        return true;
    }

	virtual protected void OnSceneInited()
	{
		mScriptSystem.TriggerEvent("onSceneInited", null);

		OpenUI();
	}

    public bool InvokeFunction(string name)
    {
        if(!SceneScriptSystem.Inited || mScriptSystem == null)
        {
            return false;
        }

        return mScriptSystem.InvokeFunction(name);
    }

    public bool GetIntParam(int idx, ref int param)
    {
        if (idx < 0 || idx >= SceneParam_Count)
        {
            return false;
        }

        param = mSceneIntParams[idx];
        return true;
    }

    public bool SetIntParam(int idx, int param)
    {
        if (idx < 0 || idx >= SceneParam_Count)
        {
            return false;
        }

        mSceneIntParams[idx] = param;
        return true;
    }

    public bool GetFloatParam(int idx, ref float param)
    {
        if (idx < 0 || idx >= SceneParam_Count)
        {
            return false;
        }

        param = mSceneFloatParams[idx];
        return true;
    }

    public bool SetFloatParam(int idx, float param)
    {
        if (idx < 0 || idx >= SceneParam_Count)
        {
            return false;
        }

        mSceneFloatParams[idx] = param;
        return true;
    }

	public bool RemoveObjectByAlias(string alias)
	{
		if(mSceneObjManager == null)
		{
			return false;
		}

		return mSceneObjManager.RemoveObjectByAlias(alias);
	}

    public bool RemoveObjsByAlias<T>(string alias) where T : ObjectBase
    {
        if (mSceneObjManager == null)
        {
            return false;
        }

        return mSceneObjManager.RemoveObjsByAlias<T>(alias);
    }

	public bool RemoveParticleByAlias(string alias)
	{
		if (mParticleManager == null)
		{
			return false;
		}

		mParticleManager.RemoveParticleByAlias(alias);
		return true;
	}

	public bool KillObjectByAlias(string alias)
	{
		if (mSceneObjManager == null)
		{
			return false;
		}

		return mSceneObjManager.KillObjectByAlias(alias);
	}

    public List<T> SearchObjsByAlias<T>(string alias) where T : ObjectBase
    {
        if (mSceneObjManager == null)
        {
            return null;
        }

        return mSceneObjManager.SearchObjsByAlias<T>(alias);
    }

    // 注册界面
    virtual protected void RegisterAllSceneUI()
    {
        
    }

    // 注册界面
    public void RegisterSceneUI()
    {
        
    }

    // 打开界面
    private void OpenAllSceneUI()
    {
        if(mSceneRes == null)
		{
			return;
		}

		string[] uiList = mSceneRes.uiconfig.Split(';');
		for(int i = 0; i < uiList.Length; ++i)
		{
			string uistr = uiList[i];
			if(uistr == null || uistr.Length < 1)
			{
				continue;
			}

			if(!DataManager.UITable.ContainsKey(uistr))
			{
				continue;
			}

			WindowManager.Instance.OpenUI(uistr);
		}
    }

	virtual protected void OpenUI()
	{
		TouchManager.Instance.OpenTouch();
	}

	virtual protected void CloseUI()
	{
		TouchManager.Instance.HideTouch();
	}

    // 关闭界面
    private void CloseAllSceneUI()
    {
		if (mSceneRes == null)
		{
			return;
		}

		string[] uiList = mSceneRes.uiconfig.Split(';');
		for (int i = 0; i < uiList.Length; ++i)
		{
			string uistr = uiList[i];
			if (uistr == null || uistr.Length < 1)
			{
				continue;
			}

			if (!DataManager.UITable.ContainsKey(uistr))
			{
				continue;
			}

			WindowManager.Instance.CloseUI(uistr);
		}
    }

    // 初始化格子区域
    private void InitCell()
    {
		mCellNumX = (uint)(mWidth / mMetaCellLength);
		mCellNumY = (uint)(mHeight / mMetaCellWidth);

        if (mCellNumX < 1)
            mCellNumX = 1;

        if (mCellNumY < 1)
            mCellNumY = 1;

        // 相交的事件区域加入列表
        for(int row = 0; row < mCellNumX; ++row)
        {
            ArrayList rowArray = null;
            if(row < mCells.Count)
            {
                rowArray = mCells[row] as ArrayList;
            }
            
            if(rowArray == null)
            {
                rowArray = new ArrayList();
                mCells.Add(rowArray);
            }

            for(int col = 0; col < mCellNumY; ++col)
            {
                Cell cell = new Cell(row, col);

				Vector2 pos = new Vector2(col * mMetaCellLength, row * mMetaCellWidth);
				SceneShapeRect rect = new SceneShapeRect(pos.x, pos.y + mMetaCellWidth, pos.x + mMetaCellLength, pos.y);

                foreach(Zone zone in mScriptSystem.Zones.Values)
                {
                    if(zone.intersect(rect))
                    {
                        cell.AddZone(zone);
                    }
                }

                (mCells[row] as ArrayList).Add(cell);
            }
        }
    }

    // 坐标所在格子
    public Cell GetCellByPosition(Vector2 pos)
    {
        int row = (int)(pos.y / mMetaCellWidth);
        if( row < 0 || row >= mCells.Count)
        {
            return null;
        }

        ArrayList rowArray = mCells[row] as ArrayList;
        if (rowArray == null)
        {
            return null;
        }

        int col = (int)(pos.x / mMetaCellLength);

        if (col < 0 || col >= rowArray.Count)
            return null;

        return rowArray[col] as Cell;
    }

    // 场景内精灵位置改变
    virtual public void OnSpriteChangePosition(ObjectBase sprite, Vector3 oldPos, Vector3 curPos)
    {
        UpdateSpriteZone(sprite, oldPos, curPos);
        UpdateSpriteCell(sprite);
    }

    private Vector2 oldPos2 = Vector2.zero;
    private Vector2 curPos2 = Vector2.zero;

    private ArrayList dirtyZones = new ArrayList();
    private ArrayList leaveZones = new ArrayList();
    private ArrayList enterZones = new ArrayList();

    // 更新角色区域
    public void UpdateSpriteZone(ObjectBase sprite, Vector3 oldPos, Vector3 curPos)
    {
        //Vector2 oldPos2 = new Vector2(oldPos.x, oldPos.z);
        //Vector2 curPos2 = new Vector2(curPos.x, curPos.z);

        oldPos2.x = oldPos.x;
        oldPos2.y = oldPos.z;

        curPos2.x = curPos.x;
        curPos2.y = curPos.z;


        Cell oldCell = GetCellByPosition(oldPos2);
        Cell newCell = GetCellByPosition(curPos2);

        dirtyZones.Clear();
        leaveZones.Clear();
        enterZones.Clear();

//         ArrayList dirtyZones = new ArrayList();
//         ArrayList leaveZones = new ArrayList();
//         ArrayList enterZones = new ArrayList();

        if(oldCell != null)
        {
            ArrayList zones = oldCell.GetZoneList();

            for (int i = 0; i < zones.Count; ++i )
            {
                Zone zone = zones[i] as Zone;
                dirtyZones.Add(zone);

                bool containOld = zone.contains(oldPos2);
                bool containCur = zone.contains(curPos2);
                if (containOld == containCur)
                {
                    continue;
                }

                if (containOld)
                {
                    leaveZones.Add(zone);
                }
                else
                {
                    enterZones.Add(zone);
                }
            }
        }

        if(newCell != null)
        {
            ArrayList zones = newCell.GetZoneList();
            for (int i = 0; i < zones.Count; ++i )
            {
                Zone zone = zones[i] as Zone;
                if (dirtyZones.Contains(zone))
                {
                    continue;
                }

                if (!zone.contains(curPos2))
                {
                    continue;
                }

                enterZones.Add(zone);
             }
        }

        foreach(Zone zone in leaveZones)
        {
            OnSpriteLeaveZone(sprite, zone);
        }

        foreach(Zone zone in enterZones)
        {
            OnSpriteEnterZone(sprite, zone);
        }
    }

    // 更新角色所在格子
    public void UpdateSpriteCell(ObjectBase sprite)
    {
        Cell oldCell = sprite.GetCell();
        Cell newCell = GetCellByPosition(new Vector2(sprite.GetPosition().x, sprite.GetPosition().z));
        
        if(oldCell == newCell)
        {
            return;
        }

        if(oldCell != null)
        {
            oldCell.RemoveSprite(sprite);
        }

        if(newCell != null)
        {
            newCell.AddSprite(sprite);
        }

        OnSpriteCellChanged(sprite, oldCell, newCell);
    }

    // 角色格子变化
    virtual protected void OnSpriteCellChanged(ObjectBase sprite, Cell oldCell, Cell newCell)
    {

    }

	private void onLoadProgress(int value)
	{
		LoadingEvent evt = new LoadingEvent(LoadingEvent.LOADING_PROGRESS);
		evt.progress = value;
		EventSystem.Instance.PushEvent( evt );
	}

	private void onLoadComplete()
	{
		WindowManager.Instance.CloseUI("loading");

        EventSystem.Instance.PushEvent(new SceneLoadEvent(SceneLoadEvent.SCENE_LOAD_COMPLETE));

        mState = SceneState.SceneState_Initialize;
	}

    virtual protected void OnSceneDestroy()
    {
		CloseUI();
    }

	virtual public void Destroy()
	{
        OnSceneDestroy();

        if (mBgSoundInstanceId != -1)
        {
            SoundManager.Instance.RemoveSoundByID(mBgSoundInstanceId);
        }

		if(mSceneObjManager != null)
        {
            mSceneObjManager.Destroy();
			mSceneObjManager = null;
        }

		if(mScriptSystem != null)
		{
			mScriptSystem.Destroy();
			mScriptSystem = null;
		}
        if(mEffectManager != null)
        {
            mEffectManager.Destroy();
            mEffectManager = null;
        }

		mSceneRes = null;
		mSceneID = uint.MaxValue;

        if (mGhostObjects != null)
        {
            mGhostObjects.Clear();
            mGhostObjects = null;
        }
	}

    public uint getCellCount()
    {
        return (uint)mWalkableTriangles.Count;
    }
    public uint getCellIndex(Vector2f pos)
    {
        WalkableTriangle tri = GetWalkableTriangle(pos);
        if(tri == null)
            return PAHT_FINDER_ENUM.INVAILD_INDEX;

        return (uint)tri.index;
    }

    public float getHValue(uint cellIndex, Vector2f targetPoint)
    {
        if(mWalkableTriangles.Count <= cellIndex)
		    return float.MaxValue;

        WalkableTriangle triangle = (WalkableTriangle)mWalkableTriangles[(int)cellIndex];

        return (new Vector2f(triangle.center.x - targetPoint.x, triangle.center.y - targetPoint.y)).length();
    }

    public List<uint> getAdjanceCells(uint cellIndex)
    {
        if(mWalkableTriangles.Count <= cellIndex)
		    return null;

        WalkableTriangle triangle = (WalkableTriangle)mWalkableTriangles[(int)cellIndex];
        return triangle.adjances;
    }

    public float getGValue(uint cellIndex_0, uint cellIndex_1, uint cellIndex_2)
    {
        if(mWalkableTriangles.Count <= cellIndex_0)
		    return 0;

	    if(mWalkableTriangles.Count <= cellIndex_1)
		    return 0;

	    if(mWalkableTriangles.Count <= cellIndex_2)
		    return 0;
	
	    WalkableTriangle cell_0 = mWalkableTriangles[(int)cellIndex_0];
	    WalkableTriangle cell_1 = mWalkableTriangles[(int)cellIndex_1];
	    WalkableTriangle cell_2 = mWalkableTriangles[(int)cellIndex_2];

	    LineSegf out0 = new LineSegf();
	    if(!getWalkableTriangleOutSide(cell_0, cell_1, ref out0))
		    return 0;

	    LineSegf out1 = new LineSegf();
	    if(!getWalkableTriangleOutSide(cell_1, cell_2, ref out1))
		    return 0;

	    Vector2f center_0 = new Vector2f((out0.vertex0.x + out0.vertex1.x) / 2, (out0.vertex0.y + out0.vertex1.y) / 2);
	    Vector2f center_1 = new Vector2f((out1.vertex0.x + out1.vertex1.x) / 2, (out1.vertex0.y + out1.vertex1.y) / 2);

        return (new Vector2f(center_0.x - center_1.x, center_0.y - center_1.y)).length();
    }

    public bool smoothPath(Vector2f startpoint,
                    Vector2f endpoint,
                    List<uint> cellIndexs,
                    ref List<Vector2f> path)
    {
        Vector2f startPoint = new Vector2f(startpoint.x, startpoint.y);
        Vector2f endPoint = new Vector2f(endpoint.x, endpoint.y);

        if(cellIndexs.Count < 2)
		    return false;

        Vector2f wayPoint = new Vector2f(startPoint.x, startPoint.y);
        uint cellIndex = 0;
	   
	    while(true)
	    {
		    Vector2f curPoint = new Vector2f(wayPoint.x, wayPoint.y);
		    uint currentCellIndex    = cellIndex;
		    uint lastLeftCellIndex	 = currentCellIndex + 1;
		    uint lastRightCellIndex	 = currentCellIndex + 1;

		    Vector2f lastLeftPoint = new Vector2f();
		    Vector2f lastRightPoint= new Vector2f();

            path.Add(new Vector2f(curPoint.x, curPoint.y));
		    if(curPoint.x == endPoint.x && curPoint.y == endPoint.y)
			    break;

		    if(path.Count > cellIndexs.Count + 2)
			    return false;

		    if(currentCellIndex + 1 >= cellIndexs.Count)
		    {
                path.Add(new Vector2f(endPoint.x, endPoint.y));
			    return true;
		    }
		    else
		    {
		
			    WalkableTriangle cell_1 = mWalkableTriangles[(int)cellIndexs[(int)currentCellIndex]];
			    WalkableTriangle cell_2 = mWalkableTriangles[(int)cellIndexs[(int)(currentCellIndex + 1)]];
			
			    LineSegf outLine = new LineSegf();
			    if(!getWalkableTriangleOutSide(cell_1, cell_2, ref outLine))
				    return false;
		
			    lastLeftPoint.x = outLine.vertex0.x;
                lastLeftPoint.y = outLine.vertex0.y;

			    lastRightPoint.x = outLine.vertex1.x;
                lastRightPoint.y = outLine.vertex1.y;
		    }

		    Vector2f testPointLeft = new Vector2f();
		    Vector2f testPointRight= new Vector2f();

		    bool bfindwaypoint = false;
		    for(uint k = currentCellIndex + 1; k < cellIndexs.Count; k++)
		    {
			    if(k + 1 >= cellIndexs.Count)
			    {
				    testPointLeft.x	= endPoint.x;
                    testPointLeft.y = endPoint.y;
				    testPointRight.x = endPoint.x;
                    testPointRight.y = endPoint.y;
			    }
			    else
			    {
				    WalkableTriangle cell_1 = mWalkableTriangles[(int)cellIndexs[(int)k]];
				    WalkableTriangle cell_2 = mWalkableTriangles[(int)cellIndexs[(int)(k + 1)]];
			
				    LineSegf outLine = new LineSegf(); 
				    if(!getWalkableTriangleOutSide(cell_1, cell_2, ref outLine))
					    return false;

				    testPointLeft.x	= outLine.vertex0.x;
                    testPointLeft.y	= outLine.vertex0.y;
				    testPointRight.x = outLine.vertex1.x; 
                    testPointRight.y = outLine.vertex1.y; 
			    }

			    if(lastLeftPoint.x != testPointLeft.x || lastLeftPoint.y != testPointLeft.y)
			    {
				    if(Geometryalgorithm2d.point_lineseg_direction(new Vector2f(testPointLeft.x, testPointLeft.y), 
					    new Vector2f(curPoint.x, curPoint.y), new Vector2f(lastRightPoint.x, lastRightPoint.y)) 
                        == Geometryalgorithm2d.POINT_LINESEG_ENUM.POINT_LINESEG_RIGHT)
				    {

					    cellIndex = lastRightCellIndex;
					    wayPoint.x = lastRightPoint.x;
                        wayPoint.y = lastRightPoint.y;

					    bfindwaypoint = true;
					    break;
				    }
				    else 
				    {
					    if(Geometryalgorithm2d.point_lineseg_direction(new Vector2f(testPointLeft.x, testPointLeft.y), 
						    new Vector2f(curPoint.x, curPoint.y), new Vector2f(lastLeftPoint.x, lastLeftPoint.y)) 
                            != Geometryalgorithm2d.POINT_LINESEG_ENUM.POINT_LINESEG_LEFT)
					    {
						    lastLeftPoint.x = testPointLeft.x;
                            lastLeftPoint.y = testPointLeft.y;
						    lastLeftCellIndex  = k;
					    }
				    }
			    }
			    else
			    {
				    lastLeftCellIndex = k;
			    }
			
			    if(lastRightPoint.x != testPointRight.x || lastRightPoint.y != testPointRight.y)
			    {
				    if(Geometryalgorithm2d.point_lineseg_direction(new Vector2f(testPointRight.x, testPointRight.y), 
					    new Vector2f(curPoint.x, curPoint.y), new Vector2f(lastLeftPoint.x, lastLeftPoint.y)) 
                        == Geometryalgorithm2d.POINT_LINESEG_ENUM.POINT_LINESEG_LEFT)
				    {
					    cellIndex = lastLeftCellIndex;
					    wayPoint.x = lastLeftPoint.x;
                        wayPoint.y = lastLeftPoint.y;
					
					    bfindwaypoint = true;
					    break;
				    }
				    else
				    {
					    if(Geometryalgorithm2d.point_lineseg_direction(new Vector2f(testPointRight.x, testPointRight.y), 
						    new Vector2f(curPoint.x, curPoint.y), new Vector2f(lastRightPoint.x, lastRightPoint.y)) 
                            != Geometryalgorithm2d.POINT_LINESEG_ENUM.POINT_LINESEG_RIGHT)
					    {
						    lastRightPoint.x = testPointRight.x;
                            lastRightPoint.y = testPointRight.y;
						    lastRightCellIndex  = k;
					    }
				    }
			    }
			    else
			    {
				    lastRightCellIndex  = k;
			    }
		    }

		    if(!bfindwaypoint)
		    {
			    wayPoint.x = endPoint.x;
                wayPoint.y = endPoint.y;
			    cellIndex = (uint)(cellIndexs.Count - 1);
		    }
	    }

	    return true;
    }

   public bool sameArea(uint cellIndex_1, uint cellIndex_2)
   {
        if(mWalkableTriangles.Count <= cellIndex_1)
            return false;

        if(mWalkableTriangles.Count <= cellIndex_2)
	        return false;

        WalkableTriangle cell_1 = mWalkableTriangles[(int)cellIndex_1];
        WalkableTriangle cell_2 = mWalkableTriangles[(int)cellIndex_2];

        return cell_1.areaid == cell_2.areaid;
   }

    public bool getWalkableTriangleOutSide(WalkableTriangle cell_1, WalkableTriangle cell_2, ref LineSegf line)
    {

	    for(uint j = 0; j < 3; j++)
	    {
            LineSegf l1 = cell_1.tri.side(j);
            
		    for(uint k = 0; k < 3; k++)
		    {
                LineSegf l2 = cell_2.tri.side(k);
                	
			    if(l1.Equal(l2))
			    {
                    line = l1;
				    return true;
			    }
		    }
	    }

        //line = new LineSegf();
	    return false;
    }

   private bool loadBlock(string blockFilePath)
   {
        if (blockFilePath.Length == 0)
            return false;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(blockFilePath);

        XmlNode node = xmlDoc.FirstChild;
        node = node.NextSibling;
        if (node.Name != "Root")
            return false;

        XmlNodeList nodeList = node.ChildNodes;
        if (nodeList == null)
            return false;

        if(nodeList.Count != 1)
            return false;

        XmlNode sceneInfo = nodeList[0];
        if(sceneInfo.Name != "Blocks")
            return false;

        mWidth = System.Convert.ToSingle(sceneInfo.Attributes["SceneX"].Value);
        mHeight = System.Convert.ToSingle(sceneInfo.Attributes["SceneZ"].Value);

        mWalkableRegion.CreateTree(0, mWidth, 0, mHeight);
        mQuadtreeHighBlockWalls.CreateTree(0, mWidth, 0, mHeight);
        mQuadtreeLowBlockWalls.CreateTree(0, mWidth, 0, mHeight);

        nodeList = sceneInfo.ChildNodes;
        for(int i = 0; i < nodeList.Count; i++)
        {
            XmlNode block = nodeList[i];
            if (block.Name != "Block")
                return false;

            string blockType = block.Attributes["type"].Value;
            if (blockType == "walkable")
            {
                string data = block.Attributes["data"].Value;

                string[] strTriangles = data.Split(new char[] {';'});
                foreach(string tris in strTriangles)
                {
                    string[] triangleVertexs = tris.Split(new char[] { ',' });
                    if (triangleVertexs.Length != 9)
                        continue;

                    WalkableTriangle walkableTriangle = new WalkableTriangle();
                    walkableTriangle.index = mWalkableTriangles.Count;
                    walkableTriangle.center.x = 0;
                    walkableTriangle.center.y = 0;
                    walkableTriangle.areaid = 0;

                    walkableTriangle.tri.v0.x = System.Convert.ToSingle(triangleVertexs[0]);
                    walkableTriangle.tri.v0.y = System.Convert.ToSingle(triangleVertexs[2]);
                    walkableTriangle.height[0] = System.Convert.ToSingle(triangleVertexs[1]);
                    walkableTriangle.center.x += walkableTriangle.tri.v0.x / 3.0f;
                    walkableTriangle.center.y += walkableTriangle.tri.v0.y / 3.0f;

                    walkableTriangle.tri.v1.x = System.Convert.ToSingle(triangleVertexs[3]);
                    walkableTriangle.tri.v1.y = System.Convert.ToSingle(triangleVertexs[5]);
                    walkableTriangle.height[1] = System.Convert.ToSingle(triangleVertexs[4]);
                    walkableTriangle.center.x += walkableTriangle.tri.v1.x / 3.0f;
                    walkableTriangle.center.y += walkableTriangle.tri.v1.y / 3.0f;

                    walkableTriangle.tri.v2.x = System.Convert.ToSingle(triangleVertexs[6]);
                    walkableTriangle.tri.v2.y = System.Convert.ToSingle(triangleVertexs[8]);
                    walkableTriangle.height[2] = System.Convert.ToSingle(triangleVertexs[7]);
                    walkableTriangle.center.x += walkableTriangle.tri.v2.x / 3.0f;
                    walkableTriangle.center.y += walkableTriangle.tri.v2.y / 3.0f;

                    mWalkableTriangles.Add(walkableTriangle);
                    mWalkableRegion.AddElement(walkableTriangle);
                }

                data = block.Attributes["topology"].Value;

                string[] triangleTopology = data.Split(new char[] {';'});
                foreach(string adjancesItem in triangleTopology)
                {
                    WalkableTriangle targettri = null;

                    string[] adjancesInfo = adjancesItem.Split(new char[] {','});
                    for(int j = 0; j < adjancesInfo.Length; j++)
                    {
                        if (adjancesInfo[j].Length == 0)
                            continue;

                        int index = System.Convert.ToInt32(adjancesInfo[j]);
                        if (index >= mWalkableTriangles.Count)
                            continue;

                        if (targettri == null && j == 0)
                        {
                            targettri = mWalkableTriangles[index];
                        }
                        else if (targettri != null)
                        {
                            targettri.adjances.Add((uint)index);
                        }
                    }
                }
            }
            else if(blockType == "highblock")
            {
                string data = block.Attributes["data"].Value;

                string[] lines = data.Split(new char[] { ';' });
                foreach(string line in lines)
                {
                    string[] linevertexs = line.Split(new char[] { ','});
                    if (linevertexs.Length != 4)
                        continue;

                    BlockWall wall = new BlockWall();
                    wall.blockType = BlockType.BLOCK_HIGH;
                    for(int j = 0; j < 2; j++)
                    {
                        if(j == 0)
                        {
                            wall.lineSeg.vertex0.x = System.Convert.ToSingle(linevertexs[j * 2]);
                            wall.lineSeg.vertex0.y = System.Convert.ToSingle(linevertexs[j * 2 + 1]);
                        }
                        else
                        {
                            wall.lineSeg.vertex1.x = System.Convert.ToSingle(linevertexs[j * 2]);
                            wall.lineSeg.vertex1.y = System.Convert.ToSingle(linevertexs[j * 2 + 1]);
                        }
                    }

                    mBlockWalls.Add(wall);
                    mQuadtreeHighBlockWalls.AddElement(wall);
                }
            }
            else if(blockType == "lowblock")
            {
                string data = block.Attributes["data"].Value;

                string[] lines = data.Split(new char[] { ';' });
                foreach (string line in lines)
                {
                    string[] linevertexs = line.Split(new char[] { ',' });
                    if (linevertexs.Length != 4)
                        continue;

                    BlockWall wall = new BlockWall();
                    wall.blockType = BlockType.BLOCK_LOW;
                    for (int j = 0; j < 2; j++)
                    {
                        if(j == 0)
                        {
                            wall.lineSeg.vertex0.x = System.Convert.ToSingle(linevertexs[j * 2]);
                            wall.lineSeg.vertex0.y = System.Convert.ToSingle(linevertexs[j * 2 + 1]);
                        }
                        else
                        {
                            wall.lineSeg.vertex1.x = System.Convert.ToSingle(linevertexs[j * 2]);
                            wall.lineSeg.vertex1.y = System.Convert.ToSingle(linevertexs[j * 2 + 1]);
                        }
                    }

                    mBlockWalls.Add(wall);
                    mQuadtreeLowBlockWalls.AddElement(wall);
                }
            }
        }

        mWalkableCellRowCnt = (int)(mHeight / WALKABLE_CELL_WIDTH) + 1;
        mWalkableCellColCnt = (int)(mWidth / WALKABLE_CELL_WIDTH) + 1;

        mWalkableCells = new ArrayList[mWalkableCellRowCnt, mWalkableCellColCnt];

        Rectanglef rect = new Rectanglef();

        foreach (WalkableTriangle tri in mWalkableTriangles)
        {
            float minx = System.Math.Min(tri.tri.v0.x, System.Math.Min(tri.tri.v1.x, tri.tri.v2.x));
            float minz = System.Math.Min(tri.tri.v0.y, System.Math.Min(tri.tri.v1.y, tri.tri.v2.y));
            float maxx = System.Math.Max(tri.tri.v0.x, System.Math.Max(tri.tri.v1.x, tri.tri.v2.x));
            float maxz = System.Math.Max(tri.tri.v0.y, System.Math.Max(tri.tri.v1.y, tri.tri.v2.y));

            for (int row = (int)(minx / WALKABLE_CELL_WIDTH); row <= (int)(maxx / WALKABLE_CELL_WIDTH) && row >= 0 && row < mWalkableCellRowCnt; row++)
            {
                for (int col = (int)(minz / WALKABLE_CELL_WIDTH); col <= (int)(maxz / WALKABLE_CELL_WIDTH) && col >= 0 && col < mWalkableCellColCnt; col++)
                {
                    rect.minv.x = row * WALKABLE_CELL_WIDTH;
                    rect.minv.y = col * WALKABLE_CELL_WIDTH;

                    rect.maxv.x = rect.minv.x + WALKABLE_CELL_WIDTH;
                    rect.maxv.y = rect.minv.y + WALKABLE_CELL_WIDTH;

                    Vector2f center = rect.minv.Add(rect.maxv).MultiplyBy(0.5f);

                    if (Geometryalgorithm2d.circle_intersect_triangle(center, WALKABLE_CELL_WIDTH, tri.tri))
                    {
                        if (mWalkableCells[row, col] == null)
                        {
                            mWalkableCells[row, col] = new ArrayList();
                        }

                        mWalkableCells[row, col].Add(tri.index);
                    }
                }
            }
            
        }

        mPathFinder = new pathFinder();
        mPathFinder.init(this);

        GenerateWalkableAreaId();
        return true;
   }

    private void GenerateWalkableAreaId()
    {
        uint areaId = 100;

        int count = mWalkableTriangles.Count;
        for(int i = 0; i < count; i++)
        {
            WalkableTriangle tri = mWalkableTriangles[i];
            if (tri.areaid > 0)
                continue;

            GenerateWalkableAreaId(tri, areaId);

            areaId++;
        }
    }

    private void GenerateWalkableAreaId(WalkableTriangle tri, uint areaId)
    {
        tri.areaid = areaId;
        int adjancesCount = tri.adjances.Count;

        for(int i = 0; i < adjancesCount; i++)
        {
            WalkableTriangle nextTri = mWalkableTriangles[(int)tri.adjances[i]];
            if (nextTri.areaid > 0)
                continue;

            GenerateWalkableAreaId(nextTri, areaId);
        }
    }

    public WalkableTriangle GetWalkableTriangle(Vector2f pos)
    {
        if (pos.x < 0 || pos.x > mWidth || pos.y < 0 || pos.y > mHeight)
            return null;

        ArrayList lst = mWalkableCells[(int)(pos.x / WALKABLE_CELL_WIDTH), (int)(pos.y / WALKABLE_CELL_WIDTH)];
        if (lst == null)
            return null;

        int cnt = lst.Count;
        for(int i = 0; i < cnt; i++)
        {
            int id = (int)lst[i];
            if (id < 0 || id >= mWalkableTriangles.Count)
                continue;

            WalkableTriangle tri = mWalkableTriangles[id];
            if (tri != null)
            {
                if(Geometryalgorithm2d.point_in_triangle(pos, tri.tri))
                {
                    return tri;
                }
            }
        }

        return null;
    }

    public List<Vector2f> FindPath(ObjectBase obj, Vector2f startPoint, Vector2f endPoint)
    {
        if (mPathFinder == null)
            return null;

        List<Vector2f> rst;
        if (!mPathFinder.findPath(startPoint, endPoint, out rst))
            return null;

        if (!CheckBarrier(obj, ref rst))
            return null;
        return rst;
    }

	private bool CheckBarrier(ObjectBase obj, ref List<Vector2f> rst)
	{
		if (rst == null || rst.Count < 2)
			return false;

		List<Vector2f> outList = new List<Vector2f>();
		outList.Add(rst[0]);

		bool barrier = false;
		for (int i = 0; i < rst.Count - 1; ++i)
		{
			Vector2f beginpt = rst[i];
			Vector2f endpt = rst[i+1];
			Vector2f midpt = new Vector2f((beginpt.x + endpt.x) * 0.5f, (beginpt.y + endpt.y) * 0.5f);
            
            float radius = beginpt.Subtract(endpt).length();
            SceneShapeRect region = new SceneShapeRect(new Vector2(midpt.x, midpt.y), radius, radius);
            ArrayList list = SearchObject(region, ObjectType.OBJ_SEARCH_BUILD);

			foreach (BuildObj build in list)
			{
                SceneShapeLine line = new SceneShapeLine(new Vector2(beginpt.x, beginpt.y), new Vector2(endpt.x, endpt.y));
                if(line.intersect(build.GetShape()))
                {
                    barrier = true;
                    break;
                }
	
			}

			if(barrier)
			{
				break;
			}

			outList.Add(endpt);
		}

		if(outList.Count < 2)
		{
			return false;
		}

		rst = outList;
		return true;
	}

	public float GetHeight(float x, float z)
	{
        WalkableTriangle tri = GetWalkableTriangle(new Vector2f(x, z));
        if (tri == null)
            return 0;

        Vector3f result = Geometryalgorithm2d.point_project_plane(new Vector3f(tri.tri.v0.x, tri.height[0], tri.tri.v0.y),
                            new Vector3f(tri.tri.v1.x, tri.height[1], tri.tri.v1.y),
                            new Vector3f(tri.tri.v2.x, tri.height[2], tri.tri.v2.y), x, z);
        return result.y;
	}

	public bool TestLineBlock(Vector3 startPosition, Vector3 endPosition, bool testHighBlock, bool testLowBlock, out Vector3 farthest)
	{
        Vector2f hitTestEndPos = new Vector2f();
        farthest = endPosition;

        bool rst = TestLineBlockFarthest(startPosition.x, startPosition.z, endPosition.x, endPosition.z, testHighBlock, testLowBlock, ref hitTestEndPos);
        if(rst && farthest != null)
        {
            farthest.x = hitTestEndPos.x;
            farthest.y = GetHeight(hitTestEndPos.x, hitTestEndPos.y);
			farthest.z = hitTestEndPos.y;
        }

        return rst;
	}

	public Vector3 FarthestWalkable(Vector3 startPosition, Vector3 endPosition)
	{
		float distance = Utility.Distance2D(startPosition, endPosition);

		Vector3 farthest = startPosition;

		for (; Utility.Distance2DSquared(startPosition, endPosition) > 0.25f; )
		{
			Vector3 midPosition = (startPosition + endPosition) / 2f;
			if (MayLineArrive(startPosition, midPosition))
			{
				farthest = startPosition = midPosition;
			}
			else
			{
				endPosition = midPosition;
			}
		}

		farthest.y = GetHeight(farthest.x, farthest.z);
		return farthest;
	}

	virtual public void pass()
	{
        
	}

	virtual public bool isPassed()
	{
		return false;
	}

    public uint GetRunTime()
    {
        return mRunTime;
    }

    virtual public bool isSafeScene()
    {
       return true;
    }

	public uint CreateEffect(int resId, Vector3 scale/* = Vector3.one 不是编译时常量?*/, Vector3 scnPos, float dir = float.NaN, string alias = null,bool forceunloop = false)
    {
        if (mParticleManager == null)
            return uint.MaxValue;
        if (!DataManager.EffectTable.ContainsKey(resId))
            return uint.MaxValue;
        EffectTableItem item = DataManager.EffectTable[resId] as EffectTableItem;

        TransformData data = new TransformData();
        data.Pos = scnPos;
		data.Scale = scale * item.scale;

		if (!float.IsNaN(dir))
			data.Rot = new Vector3(0f, dir * Mathf.Rad2Deg, 0f);

		if (!string.IsNullOrEmpty(item.soundId))
		{
			string[] array = item.soundId.Split('|');
			SoundManager.Instance.Play(
				int.Parse(array[UnityEngine.Random.Range(0, array.Length)]),
				item.soundDelay
				);
		}
        return mParticleManager.PlayFx(item.effect_name, forceunloop ? false : item.loop, null, data, -1, alias);
    }

	public void RemoveEffect(uint instId)
	{
		SceneParticleManager mng = GetParticleManager();
		mng.RemoveParticle(instId);
	}

    public ObjectBase CreateSprite(ObjectInitParam param)
    {
        Type type = param.GetType();
        ObjectBase obj = null;
        if (type == typeof(NpcInitParam))
        {
            obj = new Npc();
        }
        else if (type == typeof( PlayerInitParam ))
        {
            obj = new Player();
			//Debug.Log("CreatePlayer");
        }
		else if (type == typeof(TrapInitParam))
		{
			obj = new Trap();
		}
		else if (type == typeof(PickInitParam))
		{
			obj = new Pick();
		}
        else if(type == typeof(BuildInitParam))
        {
            obj = new BuildObj();
        }
		else if (type == typeof(GhostInitParam))
		{
			obj = new Ghost();
		}
        else if (type == typeof(CropsInitParam))
        {
            obj = new Crops();
        }

        if (obj == null || !obj.Init(param))
        {
            GameDebug.LogError("CreateSpriteError: type = " + type.ToString());
            return null;
        }

        return AddSprte(obj) == uint.MaxValue ? null : obj;
    }

	public void SetOwner(ObjectBase owner)
	{
		mOwner = owner;
	}

	public ObjectBase GetOwner()
	{
		return mOwner;
	}

    public bool IsInWalkableRegion(float x, float z)
    {
        return getCellIndex(new Vector2f(x, z)) != PAHT_FINDER_ENUM.INVAILD_INDEX;
    }

    public bool IsInWalkableRegion(Vector3 pos)
    {
        return getCellIndex(new Vector2f(pos.x, pos.z)) != PAHT_FINDER_ENUM.INVAILD_INDEX;
    }

    public bool MayLineArrive(Vector3 start, Vector3 end, float step = 0.05f)
    {
        if (!IsInWalkableRegion(start) || !IsInWalkableRegion(end))
            return false;

        start.y = 0;
        end.y = 0;

        float length = Vector3.Distance(start, end);

        if (length == 0)
            return false;

        Vector3 dirv = start - end;

        float dir = Utility.Vector3ToRadian(dirv);
        float sinvalue = Mathf.Sin(dir);
        float cosvalue = Mathf.Cos(dir);
        float distance = 0.0f;
        Vector3 testPos = Vector3.zero;
        for( int i = 0 ; i < 10000 ;++i )
        {
            distance = step * i;
            if (distance > length)
                return true;
            testPos.x = end.x + sinvalue * distance;
            testPos.z = end.z + cosvalue * distance;

            if (!IsInWalkableRegion(testPos.x, testPos.z))
                return false;
        }
        return true;
    }

    // BLOCK_HIGH 为绝对阻挡
	public bool TestLineBlock(float sx, float sz, float ex, float ez, BlockType type = BlockType.BLOCK_HIGH)
    {
        LineBlockWallIntersect testElement = new LineBlockWallIntersect();
        testElement.blockType = type;
        testElement.line = new LineSegf(new Vector2f(sx, sz), new Vector2f(ex, ez));

        if (type == BlockType.BLOCK_LOW)
        {
            return !mQuadtreeLowBlockWalls.IntersectElement(testElement);
        }

        return !mQuadtreeHighBlockWalls.IntersectElement(testElement);
    }

	// BLOCK_HIGH 为绝对阻挡
	public bool TestLineBlockFarthest(float sx, float sz, float ex, float ez, BlockType type, ref Vector2f farthestPoint)
    {
        Vector2f startPoint = new Vector2f(sx, sz);
	    Vector2f endPoint = new Vector2f(ex, ez);

	    if (TestLineBlock(sx, sz, ex, ez, type))
	    {// 可以直接到达

			farthestPoint.x = endPoint.x;
			farthestPoint.y = endPoint.y;

		    return true;
	    }

	    if (sx == ex && sz == ez)
	    {// 起点于终点相同，并且在阻挡线段上
		    
			farthestPoint.x = sx;
			farthestPoint.y = sz;
		    
		    return true;
	    }

	    // 不可直接到达 并需要计算出最远可达位置
	    Vector2f dir = endPoint.Subtract(startPoint);
	    dir.MultiplyBy(1 / dir.length());

	    // 1. 通过二分法计算出大致最远的可达位置
	    Vector2f newStartPoint = new Vector2f(startPoint.x, startPoint.y);
	    Vector2f newEndPoint = new Vector2f(endPoint.x, endPoint.y);

	    farthestPoint.x = newStartPoint.x;
	    farthestPoint.y = newStartPoint.y;

	    for (int j = 0; j < 100; j++)
	    {// 防止死循环
		    float length = (newEndPoint.Subtract(newStartPoint)).length();	
		    if (length < 0.1f)
			    break;

		    Vector2f centerPoint = new Vector2f(newStartPoint.x + dir.x * length / 2.0f, newStartPoint.y + dir.y * length / 2.0f);	

		    if (TestLineBlock(newStartPoint.x, newStartPoint.y, centerPoint.x, centerPoint.y, type))
		    {
			    newStartPoint.x = centerPoint.x;
			    newStartPoint.y = centerPoint.y;

			    farthestPoint.x = newStartPoint.x;
			    farthestPoint.y = newStartPoint.y;
		    }
		    else
		    {
			    newEndPoint.x = centerPoint.x;
			    newEndPoint.y = centerPoint.y;
		    }
	    }
	    return false;
    }

	// 检测阻挡并获取最远到达距离
	public bool TestLineBlockFarthest(float sx, float sz, float ex, float ez, bool highTest, bool lowTest, ref Vector2f farthestPoint)
    {
       
        Vector2f hitTestEndPos = new Vector2f();

        bool ret = false;
        if (highTest && !TestLineBlockFarthest(sx, sz, ex, ez, BlockType.BLOCK_HIGH, ref hitTestEndPos))
        {
            ex = hitTestEndPos.x;
            ez = hitTestEndPos.y;
            ret = true;
        }

        if (lowTest && !TestLineBlockFarthest(sx, sz, ex, ez, BlockType.BLOCK_LOW, ref hitTestEndPos))
        {
            ret = true;
        }

        if (ret)
        {
            farthestPoint.x = hitTestEndPos.x;
            farthestPoint.y = hitTestEndPos.y;
        }
        return ret;
       
    }

    public float GetXSize()
    {
        return mWidth;
    }

    public float GetZSize()
    {
        return mHeight;
    }

	public bool IsBarrierRegion(ObjectBase obj, float x, float z)
	{
        SceneShapeRect region = new SceneShapeRect(new Vector2(obj.GetPosition().x, obj.GetPosition().z), 6.0f, 6.0f);
        ArrayList list = SearchObject(region, ObjectType.OBJ_SEARCH_BUILD, false);
		foreach(BuildObj build in list)
		{
			if(!build.TestBarrier(obj) && build.TestTransformBarrier(obj, new Vector2(x, z), obj.GetDirection()))
			{
				return true;
			}
		}

		return false;
	}

    public void ClearCurGrowthTrigger()
    {
        mScriptSystem.ClearCurGrowthTrigger();
    }

    public void DestroyCurGrowthTrigger()
    {
        mScriptSystem.DestroyCurGrowthTrigger();
    }

	// 场景是否允许该行为
	public bool IsActionFlagAllowed(SceneActionFlag flag)
	{
		return (mActionFlag & (int)flag) > 0;
	}

	// 增加权限
	public void AddActionFlag(SceneActionFlag flag)
	{
		mActionFlag |= (int)flag;
	}

	// 移除权限
	public void RemoveActionFlag(SceneActionFlag flag)
	{
		mActionFlag &= ~(int)flag;
	}

	// 增加全部权限
	public void AddAllActionFlag()
	{
		mActionFlag = (int)SceneActionFlag.SceneActionFlag_All;
	}

	// 移除全部权限
	public void RemoveAllActionFlag()
	{
		mActionFlag = 0;
	}

    public Vector3 GetTransport()
    {
       return mScriptSystem.GetTransPort();
    }

    //---------------------动态场景--------------------------

    public void InitDynamicAnim(DynamicAnimation[] dynamics)
    {
        mDynamicAnimations = dynamics;

        if (dynamics != null)
        {
            foreach (DynamicAnimation anim in dynamics)
            {
                anim.TargetStopCallBack += OnSceneAniFinished;
            }
        }
    }
    public void ChangeDynamicSpeed(float speed)
    {
        if (mDynamicAnimations == null)
            return;
        foreach(DynamicAnimation anim in mDynamicAnimations)
        {
            anim.movspeed = speed;
        }
    }
    public void StopDynamicToTarget()
    {
        if (mDynamicAnimations == null)
            return;
        foreach (DynamicAnimation anim in mDynamicAnimations)
        {
            anim.InvalidToTarget();
        }
    }
    public void PauseDynamic()
    {
        if (mDynamicAnimations == null)
            return;
        foreach (DynamicAnimation anim in mDynamicAnimations)
        {
            anim.Pause();
        }
    }
    public void PlayDynamicAnimation()
    {
        if (mDynamicAnimations == null)
            return;
        foreach (DynamicAnimation anim in mDynamicAnimations)
        {
            anim.Play();
        }
    }

    public void PlayGameObjAnim(string anim)
    {
       GameobjectMecAnim[] objs = GameObject.FindObjectsOfType<GameobjectMecAnim>();
        foreach(GameobjectMecAnim obj in objs)
        {
            if (obj.animator == null)
                continue;
            obj.animator.Play("Base Layer." + anim);
        }
    }

   

	virtual public ObjectBase CreateMainPlayer()
	{
		PlayerPropertyModule module = ModuleManager.Instance.FindModule<PlayerPropertyModule>();

		PlayerInitParam param = new PlayerInitParam();
		param.init_property = module.GetPlayerProperty();
		param.player_data = PlayerDataPool.Instance.MainData;
		param.init_pos = GetInitPos();
		param.init_pos.y = GetHeight(param.init_pos.x, param.init_pos.z);
		param.init_dir = GetInitDir();

		ObjectBase player = CreateSprite(param);
		if (player == null)
			return null;

		SetOwner(player);

		PlayerController.Instance.SetControl(player.InstanceID);

		return player;
	}

    virtual public void CreateCrops()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == module)
            return;
        CropsInitParam cropsParam = new CropsInitParam();

        if (Vector3.zero != GetInitPosByName("crops1"))
        {
            cropsParam.crops_res_id = module.GetMainCropsId();
            cropsParam.init_pos = GetInitPosByName("crops1");
            cropsParam.init_pos.y = GetHeight(cropsParam.init_pos.x, cropsParam.init_pos.z);
            cropsParam.init_dir = GetInitDirByName("crops1");
            cropsParam.cropsinfo = module.GetMainCropsInfo();


            ObjectBase obj = CreateSprite(cropsParam);
            if (obj == null)
            {
                GameDebug.LogError("创建主佣兵失败。cropsId:" + module.GetMainCropsId());
                return;
            }

            PlayerController.Instance.SetMainCropsControl(obj.mInstanceID);
        }
        else
        {
            return;
        }

        if (Vector3.zero != GetInitPosByName("crops2"))
        {
            cropsParam.crops_res_id = module.GetSubCropsId();
            cropsParam.init_pos = GetInitPosByName("crops2");
            cropsParam.init_pos.y = GetHeight(cropsParam.init_pos.x, cropsParam.init_pos.z);
            cropsParam.init_dir = GetInitDirByName("crops2");
            cropsParam.cropsinfo = module.GetSubCropsInfo();

            ObjectBase obj = CreateSprite(cropsParam);
            if (obj == null)
            {
                GameDebug.LogError("创建副佣兵失败。cropsId:" + module.GetSubCropsId());
                return;
            }

            PlayerController.Instance.SetSubCropsControl(obj.mInstanceID);
        }
    }

	public virtual void onDamage(BattleUnit damageTo, DamageInfo damage, AttackerAttr attackerAttr)
	{
	
	}

	public virtual void onRoleModelLoaded(Role role)
	{

	}
 }
