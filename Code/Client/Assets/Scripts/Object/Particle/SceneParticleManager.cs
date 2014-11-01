using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using FantasyEngine;
using System.Linq;
//public enum FxLifeType
//{
//    FXLT_RUN_ITS_COURSE,//自动销毁特效
//    RUN_CONTROL_BY_OWNER,//由外界控制特效
//}

/// <summary>
/// 特效
/// </summary>
public class ParticleItem
{
	private string mAlias;

    private int mLayer = 0;
    private bool mLimitRy = false;

    public Transform parent;//挂接到的父节点
    public TransformData localTrans;//自身变化属性
    public ParticleVisual visual;

    public bool init = false;
    public float CustomLifetime = -1f;
    public float elapsedTime = 0;
    private bool mLoop = false;//是否循环

    public ParticleItem()
    {
    }
	public string Alias
	{
		get
		{
			return mAlias;
		}
		set
		{
			mAlias = value;
		}
	}
    public float LifeTime
    {
        get
        {
            if(CustomLifetime > 0)
                return CustomLifetime;
            if (visual == null)
                return 0;
            return visual.LifeTime;
        }
    }
    public int Layer
    {
        get
        {
            return mLayer;
        }
        set
        {
            mLayer = value;
            if (visual != null)
                visual.Layer = mLayer;
        }
    }

    public bool LimitRy
    { 
        get
        {
            return mLimitRy;
        }
        set
        {
            mLimitRy = value;
            if (visual != null)
                visual.LimitRotate = mLimitRy;
        }
    }

    public void OnUpdate()
    {
        if(elapsedTime < LifeTime)
            elapsedTime += Time.deltaTime;
        if(Loop && visual != null && !visual.Loop && elapsedTime > visual.LifeTime)
        {
            elapsedTime = 0;
            try
            {
                visual.Replay();
            }
            catch
            {
                Debug.LogError(visual.ParticleName + "出问题了！！！！");
            }
        }
    }
    public bool IsDead()
    {
        if (!init)
            return false;
        if (visual != null && Loop)
            return false;
        return elapsedTime >= LifeTime;
    }

    public bool IsLoop()
    {
        if (!init)
            return false;
        if (visual != null)
            return visual.Loop;
        return false;
    }
    public bool Loop
    {
        get
        {
            return mLoop;
        }
        set
        {
            mLoop = value;
        }
    }

}

/// <summary>
/// 场景特效管理器
/// </summary>
public class SceneParticleManager
{
    private Dictionary<uint, ParticleItem> mEffects = new Dictionary<uint, ParticleItem>();

    private List<uint> mDestroys = new List<uint>();

    private static uint mLastEffectID = 0;
    public SceneParticleManager()
    {

    }

    /// <summary>
    /// 播放一个特效
    /// </summary>
    /// <param name="fxname"></param>
    /// <param name="loop"></param>
    /// <param name="fxtype"></param>
    /// <param name="parent"></param>
    /// <param name="trans"></param>
    /// <param name="lifetime"></param>
    /// <param name="alias"></param>
    /// <param name="limitry"></param>
    /// <returns></returns>
    public uint PlayFx(string fxname, bool loop,Transform parent = null, TransformData trans = null, float lifetime = -1, string alias = null, bool limitry = false)
    {
        ParticleData data = ParticleConfigManager.Instance.GetParticleData(fxname);
        if (data == null)
		{
			GameDebug.LogError("没有找到特效: " + fxname);
			return uint.MaxValue;
		}

		uint instID = mLastEffectID++;
        ParticleItem item = new ParticleItem();
        item.localTrans = trans;
        item.parent = parent;
		item.Alias = alias;
        item.LimitRy = limitry;
        item.Loop = loop;
        if (mEffects.ContainsKey(instID))
            mEffects.Remove(instID);
        mEffects.Add(instID, item);

        BehaviourUtil.StartCoroutine(GenerateVisual(instID, fxname, item));
        return instID;

    }
    /// <summary>
    /// 角色的某根骨骼上挂特效
    /// </summary>
    /// <param name="fxname"></param>
    /// <param name="boneName"></param>
    /// <param name="owner"></param>
    /// <param name="trans"></param>
    /// <returns></returns>
    public uint AddParticle(string fxname, bool loop, Transform parent, TransformData trans, bool limitry = false)
    {
        return PlayFx(fxname, loop, parent, trans, -1, null, limitry);
    }

    public ParticleItem GetParticle(uint instID)
    {
        if( !mEffects.ContainsKey(instID) )
        {
            return null;
        }
       return mEffects[instID] as ParticleItem;
    }
    public void RemoveParticle(uint instID)
    {
        if (!mEffects.ContainsKey(instID))
            return;
        ParticleItem item = mEffects[instID] as ParticleItem;

        mDestroys.Add(instID);
    }
	public void RemoveParticleByAlias(string alias)
	{
        IDictionaryEnumerator itr = mEffects.GetEnumerator();
        while (itr.MoveNext())
        {
            ParticleItem item = itr.Value as ParticleItem;
            if (item == null || string.Equals(item.Alias, alias))
            {
                mDestroys.Add((uint)itr.Key);
            }
        }
//         for( int i = 0 ; i < mEffects.Keys.Count ; ++i )
//         {
//             uint key = mEffects.Keys.ElementAt(i);
//             if( !mEffects.ContainsKey(key) )
//             {
//                 continue;
//             }
//             ParticleItem item = mEffects[key];
//             if (item == null || string.Equals(item.Alias, alias))
//             {
//                 mDestroys.Add(key);
//             }
//         }
	}
    private IEnumerator GenerateVisual(uint instId , string effectName, ParticleItem item)
    {
        item.visual = ParticleVisual.CreateParticle(effectName,null);
        item.Layer = item.Layer;
        if( item.localTrans != null )
        {
            if(item.visual == null)
            {
                yield break;
            }
            item.visual.Scale = item.localTrans.Scale.x;
        }

        //5s钟加载不出来的特效放弃
        float timeElapsed = 0;
        while (!item.visual.IsCompleteOrDestroy && timeElapsed < 5)
        {
            timeElapsed += Time.fixedDeltaTime;
            yield return 2;
        }


        if (!item.visual.IsCompleteOrDestroy)
        {
            //Debug.Log("就是加载不出来" + effectName);

            mDestroys.Add(instId);
            yield break;
        }

        item.init = true;
        //特效创建完毕之后

        Transform visualTrans = item.visual.VisualTransform;


        if(item.parent != null )
        {
            //判定effect.txt 里是否设置 不跟随
            if (item.localTrans == null || (item.localTrans != null && !item.localTrans.notFollow))
            {
                visualTrans.parent = item.parent;
                visualTrans.localPosition = Vector3.zero;
                visualTrans.localEulerAngles = Vector3.zero;
                visualTrans.localScale = Vector3.one;
            }
        }

        if(item.localTrans != null)
        {
            if (item.localTrans.notFollow && item.parent != null)
            {
				if (!item.localTrans.IsModified(TransformData.ModifyFlag.Position))
					item.localTrans.Pos = item.parent.position;

				if(!item.localTrans.IsModified(TransformData.ModifyFlag.Rotation))
				 item.localTrans.Rot = item.parent.eulerAngles;
            }

            item.localTrans.Apply(visualTrans);
            item.visual.Scale = item.localTrans.Scale.x;//缩放只有个值

        }
        item.visual.LimitRotate = item.LimitRy;
    }

    public void Update()
    {
//         for (int i = 0; i < mEffects.Keys.Count; ++i)
//         {
//             uint key = mEffects.Keys.ElementAt(i);
//             if (!mEffects.ContainsKey(key))
//             {
//                 continue;
//             }
// 
//         }
        IDictionaryEnumerator itr = mEffects.GetEnumerator();
        while( itr.MoveNext() )
        {
            ParticleItem item = itr.Value as ParticleItem;
            if (item != null)
            {
                //检测特效是否播放完成
                if (item == null || item.IsDead())
                {
                    //自然消失的特效放在这里，管理器将其销毁
                    if (item != null)
                        mDestroys.Add((uint)itr.Key);
                }
                else
                {
                    item.OnUpdate();
                }
            }
        }
        for (int i = 0; i < mDestroys.Count; ++i)
        {
            uint key = (uint)mDestroys[i];

            if( mEffects.ContainsKey( key ) )
            {
                ParticleItem item = mEffects[key] as ParticleItem;
                if (item.visual != null)
                {
                    ParticleVisual.DestroyParticle(item.visual);
                }
                mEffects.Remove(key);
            }
        }

        mDestroys.Clear();
    }

    public void Destroy()
    {

    }
}
