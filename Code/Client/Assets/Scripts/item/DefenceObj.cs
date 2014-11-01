using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DefenceObjInit : ItemObjInit
{
    //强化等级
    public int mStrenlevel;
    //星阶
    public int mStarslevel;

    //宝石信息
    public List<Message.stone_info> stoneinfo = new List<Message.stone_info>();
};


public class DefenceObj : ItemObj
{
    //强化等级
    private int mStrenlevel;
    //星阶
    private int mStarslevel;

    private int fightvalue;

    //强化、星阶、升阶等消耗的金币
    private int salemoney;
    private int salestars;
    private int salestones;

    //通过强化、升星等增加的属性
    private int prodamagestren;
    private int prodefencestren;
    private int prolifestren;
    private int procritstren;
    private int prodamagestars;
    private int prodefencestars;
    private int prolifestars;
    private int procritstars;
    //宝石信息
    List<Message.stone_info> stoneinfo = new List<Message.stone_info>();

    public static bool IsItemType(int type)
    {
        return type == (int)ItemType.Defence;
    }

    public DefenceObj()
    {
        mStrenlevel = 0;
        mStarslevel = 0;
        fightvalue = 0;
        salemoney = 0;
        salestones = 0;
        salestars = 0;
    }
    override public ItemType GetType()
    {
        return ItemType.Defence;
    }

    //服务器数据信息,临时数据
    override public bool Init(ItemObjInit initData)
    {
        if (!base.Init(initData))
            return false;

        DefenceObjInit defenceInit = initData as DefenceObjInit;
        if (defenceInit == null)
            return false;

        mStrenlevel = defenceInit.mStrenlevel;
        mStarslevel = defenceInit.mStarslevel;

        for (int i = 0; i < defenceInit.stoneinfo.Count; ++i)
        {
            stoneinfo.Add(defenceInit.stoneinfo[i]);
            StoneTableItem item = DataManager.StoneTable[stoneinfo[i].stoneid] as StoneTableItem;
            if (null == item)
                continue;
            //+宝石战力
            fightvalue += item.score;
        }
        salestones = stoneinfo.Count;
        SetProperty();
        return true;
    }
    override public bool BuildProperty(PropertyOperation operation)
    {
        DefenceTableItem itemdefence = GetDeRes();

        operation.AddPro((int)PropertyTypeEnum.PropertyTypeMaxHP, prolifestren == -1 ? 0 : prolifestren + itemdefence.basePropertyLife);
        operation.AddPro((int)PropertyTypeEnum.PropertyTypeMaxHP, prolifestars == -1 ? 0 : prolifestars);

        operation.AddPro((int)PropertyTypeEnum.PropertyTypeDamage, prodamagestren == -1 ? 0 : prodamagestren + itemdefence.basePropertyDamage);
        operation.AddPro((int)PropertyTypeEnum.PropertyTypeDamage, prodamagestars == -1 ? 0 : prodamagestars);

        operation.AddPro((int)PropertyTypeEnum.PropertyTypeCrticalLV, procritstren == -1 ? 0 : procritstren + itemdefence.basePropertyCrit);
        operation.AddPro((int)PropertyTypeEnum.PropertyTypeCrticalLV, procritstars == -1 ? 0 : procritstars);

        operation.AddPro((int)PropertyTypeEnum.PropertyTypeDefance, prodefencestren == -1 ? 0 : prodefencestren + itemdefence.basePropertyDefence);
        operation.AddPro((int)PropertyTypeEnum.PropertyTypeDefance, prodefencestars == -1 ? 0 : prodefencestars);

        for (int i = 0; i < stoneinfo.Count; ++i)
        {
            StoneTableItem item = DataManager.StoneTable[stoneinfo[i].stoneid] as StoneTableItem;
            if (null == item)
                continue;
            operation.AddPro(item.proid, item.provalue);
        }
        return false;
    }

    public void SetProperty()
    {
        DefenceTableItem item = GetDeRes();
        InitBasePro();

        for (int i = item.strenLevelMin; i <= mStrenlevel; ++i)
        {
            DefenceStrenProItem strenproitem = DataManager.DefenceStrenProTable[item.strenSerialNumber + i] as DefenceStrenProItem;
            if (null == strenproitem)
                continue;
            DefenceStrenItem strenitem = DataManager.DefenceStrenTable[i] as DefenceStrenItem;
            if (null == strenitem)
                continue;
            salemoney += (int)(strenitem.money * strenproitem.ratio);
            if (i == mStrenlevel)
            {
                //+装备强化战力
                fightvalue += strenproitem.fightvalue;
                if (-1 != procritstren)
                    procritstren = strenproitem.property;
                if (-1 != prodamagestren)
                    prodamagestren = strenproitem.property;
                if (-1 != prodefencestren)
                    prodefencestren = strenproitem.property;
                if (-1 != prolifestren)
                    prolifestren = strenproitem.property;
            }
        }

        //装备初始金币
        salemoney += item.equiprice;
        DefenceStarsProItem starsproitem = DataManager.DefenceStarsProTable[item.starsSerialNumber + mStarslevel] as DefenceStarsProItem;
        if (null == starsproitem)
        {
            GameDebug.LogError("无效的升星序列 id = " + (item.starsSerialNumber + mStarslevel));
            return;
        }

        //+装备升星战力
        fightvalue += starsproitem.fightvalue;
        if (-1 != procritstars)
            procritstars = starsproitem.property;
        if (-1 != prodamagestars)
            prodamagestars = starsproitem.property;
        if (-1 != prodefencestars)
            prodefencestars = starsproitem.property;
        if (-1 != prolifestars)
            prolifestars = starsproitem.property;

        //+装备初始战力
        fightvalue += item.fightValue;
    }

    public void InitBasePro()
    {
        DefenceTableItem item = GetDeRes();
        if (-1 != item.basePropertyCrit)
        {
            procritstren = 0;
            procritstars = 0;
        }
        else
        {
            procritstren = -1;
            procritstars = -1;
        }

        if (-1 != item.basePropertyDamage)
        {
            prodamagestren = 0;
            prodamagestars = 0;
        }
        else
        {
            prodamagestren = -1;
            prodamagestars = -1;
        }

        if (-1 != item.basePropertyDefence)
        {
            prodefencestren = 0;
            prodefencestars = 0;
        }
        else
        {
            prodefencestren = -1;
            prodefencestars = -1;
        }

        if (-1 != item.basePropertyLife)
        {
            prolifestren = 0;
            prolifestars = 0;
        }
        else
        {
            prolifestren = -1;
            prolifestars = -1;
        }
    }
    public DefenceTableItem GetDeRes()
    {
        return DataManager.DefenceTable[mResId] as DefenceTableItem;
    }

    public override ItemTableItem GetRes()
    {
        return GetDeRes();
    }

    public int GetStrenLv()
    {
        return mStrenlevel;
    }

    public int GetStarsLv()
    {
        return mStarslevel;
    }

    public string GetStarsLvPic()
    {
        DefenceStarsItem item = DataManager.DefenceStarsTable[GetStarsLv()] as DefenceStarsItem;
        if (null == item)
            return null;
        return item.starspicname;
    }

    public string GetStonePic()
    {
        int mMaxstoneid = -1;
        for (int i = 0; i < stoneinfo.Count; ++i)
        {
            if (stoneinfo[i].stoneid > mMaxstoneid)
                mMaxstoneid = stoneinfo[i].stoneid;
        }

        StoneTableItem item = DataManager.StoneTable[mMaxstoneid] as StoneTableItem;
        if (null == item)
            return null;

        return item.picname;
    }

    public int GetFightvalue()
    {
        return fightvalue;
    }

    public int GetSaleMoney()
    {
        return salemoney;
    }
    public int GetSaleStars()
    {
        return salestars;
    }

    public int GetSaleStones()
    {
        return salestones;
    }

    public int GetProdamagestren()
    {
        return prodamagestren;
    }

    public int GetProdefencestren()
    {
        return prodefencestren;
    }

    public int GetProlifestren()
    {
        return prolifestren;
    }

    public int GetProcritstren()
    {
        return procritstren;
    }

    public int GetProdamagestars()
    {
        return prodamagestars;
    }

    public int GetProdefencestars()
    {
        return prodefencestars;
    }

    public int GetProlifestars()
    {
        return prolifestars;
    }

    public int GetProcritstars()
    {
        return procritstars;
    }

    public List<Message.stone_info> GetStoneInfo()
    {
        return stoneinfo;
    }

    public Message.stone_info GetStoneInfoByPos(int pos)
    {
        for (int i = 0; i < stoneinfo.Count; ++i)
            if (stoneinfo[i].stonepos == pos)
                return stoneinfo[i];

        return null;
    }

    public void SetSomeInfo(DefenceTableItem item)
    {
        mResId = item.id;
        InitBasePro();
        this.mStrenlevel = item.strenLevelMin;
        this.mStarslevel = item.starsLevelMin;
        salemoney = item.equiprice;
        //+装备初始战力
        fightvalue = item.fightValue;

        DefenceStrenProItem strenproitem = DataManager.DefenceStrenProTable[item.strenSerialNumber + item.strenLevelMin] as DefenceStrenProItem;
        if (null == strenproitem)
            return;
        DefenceStrenItem strenitem = DataManager.DefenceStrenTable[item.strenLevelMin] as DefenceStrenItem;
        if (null == strenitem)
            return;
        //+装备强化战力
        fightvalue += strenproitem.fightvalue;
        if (-1 != procritstren)
            procritstren = strenproitem.property;
        if (-1 != prodamagestren)
            prodamagestren = strenproitem.property;
        if (-1 != prodefencestren)
            prodefencestren = strenproitem.property;
        if (-1 != prolifestren)
            prolifestren = strenproitem.property;

        DefenceStarsProItem starsproitem = DataManager.DefenceStarsProTable[item.starsSerialNumber + mStarslevel] as DefenceStarsProItem;
        if (null == starsproitem)
        {
            GameDebug.LogError("无效的升星序列 id = " + (item.starsSerialNumber + mStarslevel));
            return;
        }

        fightvalue += starsproitem.fightvalue;
        if (-1 != procritstars)
            procritstars = starsproitem.property;
        if (-1 != prodamagestars)
            prodamagestars = starsproitem.property;
        if (-1 != prodefencestars)
            prodefencestars = starsproitem.property;
        if (-1 != prolifestars)
            prolifestars = starsproitem.property;
    }
}
