using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
public class SpriteAniParam
{
    public int fps = 0;
    public bool isLoop = false;
    public bool isSelfDestroy = false;
    public GameObject obj = null;
}

public class AnimationManager /*: Singleton<AnimationManager>*/
{
    private static AnimationManager instance = null;
    public AnimationManager()
	{
		instance = this;
	}
    public static AnimationManager Instance
	{
		get
		{
			return instance;
		}
	}

    private const string mAnimPath = "Animation/";

    /// <summary>
    /// 每一个动画名字对应的参数索引表;
    /// </summary>
    Dictionary<string, List<SpriteAniParam>> mAniDatas = new Dictionary<string, List<SpriteAniParam>>();


    private static uint msParticleCount = 0;

    private Dictionary<uint, ParticleAnimation> mParticleAnimations = new Dictionary<uint, ParticleAnimation>();

   // private Hashtable mEffects = new Hashtable();

    private List<uint> mDestroys = new List<uint>();
    public void DestroyParticleAnimation(ParticleAnimation ani)
    {
        if( ani != null )
        {
            ani.Destroy();
        }
    }
    public void Update()
    {
        IDictionaryEnumerator itr = mParticleAnimations.GetEnumerator();
        while (itr.MoveNext())
        {
            ParticleAnimation item = itr.Value as ParticleAnimation;

            uint key = Convert.ToUInt32(itr.Key);

            //检测特效是否播放完成
            if (item == null || item.IsDead())
            {
                //自然消失的特效放在这里，管理器将其销毁
                if (item != null)
                    mDestroys.Add(key);

            }
            else
            {
                item.OnUpdate();
            }
        }


//         for (int i = 0; i < mParticleAnimations.Keys.Count; ++i )
//         {
//             uint key = mParticleAnimations.Keys.ElementAt(0);
//             if (!mParticleAnimations.ContainsKey(key))
//                 continue;
//             
//             
//         }

        for (int i = 0; i < mDestroys.Count; ++i)
        {
            uint key = mDestroys[i];

            if( mParticleAnimations.ContainsKey(key) )
            {
                ParticleAnimation item = mParticleAnimations[key];

                mParticleAnimations.Remove(key);

                item.Destroy();
            }
        }

        mDestroys.Clear();
    }
    public ParticleAnimation PlayParticleAnimation(int id, GameObject parent = null, int depth = 0, int w = -1, int h = -1)
    {
        ParticleAnimation ani = CreateParticleAnimation(id, parent, w, h);

        if( ani == null )
        {
            return null;
        }
        ani.SetDepth(depth);
        uint insid = msParticleCount++;
        mParticleAnimations.Add(insid, ani);

        return ani;
    }
    //调用create接口 外界需要自行销毁
    public ParticleAnimation CreateParticleAnimation(int id, GameObject parent = null , int w = -1 , int h = -1)
    {
        if( !DataManager.UIEffectTable.ContainsKey(id) )
        {
            return null;
        }

        UIEffectTableItem item = DataManager.UIEffectTable[id] as UIEffectTableItem;
        if( item == null )
        {
            return null;
        }

        GameObject obj = new GameObject(item.particle);

        GameObject.DontDestroyOnLoad(obj);
        if( parent != null )
        {
            obj.transform.parent = parent.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            obj.layer = parent.layer;
        }
        UISprite sprite = obj.AddMissingComponent<UISprite>();

        if( w == -1 )
        {
            sprite.width = item.width;
        }else
        {
            sprite.width = w;
        }

        if (h == -1)
        {
            sprite.height = item.height;
        }
        else
        {
            sprite.height = h;
        }

        UIParticlePreview preview = new UIParticlePreview(item.Camerasize);
        preview.SetTargetSprite(sprite , false);
        preview.RotationY = 180;
        preview.SetupParticle(item.particle);

        ParticleAnimation animation = new ParticleAnimation(sprite , preview);

        return animation;
    }

    //添加一个 UNITY Animation动画   Art/Anim下
    public void AddAnim(string name , GameObject parent)
    {
        //test
        UIResourceManager.Instance.LoadAnimObject("TestAnimation", OnAnimLoaded);
    }

    private void OnAnimLoaded(GameObject obj)
    {

    }

    /// <summary>
    /// 添加一个帧动画在GameObject下;
    /// </summary>
    /// <param name="name">动画名</param>
    /// <param name="parent"></param>
    /// <param name="fps">frames per second</param>
    /// <param name="isLoop"></param>
    public UISpriteAnimation AddSpriteAnimation(string name , GameObject parent , int depth = 0 , int fps = 30 , bool isLoop = true , bool selfDestroy = false)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        if (parent == null) return null;

        GameObject go = System.Activator.CreateInstance<GameObject>() as GameObject;
        go.SetActive(false);
        go.transform.parent = parent.transform;
        go.name = name;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;


        UISprite gold = go.AddMissingComponent<UISprite>();
        //gold.atlas = UIAtlasHelper.LoadAtlas("SpriteAnimation/" + name);
        gold.spriteName = "";
        gold.depth = depth;
        gold.MakePixelPerfect();
        gold.width = 1;
        gold.height = 1;

        SpriteAniParam param = new SpriteAniParam();
        param.fps = fps;
        param.isLoop = isLoop;
        param.isSelfDestroy = selfDestroy;
        param.obj = go;

        UISpriteAnimation spriteAni = go.AddMissingComponent<UISpriteAnimation>();


        if( !mAniDatas.ContainsKey( name ) )
        {
            mAniDatas.Add( name , new List<SpriteAniParam>());
            mAniDatas[name].Add(param);

            UIResourceManager.Instance.LoadAnimationRes(name, OnAnimationResLoaded);
        }else
        {
            mAniDatas[name].Add(param);
        }

        return spriteAni;
    }

    private void OnAnimationResLoaded(string aniName)
    {
        if( !mAniDatas.ContainsKey( aniName )  )
        {
            return;
        }
        List<SpriteAniParam> lst = mAniDatas[aniName];

        for( int i = 0 ; i < lst.Count ; ++i )
        {
            SpriteAniParam param = lst[i];

            GameObject go = param.obj;

            UISprite gold = go.GetComponent<UISprite>();
            gold.atlas = UIAtlasHelper.LoadAtlas(aniName);
            gold.MakePixelPerfect();

            UISpriteAnimation spriteAni = go.GetComponent<UISpriteAnimation>();
            if (spriteAni == null)
            {
                GameDebug.LogError("怎么可能出错了");
                return;
            }

            spriteAni.framesPerSecond = param.fps;
            spriteAni.loop = param.isLoop;
            if (param.isSelfDestroy)
                spriteAni.onFinished += DestroyObj;

            spriteAni.RebuildSpriteList();
            spriteAni.Reset();
            go.SetActive(true);
        }

        mAniDatas.Remove(aniName);
    }

    static void DestroyObj(GameObject go)
    {
        if(go != null)
            GameObject.DestroyImmediate(go);
    }

    public static void DestroySpriteAni(string name , GameObject parent)
    {
        if (parent == null) return;

        Transform child = parent.transform.FindChild(name);
        if (child != null)
            GameObject.DestroyImmediate(child.gameObject);
    }

    public static void DestroySpriteAni(UISpriteAnimation ani)
    {
        if (ani == null) return;

        GameObject.DestroyImmediate(ani.gameObject);
    }
}
