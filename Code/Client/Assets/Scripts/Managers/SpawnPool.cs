using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class SpawnPool : MonoBehaviour
{
    private static SpawnPool mInstance;

    private static int mMaxPoolSize = 20;

    private List<GameObject> mSpawns;

    public static SpawnPool Instance()
    {
        if (mInstance == null)
        {
            GameObject soundObj = new GameObject();
            soundObj.name = "SoundObjCaches";
            mInstance = soundObj.AddComponent<SpawnPool>();
        }

        return mInstance;
    }

    private void Awake()
    {
        mSpawns = new List<GameObject>();
        for (int i = 0; i < mMaxPoolSize; i++)
        {
            GameObject obj = new GameObject();
            obj.name = "(not use)";
            obj.AddComponent<AudioSource>();
            obj.transform.parent = transform;

            mSpawns.Add(obj);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {

    }

    private void Update()
    {


    }

    private void FixedUpdate()
    {
        if (CameraController.Instance.CurCamera != null)
        {
            gameObject.transform.position = CameraController.Instance.CurCamera.gameObject.transform.position;
        }
    }

    public void SetMaxPoolSize(int num)
    {
        mMaxPoolSize = num;
    }

    public GameObject GetSpawn()
    {
        GameObject re;
        if (mSpawns.Count < 1)
        {
            re = new GameObject();
            re.AddComponent<AudioSource>();
            re.transform.parent = transform;
            UnityEngine.Object.DontDestroyOnLoad(re);
            mSpawns.Add(re);
            return re;
        }

        re = mSpawns[0];
        mSpawns.RemoveAt(0);
        return re;
    }

    public void SetSpawn(GameObject obj)
    {
        AudioSource ausr = obj.GetComponent<AudioSource>();
        ausr.Stop();
        mSpawns.Add(obj);
        obj.name = "(not use)";
    }
}

