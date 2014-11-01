using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 变换信息
/// </summary>
public class TransformData
{
	public enum ModifyFlag : uint
	{
		Position,
		Rotation,
		Scale,
		Count,
	}

    private BitArray modifyflag = new BitArray((int)ModifyFlag.Count);
    private Vector3 pos = Vector3.zero;
    private Vector3 rot = Vector3.zero;
    private Vector3 scale = Vector3.one;

    //是否跟随绑点
    public bool notFollow = false;

    public Vector3 Pos
    {
        set
        {
			SetModified(ModifyFlag.Position);
            pos = value;
        }
    }
    public Vector3 Rot
    {
        set
        {
			SetModified(ModifyFlag.Rotation);
            rot = value;
        }
    }
    public Vector3 Scale
    {
        set
        {
			SetModified(ModifyFlag.Scale);
            scale = value;
        }
        get
        {
            return scale;
        }
    }

    public void Apply(Transform trans)
    {
        if (trans == null)
            return;
        if (IsModified(ModifyFlag.Position))
            trans.localPosition = pos;
        if (IsModified(ModifyFlag.Rotation))
            trans.localEulerAngles = rot;
        if (IsModified(ModifyFlag.Scale))
            trans.localScale = scale;
    }
    public void CopyTrans(Transform trans)
    {
        if (trans == null)
            return;
        Pos = trans.localPosition;
        Scale = trans.localScale;
        Rot = trans.localEulerAngles;
    }

	void SetModified(ModifyFlag pos) {
		modifyflag.Set((int)pos, true);
	}

	public bool IsModified(ModifyFlag pos) {
		return modifyflag.Get((int)pos);
	}
}
