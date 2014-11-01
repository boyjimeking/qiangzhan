
/// <summary>
/// 任务
/// </summary>
public abstract class ObjectTaskBase
{
    protected VisualObject mOwner;

    public ObjectTaskBase(VisualObject owner)
    {
        mOwner = owner;
    }
    public virtual void Start()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void Destroy()
    {

    }

    public virtual bool Finish()
    {
        return false;
    }
}

