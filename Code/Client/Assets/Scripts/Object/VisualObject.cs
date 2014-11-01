using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using FantasyEngine;

// 可视化对象
public class VisualObjectInitParam : ObjectInitParam
{
	
}
public abstract class VisualObject : ObjectBase
{
	// 当前的模型Id
	protected int mModelResID = -1;
    protected bool mHasShadow = false;

	// 模型
    protected MeshVisual mVisual;
	// 动作
    protected MecanimStateController mStateController = new MecanimStateController();


    public IdleStateDef IdleIndex = IdleStateDef.Rest;

    protected StringBuilder mTempAnimStr = new StringBuilder(256);

	// 挂件列表
    protected List<AttachMent> attachments = new List<AttachMent>();

    protected Material[] mOriginalMtl;
    protected Material[] mOriginalInstMtl;

    protected List<ObjectTaskBase> mVisualTasks = new List<ObjectTaskBase>();


    protected AttachMent[] mAttachMents = new AttachMent[(uint)AttachMountType.AttachCount];
    protected List<ParticleAttachMent> mAttachParticles = new List<ParticleAttachMent>();

    override public bool Init(ObjectInitParam param)
    {
        if (!base.Init(param))
            return false;

		InitModelID = -1;
        mHasShadow = false;
        return true;
    }

	// 初始模型ID.
	protected int InitModelID { get; private set; }

	/// <summary>
	/// 获取该对象的标识, 包含该单位的类型以及资源ID, 用于调试时使用.
	/// 如: "NPC: 1", 表示该对象是NPC, 资源ID是1.
	/// </summary>
	/// <returns></returns>
	public abstract string dbgGetIdentifier();

	public void LoadModel()
	{
		InitModelID = mModelResID;
		ChangeModel(mModelResID);
	}

    protected virtual void DestroyVisual()
    {
        if (mVisual != null)
        {
            mVisual.Destroy();
            mVisual = null;
        }
    }
    private void onVisualSucess()
    {
        if (mVisual == null || mVisual.IsDestroy )
            return;

        if(mHasShadow)
            mVisual.CreateShadow(1, 1, 1);

        onModelLoaded(mVisual.Visual);
    }

    protected void ShowShadow()
    {
        if (mVisual == null || mVisual.IsDestroy || !mHasShadow)
            return;
        mVisual.ShowShadow();
    }

    protected void HideShadow()
    {
        if (mVisual == null || mVisual.IsDestroy || !mHasShadow)
            return;
        mVisual.HideShadow();
    }

    private void onVisualFail()
    {
        GameDebug.LogError(dbgGetIdentifier() + " 角色模型加载失败");
    }
//     IEnumerator GenerateVisual(string filename)
//     {
// 		if(mVisual != null)
// 		{
// 		    mVisual.Destroy();
// 		    mVisual = null;
// 		}
//         mVisual = new MeshVisual();
//         mVisual.CreateWithConfig(filename,true);
// 
//         while (mVisual != null && !mVisual.IsCompleteOrDestroy)
//             yield return 1;
// 
//         if (mVisual == null || mVisual.IsDestroy)
//             yield break;
//         onModelLoaded(mVisual.Visual);
//     }

    virtual public void ApplyEquipConfig(int[] equipconfigs)
    {
//         Dictionary<string, string> parts = new Dictionary<string, string>();
//         for( int i = 0 ; i < equipconfigs.Count ; ++i )
//         {
//             int id = (int)equipconfigs[i];
//             if (id < 0)
//                 continue;
//             if (!DataManager.PartModelTable.ContainsKey(id))
//                 continue;
//             PartModelTableItem item = DataManager.PartModelTable[id] as PartModelTableItem;
//             parts.Add(item.solt, item.file);
//         }

        //todo;
    }

    virtual protected bool ChangeModel(int model)
    {
		if (model == -1)
			return false;

		if (!DataManager.ModelTable.ContainsKey(model))
			return false;

		ModelTableItem modelTab = DataManager.ModelTable[model] as ModelTableItem;

		if (modelTab == null)
		{
			GameDebug.LogError(dbgGetIdentifier() + " 没有找到模型 id = " + model.ToString());
			return false;
		}

		DestroyVisual();
		
		mModelResID = model;

		mVisual = new MeshVisual();
		mVisual.CreateWithConfig(modelTab.filename, onVisualSucess, onVisualFail, true);

		return true;
    }
    virtual public uint AddEffect(uint resId, string boneName, float dir = float.NaN,AttachMountType atype = AttachMountType.AttachCount )
    {
        if( this.Scene == null )
            return uint.MaxValue;
        if (!DataManager.EffectTable.ContainsKey(resId))
            return uint.MaxValue;

        EffectTableItem item = DataManager.EffectTable[resId] as EffectTableItem;
        SceneParticleManager mng = this.Scene.GetParticleManager();

        Transform trans = null;
        if (atype != AttachMountType.Wing)
        {
            if (!string.IsNullOrEmpty(boneName) && boneName.StartsWith("%"))
            {
                atype = AttachMountType.Weapon;
                boneName = boneName.Replace("%", "");
            }
        }
        TransformData data = new TransformData();
        data.notFollow = item.notFollow;
        data.Scale = new Vector3(item.scale, item.scale, item.scale);

		if (!float.IsNaN(dir))
			data.Rot = new Vector3(0f, dir * Mathf.Rad2Deg, 0f);

        if (!string.IsNullOrEmpty(item.soundId))
        {
			string[] array = item.soundId.Split('|');
            SoundManager.Instance.Play(
				int.Parse(array[UnityEngine.Random.Range(0, array.Length)]),
				item.soundDelay
				);
        }
       
        uint instID = mng.AddParticle(item.effect_name,item.loop, trans, data, item.limitry);
        ParticleAttachMent attach = new ParticleAttachMent();
        attach.parent = trans == null ? null : trans.gameObject;
        attach.particleid = instID;
        attach.transform = data;
        attach.socketname = boneName;
        attach.atype = atype;
        attach.resid = resId;

        AttachParticle(attach);

        return instID;
    }

    virtual public uint AddEffect(uint resId, Vector3 pos, float dir = float.NaN)
    {
        if (this.Scene == null)
            return uint.MaxValue;
        if (!DataManager.EffectTable.ContainsKey(resId))
            return uint.MaxValue;
        EffectTableItem item = DataManager.EffectTable[resId] as EffectTableItem;
        SceneParticleManager mng = this.Scene.GetParticleManager();

        TransformData data = new TransformData();
		data.Scale = new Vector3(item.scale, item.scale, item.scale);
        data.notFollow = item.notFollow;
        data.Pos = pos;
		if(!float.IsNaN(dir))
			data.Rot = new Vector3(0f, dir * Mathf.Rad2Deg, 0f);

		if (!string.IsNullOrEmpty(item.soundId))
		{
			string[] array = item.soundId.Split('|');
			SoundManager.Instance.Play(
				int.Parse(array[UnityEngine.Random.Range(0, array.Length)]),
				item.soundDelay
				);
		}

        uint instID = mng.PlayFx(item.effect_name, item.loop, null, data,-1,null,item.limitry);
        ParticleAttachMent attach = new ParticleAttachMent();
        attach.parent = null;
        attach.atype = AttachMountType.AttachCount;
        attach.particleid = instID;
        attach.resid = resId;

        AttachParticle(attach);

        return instID;
    }

	virtual public void RemoveEffect(uint instId)
    {
        if( this.Scene == null || instId == uint.MaxValue )
            return ;
        SceneParticleManager mng = this.Scene.GetParticleManager();
        mng.RemoveParticle(instId);
    }

    virtual public Transform GetBoneTransofrm(string bonename)
    {
        if (string.IsNullOrEmpty(bonename))
            return null;

        if (mVisual == null || mVisual.Visual == null)
            return null;

        Transform t = mVisual.GetBoneByName(bonename);

        if (t == null)
        {
            GameDebug.LogError(dbgGetIdentifier() + " 未在角色模型上找到骨骼" + bonename);
        }

        return t;
    }

	virtual public Vector3 GetBonePositionByName(string boneName)
	{
		if (string.IsNullOrEmpty(boneName))
			return Vector3.zero;

		Transform t = GetBoneTransofrm(boneName);

		return t != null ? t.position : Vector3.zero;
	}

	virtual protected void onModelLoaded(GameObject obj)
    {
		obj.layer = GetObjectLayer();


		if (mVisual != null && mVisual.Visual != null)
		{
			mVisual.Visual.tag = GetObjectTag();
		}

        if (DataManager.ModelTable.ContainsKey(mModelResID))
        {
            ModelTableItem modelTab = DataManager.ModelTable[mModelResID] as ModelTableItem;

            obj.transform.localScale = Vector3.one * modelTab.scale;
        }

        mStateController = new MecanimStateController();
        mStateController.Setup(mVisual.AnimManager, this);
    }

    public int ModelID
    {
       get
        {
            return mModelResID;
        }
    }

    public MecanimStateController GetStateController()
    {
        return mStateController;
    }
	virtual public int GetObjectLayer()
	{
		return (int)ObjectLayerType.ObjectLayerPlayer;
	}
    public virtual MovingType GetMovingType()
    {
        return MovingType.MoveType_Name;
    }

	virtual public string GetObjectTag()
	{
		return ObjectType.ObjectTagNone;
	}

    /// <summary>
    /// 挂接一个显示对象
    /// </summary>
    protected AttachMent AttachVisual(PrimitiveVisual visual,string socketname,TransformData trans)
    {
        AttachMent attach = new AttachMent();
        attach.socketname = socketname;
        attach.transform = trans;
        attach.visual = visual;

        Transform bone = mVisual.GetBoneByName(attach.socketname);
        if (bone == null)
        {
            bone = mVisual.VisualTransform;
        }
        DressingRoom.AttachObjectTo(bone, attach.visual.VisualTransform, attach.transform);
       // BehaviourUtil.StartCoroutine(WaitForAttachComplete(attach));

        return attach;
    }

    public void DetachVisual(AttachMent attach)
    {
        if (attach == null)
            return;
        if(attach.visual != null)
        {
            Transform trans = null;
            if (attach.parent != null)
                trans = attach.parent.transform;
            DressingRoom.DetachObjectFrom(trans, attach.visual.Visual);
        }
        attachments.Remove(attach);
    }
    virtual public string CombineAnimname(string aniName)
    {
        if (string.IsNullOrEmpty(aniName))
            return aniName;
        if (aniName[0]== '%')
        {
            mTempAnimStr.Length = 0;
            mTempAnimStr.AppendFormat("Base Layer.{0}", aniName);
            return mTempAnimStr.ToString();
        }
		return aniName;
    }

    virtual protected void refreshAnimation()
    {

    }

    virtual public float GetAnimAngle()
    {
        return this.GetDirection() + Mathf.PI / 2;
    }

    override public bool Update(uint elapsed)
    {
		if(!base.Update(elapsed))
		{
			return false;
		}

        if( mVisual != null && mVisual.Visual )
        {
            //pos
            Transform tr = mVisual.VisualTransform;
            tr.position = GetPosition();

            //dir
            Quaternion rot = tr.rotation;
            rot.eulerAngles = new Vector3(rot.eulerAngles.x, GetDirection() * Mathf.Rad2Deg, rot.eulerAngles.z);
            tr.rotation = rot;
        }
       // if (mStateController != null)
         //   mStateController.Update(elapsed);

        UpdateVisualTask();
        UpdateAttachParticle();
        UpdateMecanim(elapsed);
		return true;
    }

    public override bool UpdateDestroy(uint elapsed)
    {
        //if (mDeadMtl != null)
        //{
        //    f_deadTime += (float)elapsed / 10000;
        //    if (f_deadTime >= 1.5f)
        //        f_deadTime = 1.5f;

        //    foreach (Material mtl in mDeadMtl)
        //    {
        //        mtl.SetFloat("_Amount", f_deadTime);
        //    }
        //}

        UpdateVisualTask();

        return base.UpdateDestroy(elapsed);
    }

    protected void UpdateMecanim(uint elapsed)
    {
        if (mStateController != null && mVisual != null && mVisual.AnimManager != null && mStateController.AnimSet == mVisual.AnimManager.Property)
            mStateController.Update(elapsed);
    }

    protected override void OnChangePosition(Vector3 oldPos, Vector3 curPos)
    {
        if( mVisual != null && mVisual.Visual != null && oldPos != curPos )
        {
            Transform tr = mVisual.VisualTransform;
            tr.position = curPos;
        }
        base.OnChangePosition(oldPos, curPos);
    }

    protected override void OnChangeDirection(float oldDir, float curDir)
    {
        if (mVisual != null && mVisual.Visual != null)
        {
            Transform tr = mVisual.VisualTransform;
            Quaternion rot = tr.rotation;
            rot.eulerAngles = new Vector3(rot.eulerAngles.x, curDir * Mathf.Rad2Deg, rot.eulerAngles.z);
            tr.rotation = rot;
        }
        base.OnChangeDirection(oldDir, curDir);
    }

    public virtual void RecieveMecAnimEvent(MecanimEvent eve)
    {

    }

    /// <summary>
    /// 获得移动状态
    /// </summary>
    /// <returns></returns>
    public virtual uint GetMoveState()
    {
        return 0;
    }

	override public void Destroy()
	{
        //清除挂接的特效的关联，特效就不会随着物体的删除而删除
        foreach (ParticleAttachMent attach in mAttachParticles)
        {
            if (attach != null)
            {
                attach.parent = null;
                ParticleItem item = SceneManager.Instance.GetCurScene().GetParticleManager().GetParticle(attach.particleid);
                if (item != null && item.visual != null && item.visual.Visual != null)
                {
                    item.visual.VisualTransform.parent = null;

                    ParticleVisual.DestroyParticle(item.visual);

                }

            }
        }

        DestroyVisual();

        mStateController = null;

        mVisualTasks = null;
//         if(mVisual != null)
//         {
//             mVisual.Destroy();
//             mVisual = null;
//         }
        base.Destroy();
	}

    public override void OnEnterScene(BaseScene scene, uint instanceid)
    {
        base.OnEnterScene(scene, instanceid);

        LoadModel();
    }

    public Material[] OriginalMtl
    {
        get
        {
            //这里可能会出现问题。在制作MeshVisual时材质可能会有可能不全
            if (mOriginalMtl != null)
            {
                return mOriginalMtl;

            }
            if(mVisual != null && mVisual.GetRenderer() != null)
            {
                mOriginalMtl = mVisual.GetRenderer().sharedMaterials;
                return mOriginalMtl;
            }
            return null;

        }
    }
    public Material[] OriginalInstMtl
    {
        get
        {
            //这里可能会出现问题。在制作MeshVisual时材质可能会有可能不全
            if (mOriginalInstMtl != null)
            {
                return mOriginalInstMtl;

            }
            if (mVisual != null && mVisual.GetRenderer() != null)
            {
                mOriginalInstMtl = mVisual.GetRenderer().materials;
                return mOriginalInstMtl;
            }
            return null;

        }
    }


    public void SetDeathMaterial(string name)
    {
        Material mtl = CustomMaterialManager.Instance.GetMaterial(name);
        Renderer renderer = mVisual.GetRenderer();

        if (mtl == null || renderer == null)
            return;

        MaterialObjTask mtlTask = null;
        switch(name)
        {
            case "burn_out":
            case "poison":
                mtlTask = new BurnMaterialObjTask(this,name);
                break;

            default:
                mtlTask = new MaterialObjTask(this, name);
                break;
        }

        AddMaterialTask(mtlTask);
       // f_deadTime = 0;

       // mDeadMtl = new List<Material>();
       //for(int i=0; i < renderer.sharedMaterials.Length; ++i)
       //{
       //   Material originalmtl = renderer.sharedMaterials[i];

       //  Material mtlInstance = Object.Instantiate(mtl) as Material;
       //  mtlInstance.mainTexture = originalmtl.mainTexture;

       //  mtlInstance.SetFloat("_startTime", Time.timeSinceLevelLoad);
       //  mtlInstance.SetVector("_worldOrigin", mVisual.Visual.transform.position);
       //  mtlInstance.SetFloat("_maxHeight", renderer.bounds.extents.y);

       //  mDeadMtl.Add(mtlInstance);
       //}

       //mOriginalMtl = renderer.sharedMaterials;
       //renderer.materials = mDeadMtl.ToArray();

    }
    public void RemoveDeathMaterial()
    {
        List<ObjectTaskBase> taskList = new List<ObjectTaskBase>();
        foreach (ObjectTaskBase tk in mVisualTasks)
        {
            if (tk is MaterialObjTask)
                taskList.Add(tk);
        }

        foreach (ObjectTaskBase tk in taskList)
        {
            tk.Destroy();
            mVisualTasks.Remove(tk);
        }
    }

    public void ChangeColor(Color color)
    {
        if (mVisual != null && mVisual.GetRenderer() != null)
        {
            Renderer renderer = mVisual.GetRenderer();
            foreach (Material mtl in renderer.materials)
            {
                mtl.SetColor("_Color", color);
            }
        }
    }

    public void ChangeAlpha(float alpha)
    {
        bool removetask = alpha >= 1;

        AlphaMaterialObjTask task = GetObjTask<AlphaMaterialObjTask>();
        if (task != null)
        {
            if(removetask)
            {
                task.Destroy();
                RemoveVisualTask(task);
                return;
            }

            task.alpha = alpha;
            task.ReActivate();


            return;
        }

        if (removetask)
            return;

        task = new AlphaMaterialObjTask(this,"transparent");
        task.alpha = alpha;
        AddMaterialTask(task);

    }

    public Material[] ChangeMaterial(string name)
    {
        List<Material> newMtl = new List<Material>();
        if (mVisual == null)
            return newMtl.ToArray();
        Material mtl = CustomMaterialManager.Instance.GetMaterial(name);
        Renderer renderer = mVisual.GetRenderer();

        if (mtl == null || renderer == null)
            return newMtl.ToArray();

        if(mOriginalMtl == null)
            mOriginalMtl = renderer.sharedMaterials;

        for (int i = 0; i < mOriginalMtl.Length; ++i)
        {
            Material originalmtl = mOriginalMtl[i];
            if (originalmtl == null)
                return newMtl.ToArray();

            Material mtlInstance = Object.Instantiate(mtl) as Material;
            mtlInstance.mainTexture = originalmtl.mainTexture;

            newMtl.Add(mtlInstance);
        }
        Material[] mtlArray =  newMtl.ToArray();

        renderer.materials = mtlArray;

        return renderer.materials;
    }
    public void RevertMaterial()
    {
        Renderer renderer = mVisual.GetRenderer();

        if (renderer == null)
            return;

        if (mOriginalMtl != null && renderer.materials != mOriginalMtl)
            renderer.materials = mOriginalMtl;
    }

    public void AddVisualTask(ObjectTaskBase task)
    {
        if (task == null)
            return;
        if (mVisualTasks.IndexOf(task) >= 0)
            return;
        task.Start();
        mVisualTasks.Add(task);
    }
    public void RemoveVisualTask(ObjectTaskBase task)
    {
        if (task == null)
            return;
        mVisualTasks.Remove(task);
    }

    public void UpdateVisualTask()
    {
        if (mVisualTasks == null)
        {
            return;
        }
        for (int i = 0; i < mVisualTasks.Count; ++i )
        {
            ObjectTaskBase task = mVisualTasks[i];
            task.Update();

        }
// 
//             foreach (ObjectTaskBase task in mVisualTasks)
//             {
//                 task.Update();
//             }

    }
    public void AddMaterialTask(MaterialObjTask task)
    {
        List<ObjectTaskBase> taskList = new List<ObjectTaskBase>();
        foreach(ObjectTaskBase tk in mVisualTasks)
        {
            if (tk is MaterialObjTask)
                taskList.Add(tk);
        }

        foreach(ObjectTaskBase tk in taskList)
        {
            tk.Destroy();
            mVisualTasks.Remove(tk);
        }

        AddVisualTask(task);
    }

    public T GetObjTask<T>() where T : ObjectTaskBase
    {
        foreach (ObjectTaskBase tk in mVisualTasks)
        {
            if (tk is T)
                return (T)tk;
        }
        return null;
    }



    //-----------------------------特效---------------------------------------

    public virtual void AttachParticle(ParticleAttachMent attach)
    {
        mAttachParticles.Add(attach);
    }
    /// <summary>
    /// 更新挂接特效
    /// </summary>
    public void UpdateAttachParticle()
    {
        SceneParticleManager particlemng = SceneManager.Instance.GetCurScene().GetParticleManager();
        int nCount = mAttachParticles.Count;
        List<ParticleAttachMent> toDel = null;
        for (int i = 0; i < nCount; ++i)
        {
            ParticleAttachMent attach = mAttachParticles[i];
            ParticleItem item = particlemng.GetParticle(attach.particleid);

            if (attach == null || item == null || item.IsDead())
            {
                if (toDel == null)
                    toDel = new List<ParticleAttachMent>();
                toDel.Add(attach);
                continue;

            }
            //将特效更新到对应位置上
            if (attach.parent == null || item.parent == null)
            {
                PrimitiveVisual aVisual = null;
                if (attach.atype != AttachMountType.AttachCount)
                {
                    AttachMent buildinAttach = mAttachMents[(int)attach.atype];
                    if (buildinAttach != null)
                        aVisual = buildinAttach.visual;

                }
                else
                {
                    aVisual = mVisual;
                }
                if (aVisual != null && aVisual is MeshVisual && aVisual.Visual != null)
                {
                    Transform tr = null;
                    if (string.IsNullOrEmpty(attach.socketname))
                    {
                        tr = aVisual.VisualTransform;
                    }
                    else
                    {
                        tr = (aVisual as MeshVisual).GetBoneByName(attach.socketname);
                        if (tr == null)
                            tr = aVisual.VisualTransform;
                    }

                    attach.parent = tr.gameObject;


                    EffectTableItem effectitem = DataManager.EffectTable[attach.resid] as EffectTableItem;

                    //不跟随释放者的特效，取挂点的方向
                    if (effectitem.notFollow && tr != null && attach.transform != null)
                    {
                        if (tr != null)
                            attach.transform.Rot = tr.rotation.eulerAngles;
                        else
                            attach.transform.Rot = Vector3.zero;
                    }
                }

                if (attach.parent != null)
                {
                    if (item.visual != null && item.visual.Visual != null)
                        item.visual.Visual.SetActive(true);
                    DressingRoom.AttachParticleTo(item, attach.parent.transform);
                }
            }

        }

        if (toDel != null)
        {
            foreach (ParticleAttachMent at in toDel)
            {
                mAttachParticles.Remove(at);
            }
        }
    }

}