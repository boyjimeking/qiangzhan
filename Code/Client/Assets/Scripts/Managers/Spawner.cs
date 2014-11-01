using UnityEngine;
using System.Collections;

//对象构建缓存器

public class ObjectCache {
    //缓存的预设
    public GameObject mPrefab = null;
	private int mCount = 0;

    private ArrayList caches = new ArrayList();

    public GameObject MallocObject()
    {
        GameObject obj = null;
        if( caches.Count > 0 )
        {
            obj = caches[0] as GameObject;
            obj.SetActive(true);
            caches.RemoveAt(0);
            return obj;
        }
        obj = (GameObject)UnityEngine.Object.Instantiate(mPrefab);
        return obj;
    }

    public void FreeObject(GameObject obj)
    {
        obj.SetActive(false);
        caches.Add(obj);
    }
}

public class Spawner : Singleton<Spawner>
{
    public Hashtable mCaches = new Hashtable();
    public Hashtable mCachedObjects = new Hashtable();

    public static GameObject Spawn(string prefabName)
    {
        Object res = Resources.Load(prefabName);
        if( res != null )
        {
            return Spawn((res as GameObject), Vector3.zero, Quaternion.identity);
        }
        return null;
    }
    public static GameObject Spawn (GameObject prefab, Vector3 position, Quaternion rotation)  {

        if( prefab == null )
            return null;

        Spawner spawner = Spawner.Instance;
        ObjectCache cache = null;
        if( spawner != null )
        {
            if( !spawner.mCaches.ContainsKey( prefab ) )
            {
                cache = new ObjectCache();
                cache.mPrefab = prefab;
                spawner.mCaches.Add( prefab , cache );
            }
        }

        if( cache == null )
        {
            return null;
        }

        GameObject obj = cache.MallocObject();

        if( obj == null )
        {
            return null;
        }
        obj.transform.position = position;
	    obj.transform.rotation = rotation;
        obj.SetActive (true);

        spawner.mCachedObjects.Add( obj , cache );

	    return obj;
    }
    public static void Destroy (GameObject objectToDestroy) {
        Spawner spawner = Spawner.Instance;
	if ((spawner != null) && spawner.mCachedObjects.ContainsKey (objectToDestroy)) {
        ObjectCache cache = spawner.mCachedObjects[objectToDestroy] as ObjectCache;
        cache.FreeObject(objectToDestroy);
        spawner.mCachedObjects.Remove(objectToDestroy);
	}
	else {
        UnityEngine.Object.Destroy(objectToDestroy);
	}
}
}
