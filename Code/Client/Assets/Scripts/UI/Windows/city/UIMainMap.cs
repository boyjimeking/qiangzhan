using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CityLine
{
    public GameObject mObj;
    public UISprite mMapLine;
    public CityLine(GameObject gameObj)
    {
        mObj = gameObj;
        mMapLine = mObj.GetComponent<UISprite>();
    }

    public void updateline(bool isActive, int index = 1)
    {
        if (isActive)
        {
            mMapLine.gameObject.SetActive(true);
            UIAtlasHelper.SetSpriteImage(mMapLine, "citymap:citymap_xianlu" + index.ToString(), true);
        }

        else
        {
            mMapLine.gameObject.SetActive(false);
            UIAtlasHelper.SetSpriteImage(mMapLine, "", true);
        }
    }
          
}
public class CityPanel
{
    public GameObject mObj;
    public UISprite mBg;
    public UILabel mCondition;
    public UISprite mLock;
    public UISprite mTip;
    public UIButton mMapBtn;
    public UISprite mGuide;
    public Scene_CitySceneTableItem mItem = null;
    public CityLine mLine = null;
    public bool isLine = false;

    public CityPanel(GameObject gameObj)
    {
        mObj = gameObj;
        mCondition = ObjectCommon.GetChildComponent<UILabel>(mObj, "condition");
        mLock = ObjectCommon.GetChildComponent<UISprite>(mObj, "lock");
        mTip = ObjectCommon.GetChildComponent<UISprite>(mObj, "tip");
        mGuide = ObjectCommon.GetChildComponent<UISprite>(mObj, "guide");
        mMapBtn = mObj.GetComponent<UIButton>();
    }

    public int GetResId()
    {
        return mItem.resID;
    }
}

public class UIMainMap : UIWindow
{
    private GameObject mMapItemUnit;
    private GameObject mMapLineUnit;
    private UIButton mCloseBtn;
    private UISprite mContainer;
    private List<CityPanel> mUnlocks = new List<CityPanel>();//没锁的
    private List<CityPanel> mlocks = new List<CityPanel>();//锁住的
    private float distanceY = 0.0f;
    private float distanceX = 0.0f;
    Transform mtransform = null;
    private bool isplay = false;

    private WorldMapModule mWorldMapModule = null;
    WorldMapModule Pdm
    {
        get
        {
            if (mWorldMapModule == null)
                mWorldMapModule = ModuleManager.Instance.FindModule<WorldMapModule>();

            return mWorldMapModule;
        }
    }
    private class myCompare : System.Collections.IComparer
    {
        public int Compare(object a, object b)
        {
            return ((Scene_CitySceneTableItem)a).mUnlock < ((Scene_CitySceneTableItem)b).mUnlock ? -1 : (((Scene_CitySceneTableItem)a).mUnlock == ((Scene_CitySceneTableItem)b).mUnlock ? 0 : 1);
        }
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        mMapItemUnit = FindChild("MapItemUnit");
        mMapLineUnit = FindChild("MaplineUnit");
        mCloseBtn = FindComponent<UIButton>("mCloseBtn");
        mContainer = FindComponent<UISprite>("container");
        UIEventListener.Get(mContainer.gameObject).onDrag = onMapItemDrag;
        mtransform = this.mView.gameObject.transform;

        float aspectRatio = UIRootAdaptive.manualHeight / (float)Screen.height;

        float height = (int)UIRootAdaptive.manualHeight;
        float width = (int)Mathf.Ceil(Screen.width * aspectRatio);

        distanceX = (mContainer.width - width) / 2;
        distanceY = (mContainer.height - height) / 2;
    }
 
   private void onMapItemDrag(GameObject go, Vector2 delta)
    {
        mtransform.localPosition += new Vector3(delta.x, delta.y, 0);
        RevisePos();
    }

    public Hashtable HashValSort(DataTable ht)
    {
        ArrayList list = ht.GetValues();
        myCompare Compare = new myCompare();
        list.Sort(Compare);

        Hashtable hb = new Hashtable();
        foreach (Scene_CitySceneTableItem svalue in list)
        {
            IDictionaryEnumerator ide = ht.GetEnumerator();
            while (ide.MoveNext())
            {
                if (ide.Value == svalue)
                {
                    hb.Add(ide.Key, svalue);
                }
            }
        }
        return hb;
    }

    private void RevisePos()
    {
        if (distanceX < mtransform.localPosition.x)
            mtransform.localPosition = new Vector3(distanceX, mtransform.localPosition.y, 0);

        if (distanceY < mtransform.localPosition.y)
            mtransform.localPosition = new Vector3(mtransform.localPosition.x, distanceY, 0);

        if (-distanceX > mtransform.localPosition.x)
            mtransform.localPosition = new Vector3(-distanceX, mtransform.localPosition.y, 0);

        if (-distanceY > mtransform.localPosition.y)
            mtransform.localPosition = new Vector3(mtransform.localPosition.x, -distanceY, 0);
    }
    private void CorrectMap()
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (null == scn)
            return;

        int resid = Pdm.GuideResId == -1 ? scn.GetSceneResId() : Pdm.GuideResId;
        if (0 > resid)
            return;

        if (!DataManager.SceneTable.ContainsKey(resid))
            return;

        Scene_CitySceneTableItem mSceneRes = DataManager.SceneTable[resid] as Scene_CitySceneTableItem;

        ArrayList list = mSceneRes.GetResolving(mSceneRes);
        if (null == list || null == list[1])
            return;

        Vector3 vec = (Vector3)list[1];
        mtransform.localPosition = new Vector3(vec.x, vec.y, vec.z);
        RevisePos();
    }

    private void UpdateGuideUI(int resid)
    {
        for (int i = 0; i < mUnlocks.Count; i++)
        {
            if (resid == mUnlocks[i].mItem.resID)
            {
                UISpriteAnimation ani = AnimationManager.Instance.AddSpriteAnimation("unlock", mUnlocks[i].mLock.gameObject, 15, 8, true, true);
                ani.transform.localPosition = new Vector3(ani.transform.localPosition.x, ani.transform.localPosition.y + 50, ani.transform.localPosition.z);
            }
            break;
        }
    }

    private void UpdateAnimation()
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (null == scn)
            return;

        for (int i = 0; i < mUnlocks.Count; i++)
        {
            if (scn.GetSceneResId() == mUnlocks[i].mItem.resID)
            {
                UISpriteAnimation ani = AnimationManager.Instance.AddSpriteAnimation("tip", mUnlocks[i].mTip.gameObject, 15, 8, true, true);
                ani.transform.localPosition = new Vector3(ani.transform.localPosition.x, ani.transform.localPosition.y + 50, ani.transform.localPosition.z);
            }
  
            if (Pdm.GuideResId  == mUnlocks[i].mItem.resID)
           {
               UISpriteAnimation ani = AnimationManager.Instance.AddSpriteAnimation("yindaojiantou", mUnlocks[i].mGuide.gameObject, 15, 8, true, true);
               ani.transform.localPosition = new Vector3(ani.transform.localPosition.x + 10, ani.transform.localPosition.y -10, ani.transform.localPosition.z);
           }
        }
    }
    private void UpdateMainUI()
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if ( null == scn)
            return;

        for (int i = 0; i < mUnlocks.Count; i++)
        {
            if (null == mUnlocks[i].mItem || null == mUnlocks[i])
                continue;

            string[] str = mUnlocks[i].mItem.GetBg(mUnlocks[i].mItem);
            if (null == str)
            {
                Debug.LogError("scene_city.txt error");
                return;
            }

            mUnlocks[i].mLock.gameObject.SetActive(false);
            UIAtlasHelper.SetButtonImage( mUnlocks[i].mMapBtn, str[0], true);   
        }

        for (int i = 0; i < mlocks.Count; i++)
        {
            if (null == mlocks[i].mItem || null == mlocks[i])
                continue;

            string[] str = mlocks[i].mItem.GetBg(mlocks[i].mItem);
            if (null == str)
            {
                Debug.LogError("scene_city.txt error");
                return;
            }

            if (!Pdm.isLevel(mlocks[i].mItem))
                mlocks[i].mCondition.text = mlocks[i].mItem.mUnlock.ToString() + "级解锁";
            else
            {
                if (Pdm.isLock(mlocks[i].mItem))
                    mlocks[i].mCondition.text = "主线关卡未完成";
            }

            Pdm.addDictData(mlocks[i].GetResId(), mlocks[i].mCondition.text);

            if (0 == i)
            {
                mlocks[i].mLock.gameObject.SetActive(true);
                UIAtlasHelper.SetButtonImage(mlocks[i].mMapBtn, str[1], true);
            }
            else
            {
                mlocks[i].mLock.gameObject.SetActive(false);
                UIAtlasHelper.SetButtonImage(mlocks[i].mMapBtn, str[2], true);
                mlocks[i].mCondition.text = "";
            }
            //人不可能在没有解锁的城市
            mlocks[i].mTip.gameObject.SetActive(false);
            mlocks[i].mGuide.gameObject.SetActive(false);
        }
    }

    private void ClearAll()
    {
        for (int i = 0; i < mUnlocks.Count; i++)
        {
            if (null == mUnlocks[i])
                continue;

            if (null != mUnlocks[i].mObj)
                UnityEngine.Object.DestroyObject(mUnlocks[i].mObj);

            if (null != mUnlocks[i].mLine && null != mUnlocks[i].mLine.mObj)
                UnityEngine.Object.DestroyObject(mUnlocks[i].mLine.mObj);
        }

        for (int i = 0; i < mlocks.Count; i++)
        {
            if (null == mlocks[i])
                continue;

            if (null != mlocks[i].mObj)
                UnityEngine.Object.DestroyObject(mlocks[i].mObj);
        }

        mUnlocks.Clear();
        mlocks.Clear();
    }

    private void UpdateMapData()
    {
        foreach (CityPanel panel in mUnlocks)
        {
            Pdm.UpdateMapData(panel.GetResId(), false);
        }
    }
    private void UpdateLockUI()
    {
        isplay = false;
        foreach (int resid in Pdm.mPlayerNewMapData)
        {
            if (0 == resid)
                continue;

            if (!Pdm.mPlayerOldMapData.Contains(resid))
            {
                CityPanel temp = Findpanel(resid);
                if (null == temp)
                    return;

                string[] str = temp.mItem.GetBg(temp.mItem);
                if (null == str)
                {
                    Debug.LogError("scene_city.txt error");
                    return;
                }

                temp.mLock.gameObject.SetActive(true);
                UIAtlasHelper.SetButtonImage(temp.mMapBtn, str[1], true);
                temp.mCondition.gameObject.SetActive(true);
                temp.mCondition.text = Pdm.getDictData(temp.GetResId());
                Pdm.UpdateMapData(temp.GetResId(), true);
                UISpriteAnimation ani = AnimationManager.Instance.AddSpriteAnimation("unlock", temp.mLock.gameObject, 15, 8, true, false);
                ani.transform.localPosition = new Vector3(ani.transform.localPosition.x, ani.transform.localPosition.y - 22, ani.transform.localPosition.z);
                temp.mLock.gameObject.name = str[0];
                ani.loop = false;
                isplay = true;
                ani.onFinished += DestroyUnlock;
            }
        }
    }
    void DestroyUnlock(GameObject go)
    {
        go.SetActive(false);
        GameObject Obj = go.transform.parent.gameObject.transform.parent.gameObject;
        if (!Obj)
            return;

        UIButton MapBtn = Obj.GetComponent<UIButton>();
        if (!MapBtn)
            return ;

        UIAtlasHelper.SetButtonImage(MapBtn, go.transform.parent.gameObject.name, true);
        UILabel Condition = ObjectCommon.GetChildComponent<UILabel>(Obj, "condition");

        Condition.gameObject.SetActive(false);
        Condition.text = "";
        go.transform.parent.gameObject.SetActive(false);
        UpdateAnimation();
    }

 
    protected override void OnOpen(object param = null)
    {
        base.OnOpen();
        CorrectMap();
        ClearAll();
        RefreshUI();
        updateLineUI();
        UpdateMainUI();
        UpdateMapData();
        UpdateLockUI();
        if (!isplay)
            UpdateAnimation();

        EventDelegate.Add(mCloseBtn.onClick, OnCloseClick);
    }

    private void OnCloseClick()
    {
        WindowManager.Instance.CloseUI("mainmap");
    }

    protected override void OnClose()
    {
        base.OnClose();
        EventDelegate.Remove(mCloseBtn.onClick, OnCloseClick);

        Pdm.GuideResId = -1;
    }

    private void CheckPush(Scene_CitySceneTableItem item, CityPanel citypan)
    {
        citypan.mItem = item;
        if (0 == item.resID || (Pdm.isLevel(item) && !Pdm.isLock(item)))
        {
            mUnlocks.Add(citypan);     
        }
        else
        {
            mlocks.Add(citypan);
        }
    }

    private CityPanel Findpanel(int resid)
    {
        for (int i = 0; i < mUnlocks.Count; i++)
        {
            if (mUnlocks[i].GetResId() == resid)
            {
                return mUnlocks[i];
            }
        }
        return null;
    }

    private void RefreshUI()
    {
        PlayerDataModule ply = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == ply)
            return;

        Hashtable shortTable = HashValSort(DataManager.SceneCityTable);

        foreach (DictionaryEntry de in shortTable)
        {
            Scene_CitySceneTableItem item = de.Value as Scene_CitySceneTableItem;
            if (null == item)
                continue;

            GameObject gameObj = WindowManager.Instance.CloneGameObject(mMapItemUnit);
            if (gameObj == null)
            {
                GameDebug.LogError("instance citypan error");
                return;
            }

            ArrayList list = item.GetResolving(item);
            if (null == list || null == list[0])
            {
                GameDebug.LogError("Scene_city.txt error");
                return;
            }

            gameObj.SetActive(true);
            gameObj.transform.parent = mContainer.gameObject.transform;
            gameObj.transform.localScale = Vector3.one;
            gameObj.transform.localPosition = (Vector3)list[0];
            gameObj.name = item.resID.ToString();
            UIEventListener.Get(gameObj).onClick = onEnterScene;

            CheckPush(item,new CityPanel(gameObj));    
        }
    }

    private void updateLineUI()
    {
        for (int i = 1; i < mUnlocks.Count; i++)
        {
            if (null == mUnlocks[i] || null == mUnlocks[i].mItem)
                continue;

            ArrayList list = mUnlocks[i].mItem.GetResolving(mUnlocks[i].mItem);
            if (null == list || 2 >= list.Count)
                continue;

            GameObject gameObj = WindowManager.Instance.CloneGameObject(mMapLineUnit);
            if (gameObj == null)
            {
                GameDebug.LogError("instance line error");
                return;
            }

            gameObj.SetActive(true);
            gameObj.transform.parent = this.mView.gameObject.transform;
            gameObj.transform.localScale = Vector3.one;
            gameObj.transform.localPosition = (Vector3)list[2];
            gameObj.name = mUnlocks[i].mItem.resID.ToString();
            mUnlocks[i].mLine = new CityLine(gameObj);
            mUnlocks[i].mLine.updateline(true,i);
        }
    }
    private void onEnterScene(GameObject gameObj)
    {
        int resId = System.Convert.ToInt32(gameObj.name);

        CityPanel citypan = null;
        foreach (CityPanel temp in mUnlocks)
        {
            if (temp.GetResId() == resId)
            {
                citypan = temp;
            }
        }
        if (null == citypan)
            return;

        BaseScene scn = SceneManager.Instance.GetCurScene();
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == scn || null == module)
            return ;

        if (scn.GetSceneResId() == citypan.GetResId())
        {
            OnCloseClick();
            return;
        } 

        if (citypan.mItem.mUnlock > module.GetLevel())
            return;

        SceneManager.Instance.RequestEnterScene(citypan.GetResId());
    }
  
}
