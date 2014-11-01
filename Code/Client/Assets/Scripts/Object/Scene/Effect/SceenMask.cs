using UnityEngine;
using System.Collections;

/// <summary>
/// 场景变暗的接口
/// </summary>
public class SceenMask : MonoBehaviour {

    private Mesh mMesh;
    public Material mMtl;
	// Use this for initialization
	void Start () 
    {

        if(mMesh==null)
            mMesh = CreateMesh();
        if (mMtl == null)
        {
            mMtl = new Material(Shader.Find("qmqz/ScreenMask"));
            mMtl.SetColor("_Color", new Color(0, 0, 0, 0.7f));
        }
	}

    public void SetAlpha(float alpha)
    {
        if (mMtl == null)
        {
            mMtl = new Material(Shader.Find("qmqz/ScreenMask"));
        }
        mMtl.SetColor("_Color", new Color(0, 0, 0, alpha));

    }
	
	// Update is called once per frame
	void Update () 
    {
        Graphics.DrawMesh(mMesh, transform.localToWorldMatrix, mMtl,0,CameraController.Instance.CurCamera);
	}

	static Mesh CreateMesh()
	{
		Mesh mesh = new Mesh ();
		Vector3[] vertex = new Vector3[]{new Vector3 (-1, 1, 0),new Vector3 (1, 1, 0),new Vector3 (1, -1, 0),new Vector3 (-1, -1, 0)};
		Vector2[] uv = new Vector2[]{new Vector2 (0, 1),new Vector2 (1, 1),new Vector2 (1, 0),new Vector2 (0, 0)};
		mesh.vertices = vertex;
		mesh.uv = uv;
		int[] indices = new int[]{0,1,3,1,2,3};
        mesh.triangles = indices;
        return mesh;
	}
}
