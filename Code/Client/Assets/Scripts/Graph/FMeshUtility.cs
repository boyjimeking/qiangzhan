
using UnityEngine;
public sealed class FMeshUtility
{

    /// <summary>
    /// 创建一个矩形
    /// </summary>
    /// <returns></returns>
    public static Mesh CreateRectangle()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertex = new Vector3[] { new Vector3(-1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, 1, 0), new Vector3(1, -1, 0) };
        Vector2[] uv = new Vector2[] { new Vector2(0, 0) , new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0)};
        mesh.vertices = vertex;
        mesh.uv = uv;
        int[] indices = new int[] { 0, 1, 2, 2, 3, 0 };
        mesh.triangles = indices;
        return mesh;
    }
}

