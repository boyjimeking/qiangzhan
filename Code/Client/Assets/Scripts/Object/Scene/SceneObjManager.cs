using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

public class SceneObjManager
{
    private BaseScene mScene = null;

    private List<ObjectBase> mWaitDestoryObjects = null;   //等待销毁
    private List<ObjectBase> mDestoryObjects = null;        //销毁

  
    private Pool mObjPool = null;

    private bool mIsDelete = false;

    public bool IsDelete
    {
        get { return mIsDelete;}
        set { mIsDelete = value;}
    }

	public SceneObjManager(BaseScene scene)
	{
        mScene = scene;

        mDestoryObjects = new List<ObjectBase>();

        mWaitDestoryObjects = new List<ObjectBase>();

        mObjPool = new Pool(1000);

        Clear();
	}

    public void Destroy()
    {
        int maxCount = mObjPool.GetMaxCount();
        for(int i = 0; i < maxCount; i++)
        {
            ObjectBase obj = mObjPool.Get(i) as ObjectBase;
            if (obj == null)
                continue;

            obj.Destroy();
        }

        Clear();
    }

    public void Clear()
    {
        mDestoryObjects.Clear();
        mObjPool.Clear();
    }

    public void Update(uint elapsed)
    {
        for (int i = 0; i < mWaitDestoryObjects.Count; ++i)
        {
            ObjectBase obj = mWaitDestoryObjects[i];
            if (obj == null)
                continue;

            if (!obj.UpdateDestroy(elapsed))
            {
                mWaitDestoryObjects.RemoveAt(i);

                mDestoryObjects.Add(obj);

                break;
            }
        }

        

  
        PoolSlot slot = mObjPool.UsedList();
        while(slot != null)
        {
            ObjectBase obj = slot.Content as ObjectBase;
            if(obj == null)
            {
                slot = slot.NextSlot;
                continue;
            }

            if (!obj.Update(elapsed))
            {
                if (obj.IsDestroyWaiting())
                {
                    mWaitDestoryObjects.Add(obj);
                }
                else
                {
                    mDestoryObjects.Add(obj);
                }

                slot = slot.NextSlot;
                mObjPool.Remove((int)obj.InstanceID);
                continue;
            }

            slot = slot.NextSlot;
        }

        for (int i = 0; i < mDestoryObjects.Count; ++i)
        {
            ObjectBase obj = mDestoryObjects[i];
            if (obj == null)
                continue;

            obj.Destroy();
        }

        mDestoryObjects.Clear();
    }

    //添加一个Object
    public uint AddSprte(ObjectBase sprite)
	{
        int tempId = mObjPool.Put(sprite);
		if (tempId < 0)
		{
			GameDebug.LogError("mObjPool: too many objects");
			return (uint)tempId;
		}

        sprite.InstanceID = (uint)tempId;
        sprite.OnEnterScene(mScene, (uint)tempId);
        mScene.OnSpriteEnterScene(sprite);

        return (uint)tempId;
	}

    //找一个Object
    public ObjectBase FindObject(uint instid)
    {
        ObjectBase obj = mObjPool.Get((int)instid) as ObjectBase;
        return obj;
    }

	public bool RemoveObject(uint instid) 
	{
        ObjectBase obj = mObjPool.Get((int)instid) as ObjectBase;
        if (obj == null)
            return false;

        obj.Disappear();
		return true;
	}

    public bool HasObjectByAlias(string alias)
    {
        PoolSlot slot = mObjPool.UsedList();
        while (slot != null)
        {
            ObjectBase obj = slot.Content as ObjectBase;
            if (obj == null)
            {
                slot = slot.NextSlot;
                continue;
            }

            if (string.Equals(obj.GetAlias(), alias))
            {
                return true;
            }

            slot = slot.NextSlot;
        }

        return false;
    }

	public bool RemoveObjectByAlias(string alias)
	{
        PoolSlot slot = mObjPool.UsedList();
        while(slot != null)
        {
            ObjectBase obj = slot.Content as ObjectBase;
            if(obj == null)
            {
                slot = slot.NextSlot;
                continue;
            }

            if(string.Equals(obj.GetAlias(), alias))
            {
                RemoveObject(obj.InstanceID);
            }

            slot = slot.NextSlot;
        }

        return true;
	}

	public bool KillObjectByAlias(string alias)
	{
        PoolSlot slot = mObjPool.UsedList();
        while(slot != null)
        {
            ObjectBase obj = slot.Content as ObjectBase;
            if(obj == null)
            {
                slot = slot.NextSlot;
                continue;
            }

            if(!string.Equals(obj.GetAlias(), alias))
            {
                slot = slot.NextSlot;
                continue;
            }

            if(!typeof(BuildObj).IsAssignableFrom(obj.GetType()))
            {
                slot = slot.NextSlot;
                continue;
            }

            BattleUnit unit = obj as BattleUnit;
            unit.Die(new AttackerAttr(unit));

            slot = slot.NextSlot;
        }
		return true;
	}

    public List<T> SearchObjsByAlias<T>(string alias) where T:ObjectBase
    {
        List<T> re =new List<T>();

        PoolSlot slot = mObjPool.UsedList();
        while(slot != null)
        {
            T obj = slot.Content as T;

            if(obj == null)
            {
                slot = slot.NextSlot;
                continue;
            }

            if(!string.Equals(obj.GetAlias(), alias))
            {
                slot = slot.NextSlot;
                continue;
            }

            re.Add(obj);
            slot = slot.NextSlot;
        }
      
        return re;
    }

    public bool RemoveObjsByAlias<T>(string alias) where T : ObjectBase
    {
        List<T> objs = SearchObjsByAlias<T>(alias);
        foreach (var obj in objs)
        {
            RemoveObject(obj.InstanceID);
        }

        objs.Clear();
        return true;
    }

    public int GetObjectCount()
    {
        return mObjPool.GetCount();
    }

    //搜索区域内的 BattleUnit
    public ArrayList SearchObject(SceneShapeRect shape, int searchType, bool ignoreDead = true)
    {
        ArrayList objs = new ArrayList();

        PoolSlot slot = mObjPool.UsedList();
        while(slot != null)
        {
            ObjectBase unit = slot.Content as ObjectBase;
            if(unit == null)
            {
                slot = slot.NextSlot;
                continue;
            }

            if(!ObjectType.IsCanSearch(searchType , unit.Type))
            {
                slot = slot.NextSlot;
                continue;
            }

            Vector3 pos = unit.GetPosition();

            if (shape.contains(new Vector2(pos.x, pos.z)))
            {
                objs.Add(unit);
            }
 
            slot = slot.NextSlot;
        }
        
		if(!ignoreDead)
		{
			for (int i = 0; i < mWaitDestoryObjects.Count; ++i)
			{
				ObjectBase obj = mWaitDestoryObjects[i];
				if (obj == null)
					continue;

                if (!ObjectType.IsCanSearch(searchType, obj.Type))
                    continue;

                Vector3 pos = obj.GetPosition();

				if (shape.contains(new Vector2(pos.x, pos.z)))
				{
					objs.Add(obj);
				}
			}
		}

        return objs;
    }

    public ArrayList SearchBattleUnit(Vector2f center, float radius)
    {
        Rectanglef rect = new Rectanglef(new Vector2f(center.x - radius, center.y - radius), new Vector2f(center.x + radius, center.y + radius));

        ArrayList objs = new ArrayList();

        PoolSlot slot = mObjPool.UsedList();
        while(slot != null)
        {
            BattleUnit unit = slot.Content as BattleUnit;
            if(unit == null)
            {
                slot = slot.NextSlot;
                continue;
            }

            Vector3f pos = unit.GetPosition3f();
            if (Geometryalgorithm2d.point_in_rectangle(new Vector2f(pos.x, pos.z), rect))
            {
                objs.Add(unit);
            }

            slot = slot.NextSlot;
        }
        
        return objs;
    }

    //搜索一个自动瞄准的目标
    public BattleUnit SearchAutoAimEnemy(BattleUnit owner, float distance)
    {
        BattleUnit nearestObj = null;

        float minDis = distance;

        PoolSlot slot = mObjPool.UsedList();
        while(slot != null)
        {
            BattleUnit unit = slot.Content as BattleUnit;
            if (unit == null)
            {
                slot = slot.NextSlot;
                continue;
            }
            
            if(unit == owner)
            {
                slot = slot.NextSlot;
                continue;
            }

            if(!unit.isAlive())
            {
                slot = slot.NextSlot;
                continue;
            }

            if(!owner.IsEnemy(unit))
            {
                slot = slot.NextSlot;
                continue;
            }
  
            float dis = Vector3.Distance(unit.GetPosition(), owner.GetPosition());
            //小于最大距离 
            if (dis <= distance)
            {
                if (dis < minDis)
                {
                    minDis = dis;
                    nearestObj = unit;
                }
            }

            slot = slot.NextSlot;
             
         }

        return nearestObj;
    }

	// 搜索战斗指引目标
	public BattleUnit SearchGuideTarget(ObjectBase owner)
	{
		// 视野内有可攻击目标
		BattleUnit target = SearchGuideTarget(owner as BattleUnit, 12);
		if (target != null)
		{
			return null;
		}

		return SearchGuideTarget(owner as BattleUnit, float.MaxValue);
	}

	// 搜索战斗指引目标
	public BattleUnit SearchGuideTarget(BattleUnit owner, float distance)
	{
		BattleUnit nearestObj = null;

        float minDis = distance;

        PoolSlot slot = mObjPool.UsedList();

        while(slot != null)
        {
            BattleUnit unit = slot.Content as BattleUnit;
            if (unit == null || unit == owner)
            {
                slot = slot.NextSlot;
                continue;
            }

            if (!unit.isAlive())
            {
                slot = slot.NextSlot;
                continue;
            }

			if (!owner.IsEnemy(unit))
            {
                slot = slot.NextSlot;
				continue;
            }

			if (!unit.IsGuildTarget())
            {
                slot = slot.NextSlot;
				continue;
            }

            float dis = Vector3.Distance(unit.GetPosition(), owner.GetPosition());
            //小于最大距离 
            if (dis <= distance)
            {
                if (dis < minDis)
                {
                    minDis = dis;
                    nearestObj = unit;
                }
            }

            slot = slot.NextSlot;
        }
        return nearestObj;
	}

	public Pick SearchPickTarget(ObjectBase startObj)
	{
		// 视野外有可拾取道具;
		Pick target = SearchPickTarget(startObj , 12f);
		if (target != null)
		{
			return target;
		}
		
		return SearchPickTarget(startObj , float.MaxValue);
	}

	/// 搜索掉落物的目标（超出distance距离以外的最近的道具目标）;
	public Pick SearchPickTarget(ObjectBase startObj , float distance)
	{
		Pick nearestObj = null;

		float minDis = float.MaxValue;

        PoolSlot slot = mObjPool.UsedList();
        while(slot != null)
		{	
			Pick unit = slot.Content as Pick;
            if (unit == null || unit == startObj || (unit.GetCurPickTableItem().resID != ZombiesStageModule.PickId && unit.GetCurPickTableItem().resID != 26))
            {
                slot = slot.NextSlot;
				continue;
            }

			float dis = Vector3.Distance(unit.GetPosition(), startObj.GetPosition());
			//大于最小距离 
			if (dis >= distance)
			{
				if (dis < minDis)
				{
					minDis = dis;
					nearestObj = unit;
				}
			}

            slot = slot.NextSlot;
		}
		return nearestObj;
	}

	// 生成掉落物
	public static bool CreatePickInitParam(Pick.PickType picktype, int resid, int content, Vector3 pos, float dir, out List<PickInitParam> paramList, bool randomPos = false, Pick.FlyType flytype = Pick.FlyType.FLY_OUT, bool isDropBoxId = true)
	{
		paramList = new List<PickInitParam>();

		if (content < 0)
			return false;

		if (picktype <= Pick.PickType.INVALID || picktype >= Pick.PickType.TYPE_COUNT)
			return false;

		if(isDropBoxId)
		{
			ArrayList buffList = new ArrayList();
			if (DropManager.Instance.GenerateDropBox(content, out buffList))
			{
				foreach (DropBoxItem item in buffList)
				{
					int pickres_id = item.itemid;

					if (picktype == Pick.PickType.ITEM)
					{
						ItemTableItem itemres = ItemManager.GetItemRes(item.itemid);
						if (itemres == null)
							continue;

						pickres_id = itemres.pickId;
					}

					for (int i = 0; i < item.itemnum; ++i)
					{
						PickInitParam initParam = new PickInitParam();
						initParam.pick_type = picktype;
						initParam.pick_res_id = pickres_id;
						initParam.init_dir = dir;
						initParam.init_pos = pos;
						initParam.random_pos = randomPos;
						initParam.fly_type = flytype;

						paramList.Add(initParam);
					}
				}
			}
		}
		else
		{
			int pickres_id = resid;

			if(picktype == Pick.PickType.MONEY)
			{
				if(content < 1)
					return false;
			}
			else if (picktype == Pick.PickType.SUPER_WEAPON)
			{
				if (!DataManager.SuperWeaponTable.ContainsKey(content))
				{
					GameDebug.LogError("没找到超级武器 id = " + content.ToString());
					return false;
				}
			}
			else if (picktype == Pick.PickType.ITEM)
			{
				ItemTableItem itemres = ItemManager.GetItemRes(content);
				if (itemres == null)
					return false;

				pickres_id = itemres.pickId;
			}

			PickInitParam initParam = new PickInitParam();
			initParam.pick_type = picktype;
			initParam.pick_res_id = pickres_id;
			initParam.content = content;
			initParam.init_dir = dir;
			initParam.init_pos = pos;
			initParam.random_pos = randomPos;
			initParam.fly_type = flytype;

			paramList.Add(initParam);
		}

		return true;
	}

}