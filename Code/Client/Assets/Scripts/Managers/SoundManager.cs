using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Managers.Data.DataField;
using UnityEngine;

public class AudioTime
{
    public AudioTime(float totalTime )
    {
        mTotalTime = totalTime;
        mElapseTime = 0;
       
    }
    //声音总时间(秒)
    public float mTotalTime;
    //播放时间
    public float mElapseTime;

    public OnSoundFinish onFinish;
}


public delegate  void OnSoundFinish();


public class SoundManager
{

    private static SoundManager _ins;
    private readonly Dictionary<string, AudioClip> mAudios;
    //附加音乐GameObject列表
    private readonly Dictionary<int, GameObject> mOwnedObjects;
  
    /// <summary>
    /// 不是循环的，正在播放的声音
    /// </summary>
    private Dictionary<int, AudioTime> mPlayingAudios;

    /// <summary>
    /// 最近播放过的声音
    /// </summary>
    private Dictionary<string, AudioTime> mRecentAudios;
    private static int mSoundID;

    //是否静音
    private bool mIsMute = false;
    //是否音效音乐
    private bool mIsSound = true;

    //是否背景音乐
    private bool mIsBkSound = true;

    private int mPlayingDeleteKey = -1;
    private string mRecentDeleteKey = "x";


    private Dictionary<int, bool> mSceneNormalSounds = new Dictionary<int, bool>();

    public bool getIsBkSound(int BkSound)
    {
        if (mSceneNormalSounds.ContainsKey(BkSound))
        {
            return true;
        }
        return false;
    }

    public bool IsMute
    {
        get { return mIsMute; }
        set { mIsMute = value; }
    }

    public bool IsSound 
    {
        get { return mIsSound; }
        set { mIsSound = value; }
    }

    public bool IsBkSound
    {
        get { return mIsBkSound; }
        set { mIsBkSound = value; }
    }

    public static int CreateSoundID()
    {
        mSoundID++;
        return mSoundID;
    }

    public SoundManager()
    {
        mAudios = new Dictionary<string, AudioClip>();
        mOwnedObjects = new Dictionary<int, GameObject>();
        mPlayingAudios = new Dictionary<int, AudioTime>();
        mRecentAudios= new Dictionary<string, AudioTime>();
        _ins = this;
    }

    public static SoundManager Instance
    {
        get { return _ins; }

    }

    public bool Init(DataTable table)
    {
        IDictionaryEnumerator itr = table.GetEnumerator();
        while( itr.MoveNext() )
        {
            SoundTableItem item = itr.Value as SoundTableItem;
            if( item != null && item.prefab > 0 )
            {
                mAudios.Add(item.path, ResourceManager.Instance.LoadSoundClip(item.path));
            }
        }

        itr = DataManager.SceneTable.GetEnumerator();
        while( itr.MoveNext() )
        {
            SceneTableItem item = itr.Value as SceneTableItem;
            if (item != null && item.normalSound >= 0 )
            {
                if (!mSceneNormalSounds.ContainsKey(item.normalSound))
                {
                    mSceneNormalSounds.Add(item.normalSound, true);
                }
            }
        }
        return true;
    }

    public void Update(float detalTime)
    {
        for (int i = 0; i < mPlayingAudios.Count; ++i)
        {
            foreach (KeyValuePair<int, AudioTime> kvp in mPlayingAudios)
            {
                kvp.Value.mElapseTime += detalTime;
                if (kvp.Value.mElapseTime > kvp.Value.mTotalTime)
                {
                    if (kvp.Value.onFinish != null)
                    {
                        kvp.Value.onFinish();
                    }

                    mPlayingDeleteKey = kvp.Key;
                    break;
                }
            }
            if( mPlayingDeleteKey >= 0 )
            {
                mPlayingAudios.Remove(mPlayingDeleteKey);
                RemoveSoundByID(mPlayingDeleteKey);
            }
        }

        for (int i = 0; i < mRecentAudios.Count; ++i)
        {
            foreach (KeyValuePair<string, AudioTime> kvp in mRecentAudios)
            {
                kvp.Value.mElapseTime += detalTime;
                if (kvp.Value.mElapseTime > kvp.Value.mTotalTime)
                {
                    mRecentDeleteKey = kvp.Key;
                    break;
                }
            }

           mRecentAudios.Remove(mRecentDeleteKey);
        }
    }

    public AudioClip GetAudioClipByName(string name)
    {
        if (!mAudios.ContainsKey(name))
        {
            mAudios.Add(name, ResourceManager.Instance.LoadSoundClip(name));
        }
        return mAudios[name];


    }

    /// <summary>
    /// 在给定坐标播放声音
    /// </summary>
    /// <param name="tempID"></param>
    /// <param name="pos"></param>
    /// <param name="isLoop"></param>
    /// <returns></returns>
    //public int Play(int tempID, Vector3 pos, bool isLoop = false)
    //{
    //    GameObject emitter = new GameObject();
    //    emitter.transform.position = pos;
    //    emitter.name = "emiPos";
    //    return Play(tempID, emitter, isLoop);

    //}


    public int Play(int tempId, bool isLoop)
    {
        return Play(tempId, null, isLoop);
    }

    public int Play(int tempId, int deelayTime)
    {
        return Play(tempId, null, false, 1, deelayTime / 1000f);
    }
    /// <summary>
    /// 播放音乐
    /// </summary>
    /// <param name="tempID">模板ID</param>
    /// <param name="emitter">声音源，现在第二个参数先不用</param>
    /// <returns>返回声音的实例id</returns>
    public int Play(int tempID, GameObject emitter = null, bool isLoop = false,float volumn=1,float delayTime=0)
    {
        var soundVo = DataManager.SoundTable[tempID] as SoundTableItem;
        if (soundVo == null)
        {
            GameDebug.LogError("sound 表没有id："+tempID+"的资源");
            return -1;
        }
        if (mRecentAudios.ContainsKey(soundVo.path)) return -1;

        AudioClip audioClip = (GetAudioClipByName(soundVo.path));
        if (audioClip == null) return -1;

        mRecentAudios.Add(soundVo.path,new AudioTime(30));
        int soundInsId = CreateSoundID();
        
        if (emitter == null)
        {
            emitter = SpawnPool.Instance().GetSpawn();
            emitter.name = tempID.ToString();
            mOwnedObjects.Add(soundInsId, emitter);
            if (isLoop == false)
            {
                mPlayingAudios.Add(soundInsId, new AudioTime(audioClip.length * 1000));
            }
           
        }else if (emitter.name == "emiPos")
        {
            mOwnedObjects.Add(soundInsId, emitter);
            if (isLoop == false)
            {
                mPlayingAudios.Add(soundInsId, new AudioTime(audioClip.length * 1000));
            }
        }
        else
        {
            if (emitter.GetComponent<AudioSource>() == null)
            {
                emitter.AddComponent<AudioSource>();
            }
        }
        AudioSource  audioSrc = emitter.GetComponent<AudioSource>();
        audioSrc.clip = audioClip;
        audioSrc.rolloffMode = AudioRolloffMode.Custom;
        audioSrc.volume = soundVo.volumn != 1 ? soundVo.volumn : volumn;
        audioSrc.loop = isLoop;
        audioSrc.playOnAwake = false;
        audioSrc.mute = IsMute;
        if (!IsMute)
        {
            if (getIsBkSound(tempID))
            {
                if (!mIsBkSound)
                    audioSrc.mute = true;
                else
                    audioSrc.mute = false;
            }
            else
            {
                if (!mIsSound)
                    audioSrc.mute = true;
                else
                    audioSrc.mute = false;
            }
        }
     
        audioSrc.spread = 180;
        audioSrc.PlayDelayed(delayTime);
        return soundInsId;
    }

    /// <summary>
    ///删除声音，id为Play方法的返回值
    /// </summary>
    /// <param name="id"></param>
    public void RemoveSoundByID(int id)
    {
        if (mOwnedObjects.ContainsKey(id))
        {
            var obj = mOwnedObjects[id];
            mOwnedObjects.Remove(id);
            SpawnPool.Instance().SetSpawn(obj);
        }
    }

   
    public void Pause(int instanceId)
    {
        if (mOwnedObjects.ContainsKey(instanceId))
        {
            mOwnedObjects[instanceId].GetComponent<AudioSource>().Pause();
        }
    }

    public void Stop(int instanceId)
    {
        if (mOwnedObjects.ContainsKey(instanceId))
        {
            mOwnedObjects[instanceId].GetComponent<AudioSource>().Stop();
        }
    }

    public void ContinuePlay(int instanceId)
    {
        if (mOwnedObjects.ContainsKey(instanceId))
        {
            mOwnedObjects[instanceId].GetComponent<AudioSource>().Play();
        }
    }

    //静音
    public void SetMute(bool param)
    {
        IsMute = param;

        foreach (KeyValuePair<int, GameObject> kvp in mOwnedObjects)
        {
            mOwnedObjects[kvp.Key].GetComponent<AudioSource>().mute = IsMute;
        }
        
    }

    //音效
    public void SetSound(bool param)
    {

        foreach (KeyValuePair<int, GameObject> kvp in mOwnedObjects)
        {
            if (!getIsBkSound(System.Convert.ToInt32(kvp.Value.name)))
                mOwnedObjects[kvp.Key].GetComponent<AudioSource>().mute = param;
        }

    }

    public void SetBkSound(bool param)
    {

        foreach (KeyValuePair<int, GameObject> kvp in mOwnedObjects)
        {
            if (getIsBkSound(System.Convert.ToInt32(kvp.Value.name)))
                mOwnedObjects[kvp.Key].GetComponent<AudioSource>().mute = !param;
        }

    }

    public void AddFinishCallback(int insId, OnSoundFinish callback)
    {
        if (mPlayingAudios.ContainsKey(insId))
        {
            if (mPlayingAudios[insId].onFinish == null)
            {
                mPlayingAudios[insId].onFinish = callback;
            }
            else
            {
                mPlayingAudios[insId].onFinish += callback;
            }
        }
        
    }

}

