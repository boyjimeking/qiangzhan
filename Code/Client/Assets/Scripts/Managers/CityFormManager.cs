using UnityEngine;
using System.Collections;

public class CityFormManager
{
    private static CityFormManager mInstance = null;

    public CityFormManager()
    {
 
    }
    
    public static CityFormManager Instance
    {
        get
        {
            if (mInstance == null)
                mInstance = new CityFormManager();

            return mInstance;
        }
    }
    public static void SetRedPointActive(int functionId, bool active)
    {
        FunctionEvent fe = new FunctionEvent(FunctionEvent.FUNCTION_RED_POINT);

        fe.functionid = functionId;
        fe.isShow = active;

        EventSystem.Instance.PushEvent(fe);
    }

    public static void SetRedPointActive(string uiName, bool active)
    {
        int resId =  WindowManager.Instance.GetUIResID(uiName);
        if (resId < 0)
        {
            Debug.LogError("找不到对应的功能图标，UIName：" + uiName);
            return;
        }

        SetRedPointActive(resId, active);
    }

    /// <summary>
    /// 根据ui名字打开二级按钮;
    /// </summary>
    /// <param name="uiname"></param>
    public static void OpenChildFunc(string uiname)
    {
        FunctionModule fm = ModuleManager.Instance.FindModule<FunctionModule>();

        if (fm == null)
            return;

        int id = FunctionModule.GetMenuIdByUIName(uiname);

        if (id < 0)
            return;

        fm.OpenChildFunc(id);
    }

    /// <summary>
    /// 建议不要用这个，因为策划填表时候id可能会变;
    /// </summary>
    /// <param name="resid"></param>
    //public static void OpenChildFunc(int resid)
    //{
    //    FunctionModule fm = ModuleManager.Instance.FindModule<FunctionModule>();

    //    if (fm == null)
    //        return;

    //    fm.OpenChildFunc(resid);
    //}

    public static void CloseChildFunc(string uiname)
    {
        FunctionModule fm = ModuleManager.Instance.FindModule<FunctionModule>();

        if (fm == null)
            return;

        int id = FunctionModule.GetMenuIdByUIName(uiname);

        if (id < 0)
            return;

        fm.CloseChildFunc(id);
    }

    /// <summary>
    /// 不要用这个，因为策划填表时候id会变;
    /// </summary>
    /// <param name="resid"></param>
    //public static void CloseChildFunc(int resid)
    //{
    //    FunctionModule fm = ModuleManager.Instance.FindModule<FunctionModule>();

    //    if (fm == null)
    //        return;

    //    fm.CloseChildFunc(resid);
    //}
}
