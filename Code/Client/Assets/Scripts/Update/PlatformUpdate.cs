
public class PlatformUpdate
{
    public delegate void VOIDDelegate();
    public VOIDDelegate CompleteDelegate;

    public virtual void CheckNeedUpdate()
    {

    }

    public virtual void OnCheckNeedUpdateInfo(string param)
    {
    }

    public virtual void OnDownloadAppProgressChanged(string param)
    {

    }
}

