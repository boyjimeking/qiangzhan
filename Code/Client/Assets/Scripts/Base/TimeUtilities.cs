using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public static class TimeUtilities
{
    private static DateTime sStartTime = DateTime.Parse("1970-01-01 00:00:00");
    //返回 1970.1.1 到现在的毫秒数
    public static ulong GetNow()
    {
        //优化
        DateTime startTime = sStartTime;// DateTime.Parse("1970-01-01 00:00:00");

        DateTime nowTime = DateTime.Now;

        ulong unixTime = (ulong)Math.Round((nowTime - startTime).TotalMilliseconds, MidpointRounding.AwayFromZero);
        return unixTime;
    }
	
	//返回 不带时区的1970.1.1 到现在的毫秒数
	public static ulong GetUtcNowMilliseconds()
	{
        TimeSpan ts = DateTime.UtcNow - sStartTime;// DateTime.Parse("1970-01-01 00:00:00");
		return (ulong)ts.TotalMilliseconds;
	}

	//返回 不带时区的1970.1.1 到现在的秒数
	public static ulong GetUtcNowSeconds()
	{
        TimeSpan ts = DateTime.UtcNow - sStartTime;//DateTime.Parse("1970-01-01 00:00:00");
		return (ulong)ts.TotalSeconds;
	}

    public static int GetSecond(int time)
    {
        return time / 1000;
    }

    /// <summary>
    /// 时间格式为：XX天XX小时XX分XX秒;
    /// </summary>
    /// <param name="curTime"></param>
    /// <returns></returns>
    public static string GetCountDownDHMS(int curTime)
    {
        int day = curTime / (1000 * 60 * 60 * 24);
        int hour = (curTime - day * (1000 * 60 * 60 * 24)) / (1000 * 60 * 60);
        int min = (curTime - day * (1000 * 60 * 60 * 24) - hour * (1000 * 60 * 60)) / (1000 * 60);
        int sec = (curTime - day * (1000 * 60 * 60 * 24) - hour * (1000 * 60 * 60) - min * (1000 * 60)) / 1000;

        string timeString = "";

        timeString += day + "天";

        if (hour < 10)
        {
            timeString += "0" + hour;
        }
        else
        {
            timeString += hour.ToString();
        }

        timeString += "时";

        if (min < 10)
        {
            timeString += ("0" + min);
        }
        else
        {
            timeString += min.ToString();
        }

        timeString += "分";

        if (sec < 10)
        {
            timeString += ("0" + sec);
        }
        else
        {
            timeString += sec.ToString();
        }

        return timeString + "秒";
    }

    /// <summary>
    /// 时间格式为  00：00：00
    /// </summary>
    /// <param name="curTime">毫秒</param>
    /// <returns></returns>
    public static string GetCountDownHMS(int curTime)
    {
        int hour = curTime / (1000 * 60 * 60);
        int min = (curTime - hour * (1000 * 60 * 60)) / (1000 * 60);
        int sec = (curTime - hour * (1000 * 60 * 60) - min * (1000 * 60)) / 1000;
        
        string timeString = "";

        if (hour < 10)
        {
            timeString += "0" + hour;
        }
        else
        {
            timeString += hour.ToString();
        }

        timeString += ":";

        if (min < 10)
        {
            timeString += ("0" + min);
        }
        else
        {
            timeString += min.ToString();
        }

        timeString += ":";

        if (sec < 10)
        {
            timeString += ("0" + sec);
        }
        else
        {
            timeString += sec.ToString();
        }

        return timeString;
    }

    /// <summary>
    /// 将时间转换为字符串 格式为 00:00
    /// </summary>
    /// <param name="curTime">时间以毫秒为单位</param>
    public static string GetCountDown(float curTime)
    {
        int time = Convert.ToInt32(Mathf.Ceil(curTime));
        int min = time / (1000 * 60);
        int sec = (time - min * (1000 * 60)) / 1000;
        int msec = (time - min*(1000)*60 - sec*1000)/10;
        string timeString = "";
        if (min < 10)
        {
            timeString += ("0" + min);
        }
        else
        {
            timeString += min.ToString();
        }

        timeString += ":";

        if (sec < 10)
        {
            timeString += ("0" + sec);
        }
        else
        {
            timeString += sec.ToString();
        }

        if (min == 0 && sec < 10)
        {
            timeString += ":";
            if (msec < 10)
            {
                timeString += "0" + msec;
            }
            else
            {
                timeString += msec.ToString();
            }
        }

        return timeString;
    }

    /// <summary>
    /// 将时间转换为字符串 格式为 00:00如果小于10秒 格式为 00:00:00
    /// </summary>
    /// <param name="curTime">时间以毫秒为单位</param>
    public static string GetTowerCountDown(float curTime)
    {
        string timeString = "";
        var ts=TimeSpan.FromMilliseconds(curTime);
        if (ts.Minutes < 10)
        {
            timeString = timeString+ "0" + ts.Minutes.ToString();
        }
        else
        {
            timeString += ts.Minutes.ToString();
        }

        timeString += " ";
        if (ts.Seconds < 10)
        {
            timeString += ("0" + ts.Seconds);
        }
        else
        {
            timeString += ts.Seconds;
        }

        return timeString;
    }

}
