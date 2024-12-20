using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatmullRomCurve
{
    private List<Vector3> points;
    private bool closed;
    private bool needsUpdate;
    private float[] cacheArcLengths;

    public CatmullRomCurve(List<Vector3> points, bool closed)
    {
        this.points = points;
        this.closed = closed;
    }

    public Vector3 GetPoint(float t)
    {
        List<Vector3> points = this.points;
        int count = points.Count;

        float tIndex = (count - (this.closed ? 0 : 1)) * t;
        int intPoint = Mathf.FloorToInt(tIndex);
        float localT = tIndex - intPoint;

        if (this.closed)
        {
            intPoint += intPoint > 0 ? 0 : (Mathf.FloorToInt(Mathf.Abs(intPoint) / points.Count) + 1) * points.Count;
        }
        else if (localT == 0 && intPoint == count - 1)
        {
            intPoint = count - 2;
            localT = 1;
        }

        Vector3 temp, p0, p1, p2, p3;
        if (this.closed || intPoint > 0)
        {
            p0 = points[(intPoint - 1) % count];
        }
        else
        {
            temp = (points[0] - points[1]) + points[0];
            p0 = temp;
        }

        p1 = points[intPoint % count];
        p2 = points[(intPoint + 1) % count];
        if (this.closed || intPoint + 2 < count)
        {
            p3 = points[(intPoint + 2) % count];
        }
        else
        {
            temp = (points[count - 1] - points[count - 2]) + points[count - 1];
            p3 = temp;
        }

        var poly = new CubicPoly(p0, p1, p2, p3);
        Vector3 val = poly.Calc(localT);
        return val;

    }

    public Vector3 GetPointAt(float t)
    {
        float u = GetUtoTmapping(t);
        return GetPoint(u);
    }

    public float GetUtoTmapping(float u)
    {
        var arcLengths = this.GetLengths();

        int i = 0, il = arcLengths.Length;

        float targetArcLength = u * arcLengths[il - 1];
        int low = 0, high = il - 1;
        float comparison;

        while (low <= high)
        {

            i = Mathf.FloorToInt(low + (high - low) / 2f);
            comparison = arcLengths[i] - targetArcLength;

            if (comparison < 0f)
            {
                low = i + 1;
            }
            else if (comparison > 0f)
            {
                high = i - 1;
            }
            else
            {
                high = i;
                break;
            }

        }

        i = high;

        if (Mathf.Approximately(arcLengths[i], targetArcLength))
        {

            return 1f * i / (il - 1);

        }
        

        var lengthBefore = arcLengths[i];
        var lengthAfter = arcLengths[i + 1];

        var segmentLength = lengthAfter - lengthBefore;
        

        var segmentFraction = (targetArcLength - lengthBefore) / segmentLength;

        var t = 1f * (i + segmentFraction) / (il - 1);

        return t;
    }


    public List<FrenetFrame> ComputeFrenetFrames(int segments, bool closed)
    {
        var normal = new Vector3();

        var tangents = new Vector3[segments + 1];
        var normals = new Vector3[segments + 1];
        var binormals = new Vector3[segments + 1];

        float u, theta;
        
        for (int i = 0; i <= segments; i++)
        {
            u = (1f * i) / segments;
            tangents[i] = GetTangentAt(u).normalized;
        }

        normals[0] = new Vector3();
        binormals[0] = new Vector3();

        var min = float.MaxValue;
        var tx = Mathf.Abs(tangents[0].x);
        var ty = Mathf.Abs(tangents[0].y);
        var tz = Mathf.Abs(tangents[0].z);
        if (tx <= min)
        {
            min = tx;
            normal.Set(1, 0, 0);
        }

        if (ty <= min)
        {
            min = ty;
            normal.Set(0, 1, 0);
        }

        if (tz <= min)
        {
            normal.Set(0, 0, 1);
        }

        var vec = Vector3.Cross(tangents[0], normal).normalized;
        normals[0] = Vector3.Cross(tangents[0], vec);
        binormals[0] = Vector3.Cross(tangents[0], normals[0]);

        for (int i = 1; i <= segments; i++)
        {
            normals[i] = normals[i - 1];
            binormals[i] = binormals[i - 1];
            
            var axis = Vector3.Cross(tangents[i - 1], tangents[i]);
            if (axis.magnitude > float.Epsilon)
            {
                axis.Normalize();

                float dot = Vector3.Dot(tangents[i - 1], tangents[i]);
                
                theta = Mathf.Acos(Mathf.Clamp(dot, -1f, 1f));

                normals[i] = Quaternion.AngleAxis(theta * Mathf.Rad2Deg, axis) * normals[i];
            }

            binormals[i] = Vector3.Cross(tangents[i], normals[i]).normalized;
        }

        if (closed)
        {
            theta = Mathf.Acos(Mathf.Clamp(Vector3.Dot(normals[0], normals[segments]), -1f, 1f));
            theta /= segments;

            if (Vector3.Dot(tangents[0], Vector3.Cross(normals[0], normals[segments])) > 0f)
            {
                theta = -theta;
            }

            for (int i = 1; i <= segments; i++)
            {
                normals[i] = (Quaternion.AngleAxis(Mathf.Deg2Rad * theta * i, tangents[i]) * normals[i]);
                binormals[i] = Vector3.Cross(tangents[i], normals[i]);
            }
        }

        var frames = new List<FrenetFrame>();
        int n = tangents.Length;
        for (int i = 0; i < n; i++)
        {
            var frame = new FrenetFrame(tangents[i], normals[i], binormals[i]);
            frames.Add(frame);
        }

        return frames;
    }
    
    float[] GetLengths(int divisions = -1) {
        if (divisions < 0) {
            divisions = 200;
        }

        if (this.cacheArcLengths != null &&
            (this.cacheArcLengths.Length == divisions + 1) &&
            !this.needsUpdate) {
            return this.cacheArcLengths;
        }

        this.needsUpdate = false;

        var cache = new float[divisions + 1];
        Vector3 current, last = this.GetPoint(0f);

        cache[0] = 0f;

        float sum = 0f;
        for (int p = 1; p <= divisions; p ++ ) {
            current = this.GetPoint(1f * p / divisions);
            sum += Vector3.Distance(current, last);
            cache[p] = sum;
            last = current;
        }

        this.cacheArcLengths = cache;
        return cache;
    }
    
    protected virtual Vector3 GetTangentAt(float t) {
        float u = GetUtoTmapping(t);
        var delta = 0.001f;
        var u1 = u - delta;
        var u2 = u + delta;
        
        //clap
        if (u1 < 0f) u1 = 0f;
        if (u2 > 1f) u2 = 1f;

        var pt1 = GetPoint(u1);
        var pt2 = GetPoint(u2);
        return (pt2 - pt1).normalized;
    }
}


public class CubicPoly
{
    Vector3 c0,c1,c2,c3;

    public CubicPoly(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, float tension = 0.5f)
    {
        Vector3 t0 = tension * (v2 - v0);
        Vector3 t1 = tension * (v3 - v1);
        c0 = v1;
        c1 = t0;
        c2 = -3f * v1 + 3f * v2 - 2f * t0 - t1;
        c3 = 2f * v1 - 2f * v2 + t0 + t1;
    }

    public Vector3 Calc(float t)
    {
        var t2 = t * t;
        var t3 = t2 * t;
        return c0 + c1 * t + c2 * t2 + c3 * t3;
    }
}

public class FrenetFrame {
    public Vector3 Tangent { get { return tangent; } }
    public Vector3 Normal { get { return normal; } }
    public Vector3 Binormal { get { return binormal; } }

    Vector3 tangent, normal, binormal;

    public FrenetFrame(Vector3 t, Vector3 n, Vector3 bn) {
        tangent = t;
        normal = n;
        binormal = bn;
    }
}
