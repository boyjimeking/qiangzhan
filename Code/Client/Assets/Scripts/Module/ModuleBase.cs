using UnityEngine;
using System.Collections;

public abstract class ModuleBase
{
    private bool mEnabled = false;
    public void Enable()
    {
        if( !mEnabled )
        {
            mEnabled = true;
            OnEnable();
        }
    }

    public void Disable()
    {
        if (mEnabled)
        {
            mEnabled = false;
            OnDisable();
        }
    }

    virtual protected void OnEnable()
    {

    }

    virtual protected void OnDisable()
    {

    }

    virtual public void Update(uint elapsed)
    {

    }
}
