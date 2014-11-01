//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Very simple sprite animation. Attach to a sprite and specify a common prefix such as "idle" and it will cycle through them.
/// </summary>

[ExecuteInEditMode]
[RequireComponent(typeof(UISprite))]
[AddComponentMenu("NGUI/UI/Sprite Animation")]
public class UISpriteAnimation : MonoBehaviour
{
	[HideInInspector][SerializeField] int mFPS = 8;
	[HideInInspector][SerializeField] string mPrefix = "";
	[HideInInspector][SerializeField] bool mLoop = true;
	[HideInInspector][SerializeField] bool mPixelPerfect = true;
    [HideInInspector][SerializeField] float mInterval = 0f;
	UISprite mSprite;
	float mDelta = 0f;
	int mIndex = 0;
	bool mActive = true;

    private bool mIsIntervaling = false;
    private float mTimeDelta = 0f;
    //public float mInterval = 0f;

	List<string> mSpriteNames = new List<string>();

    public delegate void OnFinished(GameObject go);
    private event OnFinished mOnFinished;

    public event OnFinished onFinished
    {
        remove
        {
            if (mOnFinished != null && value != null)
            {
                mOnFinished -= value;
            }
        }
        add 
        {
            if (value != null)
            {
                if (loop == true)
                {
                    GameDebug.LogWarning("SpriteAnimation的loop属性为TRUE是，无法增加OnFinished的回调！");
                    return;
                }
                mOnFinished += value;
            }
        }
    }

	/// <summary>
	/// Number of frames in the animation.
	/// </summary>

	public int frames { get { return mSpriteNames.Count; } }

	/// <summary>
	/// Animation framerate.
	/// </summary>

	public int framesPerSecond { get { return mFPS; } set { mFPS = value; } }

	/// <summary>
	/// Set the name prefix used to filter sprites from the atlas.
	/// </summary>

	public string namePrefix { get { return mPrefix; } set { if (mPrefix != value) { mPrefix = value; RebuildSpriteList(); } } }

	/// <summary>
	/// Set the animation to be looping or not
	/// </summary>

    public bool loop { get { return mLoop; } set { mLoop = value; } }
    public bool PixelPerfect { get { return mPixelPerfect; } set { mPixelPerfect = value; } }

	/// <summary>
	/// Returns is the animation is still playing or not
	/// </summary>

	public bool isPlaying { get { return mActive; } }

    /// <summary>
    /// 循环播放时候，再次新的循环播放需要的时间间隔;
    /// </summary>
    public float Interval 
    {
        set 
        {
            if (!loop)
            {
                GameDebug.LogWarning("UISpriteAnimation 的loop为false，设置时间间隔是无效的!");
            }
            if (value != mInterval && value >= 0.0f) 
                mInterval = value; 
        }
        get
        {
            return mInterval;
        }
    }

	/// <summary>
	/// Rebuild the sprite list first thing.
	/// </summary>

	void Start () { RebuildSpriteList(); }

	/// <summary>
	/// Advance the sprite animation process.
	/// </summary>

	void Update ()
	{
		if (mActive && mSpriteNames.Count > 1 && Application.isPlaying && mFPS > 0f)
		{
            if (mIsIntervaling)
            {
                mTimeDelta += RealTime.deltaTime;

                if (mTimeDelta > mInterval)
                    StopInterval();

                return;
            }

			mDelta += RealTime.deltaTime;
			float rate = 1f / mFPS;

			if (rate < mDelta)
			{
				
				mDelta = (rate > 0f) ? mDelta - rate : 0f;
				if (++mIndex >= mSpriteNames.Count)
				{
					mIndex = 0;
					mActive = loop;

                    if(mOnFinished != null)
                        mOnFinished(this.gameObject);

                    if (loop && mInterval > 0f && !mIsIntervaling)
                        StartInterval();
				}

				if (mActive && !mIsIntervaling)
				{
					mSprite.spriteName = mSpriteNames[mIndex];
                    if (mPixelPerfect)
                    {
                        mSprite.MakePixelPerfect();
                    }
				}
			}
		}
	}

    void StartInterval()
    {
        if (mSprite != null)
            mSprite.enabled = false;

        mIsIntervaling = true;
        mTimeDelta = 0f;
        Reset();
    }

    void StopInterval()
    {
        if (mSprite != null)
            mSprite.enabled = true;

        mIsIntervaling = false;
        mTimeDelta = 0f;
    }

    public void Stop()
    {
        mActive = false;
    }

	/// <summary>
	/// Rebuild the sprite list after changing the sprite name.
	/// </summary>
	public void RebuildSpriteList ()
	{
		if (mSprite == null) mSprite = GetComponent<UISprite>();
		mSpriteNames.Clear();

		if (mSprite != null && mSprite.atlas != null)
		{
			List<UISpriteData> sprites = mSprite.atlas.spriteList;

			for (int i = 0, imax = sprites.Count; i < imax; ++i)
			{
				UISpriteData sprite = sprites[i];

				if (string.IsNullOrEmpty(mPrefix) || sprite.name.StartsWith(mPrefix))
				{
					mSpriteNames.Add(sprite.name);
				}
			}
			mSpriteNames.Sort();
		}
	}

	/// <summary>
	/// Reset the animation to frame 0 and activate it.
	/// </summary>
	
	public void Reset()
	{
		mActive = true;
		mIndex = 0;

		if (mSprite != null && mSpriteNames.Count > 0)
		{
			mSprite.spriteName = mSpriteNames[mIndex];
            if (mPixelPerfect)
            {
                mSprite.MakePixelPerfect();
            }
		}
	}
}
