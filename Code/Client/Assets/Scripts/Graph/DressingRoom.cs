using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 试衣间
/// </summary>
public class DressingRoom
{
    /// <summary>
    /// 将物体挂接到
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="obj"></param>
    public static void AttachObjectTo(Transform parent, Transform obj, TransformData transform)
    {
        if (parent == null || obj == null)
            return;

        obj.parent = parent;
        obj.localPosition = Vector3.zero;
        obj.localScale = Vector3.one;
		obj.localEulerAngles = Vector3.one;

        if (transform != null)
        {
            transform.Apply(obj.transform);
        }
    }

    /// <summary>
    /// 将挂接的物体拆分
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="obj"></param>
    public static void DetachObjectFrom(Transform parent, GameObject obj)
    {
        if(obj != null)
            obj.transform.parent = null;
    }

    public static void AttachParticleTo(ParticleItem item,Transform parent)
    {
        if (item.visual == null || item.visual.Visual == null)
            return;

        Transform visualTrans = item.visual.VisualTransform;
        item.parent = parent;
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
    }
}
