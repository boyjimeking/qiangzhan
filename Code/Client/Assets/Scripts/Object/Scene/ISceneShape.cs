using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*

    (+Y)
     |   (+Z)
     |  /
     | /
     0-------(+X)

 */

// 形状类型
public enum ShapeType : int
{
	ShapeType_Invalid = -1,         // 无效
	ShapeType_Round = 0,            // 圆
	ShapeType_Rect = 1,             // 矩形
	ShapeType_Polygon = 2,          // 多段线
    ShapeType_Line = 3,
}

// 形状参数
public class SceneShapeParam
{
	public ShapeType mType;

	public List<float> mParams = new List<float>();
}

// 形状方法
public class SceneShapeUtilities
{
	// 根据形状参数、位置、弧度 创建形状
	static public SceneShape Create(SceneShapeParam param, Vector2 pos, float radians)
	{
		if (param.mType == ShapeType.ShapeType_Round)
		{
			if (param == null || param.mParams == null || param.mParams.Count < 1)
			{
				return null;
			}

			return new SceneShapeRound(pos, param.mParams[0]);
		}
		else if (param.mType == ShapeType.ShapeType_Rect)
		{
			if (param == null || param.mParams == null || param.mParams.Count < 2)
			{
				return null;
			}

			if ((int)(radians * 100000) % (int)(Mathf.PI * 100000 * 2) == 0)
			{
				if ((int)(radians * 100000) % (int)(Mathf.PI * 100000) == 0)
				{
					return new SceneShapeRect(pos, param.mParams[0], param.mParams[1]);
				}

				return new SceneShapeRect(pos, param.mParams[1], param.mParams[0]);
			}
			else
			{
				SceneShapeRect rect = new SceneShapeRect(pos, param.mParams[0], param.mParams[1]);

				return rotate(rect, pos, radians * Mathf.Rad2Deg);
			}
		}
		else if (param.mType == ShapeType.ShapeType_Polygon)
		{
			if (param == null || param.mParams == null || param.mParams.Count < 2 || param.mParams.Count % 2 != 0)
			{
				return null;
			}

			SceneShapePolygon shape = new SceneShapePolygon();
			for (int i = 0; i < (int)(param.mParams.Count * 0.5); ++i)
			{
				shape.PushbackVector2(new Vector2(param.mParams[2 * i], param.mParams[2 * i + 1]));
			}

			return rotate(ref shape, pos, radians * Mathf.Rad2Deg);
		}

		GameDebug.LogError("SceneShape.Creat() 未知的图形类别");

		return null;
	}

	// 圆与圆重叠(相交或包含)
	static public bool overlap(SceneShapeRound round1, SceneShapeRound round2)
	{
		return (new Vector2(round2.mCenter.x - round1.mCenter.x, round2.mCenter.y - round1.mCenter.y).SqrMagnitude() <= Mathf.Pow(round1.mRadius + round2.mRadius, 2));
	}

	// 圆与矩形重叠(相交或包含)
	static public bool overlap(SceneShapeRound round, SceneShapeRect rect)
	{
		if (rect.contains(round.mCenter))
		{
			return true;
		}

		if (round.contains(new Vector2(rect.mLeft, rect.mBottom))
			|| round.contains(new Vector2(rect.mLeft, rect.mTop))
			|| round.contains(new Vector2(rect.mRight, rect.mBottom))
			|| round.contains(new Vector2(rect.mRight, rect.mTop)))
		{
			return true;
		}

		float x = rect.mLeft - round.mCenter.x;
		if (Mathf.Abs(x) > round.mRadius)
		{
			return false;
		}

		if (rect.contains(new Vector2(round.mCenter.x + x, round.mCenter.y)))
		{
			return true;
		}

		x = rect.mRight - round.mCenter.x;
		if (Mathf.Abs(x) > round.mRadius)
		{
			return false;
		}

		if (rect.contains(new Vector2(round.mCenter.x + x, round.mCenter.y)))
		{
			return true;
		}

		x = rect.mBottom - round.mCenter.y;
		if (Mathf.Abs(x) > round.mRadius)
		{
			return false;
		}

		if (rect.contains(new Vector2(round.mCenter.x, round.mCenter.y + x)))
		{
			return true;
		}

		x = rect.mTop - round.mCenter.y;
		if (Mathf.Abs(x) > round.mRadius)
		{
			return false;
		}

		if (rect.contains(new Vector2(round.mCenter.x, round.mCenter.y + x)))
		{
			return true;
		}

		return false;
	}

	// 圆与多边形重叠(相交或包含)
	static public bool overlap(SceneShapeRound round, SceneShapePolygon polygon)
	{
		if (polygon.contains(round.mCenter))
		{
			return true;
		}

		for (int i = 0; i < polygon.mPts.Count; ++i)
		{
			Vector2 pt1 = polygon.mPts[i];
			Vector2 pt2 = polygon.mPts[((i + 1) % polygon.mPts.Count)];

			float minDist = Utility.minDist(round.mCenter, pt1, pt2);

			if(minDist <= round.mRadius)
			{
				return true;
			}
		}

		return false;
	}

	// 矩形与矩形重叠(相交或包含)
	static public bool overlap(SceneShapeRect rect1, SceneShapeRect rect2)
	{
		if(Mathf.Abs((rect1.mLeft + rect1.mRight) * 0.5f - (rect2.mLeft + rect2.mRight) * 0.5f) < ((rect1.mRight + rect2.mRight - rect1.mLeft - rect2.mLeft) * 0.5f)
			&& Mathf.Abs((rect1.mTop + rect1.mBottom) * 0.5f - (rect2.mTop + rect2.mBottom) * 0.5f) < ((rect1.mTop + rect2.mTop - rect1.mBottom - rect2.mBottom) * 0.5f))
		{
			return true;
		}

		return false;
	}

	// 矩形与多边形重叠(相交或包含)
	static public bool overlap(SceneShapeRect rect, SceneShapePolygon polygon)
	{
		// 矩形有顶点在多边形内 一定相交
		if (polygon.contains(rect.leftTop()))
			return true;

		if (polygon.contains(rect.rightTop()))
			return true;

		if (polygon.contains(rect.rightBottom()))
			return true;

		if (polygon.contains(rect.leftBottom()))
			return true;

		for (int i = 0; i < polygon.mPts.Count; ++i)
		{
			Vector2 pt1 = polygon.mPts[i];
			Vector2 pt2 = polygon.mPts[((i + 1) % polygon.mPts.Count)];

			// 多边形有顶点在矩形内 一定相交
			if (rect.contains(pt1))
				return true;

			// 多边形与矩形有边相交 两图形一定相交
			if (Utility.isSegmentIntersect(pt1, pt2, rect.leftTop(), rect.rightTop()))
				return true;

			if (Utility.isSegmentIntersect(pt1, pt2, rect.rightTop(), rect.rightBottom()))
				return true;

			if (Utility.isSegmentIntersect(pt1, pt2, rect.rightBottom(), rect.leftBottom()))
				return true;

			if (Utility.isSegmentIntersect(pt1, pt2, rect.leftBottom(), rect.leftTop()))
				return true;
		}

		return false;
	}

	// 多边形与多边形重叠(相交或包含)
	static public bool overlap(SceneShapePolygon polygon1, SceneShapePolygon polygon2)
	{
		for (int i = 0; i < polygon1.mPts.Count; ++i)
		{ 
			for (int j = i + 1; j < polygon1.mPts.Count; ++j)
			{ 
				for (int s = 0; s < polygon2.mPts.Count; ++s)
				{ 
					for (int t = s + 1; t < polygon2.mPts.Count; ++t)
					{
						if (Utility.isSegmentIntersect(polygon1.mPts[i], polygon1.mPts[j], polygon2.mPts[s], polygon2.mPts[t]))
						{
							return false;
						}
					} 
				} 
			} 
		}

		return (!polygon2.contains(polygon1.mPts[polygon1.mPts.Count - 1]) && !polygon1.contains(polygon2.mPts[polygon2.mPts.Count - 1]));
	}

    static public bool overlap(SceneShapeLine shape1, SceneShapeLine shape2)
    {
        return Geometryalgorithm2d.lineseg_intersect_lineseg(
            new LineSegf(new Vector2f(shape1.mVertex1.x, shape1.mVertex1.y), new Vector2f(shape1.mVertex2.x, shape1.mVertex2.y)),
            new LineSegf(new Vector2f(shape2.mVertex1.x, shape2.mVertex1.y), new Vector2f(shape2.mVertex2.x, shape2.mVertex2.y)));

    }

    static public bool overlap(SceneShapeLine shape1, SceneShapeRect shape2)
    {
        return Geometryalgorithm2d.lineseg_intersect_rectangle(
             new LineSegf(new Vector2f(shape1.mVertex1.x, shape1.mVertex1.y), new Vector2f(shape1.mVertex2.x, shape1.mVertex2.y)),
             new Rectanglef(new Vector2f(shape2.leftBottom().x, shape2.leftBottom().y), new Vector2f(shape2.rightTop().x, shape2.rightTop().y)));
    }

    static public bool overlap(SceneShapeLine shape1, SceneShapeRound shape2)
    {
        LineSegf line = new LineSegf(new Vector2f(shape1.mVertex1.x, shape1.mVertex1.y), new Vector2f(shape1.mVertex2.x, shape1.mVertex2.y));

        Vector2f tarPos = Geometryalgorithm2d.point_line_intersect(new Vector2f(shape2.mCenter.x, shape2.mCenter.y), line);
        return (tarPos.Subtract(new Vector2f(shape2.mCenter.x, shape2.mCenter.y))).length() <= shape2.mRadius;
    }

    static public bool overlap(SceneShapeLine shape1, SceneShapePolygon shape2)
    {
        LineSegf line =  new LineSegf(new Vector2f(shape1.mVertex1.x, shape1.mVertex1.y), new Vector2f(shape1.mVertex2.x, shape1.mVertex2.y));
        for(int i = 1; i < shape2.mPts.Count; i++)
        {
            Vector2f p1 = new Vector2f(shape2.mPts[i - 1].x, shape2.mPts[i - 1].y);
            Vector2f p2 = new Vector2f(shape2.mPts[i].x, shape2.mPts[i].y);

            if (Geometryalgorithm2d.lineseg_intersect_lineseg(line, new LineSegf(p1, p2)))
                return true;
        }
            
        return false;
    }

	// 旋转图形
	static public SceneShapePolygon rotate(SceneShapeRect rect, Vector2 pos, float angles)
	{
		SceneShapePolygon shape = new SceneShapePolygon();
		Vector3 lt = new Vector3(rect.mLeft, 0.0f, rect.mTop);
		Vector3 rt = new Vector3(rect.mRight, 0.0f, rect.mTop);
		Vector3 lb = new Vector3(rect.mLeft, 0.0f, rect.mBottom);
		Vector3 rb = new Vector3(rect.mRight, 0.0f, rect.mBottom);

		Quaternion q = new Quaternion();
		q.eulerAngles = new Vector3(0, angles, 0);

		Vector3 center = new Vector3(pos.x, 0.0f, pos.y);

		Vector3 ltnew = (q * (lt - center)) + center;
		Vector3 rtnew = (q * (rt - center)) + center;
		Vector3 lbnew = (q * (lb - center)) + center;
		Vector3 rbnew = (q * (rb - center)) + center;

		shape.PushbackVector2(new Vector2(ltnew.x, ltnew.z));
		shape.PushbackVector2(new Vector2(rtnew.x, rtnew.z));
		shape.PushbackVector2(new Vector2(rbnew.x, rbnew.z));
		shape.PushbackVector2(new Vector2(lbnew.x, lbnew.z));

		return shape;
	}

	// 旋转图形
	static public SceneShapePolygon rotate(ref SceneShapePolygon polygon, Vector2 pos, float angles)
	{
		//SceneShapePolygon newPolygon = new SceneShapePolygon(polygon);
		//return newPolygon.rotate(pos, angles);
		return polygon.rotate(pos, angles);
	}

	// 用新的位置、弧度更新图形
	static public SceneShape refresh(ref SceneShape shape, SceneShapeParam param, Vector2 pos, float radians)
	{
		if(shape == null || shape.refresh(param, pos, radians) == null)
		{
			return shape = Create(param, pos, radians);
		}

		return shape;
	}
}

// 形状
public class SceneShape
{
    virtual public ShapeType GetShapeType()
    {
        return ShapeType.ShapeType_Invalid;
    }

	// 用新的位置、弧度更新图形
	virtual public SceneShape refresh(SceneShapeParam param, Vector2 pos, float radians)
	{
		return this;
	}

    // 包含点
    virtual public bool contains(Vector2 pt)
    {
        return false;
    }

	// 相交
	virtual public bool intersect(SceneShape shape)
	{
		return false;
	}
}

// 矩形
public class SceneShapeRect : SceneShape
{
    public float mLeft = 0.0f;
    public float mTop = 0.0f;
    public float mRight = 0.0f;
    public float mBottom = 0.0f;

    public SceneShapeRect()
    {
    }

    public SceneShapeRect(SceneShapeRect rect)
    {
        mLeft = rect.mLeft;
        mTop = rect.mTop;
        mRight = rect.mRight;
        mBottom = rect.mBottom;
    }

    public SceneShapeRect(float left, float top, float right, float bottom)
    {
        mLeft = left;
        mTop = top;
        mRight = right;
        mBottom = bottom;
    }

	public SceneShapeRect(Vector2 center, float length, float width)
	{
		mLeft = center.x - (length * 0.5f);
		mTop = center.y + (width * 0.5f);
		mRight = center.x + (length * 0.5f);
		mBottom = center.y - (width * 0.5f);
	}

    override public ShapeType GetShapeType()
    {
        return ShapeType.ShapeType_Rect;
    }

    public Vector2 leftTop()
    {
        return new Vector2(mLeft, mTop);
    }

    public Vector2 leftBottom()
    {
        return new Vector2(mLeft, mBottom);
    }

    public Vector2 rightTop()
    {
        return new Vector2(mRight, mTop);
    }

    public Vector2 rightBottom()
    {
        return new Vector2(mRight, mBottom);
    }

	// 用新的位置、弧度更新图形
	override public SceneShape refresh(SceneShapeParam param, Vector2 pos, float radians)
	{
		if (param == null || param.mParams == null || param.mParams.Count < 2)
		{
			return null;
		}

		// 形状已经变了 返回null 由Utility重新生成图形
		if (param.mType != ShapeType.ShapeType_Rect)
			return null;

		// 旋转后还是矩形 可以用该矩形刷新
		if ((int)(radians * 100000) % (int)(Mathf.PI * 100000 * 2) == 0)
		{
			if ((int)(radians * 100000) % (int)(Mathf.PI * 100000) == 0)
			{
				mLeft = pos.x - (param.mParams[0] * 0.5f);
				mTop = pos.y + (param.mParams[1] * 0.5f);
				mRight = pos.x + (param.mParams[0] * 0.5f);
				mBottom = pos.y - (param.mParams[1] * 0.5f);
			}
			else
			{
				mLeft = pos.x - (param.mParams[1] * 0.5f);
				mTop = pos.y + (param.mParams[0] * 0.5f);
				mRight = pos.x + (param.mParams[1] * 0.5f);
				mBottom = pos.y - (param.mParams[0] * 0.5f);
			}

			return this;
		}

		// 旋转后不能用矩形表示 返回null 由Utility重新生成多段线图形
		return null;
	}

    // 包含点
    override public bool contains(Vector2 pt)
    {
        if (pt.x < mLeft || pt.x > mRight || pt.y > mTop || pt.y < mBottom)
            return false;

        return true;
    }

    // 包含矩形
    public bool contains(SceneShapeRect rect)
    {
        return rect.mLeft >= mLeft && rect.mRight <= mRight && rect.mTop <= mTop && rect.mBottom >= mBottom;
    }

	// 相交
	override public bool intersect(SceneShape shape)
	{
		if (typeof(SceneShapeRect).IsAssignableFrom(shape.GetType()))
		{
			return SceneShapeUtilities.overlap(this, shape as SceneShapeRect);
		}
		else if (typeof(SceneShapeRound).IsAssignableFrom(shape.GetType()))
		{
			return SceneShapeUtilities.overlap(shape as SceneShapeRound, this);
		}
		else if (typeof(SceneShapePolygon).IsAssignableFrom(shape.GetType()))
		{
			return SceneShapeUtilities.overlap(this, shape as SceneShapePolygon);
		}
        else if(typeof(SceneShapeLine).IsAssignableFrom(shape.GetType()))
        {
            return SceneShapeUtilities.overlap(shape as SceneShapeLine, this);
        }

		GameDebug.LogError("SceneShapeRect.intersect() 未知的图形类别");

		return false;
	}
}

// 圆
public class SceneShapeRound : SceneShape
{
    public Vector2 mCenter = new Vector2(0.0f, 0.0f);
    public float mRadius = 0.0f;

    public SceneShapeRound()
    {

    }

    public SceneShapeRound(SceneShapeRound round)
    {
        mCenter = round.mCenter;
        mRadius = round.mRadius;
    }

    public SceneShapeRound(Vector2 center, float radius)
    {
        mCenter = center;
        mRadius = radius;
    }

    override public ShapeType GetShapeType()
    {
        return ShapeType.ShapeType_Round;
    }

	// 用新的位置、弧度更新图形
	override public SceneShape refresh(SceneShapeParam param, Vector2 pos, float radians)
	{
		if (param == null || param.mParams == null || param.mParams.Count < 1)
		{
			return null;
		}

		// 形状已经变了 返回null 由Utility重新生成图形
		if (param.mType != ShapeType.ShapeType_Round)
			return null;

		mCenter = pos;
		mRadius = param.mParams[0];

		return this;
	}

    // 包含点
    override public bool contains(Vector2 pt)
    {
        return ((mCenter - pt).SqrMagnitude() <= mRadius * mRadius);
    }

	// 相交
	override public bool intersect(SceneShape shape)
	{
		if (typeof(SceneShapeRect).IsAssignableFrom(shape.GetType()))
		{
			return SceneShapeUtilities.overlap(this, shape as SceneShapeRect);
		}
		else if (typeof(SceneShapeRound).IsAssignableFrom(shape.GetType()))
		{
			return SceneShapeUtilities.overlap(this, shape as SceneShapeRound);
		}
		else if (typeof(SceneShapePolygon).IsAssignableFrom(shape.GetType()))
		{
			return SceneShapeUtilities.overlap(this, shape as SceneShapePolygon);
		}
        else if(typeof(SceneShapeLine).IsAssignableFrom(shape.GetType()))
        {
            return SceneShapeUtilities.overlap(shape as SceneShapeLine, this);
        }

		GameDebug.LogError("SceneShapeRound.intersect() 未知的图形类别");

		return false;
	}
}

// 多段线
public class SceneShapePolygon : SceneShape
{
    public List<Vector2> mPts = new List<Vector2>();

    public SceneShapePolygon()
    {

    }

    public SceneShapePolygon(SceneShapePolygon polygon)
    {
        mPts.Clear();

        foreach(Vector2 pt in polygon.mPts)
        {
            mPts.Add(pt);
        }
    }

    public SceneShapePolygon(List<Vector2> list)
    {
        mPts.Clear();

        foreach(Vector2 pt in list)
        {
            mPts.Add(pt);
        }
    }

    override public ShapeType GetShapeType()
    {
        return ShapeType.ShapeType_Polygon;
    }

	// 用新的位置、弧度更新图形
	override public SceneShape refresh(SceneShapeParam param, Vector2 pos, float radians)
	{
		if (param == null || param.mParams == null || param.mParams.Count < 2 || param.mParams.Count % 2 != 0)
		{
			return null;
		}

		// 形状已经变了 返回null 由Utility重新生成图形
		if (param.mType != ShapeType.ShapeType_Polygon)
			return null;

		mPts.Clear();

		for (int i = 0; i < (int)(param.mParams.Count * 0.5); ++i)
		{
			PushbackVector2(new Vector2(param.mParams[2 * i], param.mParams[2 * i + 1]));
		}
		
		rotate(pos, radians * Mathf.Rad2Deg);

		return this;
	}

    // 追加点
    public void PushbackVector2(Vector2 pt)
    {
        mPts.Add(pt);
    }

	// 旋转 会改变该图形 SceneShapeUtilities中的旋转不会改变原图形
	public SceneShapePolygon rotate(Vector2 pos, float angles)
	{
		Quaternion q = new Quaternion();
		q.eulerAngles = new Vector3(0, angles, 0);

		Vector3 pos3 = new Vector3(pos.x, 0.0f, pos.y);

		for(int i = 0; i < mPts.Count; ++i)
		{
			Vector3 pt3 = new Vector3(mPts[i].x, 0.0f, mPts[i].y);
			Vector3 new3 = (q * (pt3 - pos3)) + pos3;
			mPts[i] = new3;
		}

		return this;
	}

    // 包含点
    override public bool contains(Vector2 pt)
    {
        return Geometryalgorithm2d.IsPointInPolygon(mPts, pt);
        //float angle = 0;
        //bool inside = false;

        //for (int i = 0, j = mPts.Count - 1; i < mPts.Count; j = i++)
        //{
        //    if (mPts[i].x == pt.x && mPts[i].y == pt.y) // 是否在顶点上
        //    {
        //        inside = true;
        //        break;
        //    }
        //    else if (Utility.isPointInSegment(pt, mPts[i], mPts[j])) // 是否在边界线上
        //    {
        //        inside = true;
        //        break;
        //    }

        //    float x1, y1, x2, y2;
        //    x1 = mPts[i].x - pt.x;
        //    y1 = mPts[i].y - pt.y;
        //    x2 = mPts[j].x - pt.x;
        //    y2 = mPts[j].y - pt.y;

        //    // 计算角度和
        //    float radian = Mathf.Atan2(y1, x1) - Mathf.Atan2(y2, x2);
        //    radian = Mathf.Abs(radian);
        //    if (radian > Mathf.PI)
        //        radian = Mathf.PI * 2 - radian;
        //    angle += radian;
        //}

        //if (Mathf.Abs((float)(6.28318530717958647692 - angle)) < 1e-6)
        //    inside = true;

        //return inside;
    }

	// 相交
	override public bool intersect(SceneShape shape)
	{
		if (typeof(SceneShapeRect).IsAssignableFrom(shape.GetType()))
		{
			return SceneShapeUtilities.overlap(shape as SceneShapeRect, this);
		}
		else if (typeof(SceneShapeRound).IsAssignableFrom(shape.GetType()))
		{
			return SceneShapeUtilities.overlap(shape as SceneShapeRound, this);
		}
		else if (typeof(SceneShapePolygon).IsAssignableFrom(shape.GetType()))
		{
			return SceneShapeUtilities.overlap(this, shape as SceneShapePolygon);
		}
        else if(typeof(SceneShapeLine).IsAssignableFrom(shape.GetType()))
        {
            return SceneShapeUtilities.overlap(shape as SceneShapeLine, this);
        }

		GameDebug.LogError("SceneShapePolygon.intersect() 未知的图形类别");

		return false;
	}
}

// 矩形
public class SceneShapeLine : SceneShape
{
    public Vector2 mVertex1 = new Vector2();
    public Vector2 mVertex2 = new Vector2();

    public SceneShapeLine()
    {
    }

    public SceneShapeLine(Vector2 p1, Vector2 p2)
    {
        mVertex1 = p1;
        mVertex2 = p2;
    }

    override public ShapeType GetShapeType()
    {
        return ShapeType.ShapeType_Line;
    }

    // 用新的位置、弧度更新图形
    override public SceneShape refresh(SceneShapeParam param, Vector2 pos, float radians)
    {
        return null;
    }

    // 包含点
    override public bool contains(Vector2 pt)
    {
        return Utility.isPointInSegment(pt, mVertex1, mVertex2);
    }

    // 相交
    override public bool intersect(SceneShape shape)
    {
        if (typeof(SceneShapeRect).IsAssignableFrom(shape.GetType()))
        {
            return SceneShapeUtilities.overlap(this, shape as SceneShapeRect);
        }
        else if (typeof(SceneShapeRound).IsAssignableFrom(shape.GetType()))
        {
            return SceneShapeUtilities.overlap(this, shape as SceneShapeRound);
        }
        else if (typeof(SceneShapePolygon).IsAssignableFrom(shape.GetType()))
        {
            return SceneShapeUtilities.overlap(this, shape as SceneShapePolygon);
        }
        else if(typeof(SceneShapeLine).IsAssignableFrom(shape.GetType()))
        {
            return SceneShapeUtilities.overlap(this, shape as SceneShapeLine);
        }

        GameDebug.LogError("SceneShapeLine.intersect() 未知的图形类别");
        return false;
    }
}