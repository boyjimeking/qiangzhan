using UnityEngine;
using System.Collections;
using System;
//场景表
public class Scene_CitySceneTableItem : SceneTableItem 
{
    public string mUiPos;
    public string mBg;//城镇图片
    public int mUnlock;//解锁目标等级
    public int mZoneid;
    public ArrayList GetResolving(Scene_CitySceneTableItem Item)
    {
        if (null == Item)
            return null;

        string[] splitb = Item.mUiPos.Split(new char[] { '|' });
  
        ArrayList list = new ArrayList();
        for (int i = 0; i < splitb.Length; i++)
        {
            string[] split = splitb[i].Split(new char[] { ',' });

            if (3 != split.Length)
            {
                return null;
            }
            list.Add(new Vector3(Convert.ToInt32(split[0]), Convert.ToInt32(split[1]), Convert.ToInt32(split[2])));
        }

        return list;
    }

    public string[] GetBg(Scene_CitySceneTableItem Item)
    {
        if (null == Item)
            return null;

        string[] split = Item.mBg.Split(new char[] { '|' });

        if (0 == Item.resID)
        {
            if (1 == split.Length)
                return split;
        }

        else if (1 == Item.resID)
        {
            if (2 == split.Length)
                return split;
        }

        else if (3 == split.Length)
            return split;

        return null;
    }
}
