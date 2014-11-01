using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Message;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class WorldMapModule : ModuleBase
{
    public List<int> mPlayerOldMapData = new List<int>();
    public List<int> mPlayerNewMapData = new List<int>();
    public Dictionary<int, string> mDictText = new Dictionary<int, string>();
    private int mguide = -1;
    public int GuideResId 
    {
        get
        {
            return mguide;
        }

        set
        {
            mguide = value;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        mPlayerOldMapData.Clear();
        mPlayerOldMapData.Clear();
    }

 
    public void addDictData(int mapid,string str)
    { 
        if(0 > mapid || "" == str)
            return ;

        if (!mDictText.ContainsKey(mapid))
            mDictText.Add(mapid, str);
        else
            mDictText[mapid] = str;
    }

    public string getDictData(int mapid)
    {
        if (0 > mapid )
            return "";

        foreach (var item in mDictText)
        {
            if (item.Key == mapid)
                return item.Value;
        }
        return "";
    }
    public bool isHasZoneId(int Zone)
    {
        IDictionaryEnumerator itr = DataManager.Scene_StageListTable.GetEnumerator();
        while (itr.MoveNext())
        {
            Scene_StageListTableItem item = itr.Value as Scene_StageListTableItem;
            if (null == item)
                return false;

            if (item.mZoneId == Zone)
                return true;

        }
//         foreach (DictionaryEntry de in DataManager.Scene_StageListTable)
//         {
//             Scene_StageListTableItem item = de.Value as Scene_StageListTableItem;
//             if (null == item)
//                 return false;
// 
//             if (item.mZoneId == Zone)
//                 return true;
//         }

        return false;
    }

    public bool isLevel(Scene_CitySceneTableItem Item)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == module)
            return false;

        if (Item.mUnlock <= module.GetLevel())
            return true;

        return false;
    }

    public void initWorldMapData()
    {
        IDictionaryEnumerator itr = DataManager.SceneCityTable.GetEnumerator();
        while (itr.MoveNext())
        {
            Scene_CitySceneTableItem item = itr.Value as Scene_CitySceneTableItem;
            if (null == item)
                continue;

            if (isLevel(item) && !isLock(item))
            {
                UpdateMapData(item.resID, true);
                UpdateMapData(item.resID, false);
            }   
        }
//         foreach (DictionaryEntry de in DataManager.SceneCityTable)
//         {
//             Scene_CitySceneTableItem item = de.Value as Scene_CitySceneTableItem;
//             if (null == item)
//                 continue;
// 
//             if (isLevel(item) &&!isLock(item))
//             {
//                 UpdateMapData(item.resID, true);
//                 UpdateMapData(item.resID, false);
//             }   
//         }
    }
    public bool isLock(Scene_CitySceneTableItem Item)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null || null == Item)
            return true;

        if (-1 == Item.mZoneid)
            return false;

        if (!isHasZoneId(Item.mZoneid))
        {
            GameDebug.LogError("scene_city.txt:" + Item.name + "场景在 scene_stagelist.txt 没有对应战区Id");
            return true;
        }

        if (0 >= DataManager.Scene_StageListTable.Count)
            return true;

        IDictionaryEnumerator itr = DataManager.Scene_StageListTable.GetEnumerator();
        while (itr.MoveNext())
        {
            Scene_StageListTableItem item = itr.Value as Scene_StageListTableItem;
            if (null == item)
                return true;

            if (Item.mZoneid != item.mZoneId)
                continue;

            if (0 == module.GetStagePassTimes(item.mNromalStageId))
            {
                return true;
            }
        }
//         foreach (DictionaryEntry de in DataManager.Scene_StageListTable)
//         {
//             Scene_StageListTableItem item = de.Value as Scene_StageListTableItem;
//             if (null == item)
//                 return true;
// 
//             if (Item.mZoneid != item.mZoneId)
//                 continue;
// 
//             if (0 == module.GetStagePassTimes(item.mNromalStageId))
//             {
//                 return true;
//             }
//         }

        return false;
    }



    //public void updateMapText(int mapid, string Str)
    //{

    //    if (0 > mapid || "" == Str)
    //        return;

    //    foreach (var item in mPlayerOldMapData)
    //    {
    //        if (item.Key == mapid)
    //            item.Value.contionText = Str; 
    //    }
     
    //}


    public void UpdateMapData(int mapid, bool isOld)
    {
        if (0 > mapid)
            return;

        if (isOld)
        {
            if (!mPlayerOldMapData.Contains(mapid))
                mPlayerOldMapData.Add(mapid);
        }
        else
        {
            if (!mPlayerNewMapData.Contains(mapid))
                mPlayerNewMapData.Add(mapid);
        }
    }
}
