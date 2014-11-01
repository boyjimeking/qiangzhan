using System;
using System.Collections;


/// <summary>
/// 游戏Action处理工厂
/// </summary>
public class ActionFactory
{
    private Hashtable mId2Object = new Hashtable();

    private static ActionFactory s_instance = null;

    public static ActionFactory Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new ActionFactory();
            }
            return s_instance;
        }
    }

    public  GameAction Create(int actionId)
    {
        if (!mId2Object.Contains(actionId))
            return null;

        return mId2Object[actionId] as GameAction;
    }

    public bool RegisterAction(int actionId, GameAction action)
    {
        mId2Object.Add(actionId, action);
        return true;
    }
}
