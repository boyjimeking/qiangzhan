using UnityEngine;
using System;
using System.Collections;
public class FlyEffect : MonoBehaviour 
{
    public delegate void OnFinished();
    public event OnFinished onFinished;

    private bool mAutoDestroy = true;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnEnable()
    {
 
    }

    void OnDisable()
    {
        onFinished = null;
    }

    public void Play(Vector3 from , Vector3 to , float dur , bool autoDestroy = true) 
    {
        mAutoDestroy = autoDestroy;

        transform.position = from;
        TweenPosition tp = TweenPosition.Begin(gameObject, dur, to);
        tp.SetOnFinished(onFinish);
        tp.PlayForward();
    }

    void onFinish()
    {
        //Destroy(this.gameObject);
        if (onFinished != null)
            onFinished();
        
        if(mAutoDestroy)
            GameObject.DestroyImmediate(this.gameObject);
    }
}
