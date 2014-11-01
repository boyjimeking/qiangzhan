using UnityEngine;
using System.Collections;

public static class Utility
{
	/// <summary>
	/// 获取start和end之间的2D距离的平方.
	/// </summary>
	public static float Distance2DSquared(Vector3 start, Vector3 end)
	{
		start -= end;
		start.y = 0;
		return start.sqrMagnitude;
	}

	/// <summary>
	/// 获取start和end之间的2D距离.
	/// </summary>
	public static float Distance2D(Vector3 start, Vector3 end)
	{
		start -= end;
		start.y = 0;
		return start.magnitude;
	}

	/// <summary>
	/// 通过将Vector3的y值置为0, 构造新的Vector2.
	/// </summary>
	public static Vector2 DeflatePoint(Vector3 point)
	{
		return new Vector2(point.x, point.z);
	}

	/// <summary>
	/// 将Vector2转为Vector3, y值由参数'y'给出.
	/// </summary>
	public static Vector3 InflatePoint(Vector2 point, float y = 0f)
	{
		return new Vector3(point.x, y, point.y);
	}

	/// <summary>
	/// 将点src, 绕pivot旋转radian弧度.
	/// </summary>
	public static Vector3 RotateVectorByRadian(Vector3 src, float radian, Vector3 pivot)
	{
		if (Mathf.Approximately(radian, 0.0f))
			return src;

		src -= pivot;
		Quaternion q = new Quaternion();
		q.eulerAngles = new Vector3(0f, radian * Mathf.Rad2Deg, 0);
		src = q * src;
		return src + pivot;
	}

	/// <summary>
	/// 将点src, 绕pivot旋转angle角度.
	/// </summary>
	public static Vector3 RotateVectorByAngle(Vector3 src, float angle, Vector3 pivot)
	{
		if (Mathf.Approximately(angle, 0.0f))
			return src;

		src -= pivot;
		Quaternion q = new Quaternion();
		q.eulerAngles = new Vector3(0f, angle, 0);
		src = q * src;
		return src + pivot;
	}

	/// <summary>
	///  将2D上的vector3转换为弧度.
	/// </summary>
	/// <param name="dir"></param>
	/// <returns></returns>
	public static float Vector3ToRadian(Vector3 dir, float defaultValue = 0f)
	{
		dir.y = 0;
		if (dir == Vector3.zero)
		{
			// zero的vector3, 无法计算方向.
			return defaultValue;
		}
		return Quaternion.LookRotation(dir).eulerAngles.y * Mathf.Deg2Rad;
	}

	public static float Vector3ToAngles(Vector3 dir)
	{
		dir.y = 0;
		if (dir == Vector3.zero)
		{
			// zero的vector3, 无法计算方向.
			return 0f;
		}
		
		return Quaternion.LookRotation(dir).eulerAngles.y;

	}

	/// <summary>
	/// 将一个弧度, 转换为2D上的Vector3.
	/// 返回值是单位化的(length = 1).
	/// </summary>
	public static Vector3 RadianToVector3(float radian)
	{
		Quaternion q = new Quaternion();
		q.eulerAngles = new Vector3(0f, radian * Mathf.Rad2Deg, 0f);
		return q * Vector3.forward;
	}

	/// <summary>
	/// 取得将startPosition沿radian方向延伸distance距离后的位置坐标.
	/// </summary>
	/// <param name="startPosition">起始位置</param>
	/// <param name="radian">方向(弧度)</param>
	/// <param name="distance"></param>
	/// <returns></returns>
	public static Vector3 MoveVector3Towards(Vector3 startPosition, float radian, float distance)
	{
		if (distance == 0)
			return startPosition;

		Quaternion r = new Quaternion();
		r.eulerAngles = new Vector3(0, radian * Mathf.Rad2Deg, 0);
		return startPosition + r * Vector3.forward * distance;
	}

    public static bool isZero(float v)
    {
	    return (v < 0.00001 && v > -0.00001);
	}

	public static bool isInteger(float v)
	{
		return isZero(Mathf.Floor(v) - v);
	}

	public static uint[] SplitToUnsigned(string str, char sep = '|')
	{
		if (string.IsNullOrEmpty(str))
			return new uint[0];

		string[] array = str.Split(sep);
		uint[] result = new uint[array.Length];

		int index = 0;
		foreach (string s in array)
			result[index++] = uint.Parse(s);

		return result;
	}

    public static float Angle2D(Vector3 target , Vector3 src)
    {
        Vector3 dir = target - src;
        dir.y = 0;

		if (dir == Vector3.zero)
			return 0f;

        Quaternion rot = Quaternion.LookRotation(dir);

        return rot.eulerAngles.y;
    }

    // 已知p在直线ab上 判断是在线段ab上 还是延长线上 (需保证p在直线ab上的前提)
    public static bool isPointInSegmentOrExLine(Vector2 a, Vector2 b, Vector2 p)
    {
	    return Mathf.Min(a.x, b.x) <= p.x && p.x <= Mathf.Max(a.x, b.x) && Mathf.Min(a.y, b.y) <= p.y && p.y <= Mathf.Max(a.y, b.y);
    }

    // 点pt是否在线段pt1pt2上 (无需保证pt在直线pt1pt2上)
    public static bool isPointInSegment(Vector2 pt, Vector2 pt1, Vector2 pt2)
    {
        bool inside = false;

        if (pt.y == pt1.y && pt1.y == pt2.y &&
            ((pt1.x < pt.x && pt.x < pt2.x) ||
            (pt2.x < pt.x && pt.x < pt1.x)))
        {
            inside = true;
        }
        else if (pt.x == pt1.x && pt1.x == pt2.x &&
            ((pt1.y < pt.y && pt.y < pt2.y) ||
            (pt2.y < pt.y && pt.y < pt1.y)))
        {
            inside = true;
        }
        else if (((pt1.y < pt.y && pt.y < pt2.y) ||
            (pt2.y < pt.y && pt.y < pt1.y)) &&
            ((pt1.x < pt.x && pt.x < pt2.x) ||
            (pt2.x < pt.x && pt.x < pt1.x)))
        {
            if (0 == (pt.y - pt1.y) / (pt2.y - pt1.y) - (pt.x - pt1.x) / (pt2.x - pt1.x))
            {
                inside = true;
            }
        }
        return inside;
    }

    // 和的叉积
    public static double getDirection(Vector2 p1, Vector2 p2, Vector2 p3)
    {
	    return (p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x);
    }

    // 线段是否相交
    public static bool isSegmentIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        double d1 = getDirection(p3, p4, p1);
        double d2 = getDirection(p3, p4, p2);
        double d3 = getDirection(p1, p2, p3);
        double d4 = getDirection(p1, p2, p4);

        if (d1 * d2 < 0 && d3 * d4 < 0)
            return true;
        else if (d1 == 0 && isPointInSegmentOrExLine(p3, p4, p1))
            return true;
        else if (d2 == 0 && isPointInSegmentOrExLine(p3, p4, p2))
            return true;
        else if (d3 == 0 && isPointInSegmentOrExLine(p1, p2, p3))
            return true;
        else if (d4 == 0 && isPointInSegmentOrExLine(p1, p2, p4))
            return true;
        else
            return false;
    }

    public static float normalizeRadian(float r)
    {
        r %= (float)Mathf.PI * 2.0f;
        if (r < 0)
            r += (float)Mathf.PI * 2.0f;
        return r;
    }

	// 点到线段的最短距离
	public static float minDist(Vector2 pt, Vector2 pt1, Vector2 pt2)
	{
		double x1, y1, x2, y2, x3, y3;
		double px = pt2.x - pt1.x;
		double py = pt2.y - pt1.y;
		double som = px * px + py * py;
		double u = ((pt.x - pt1.x) * px + (pt.y - pt1.y) * py) / som;
		if (u > 1)
		{
			u = 1;
		}
		if (u < 0)
		{
			u = 0;
		}

		double x = pt1.x + u * px;
		double y = pt1.y + u * py;
		double dx = x - pt.x;
		double dy = y - pt.y;

		return Mathf.Sqrt((float)(dx * dx + dy * dy));
	}

    public static bool InScreen(Vector3 screenpos)
    {
        if (screenpos.x < 0
           || screenpos.y < 0
           || screenpos.x > Screen.width
           || screenpos.y > Screen.height)
        {
            return false;

        }

        return true;
    }
}
