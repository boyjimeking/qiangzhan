using UnityEngine;
using System.Collections;

public class FittcolorTableItem
{
    public int id;
    public string desc;
    public string color;
    public int qualityid;
    public uint max_0;
    public uint max_1;
    public uint max_2;
    public uint max_3;
    public uint max_4;

    public object this[string s]
    {
        get
        {
            switch (s)
            {
                case "max_0":
                    return max_0;

                case "max_1":
                    return max_1;

                case "max_2":
                    return max_2;

                case "max_3":
                    return max_3;

                case "max_4":
                    return max_4;

                default:
                    GameDebug.LogError("配件逻辑错误");
                    return null;
            }
        }
    }
}