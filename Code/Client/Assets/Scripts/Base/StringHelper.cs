using UnityEngine;
using System.Collections;
using Message;

public enum FontColor
{
    GreenLight, // 浅绿;
    White,      // 白色;
    Yellow,     // 黄色;
    Gray,       // 灰色;
    GreenMid,   // 中绿;
    Blue,       // 蓝色;
    Pink,       // 粉色;
    Red,        // 红色;
    Green,      // 绿色;
}

public static class StringHelper
{
	public static string GetErrorString(ERROR_CODE errorcode, FontColor fc = FontColor.White)
    {
        int code = (int)errorcode;
        if ( !DataManager.ErrorCodeStringTable.ContainsKey( code ) )
        {
            return "";
        }
        ErrorStringTableItem item = DataManager.ErrorCodeStringTable[code] as ErrorStringTableItem;

		return fc == FontColor.White ? item.str : StringWithColor(fc, item.str);
    }

    public static string GetString(string key , FontColor fc = FontColor.White)
    {
        if (!DataManager.StringTable.ContainsKey(key))
        {
            return "";
        }
        StringTableItem item = DataManager.StringTable[key] as StringTableItem;

        return fc == FontColor.White ? item.str : StringWithColor(fc, item.str);
    }


    public static void StrCpy(char[] dest , string src)
    {
        if( dest == null || src == null || src.Length <= 0 )
        {
            return;
        }
        for( int i = 0 ; i < dest.Length && i < src.Length ; ++i )
        {
            dest[i] = src[i];
        }
    }

    public static string StringMoney(int moneytype)
    {
        if (moneytype <= (int)ProceedsType.Invalid || moneytype >= (int)ProceedsType.Money_Max)
            return "";

        switch(moneytype)
        {
            case (int)ProceedsType.Money_Game:
                return GetString("money_game");
            case (int)ProceedsType.Money_RMB:
                return GetString("money_rmb");
            case (int)ProceedsType.Money_Prestige:
                return GetString("money_prestige");
            case (int)ProceedsType.Money_Stren:
                return GetString("money_stren");
            case (int)ProceedsType.Money_Arena:
                return GetString("money_arena");
        }
        return "";
    }

    public static string StringWithColor(FontColor color , string text)
    {
        string head = "";
        switch (color)
        {
            case FontColor.GreenLight: // 浅绿;
                head = "[79ffdb]";
                break;
            case FontColor.White:      // 白色;
                head = "[fafdf4]";
                break;
            case FontColor.Yellow:     // 黄色;
                head = "[fed514]";
                break;
            case FontColor.Gray:       // 灰色;
                head = "[d3c0aa]";
                break;
            case FontColor.GreenMid:   // 中绿;
                head = "[81ffa5]";
                break;
            case FontColor.Blue:      // 蓝色;
                head = "[2a96c9]";
                break;
            case FontColor.Pink:       // 粉色;
                head = "[ff6bef]";
                break;
            case FontColor.Red:       // 红色;
                head = "[e92224]";
                break;
            case FontColor.Green:     // 绿色;
                head = "[3eff00]";
                break;
        }

        return head + text + "[-]";
    }
}
