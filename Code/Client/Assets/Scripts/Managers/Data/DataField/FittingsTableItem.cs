using UnityEngine;
using System.Collections;

public class FittingsTableItem
{
    public int id;
    public string name;
    public int grade;
    public string bmp;

    public int costid;
    public uint num_0;
    public uint num_1;
    public uint num_2;

    //id区间
    public int start_0;
    public int end_0;
    public int start_1;
    public int end_1;
    public int start_2;
    public int end_2;
    public int start_3;
    public int end_3;
    public int start_4;
    public int end_4;

    //值区间(忽悠人的)
    public string value_0;
    public string value_1;
    public string value_2;
    public string value_3;
    public string value_4;

    //物品类型
    public string itemtype;
    //物品描述
    public string itemdesc;
    //获得途径
    public string itemabtain;

    public object this[string s]
    {
        get
        {
            switch (s)
            {
                case "start_0":
                    return start_0;

                case "start_1":
                    return start_1;

                case "start_2":
                    return start_2;

                case "start_3":
                    return start_3;

                case "start_4":
                    return start_4;



                case "end_0":
                    return end_0;

                case "end_1":
                    return end_1;

                case "end_2":
                    return end_2;

                case "end_3":
                    return end_3;

                case "end_4":
                    return end_4;


                case "value_0":
                    return value_0;

                case "value_1":
                    return value_1;

                case "value_2":
                    return value_2;

                case "value_3":
                    return value_3;

                case "value_4":
                    return value_4;

                default:
                    GameDebug.LogError("配件逻辑错误");
                    return null;
            }
        }
    }

    public bool GetIntervalByID(int index, ref int min, ref int max)
    {
        if (index < 0 || index > 4)
            return false;

        min = System.Convert.ToInt32(this["start_" + index.ToString()]);

        max = System.Convert.ToInt32(this["end_" + index.ToString()]);

        if (min == -1 || max == -1)
            return false;
        return true;
    }

    public bool GetIntervalByValue(int index, ref int min, ref int max)
    {
        if (index < 0 || index > 4)
            return false;

        string[] lineData = null;

        string ss = System.Convert.ToString(this["value_" + index.ToString()]);
        if (ss == null || ss == "")
            return false;

        lineData = ss.Split('|');

        if (lineData == null || lineData.Length != 2)
            return false;

        min = System.Convert.ToInt32(lineData[0]);
        max = System.Convert.ToInt32(lineData[1]);

        return true;
    }

    public int GetProId()
    {
        ArrayList ro = new ArrayList();
        for (int i = 0; i < FittingsProperty.RES_PROPERTY_MAX; ++i)
        {
            int odds = System.Convert.ToInt32(this["start_" + i.ToString()]);
            if (odds <= 0)
                continue;

            ro.Add(i);
        }

        int index = UnityEngine.Random.Range(0, ro.Count);

        return FittingsProperty.GetProperty(System.Convert.ToInt32(ro[index]));
    }
}