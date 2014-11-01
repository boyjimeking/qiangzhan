
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct Vector3f
{
    public float x;
    public float y;
    public float z;

    public Vector3f(float _x, float _y, float _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public static Vector3f operator -(Vector3f a, Vector3f b)
    {
        return new Vector3f(a.x - b.x, a.y - b.y, a.z - b.z);
    }
    public static Vector3f operator *(float d, Vector3f a)
    {
        return new Vector3f(d * a.x, d * a.y, d * a.z);
    }
    public static Vector3f operator *(Vector3f a, float d)
    {
        return new Vector3f(d * a.x, d * a.y, d * a.z);
    }
    public static Vector3f operator /(Vector3f a, float d)
    {
        return new Vector3f(a.x / d , a.y / d , a.z / d );
    }
    public static Vector3f operator +(Vector3f a, Vector3f b)
    {
        return new Vector3f(a.x + b.x, a.y + b.y, a.z + b.z);
    }
    public Vector3f Subtract(Vector3f v)
    {
        return new Vector3f(x - v.x, y - v.y, z - v.z);
    }

    public Vector3f MultiplyBy(float value)
    {
        x *= value;
        y *= value;
        z *= value;

        return this;
    }

    public Vector3f Add(Vector3f v)
    {
        return new Vector3f(x + v.x, y + v.y, z + v.z);
    }

    public float Length()
    {
        return (float)Math.Sqrt(x * x + y * y + z * z);
    }

    public void Normalize()
    {
        float mag = x * x + y * y + z * z;

        if (mag > 0.0f)    // check for divide-by-zero
        {
            float one_over_mag = 1.0f / (float)Math.Sqrt(mag);

            x *= one_over_mag;
            y *= one_over_mag;
            z *= one_over_mag;
        }
    }

    public void Zero()
    {
        x = y = z = 0;
    }

    public Vector3f Cross(Vector3f v)
    {
        return new Vector3f(y * v.z - z * v.y, z * v.x - x * v.z, x * v.y - y * v.x);
    }

    public float Dot(Vector3f v)
    {
        return x * v.x + y * v.y + z * v.z;
    }
}

public struct Vector2f
{
	public float x;
	public float y;

	public Vector2f(float x_, float y_)
    {
        x = x_;
        y = y_;
    }

	public float length()
	{
        return (float)Math.Sqrt(x * x + y * y);
	}

    public Vector2f Subtract(Vector2f v)
    {
        return new Vector2f(x - v.x, y - v.y);
    }

    public Vector2f MultiplyBy(float value)
    {
        x *= value;
        y *= value;

        return this;
    }

    public Vector2f Add(Vector2f v)
    {
        return new Vector2f(x + v.x, y + v.y);
    }

    public void Normalize()
    {
        float mag = x * x + y * y;

        if (mag > 0.0f)    // check for divide-by-zero
        {
            float one_over_mag = 1.0f / (float)Math.Sqrt(mag);

            x *= one_over_mag;
            y *= one_over_mag;
        }
    }

    public bool Equal(Vector2f rhs)
    {
        return this.x == rhs.x && this.y == rhs.y;
    }
};

public struct LineSegf
{
    public Vector2f vertex0;
    public Vector2f vertex1;

	public LineSegf(Vector2f start, Vector2f end)
	{
        vertex0.x = start.x;
        vertex0.y = start.y;

        vertex1.x = end.x;
        vertex1.y = end.y;
	}

	public float length()
	{
        float x = vertex1.x - vertex0.x;
        float y = vertex1.y - vertex0.y;
        Vector2f v = new Vector2f(x, y);
        return v.length();
	}

    public bool Equal(LineSegf rhs)
    {
        return (this.vertex0.Equal(rhs.vertex0) && this.vertex1.Equal(rhs.vertex1)) || (this.vertex0.Equal(rhs.vertex1) && this.vertex1.Equal(rhs.vertex0));
    }

};

public struct Trianglef
{
    public Vector2f v0;
    public Vector2f v1;
    public Vector2f v2;

	public LineSegf side(uint index)
	{
		switch(index % 3)
		{
		case 0:
			return new LineSegf(v0, v1);
		case 1:
			return new LineSegf(v1, v2);
		case 2:
			return new LineSegf(v2, v0);
		}

		return new LineSegf();
	}
};

public struct Rectanglef
{
    public Vector2f minv;
    public Vector2f maxv;

	public Rectanglef(Vector2f minver, Vector2f maxver)
	{
        minv = minver;
        maxv = maxver;
	}

	public Vector2f point(uint index)
	{
		switch(index % 4)
		{
		case 0:
			return new Vector2f(minv.x, minv.y);
		case 1:
			return new Vector2f(minv.x, maxv.y);
		case 2:
			return new Vector2f(maxv.x, maxv.y);
		case 3:
			return new Vector2f(maxv.x, minv.y);
		}

		return minv;
	}

	public LineSegf side(int index)
	{
		switch(index % 4)
		{
		case 0:
			return new LineSegf(point(0), point(1));
		case 1:
			return new LineSegf(point(1), point(2));
		case 2:
			return new LineSegf(point(2), point(3));
		case 3:
			return new LineSegf(point(3), point(0));
		}

		return new LineSegf();
	}
};



public class Geometryalgorithm2d
{

    public enum  POINT_LINESEG_ENUM
    {
        POINT_LINESEG_LEFT, 
        POINT_LINESEG_RIGHT, 
        POINT_LINESEG_ON
    };

    static public POINT_LINESEG_ENUM point_lineseg_direction(Vector2f pt, Vector2f l0, Vector2f l1)
    {
        float exp = (float)(1e-5);
        Vector2f a = new Vector2f(l1.x - l0.x, l1.y - l0.y);
        Vector2f b = new Vector2f(pt.x - l0.x, pt.y - l0.y);
        float v = vector_ccw_vector(a, b);
        if (v > exp)
            return POINT_LINESEG_ENUM.POINT_LINESEG_RIGHT;
        else if (v < -exp)
            return POINT_LINESEG_ENUM.POINT_LINESEG_LEFT;

        return POINT_LINESEG_ENUM.POINT_LINESEG_ON;
    }

    static public float vector_ccw_vector(Vector2f vec1, Vector2f vec2)
    {
        return vec1.x * vec2.y - vec2.x * vec1.y;
    }

    static public bool point_in_lineseg(Vector2f pt, LineSegf line)
    {
        float f1 = (new Vector2f(line.vertex0.x - pt.x, line.vertex0.y - pt.y)).length();
        float f2 = (new Vector2f(line.vertex1.x - pt.x, line.vertex1.y - pt.y)).length();

        float v = Math.Abs(f1 + f2 - line.length());
        return v < 1E-5f; 
    }

    static public Rectanglef lineseg_box(LineSegf l1)
    {
        float minx = l1.vertex0.x < l1.vertex1.x ? l1.vertex0.x : l1.vertex1.x;
        float maxx = l1.vertex0.x > l1.vertex1.x ? l1.vertex0.x : l1.vertex1.x;
        float miny = l1.vertex0.y < l1.vertex1.y ? l1.vertex0.y : l1.vertex1.y;
        float maxy = l1.vertex0.y > l1.vertex1.y ? l1.vertex0.y : l1.vertex1.y;

        return new Rectanglef(new Vector2f(minx, miny), new Vector2f(maxx, maxy));
    }

    static public bool lineseg_intersect_lineseg(LineSegf l1, LineSegf l2)
    {
        Rectanglef r1 = lineseg_box(l1);
        Rectanglef r2 = lineseg_box(l2);

        if (r1.minv.x > r2.maxv.x || r1.maxv.x < r2.minv.x ||
            r1.minv.y > r2.maxv.y || r1.maxv.y < r2.minv.y)
            return false;

        float d1 = point_lineseg_side(l1.vertex0, l2);
        float d2 = point_lineseg_side(l1.vertex1, l2);
        float d3 = point_lineseg_side(l2.vertex0, l1);
        float d4 = point_lineseg_side(l2.vertex1, l1);

        if (d1 * d2 < 0 && d3 * d4 < 0)
            return true;
        else if (d1 == 0 && point_in_lineseg(l1.vertex0, l2))
            return true;
        else if (d2 == 0 && point_in_lineseg(l1.vertex1, l2))
            return true;
        else if (d3 == 0 && point_in_lineseg(l2.vertex0, l1))
            return true;
        else if (d4 == 0 && point_in_lineseg(l2.vertex1, l1))
            return true;

        return false;
    }

    static public bool point_in_triangle(Vector2f pt, Trianglef triangle)
    {
        float minX = triangle.v0.x;
        float minY = triangle.v0.y;
        float maxX = triangle.v0.x;
        float maxY = triangle.v0.y;

        minX = minX < triangle.v1.x ? minX : triangle.v1.x;
        minX = minX < triangle.v2.x ? minX : triangle.v2.x;

        minY = minY < triangle.v1.y ? minY : triangle.v1.y;
        minY = minY < triangle.v2.y ? minY : triangle.v2.y;

        maxX = maxX > triangle.v1.x ? maxX : triangle.v1.x;
        maxX = maxX > triangle.v2.x ? maxX : triangle.v2.x;

        maxY = maxY > triangle.v1.y ? maxY : triangle.v1.y;
        maxY = maxY > triangle.v2.y ? maxY : triangle.v2.y;

        if (pt.x < minX || pt.x > maxX || pt.y < minY || pt.y > maxY)
            return false;

        int count = 0;
        for(uint i = 0; i < 3; i++)
        {
            LineSegf line = triangle.side(i);

            if (point_in_lineseg(pt, line))
                return true;

            if (point_lineseg_side(pt, line) > 0)
            {
                if (count != 0 && count != 1)
                    return false;

                count = 1;
            }
            else
            {
                if (count != 0 || count != -1)
                    return false;

                count = -1;
            }
        }

        return true;
    }

    static public bool lineseg_intersect_rectangle(LineSegf lineseg, Rectanglef rect)
    {
        if (point_in_rectangle(lineseg.vertex0, rect))
            return true;

        if (point_in_rectangle(lineseg.vertex1, rect))
            return true;

        if (lineseg_intersect_lineseg(lineseg, rect.side(0)))
            return true;

        if (lineseg_intersect_lineseg(lineseg, rect.side(1)))
            return true;

        if (lineseg_intersect_lineseg(lineseg, rect.side(2)))
            return true;

        if (lineseg_intersect_lineseg(lineseg, rect.side(3)))
            return true;

        return false;
    }

    static public bool lineseg_intersect_rectangle_by_triangle(Vector2f p1_l1, Vector2f p2_l1, 
        Vector2f p1_r2, Vector2f p2_r2, 
        Vector2f p3_r2, Vector2f p4_r2)
    {
        Trianglef triangle = new Trianglef();

        triangle.v0 = p1_r2;
        triangle.v1 = p2_r2;
        triangle.v2 = p3_r2;

        if (point_in_triangle(p1_l1, triangle))
            return true;

        triangle.v0 = p1_r2;
        triangle.v1 = p4_r2;
        triangle.v2 = p3_r2;

        if (point_in_triangle(p1_l1, triangle))
            return true;

        triangle.v0 = p1_r2;
        triangle.v1 = p2_r2;
        triangle.v2 = p3_r2;

        if (point_in_triangle(p2_l1, triangle))
            return true;

        triangle.v0 = p1_r2;
        triangle.v1 = p4_r2;
        triangle.v2 = p3_r2;

        if (point_in_triangle(p2_l1, triangle))
            return true;

        LineSegf l1 = new LineSegf();
        LineSegf l2 = new LineSegf();

        l1.vertex0 = p1_l1;
        l1.vertex1 = p2_l1;

        l2.vertex0 = p1_r2;
        l2.vertex1 = p2_r2;

        if (lineseg_intersect_lineseg(l1, l2))
            return true;

        l2.vertex0 = p2_r2;
        l2.vertex1 = p3_r2;

        if (lineseg_intersect_lineseg(l1, l2))
            return true;

        l2.vertex0 = p3_r2;
        l2.vertex1 = p4_r2;

        if (lineseg_intersect_lineseg(l1, l2))
            return true;

        l2.vertex0 = p4_r2;
        l2.vertex1 = p1_r2;

        if (lineseg_intersect_lineseg(l1, l2))
            return true;

        return false;
    }

    static public bool rectangle_intersect_rectangle_by_trangle(Vector2f p1_r1, Vector2f p2_r1, 
        Vector2f p3_r1, Vector2f p4_r1, Vector2f p1_r2, Vector2f p2_r2, 
        Vector2f p3_r2, Vector2f p4_r2)
    {
        if (rectangle_in_rectangle_by_triangle(p1_r1, p2_r1, p3_r1, p4_r1, p1_r2, p2_r2, p3_r2, p4_r2))
            return true;

        if (rectangle_in_rectangle_by_triangle(p1_r2, p2_r2, p3_r2, p4_r2, p1_r1, p2_r1, p3_r1, p4_r1))
            return true;

        if (lineseg_intersect_rectangle_by_triangle(p1_r1, p2_r1, p1_r2, p2_r2, p3_r2, p4_r2))
            return true;

        if (lineseg_intersect_rectangle_by_triangle(p2_r1, p3_r1, p1_r2, p2_r2, p3_r2, p4_r2))
            return true;

        if (lineseg_intersect_rectangle_by_triangle(p3_r1, p4_r1, p1_r2, p2_r2, p3_r2, p4_r2))
            return true;

        if (lineseg_intersect_rectangle_by_triangle(p4_r1, p1_r1, p1_r2, p2_r2, p3_r2, p4_r2))
            return true;

        if (lineseg_intersect_rectangle_by_triangle(p1_r2, p2_r2, p1_r1, p2_r1, p3_r1, p4_r1))
            return true;

        if (lineseg_intersect_rectangle_by_triangle(p2_r2, p3_r2, p1_r1, p2_r1, p3_r1, p4_r1))
            return true;

        if (lineseg_intersect_rectangle_by_triangle(p3_r2, p4_r2, p1_r1, p2_r1, p3_r1, p4_r1))
            return true;

        if (lineseg_intersect_rectangle_by_triangle(p4_r2, p1_r2, p1_r1, p2_r1, p3_r1, p4_r1))
            return true;

        return true;
    }

    static public bool rectangle_intersect_rectangle(Rectanglef rect1, Rectanglef rect2)
    {
        if (rectangle_in_rectangle(rect1, rect2))
            return true;

        if (rectangle_in_rectangle(rect2, rect1))
            return true;

        for (int i = 0; i < 4; i++)
        {
            if (lineseg_intersect_rectangle(rect1.side(i), rect2))
                return true;

            if (lineseg_intersect_rectangle(rect2.side(i), rect1))
                return true;
        }

        return true;
    }

    static public bool lineseg_intersect_triangle(LineSegf lineseg, Trianglef triangle)
    {
        if (point_in_triangle(lineseg.vertex0, triangle))
            return true;

        if (point_in_triangle(lineseg.vertex1, triangle))
            return true;

        for (uint j = 0; j < 3; j++)
        {
            if (lineseg_intersect_lineseg(lineseg, triangle.side(j)))
                return true;
        }

        return false;
    }

    static public bool circle_intersect_triangle(Vector2f center, float radius, Trianglef triangle)
    {
      
        if (Geometryalgorithm2d.point_in_triangle(center, triangle))
            return true;

        for (uint i = 0; i < 3; i++)
        {
            LineSegf line = triangle.side(i);

            Vector2f pt = Geometryalgorithm2d.point_line_intersect(center, line);

            float distance = pt.Subtract(center).length();
            if (distance < radius)
                return true;
        }

        return false;
    }

    static public bool rectangle_in_rectangle_by_triangle(Vector2f p1_r1, Vector2f p2_r1, 
        Vector2f p3_r1, Vector2f p4_r1, Vector2f p1_r2, Vector2f p2_r2, 
        Vector2f p3_r2, Vector2f p4_r2)
    {
        Vector2f center_r1 = new Vector2f((p3_r1.x + p1_r1.x)/2, (p3_r1.y + p1_r1.y)/2);
        Vector2f center_r2 = new Vector2f((p3_r2.x + p1_r2.x)/2, (p3_r2.y + p1_r2.y)/2);
        Trianglef triangle = new Trianglef();
			
        triangle.v0 = p1_r1;
        triangle.v1 = p2_r1;
        triangle.v2 = p3_r1;

	    if (point_in_triangle(center_r2, triangle))
		    return true;
			
        triangle.v0 = p1_r1;
        triangle.v1 = p4_r1;
        triangle.v2 = p3_r1;

	    if (point_in_triangle(center_r2, triangle))
		    return true;

        triangle.v0 = p1_r2;
        triangle.v1 = p2_r2;
        triangle.v2 = p3_r2;
			
	    if (point_in_triangle(center_r1, triangle))
		    return true;
			
        triangle.v0 = p1_r2;
        triangle.v1 = p4_r2;
        triangle.v2 = p3_r2;

	    if (point_in_triangle(center_r1, triangle))
		    return true;
			
	    return false;
    }

    static public bool rectangle_in_rectangle(Rectanglef rect1, Rectanglef rect2)
    {
        return point_in_rectangle(rect1.minv, rect2) && point_in_rectangle(rect1.maxv, rect2);
    }

    static public float point_lineseg_side(Vector2f pt, LineSegf line)
    {
        Vector2f vec1 = new Vector2f(line.vertex1.x - line.vertex0.x, line.vertex1.y - line.vertex0.y);
        Vector2f vec2 = new Vector2f(pt.x - line.vertex1.x, pt.y - line.vertex1.y);
        return vector_ccw_vector(vec1, vec2);
    }

    static public bool point_in_rectangle(Vector2f pt, Rectanglef rect)
    {
        return pt.x >= rect.minv.x && pt.x <= rect.maxv.x && pt.y >= rect.minv.y && pt.y <= rect.maxv.y;
    }

    static public bool lineseg_in_rectangle(LineSegf lineseg, Rectanglef rect)
    {
        return point_in_rectangle(lineseg.vertex0, rect) && point_in_rectangle(lineseg.vertex1, rect);
    }

    static public Vector2f point_line_intersect(Vector2f pt, LineSegf line)
    {
        Vector2f linedir = new Vector2f(line.vertex1.x - line.vertex0.x, line.vertex1.y - line.vertex0.y);
        float length = linedir.length();
        if (length <= 0)
            return line.vertex0;

        linedir.x /= length;
        linedir.y /= length;

        float nu = linedir.x * (pt.x - line.vertex0.x) + linedir.y * (pt.y - line.vertex0.y);

        float n = nu / 1;
        if (n < 0)
            return line.vertex0;
        if (n > length)
            return line.vertex1;

        return new Vector2f(line.vertex0.x + linedir.x * n, line.vertex0.y + linedir.y * n);
    }

    static public Vector3f vector_cross_vector(Vector3f v1, Vector3f v2)
    {
        return new Vector3f(v1.y * v2.z - v1.z * v2.y, v1.z * v2.x - v1.x * v2.z, v1.x * v2.y - v1.y * v2.x);
    }

    static public float vector_dot_vector(Vector3f v1, Vector3f v2)
    {
	    return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
    }

    static public Vector3f point_project_plane(Vector3f v0, Vector3f v1, Vector3f v2, float x, float z)
    {
        Vector3f result = new Vector3f(x, (v0.y + v1.y + v2.y) / 3.0f, z);

        Vector3f orig = new Vector3f(x, 10000, z);
        Vector3f dir = new Vector3f(0, -1.0f, 0);

        float t = 0.0f;
        float u = 0.0f;
        float v = 0.0f;

        Vector3f E1 = v1.Subtract(v0);
        Vector3f E2 = v2.Subtract(v0);
        Vector3f P = dir.Cross(E2);

        float det = E1.Dot(P);

        Vector3f T;
        if (det > 0)
        {
            T = orig.Subtract(v0);
        }
        else
        {
            T = v0.Subtract(orig);
            det = -det;
        }

        if (det < 0.0001f)
            return result;

        u = T.Dot(P);
        //if (u < 0.0f || u > det)
        //    return result;

        Vector3f Q = T.Cross(E1);

        v = dir.Dot(Q);
        //if (v < 0.0f || u + v > det)
        //   return result;
        
        t = E2.Dot(Q);

        float fInvDet = 1.0f / det;
        t *= fInvDet;
        u *= fInvDet;
        v *= fInvDet;

        result.y = orig.y + dir.y * t;
        return result;
    }

    static public bool IsPointInPolygon(List<Vector2> polygon, Vector2 pt, bool side = true)
	{
		int  wn = 0;  
		for(int i = 0; i < polygon.Count; i++)
		{
			Vector2 pt1 = polygon[i];
			Vector2 pt2 = polygon[(i + 1) % (int)polygon.Count];
			
            LineSegf line = new LineSegf(new Vector2f(pt1.x, pt1.y), new Vector2f(pt2.x, pt2.y));

			if(point_in_lineseg(new Vector2f(pt.x, pt.y), line))
                return side;

			if(point_lineseg_side(new Vector2f(pt.x, pt.y), line) > 0)
            {
                if (wn != 0 && wn != 1)
                    return false;

                wn = 1;
            }
            else
            {
                if (wn != 0 && wn != -1)
                    return false;

                wn = -1;
            }
		}

		return true;
	}
};