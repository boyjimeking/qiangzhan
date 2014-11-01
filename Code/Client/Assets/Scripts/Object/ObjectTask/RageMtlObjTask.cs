
/// <summary>
/// 狂暴状态
/// </summary>
using UnityEngine;
public class RageMtlObjTask : MaterialObjTask
{
    protected static readonly float begin = 0.5f;
    protected static readonly float end = 4.0f;
    protected float mRange = begin;

    public RageMtlObjTask(VisualObject owner,string name):
        base(owner,name)
    {

    }
  
    public override void Start()
    {
        base.Start();
    }
    public override void Update()
    {
        mRange += Time.deltaTime * 3;
        if (mRange > end)
            mRange = begin;
        if (mMaterials != null)
        {
            foreach(Material mtl in mMaterials)
            {
               float ranged =  mtl.GetFloat("_RimPower");
                mtl.SetFloat("_RimPower", mRange);
            }
        }


        base.Update();
    }
}

