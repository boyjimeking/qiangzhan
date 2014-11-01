
using System.Collections.Generic;
/// <summary>
/// 场景效果管理器
/// </summary>
using UnityEngine;
public class SceneEffectManager
{
    private SceenMask mSceneMask;
    private List<DarkTask> mDarkExcludeList = new List<DarkTask>();
    /// <summary>
    /// 使场景变暗
    /// </summary>
    /// <param name="excludeobjs"></param>
    public void DarkenScene(uint[] excludeobjs,float alpha =0.7f)
    {
       GameObject cameraobj = CameraController.Instance.CurCamera.gameObject;
        //场景变暗
        if (mSceneMask == null)
        {
            mSceneMask = cameraobj.AddComponent<SceenMask>();
        }
        mSceneMask.enabled = true;
        mSceneMask.SetAlpha(alpha);
        //排除某些物体
        SceneObjManager objMng = SceneManager.Instance.GetCurScene().GetSceneObjManager();
        mDarkExcludeList.Clear();
        foreach (uint instid in excludeobjs)
        {
            ObjectBase obj = objMng.FindObject(instid);
            if (obj == null)
                continue;
            VisualObject vObj = obj as VisualObject;
            if (vObj == null)
                continue;
            if (vObj.OriginalMtl == null)
                continue;

            DarkTask task = new DarkTask();
            task.instID = instid;
            List<int> q = new List<int>();
            for (int i = 0; i < vObj.OriginalInstMtl.Length; ++i)
            {
                Material mtl = vObj.OriginalInstMtl[i];
                if (mtl == null)
                    continue;
                q.Add(mtl.renderQueue);
                mtl.renderQueue = 4000 + 1;
            }
            task.queue = q.ToArray();
            mDarkExcludeList.Add(task);
        }
    }
    public void RecoverScene()
    {
        if(mSceneMask != null)
        {
            mSceneMask.enabled = false;
        }
       SceneObjManager objMng = SceneManager.Instance.GetCurScene().GetSceneObjManager();
        //还原场景中的物体的渲染队列
        foreach(DarkTask task in mDarkExcludeList)
        {
            ObjectBase obj = objMng.FindObject(task.instID);
            if (obj == null)
                continue;
            VisualObject vObj = obj as VisualObject;
            if (vObj == null)
                continue;
            if (task.queue == null || vObj.OriginalInstMtl == null || task.queue.Length != vObj.OriginalInstMtl.Length)
                continue;
            for (int i = 0; i < vObj.OriginalInstMtl.Length; ++i)
           {
               Material mtl = vObj.OriginalInstMtl[i];
               if (mtl == null)
                   continue;
               mtl.renderQueue = task.queue[i];
           }
        }
        mDarkExcludeList.Clear();
    }

    public void Destroy()
    {
        if(mSceneMask != null)
        {
           GameObject.Destroy(mSceneMask.gameObject);
            mSceneMask = null;
        }
        mDarkExcludeList = null;

    }
}

class DarkTask
{
    public uint instID;
    public int[] queue;
}

