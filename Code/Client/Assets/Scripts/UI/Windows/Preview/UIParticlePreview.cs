using FantasyEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 角色预览窗口
/// </summary>
public class UIParticlePreview : UIPreviewBase
{
    public ParticleItem item;

    private GameObject mGameObject;

    public UIParticlePreview(float Camerasize = 0.5f)
        :base()
    {
        mOrthograhicsize = Camerasize;     
    }
    public void RePlay()
    {
        if( item != null && item.visual != null )
        {
            item.visual.Replay();
        }
    }
    public bool IsDead()
    {
        if (item != null && item.visual != null)
        {
            return item.IsDead();
        }
        return false;
    }

    public bool SetupParticle(string effectName)
    {
        item = new ParticleItem();
        item.localTrans = null;
        item.parent = null;
        item.Alias = null;
        item.LimitRy = false;
        item.Loop = false;

        BehaviourUtil.StartCoroutine(GenerateVisual(effectName, item));
        return true;
    }

    private IEnumerator GenerateVisual(string effectName, ParticleItem item)
    {
        item.visual = ParticleVisual.CreateParticle(effectName, null);
        item.Layer = item.Layer;
        if (item.localTrans != null)
        {
            if (item.visual == null)
            {
                yield break;
            }
            item.visual.Scale = item.localTrans.Scale.x;
        }

        while (!item.visual.IsCompleteOrDestroy)
            yield return 2;


        if (item.visual.IsDestroy)
        {
            item = null;
            yield break;
        }

        item.init = true;
        //特效创建完毕之后

        Transform visualTrans = item.visual.VisualTransform;


        if (item.parent != null)
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

        if (item.localTrans != null)
        {
            if (item.localTrans.notFollow && item.parent != null)
            {
                if (!item.localTrans.IsModified(TransformData.ModifyFlag.Position))
                    item.localTrans.Pos = item.parent.position;

                if (!item.localTrans.IsModified(TransformData.ModifyFlag.Rotation))
                    item.localTrans.Rot = item.parent.eulerAngles;
            }

            item.localTrans.Apply(visualTrans);
            item.visual.Scale = item.localTrans.Scale.x;//缩放只有个值

        }
        item.visual.LimitRotate = item.LimitRy;

        OnParticleSucess(item.visual);
    }

    private void OnParticleSucess(ParticleVisual instance)
    {
        mGameObject = instance.Visual;
        mGameObject.transform.parent = mPreviewRoot.transform;
        mGameObject.transform.localPosition = new Vector3(0, 0, 0);
        ObjectCommon.SetObjectAndChildrenLayer(mGameObject, layermask);
    }

    public override void Destroy()
    {
        if (item != null && item.visual != null)
        {
            ParticleVisual.DestroyParticle(item.visual);
        }
        item = null;
        base.Destroy();
    }

    public override void Update()
    {
        if( item != null )
        {
            item.OnUpdate();
        }
    }
}

