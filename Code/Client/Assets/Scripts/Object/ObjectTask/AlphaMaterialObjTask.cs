

/// <summary>
/// Alpha变化任务
/// </summary>
using UnityEngine;
public class AlphaMaterialObjTask : MaterialObjTask
{

    public float alpha = 0.5f;
    public AlphaMaterialObjTask(VisualObject owner, string name)
        :base(owner,name)
    {

    }

    public override void Start()
    {
        base.Start();

        ReActivate();
    }

    public override void Update()
    {
        if(mMaterials == null || mMaterials.Length == 0)
        {
            mMaterials = mOwner.ChangeMaterial(mtlname);
            ReActivate();
        }
    }

    public void ReActivate()
    {
        if (mMaterials != null)
        {
            foreach (Material mtl in mMaterials)
            {
                mtl.SetColor("_Color", new Color(1, 1, 1, alpha));
            }
        }
    }
}

