using UnityEngine;
using System.Collections;

//各单元基类
public class ObjectInitParam
{
    public Vector3 init_pos = Vector3.zero;
    public float init_dir = 0.0f;
	public string alias = "";
	// 形状参数
	public SceneShapeParam init_shape = null;
}
public abstract class ObjectBase  
{
    private Vector3 mInitPos = Vector3.zero;
    private float mInitDir = 0.0f;

	public uint mInstanceID = uint.MaxValue;
    public BaseScene mScene = null;

    protected bool mDestroyWaiting = false;

    //方向 弧度
    protected float mDir = 0;

    //位置
    protected Vector3 mPosition = Vector3.zero;

    //格子
    protected Cell mCell = null;

    //别名
    protected string mAlias = null;

	// 初始形状参数
	protected SceneShapeParam mShapeParam = null;

    private bool mDestoryed = false;

    private SceneShape mShape = null;
    private SceneShape mTransformShape = null;

    private bool mDisappear = false;

    virtual public bool Init( ObjectInitParam param )
    {
        mInitPos = param.init_pos;
        mInitDir = param.init_dir;
		mAlias = param.alias;

        this.SetPosition(mInitPos);
        this.SetDirection(mInitDir);

		mShapeParam = param.init_shape;

        mDestoryed = false;
        mDisappear = false;

        return true;
    }
	public uint InstanceID
	{
		get{
			return mInstanceID;
		}
		set{
			mInstanceID = value;
		}
	}
    public BaseScene Scene
    {
        get
        {
            return mScene;
        }
        set
        {
            mScene = value;
        }
    }
    virtual public int Type
    {
        get
        {
            return ObjectType.OBJ_INVAILD;
        }
    }
    public virtual bool UpdateDestroy(uint elapsed)
    {
        return false;
    }
	public virtual bool Update(uint elapsed)
	{
        if (mDisappear)
            return false;

		return true;
	}

	public virtual void Destroy()
	{
        mDestoryed = true;
	}

    public bool IsDestory()
    {
        return this.mDestoryed;
    }

    public virtual void SetPosition3f(Vector3f pos)
    {
        SetPosition(new Vector3(pos.x, pos.y, pos.z));
    }
    public Vector3f GetPosition3f()
    {
        return new Vector3f(mPosition.x, mPosition.y, mPosition.z);
    }
    protected virtual void OnChangePosition(Vector3 oldPos, Vector3 curPos)
    {
        if(mScene != null)
        {
            mScene.OnSpriteChangePosition(this, oldPos, curPos);
        }
    }
    public void SetDirection(float rad)
    {
        float oldDir = mDir;

        mDir = rad;
        
        OnChangeDirection(oldDir, mDir);
    }

    public float GetDirection()
    {
        return mDir;
    }

    protected virtual void OnChangeDirection(float oldDir, float curDir)
    {
    }

    public virtual void SetPosition(Vector3 pos)
    {
        if (mPosition.x != pos.x || mPosition.y != pos.y || mPosition.z != pos.z)
        {
            Vector3 oldPos = new Vector3(mPosition.x, mPosition.y, mPosition.z);

            mPosition.x = pos.x;
            mPosition.y = pos.y;
            mPosition.z = pos.z;

            OnChangePosition(oldPos, mPosition);
        }
    }
    public virtual Vector3 GetPosition()
    {
        return mPosition;
    }
    public virtual Vector3 GetInitPos()
    {
        return mInitPos;
    }
    public virtual float GetInitDir()
    {
        return mInitDir;
    }

    public virtual Cell GetCell()
    {
        return mCell;
    }

    public virtual void SetCell(Cell cell)
    {
        mCell = cell;
    }

    public void SetAlias(string alias)
    {
        mAlias = alias;
    }

    public string GetAlias()
    {
        return mAlias;
    }

    public bool IsDestroyWaiting()
    {
        return mDestroyWaiting;
    }

    public virtual void OnEnterScene(BaseScene scene, uint instanceid)
    {
        mScene = scene;
        //mInstanceID = instanceid;
    }

	// 获取半径.
	public virtual float GetRadius() { return 0.5f; }

	// 得到物体形状
	public SceneShape GetShape()
	{
		if(mShapeParam == null)
		{
			mShapeParam = new SceneShapeParam();
			mShapeParam.mType = ShapeType.ShapeType_Round;
			mShapeParam.mParams.Add(GetRadius());
		}

		if (mShape == null)
		{
			mShape = SceneShapeUtilities.Create(mShapeParam, new Vector2(GetPosition().x, GetPosition().z), GetDirection());
		}
		else if(!typeof(BuildObj).IsAssignableFrom(GetType()))
		{
			mShape = SceneShapeUtilities.refresh(ref mShape, mShapeParam, new Vector2(GetPosition().x, GetPosition().z), GetDirection());
		}

        return mShape;
	}
	// 得到物体变幻形状
	public SceneShape GetTransformShape(Vector2 pos, float radians)
	{
		if (mShapeParam == null)
		{
			mShapeParam = new SceneShapeParam();
			mShapeParam.mType = ShapeType.ShapeType_Round;
			mShapeParam.mParams.Add(GetRadius());
        }

		if(mTransformShape == null)
		{
			mTransformShape = SceneShapeUtilities.Create(mShapeParam, pos, radians);
		}
        else
        {
            mTransformShape = SceneShapeUtilities.refresh(ref mTransformShape, mShapeParam, pos, radians);
        }

        return mTransformShape;
	}

	// 是否为阻挡
	virtual public bool IsBarrier()
	{
		return false;
	}

	// 检测碰撞
	public bool TestCollider(ObjectBase obj)
	{
		if(obj == null)
		{
			return false;
		}

		SceneShape myShape = GetShape();
		if(myShape == null)
		{
			return false;
		}

		SceneShape objShape = obj.GetShape();
		if(objShape == null)
		{
			return false;
		}

		return myShape.intersect(objShape);
	}

	// 检测碰撞
	public bool TestTransformCollider(ObjectBase obj, Vector2 pos, float radians)
	{
		if (mShapeParam == null)
		{
			return false;
		}

		if (obj == null)
		{
			return false;
		}

		SceneShape myShape = GetShape();
		if (myShape == null)
		{
			return false;
		}

		SceneShape objShape = obj.GetTransformShape(pos, radians);
		if (objShape == null)
		{
			return false;
		}

		return myShape.intersect(objShape);
	}

	// 检测阻挡
	public bool TestBarrier(ObjectBase obj)
	{
		// 不是阻挡
		if(!IsBarrier() && !obj.IsBarrier())
		{
			return false;
		}

		return TestCollider(obj);
	}

	// 检测阻挡
	public bool TestTransformBarrier(ObjectBase obj, Vector2 pos, float radians)
	{
		// 不是阻挡
		if (!IsBarrier() && !obj.IsBarrier())
		{
			return false;
		}

		return TestTransformCollider(obj, pos, radians);
	}

    public void Disappear()
    {
        mDisappear = true;
    }
}
