using System;
using UnityEngine;
class ApplicationState : MonoBehaviour
{
    public static void Init()
    {
        GameObject obj = new GameObject("ApplicationState");
        GameObject.DontDestroyOnLoad(obj);
        obj.AddMissingComponent<ApplicationState>();
    }

    void OnApplicationPause(bool paused)
    {
        GameDebug.Log("OnApplicationPause");
    }
    void OnApplicationFocus()
    {
        GameDebug.Log("OnApplicationFocus");
    }
}

