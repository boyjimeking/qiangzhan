using FantasyEngine;
using System;
using UnityEngine;

/// <summary>
/// 可挂接的物体
/// </summary>
public class AttachMent
{
    public AttachMountType atype = AttachMountType.AttachCount;

    public PrimitiveVisual  visual;
    public GameObject parent;
    public string socketname;//绑点名称

    public TransformData transform;
}

/// <summary>
/// 可挂接的特效
/// </summary>
public class ParticleAttachMent : AttachMent
{
    public uint particleid;
    public uint resid;
    public ParticleItem item;
}

