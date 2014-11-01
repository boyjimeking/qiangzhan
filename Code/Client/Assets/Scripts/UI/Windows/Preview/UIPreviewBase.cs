
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI预览基类
/// </summary>
public abstract class UIPreviewBase
{
    protected static readonly int layermask = LayerMask.NameToLayer("UIPreview");

    private static int nCount = 0;

    private static GameObject msViewRoot = null;

    protected GameObject mPreviewRoot;
    protected Camera mPreviewCamera;

    protected Color mClearColor = new Color(49/255f, 77/255f, 121/255f, 0);
    protected float mOrthograhicsize = 0.91f;

    protected float mRotateY = 180;

    protected UIAtlas mPreviewAtlas;
    protected UISprite mTargetSprite;
    protected Material mPreviewMtl;
    protected RenderTexture mTargetTexture;

    protected Material mBackgroundMtl;
    protected MeshRenderer mBackRenderer;

    protected Light mLight;

    protected bool mEnable = true;
    

    public UIPreviewBase()
    {
        if (msViewRoot == null)
        {
            msViewRoot = new GameObject("PreviewRoot");
            Object.DontDestroyOnLoad(msViewRoot);
        }

        mPreviewRoot = new GameObject();
        mPreviewRoot.transform.parent = msViewRoot.transform;
        Object.DontDestroyOnLoad(mPreviewRoot);
        mPreviewRoot.name = "mPreview";
        mPreviewRoot.transform.localPosition = new Vector3(0,50 * ++nCount,0);

        GameObject cameraObj = new GameObject();
        cameraObj.transform.parent = mPreviewRoot.transform;
        cameraObj.transform.localPosition = new Vector3(0, 0, -2);

        mPreviewCamera = cameraObj.AddComponent<Camera>();
        mPreviewCamera.name = "mPreviewCamera";

        mPreviewCamera.clearFlags = CameraClearFlags.SolidColor;
        mPreviewCamera.depth = 5;
        mPreviewCamera.cullingMask = 1 << layermask;
        mPreviewCamera.backgroundColor = mClearColor;

        GameObject lightObj = new GameObject("PreviewLight");
        lightObj.transform.localPosition = Vector3.zero;
        mLight = lightObj.AddComponent<Light>();
        lightObj.transform.parent = mPreviewRoot.transform;
        lightObj.layer =  layermask;

        mLight.cullingMask = 1 << layermask;
        mLight.type = LightType.Point;
        mLight.intensity = 0.16f;
        mLight.range = 5;



        mPreviewMtl = new Material(Shader.Find("Unlit/Transparent Colored"));
        Enable = false;
    }

    public bool Enable
    {
        get
        {
            return mEnable;
        }
        set
        {
            mEnable = value;
            if (mPreviewRoot != null)
                mPreviewRoot.SetActive(value);
        }
    }

    public Color ClearColor
    {
        get
        {
            return mClearColor;
        }
        //set
        //{
        //    mClearColor = value;
        //    mPreviewCamera.backgroundColor = mClearColor;
        //}
    }

    public UISprite TargetSprite
    {
        get
        {
            return mTargetSprite;
        }
    }

    public void SetBackGround(Texture tex,Mesh mesh)
    {

        if (tex != null)
        {
            mBackgroundMtl = new Material(Shader.Find("qmqz/ScreenCoordinate"));
            mBackgroundMtl.mainTexture = tex;
            mBackgroundMtl.renderQueue = 1000;
            if (mBackRenderer == null)
            {
                GameObject backObj = new GameObject("backObj");
                backObj.transform.parent = mPreviewRoot.transform;
                backObj.layer = layermask;
                MeshFilter filter = backObj.AddComponent<MeshFilter>();
                filter.mesh = mesh;
                if (mesh == null)
                    backObj.GetComponent<MeshFilter>().mesh = FMeshUtility.CreateRectangle();
                mBackRenderer = backObj.AddComponent<MeshRenderer>();
                mBackRenderer.material = mBackgroundMtl;
            }
        }

        
    }

    public UISprite BackgroundSprite
    {
        set
        {
            if (value == null)
                return;
            BetterList<Vector3> vecs = new BetterList<Vector3>();
            BetterList<Vector2> uvs = new BetterList<Vector2>();
            BetterList<Color32> cols = new BetterList<Color32>();
            value.OnFill(vecs,uvs,cols);
            Mesh mesh = null;
            if (vecs.size > 0)
            {
                mesh = FMeshUtility.CreateRectangle();

                Vector2[] suv = new Vector2[uvs.size];
                suv[0] = uvs[1];
                suv[1] = uvs[0];
                suv[2] = uvs[3];
                suv[3] = uvs[2];
                mesh.uv = suv;
            }

            SetBackGround(value.mainTexture, mesh);
        }

    }
    public virtual void Update()
    {

    }
    /// <summary>
    /// 设置目标Sprite
    /// </summary>
    /// <param name="sprite"></param>
    public void SetTargetSprite(UISprite sprite , bool hasBox = true)
    {
        if (sprite == null)
            return;
        mTargetSprite = sprite;
        BoxCollider collider = mTargetSprite.GetComponent<BoxCollider>();
        if (collider == null && hasBox)
        {
            collider = mTargetSprite.gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(sprite.width, sprite.height, -1);
        }
        PreviewScript script = mTargetSprite.gameObject.GetComponent<PreviewScript>();
        if (script == null)
        {
            script = mTargetSprite.gameObject.AddComponent<PreviewScript>();
        }
        script.mPreview = this;
        SetupCamera();
        CreatePreviewSprite(mTargetSprite);
    }

    public void SetupCamera()
    {
        //
        //设置渲染目标信息
        //
        if (mTargetTexture == null)
        {
            mTargetTexture = new RenderTexture(mTargetSprite.width, mTargetSprite.height, 16, RenderTextureFormat.ARGB32);
        }
        mPreviewCamera.targetTexture = mTargetTexture;
        mPreviewCamera.pixelRect = new Rect(0,0,mTargetSprite.width,mTargetSprite.height);
        mPreviewCamera.aspect = mPreviewCamera.pixelWidth / mPreviewCamera.pixelHeight;


        Color color = ClearColor;
        //初始化摄像机的相关信息
        InitializeCamera(ref mPreviewCamera, color, layermask, mOrthograhicsize);

        mPreviewMtl.SetTexture("_MainTex", mTargetTexture);

        Enable = true;

    }
    public void InitializeCamera(ref Camera camera, Color color, int layer, float orthographicSize)
    {
        camera.clearFlags = CameraClearFlags.Color;
        //camera.backgroundColor = color;
        camera.orthographic = true;
        //camera.aspect = 1f;
        camera.cullingMask = 1 << layer;
        camera.orthographicSize = orthographicSize;
        camera.nearClipPlane = 0f;
    }
    public float RotationY
    {
        get
        {
            return mRotateY;
        }
        set
        {
            mRotateY = value;
        }
    }

    public void Scroll(float delta)
    {
        RotationY += delta < 0 ? 10 : -10;
    }
    public void CreatePreviewSprite(UISprite uiSprite)
    {
        AddPreviewSprite(uiSprite, "preview", true);
        if ((uiSprite != null) && (mPreviewAtlas != null))
        {
            mPreviewAtlas.spriteMaterial = mPreviewMtl;
            uiSprite.atlas = mPreviewAtlas;
            uiSprite.spriteName = "preview";
        }
    }
    public UIAtlas AddPreviewSprite(UISprite uiSprite, string spriteName, bool isDrag)
    {
        if (null == uiSprite)
        {
            Debug.LogError("uiSprite is null");
            return null;
        }
        GetPreviewAtlas();
        if ((mPreviewAtlas != null) && (mPreviewAtlas.GetSprite(spriteName) == null))
        {
            UISpriteData item = new UISpriteData
            {
                name = "preview",
                paddingLeft = 0,
                paddingRight = 0,
                paddingBottom = 0,
                paddingTop = 0
            };
            item.SetRect(0, 0, mTargetSprite.width, mTargetSprite.height);
            mPreviewAtlas.spriteList.Clear();
            mPreviewAtlas.spriteList.Add(item);
        }
        return mPreviewAtlas;
    }
    /// <summary>
    /// 获取当前的图集
    /// </summary>
    /// <returns></returns>
    private UIAtlas GetPreviewAtlas()
    {
        if (mPreviewAtlas == null)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = msViewRoot.transform;
            obj.name = "UIPreveiwGameObject";
            mPreviewAtlas = obj.AddComponent<UIAtlas>();
            UnityEngine.Object.DontDestroyOnLoad(mPreviewAtlas);

        }
        return mPreviewAtlas;
    }

    public virtual void Destroy()
    {
        if (mPreviewRoot != null)
        {
            UnityEngine.Object.DestroyObject(mPreviewRoot);
            mPreviewRoot = null;
        }
        if (mPreviewMtl != null)
            UnityEngine.Object.Destroy(mPreviewMtl);
        if(mPreviewAtlas != null)
        {
            UnityEngine.Object.DestroyObject(mPreviewAtlas.gameObject);
        }
    }
}

