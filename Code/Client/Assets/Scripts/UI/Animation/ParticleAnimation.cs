using UnityEngine;
public class ParticleAnimation
{
    private UISprite mSprite = null;
    private UIParticlePreview mPreview = null;
    public ParticleAnimation(UISprite sprite , UIParticlePreview preview)
    {
        mSprite = sprite;
        mPreview = preview;
    }

    public void Destroy()
    {
        if( mPreview != null )
        {
            mPreview.Destroy();
            mPreview = null;
        }
        if( mSprite != null )
        {
            GameObject.Destroy(mSprite.gameObject);
            mSprite = null;
        }
    }

    public void SetDepth(int depth)
    {
        if( mSprite != null )
            mSprite.depth = depth;
    }

    public void RePlay()
    {
        if (mPreview == null)
            return;
        mPreview.RePlay();
    }

    public bool IsDead()
    {
        return mPreview.IsDead();
    }

    public void OnUpdate()
    {
        mPreview.Update();
    }

    public GameObject gameObject
    {
        get
        {
            return mSprite != null ?  mSprite.gameObject : null;
        }
    }
}