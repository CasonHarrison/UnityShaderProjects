
using System.Collections.Generic;
using UnityEngine;
public class Tube
{
    const float PI2 = Mathf.PI * 2;

    public static Mesh Build(CatmullRomCurve curve, int tubeSegments, float radius, int radialSegments, bool closed)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector4> tangents = new List<Vector4>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> indices = new List<int>();

        List<FrenetFrame> frames = curve.ComputeFrenetFrames(tubeSegments, closed);

        for (int i = 0; i < tubeSegments; i++)
        {
            GenerateSegment(curve, frames, tubeSegments, radius, radialSegments, vertices, normals, tangents, i);
        }
        GenerateSegment(curve, frames, tubeSegments, radius, radialSegments, vertices, normals, tangents, (!closed) ? tubeSegments : 0);

        if (!closed)
        {
            Vector3 start = curve.GetPoint(0);
            Vector3 end = curve.GetPoint(1);
            GenerateCap(vertices, normals, indices, uvs, tangents, start, frames[0], radius, radialSegments, true);
            GenerateCap(vertices, normals, indices, uvs, tangents, end, frames[tubeSegments], radius, radialSegments, false);
        }

        for (int i = 0; i <= tubeSegments; i++)
        {
            float v = 1f * i / tubeSegments;
            for (int j = 0; j <= radialSegments; j++)
            {
                float u = 1f * j / radialSegments;
                uvs.Add(new Vector2(u, v));
            }
        }

        for (int i = 1; i <= tubeSegments; i++)
        {
            for (int j = 1; j <= radialSegments; j++)
            {
                int a = (radialSegments + 1) * (i - 1) + (j - 1);
                int b = (radialSegments + 1) * i + (j - 1);
                int c = (radialSegments + 1) * i + j; 
                int d = (radialSegments + 1) * (i - 1) + j;
                
                indices.Add(a);
                indices.Add(d);
                indices.Add(b);
                indices.Add(b);
                indices.Add(d);
                indices.Add(c);
            }
        }
        
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.tangents = tangents.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
        return mesh;
    }

    static void GenerateSegment(
        CatmullRomCurve curve, List<FrenetFrame> frames, int tubeSegments,
        float radius, int radialSegments, List<Vector3> vertices,
        List<Vector3> normals, List<Vector4> tangents, int i)
    {
        float u = 1f * i / tubeSegments;
        Vector3 point = curve.GetPointAt(u);
        FrenetFrame frame = frames[i];
        Vector3 normal = frame.Normal;
        Vector3 binormal = frame.Binormal;

        for (int j = 0; j <= radialSegments; j++)
        {
            float val = 1f * j / radialSegments * PI2;
            float sin = Mathf.Sin(val);
            float cos = Mathf.Cos(val);
            
            Vector3 N = (cos * normal + sin * binormal).normalized;
            vertices.Add(point + radius * N);
            normals.Add(N);
            Vector4 tangent = frame.Tangent;
            tangents.Add(new Vector4(tangent.x, tangent.y, tangent.z, 0f));
        }
    }
    
    static void GenerateCap(
        List<Vector3> vertices, List<Vector3> normals, List<int> indices,
        List<Vector2> uvs, List<Vector4> tangents, Vector3 position, 
        FrenetFrame frame, float radius, int radialSegments, bool start)
    {
        Vector3 tangent = frame.Tangent;
        Vector3 normal = frame.Normal;
        Vector3 binormal = frame.Binormal;

        int baseIndex = vertices.Count;
        float angleIncrement = Mathf.PI / (2 * radialSegments);
        
        for (int i = 0; i <= radialSegments; i++)
        {
            float theta = i * angleIncrement;
            float capRadius = radius * Mathf.Sin(theta);
            float capHeight = radius * Mathf.Cos(theta);

            Vector3 capCenterOffset = start ? -capHeight * tangent : capHeight * tangent;
            for (int j = 0; j <= radialSegments; j++)
            {
                float phi = j * PI2 / radialSegments;
                float x = capRadius * Mathf.Cos(phi);
                float y = capRadius * Mathf.Sin(phi);
                Vector3 point = position + capCenterOffset + x * normal + y * binormal;

                vertices.Add(point);
                normals.Add((point - position).normalized);

                float u = 0.5f + (x / (2 * radius));
                float v = 0.5f + (y / (2 * radius));
                uvs.Add(new Vector2(u, v));
                tangents.Add(new Vector4(tangent.x, tangent.y, tangent.z, 0f));
            }
        }

        // Generate indices for the cap
        for (int i = 0; i < radialSegments; i++)
        {
            for (int j = 0; j < radialSegments; j++)
            {
                int a = baseIndex + i * (radialSegments + 1) + j;
                int b = baseIndex + (i + 1) * (radialSegments + 1) + j;
                int c = baseIndex + (i + 1) * (radialSegments + 1) + j + 1;
                int d = baseIndex + i * (radialSegments + 1) + j + 1;

                if (start)
                {
                    indices.Add(a);
                    indices.Add(d);
                    indices.Add(b);
                    indices.Add(b);
                    indices.Add(d);
                    indices.Add(c);
                }
                else
                {
                    indices.Add(a);
                    indices.Add(b);
                    indices.Add(d);
                    indices.Add(b);
                    indices.Add(c);
                    indices.Add(d);
                }
            }
        }
    }
}
