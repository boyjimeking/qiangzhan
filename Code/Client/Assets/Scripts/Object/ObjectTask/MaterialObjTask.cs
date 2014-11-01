using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 材质任务处理
/// </summary>
public class MaterialObjTask : ObjectTaskBase
{
    public Material[] mMaterials;
    public string mtlname;

    public MaterialObjTask(VisualObject owner,string name):base(owner)
    {
        mtlname = name;
    }
    public override void Start()
    {
        mMaterials = mOwner.ChangeMaterial(mtlname);
    }

    public override void Destroy()
    {
        mOwner.RevertMaterial();
        base.Destroy();
    }
}

