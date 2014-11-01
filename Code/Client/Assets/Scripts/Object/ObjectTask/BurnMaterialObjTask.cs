using UnityEngine;

/// <summary>
/// 燃尽的材质任务
/// </summary>
public class BurnMaterialObjTask : MaterialObjTask
{
    private float f_deadTime = 0.0f;
    public BurnMaterialObjTask(VisualObject owner,string name)
        :base(owner,name)
    {

    }

    public override void Start()
    {
        base.Start();

        if (mMaterials != null)
        {
            foreach (Material mtl in mMaterials)
            {
                mtl.SetFloat("_startTime",0);
            }
        }
    }

    public override void Update()
    {
        if (mMaterials != null)
        {
            f_deadTime += Time.deltaTime * 0.2f;
            if (f_deadTime >= 1.5f)
                f_deadTime = 1.5f;

            foreach (Material mtl in mMaterials)
            {
               mtl.SetFloat("_Amount", f_deadTime);
            }
        }

        base.Update();
    }
}

