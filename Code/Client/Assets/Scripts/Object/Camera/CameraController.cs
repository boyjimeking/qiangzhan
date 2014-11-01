using UnityEngine;
using System.Collections;
using System.IO;
using FantasyEngine;
using System.Xml;

public enum ShakeType
{
    Rotate = 1,
    Position = 2,

}
/// <summary>
/// 摄像机控制
/// </summary>
public class CameraController
{
    //运动事件?
    private Hashtable mEvents = new Hashtable();

    //摄像机跟随谁
    private ObjectBase mLookAt = null;
    //当前控制的摄像机
    private GameObject mCameraObj = null;

    //摄像机得到动画路径
    private string mCurnAnimatorame;
    private CameraPathAnimator mAnimatorPath;
    private CameraPath mFollowPath;
    private float lastPercent = 0;

    private CameraEffect mEffect = new CameraEffect();

    private Camera mCamera = null;
    private Transform mCameraTransform = null;

    //自由摄像机
    private bool mFree = false;
    public float step = 80f;


    public bool allowRotate = true;
    private float mAnglex = -45;
    private float mAngley = -45;
    private float mDistance = 5;
    private bool mInvalidInfo = true;



    //
    //测试代码
    private CameraTest mCameraTest = null;

    private TransformData mOriginalData = new TransformData();
    private static CameraController instance;
    public CameraController()
    {
        instance = this;

        LockCamera = false;
    }

    public static CameraController Instance
    {
        get
        {
            return instance;
        }
    }
    public Camera CurCamera
    {
        get
        {
            return mCamera;
        }
        set
        {
            mCamera = value;
            if (value != null)
                mCameraTransform = mCamera.transform;
            else
                mCameraTransform = null;
        }
    }

    public CameraPathAnimator FollowAnimator
    {
        get
        {
            return mAnimatorPath;
        }
        set
        {

            if (mCamera)
            {
                if (mAnimatorPath == null && value != null)
                    mOriginalData.CopyTrans(mCameraTransform);
                if (mAnimatorPath != null && value == null)
                    mOriginalData.Apply(mCameraTransform);
            }
            mAnimatorPath = value;


        }
    }

    public bool Free
    {
        set
        {
            if (value == false && mFree == true)
            {
                mOriginalData.Apply(mCameraTransform);
                FreeSceneCamera freeScript = mCamera.gameObject.GetComponent<FreeSceneCamera>();
                if (freeScript != null)
                {
                    freeScript.enabled = false;
                }
            }

            mFree = value;
            if (mCamera != null && mFree)
            {
                mOriginalData.CopyTrans(mCameraTransform);

                // 查看是否有FreeCamera的脚本
                FreeSceneCamera freeScript = mCamera.gameObject.GetComponent<FreeSceneCamera>();
                if (freeScript == null)
                {
                    freeScript = mCamera.gameObject.AddComponent<FreeSceneCamera>();
                    freeScript.mCamera = mCamera;

                }
                freeScript.enabled = true;
            }
        }
    }

    public bool LockCamera { get; set; }

    public float AngleX
    {
        get
        {
            return mAnglex;
        }
    }
    public float AngleY
    {
        get
        {
            return mAngley;
        }
    }
    public float Distance
    {
        get
        {
            return mDistance;
        }
    }


    public float FieldOfView
    {
        get
        {
            if (mCamera != null)
                return mCamera.fieldOfView;
            return 0.0f;
        }
    }
    public void SetControlCamera(GameObject camera, CameraPathAnimator animator)
    {
        mFollowPath = null;
        if (animator != null)
            mFollowPath = animator.cameraPath;

        mCameraObj = camera;

        mCameraTest = mCameraObj.GetComponent<CameraTest>();
        if (mCameraTest == null)
        {
            mCameraTest = mCameraObj.AddComponent<CameraTest>();
        }
        mCamera = mCameraObj.GetComponent<Camera>();
        mCameraTransform = mCameraObj.transform;
        mCameraObj.GetComponent<AudioListener>().enabled = false;
        int layer2DUI = LayerMask.NameToLayer("2D UI");
        int layer3DUI = LayerMask.NameToLayer("3D UI");
        int layerPreview = LayerMask.NameToLayer("UIPreview");

        int layerMask = (1 << layer2DUI) | (1 << layer3DUI) | (1 << layerPreview);

        int oldCullingMask = mCamera.cullingMask;

        mCamera.cullingMask = oldCullingMask & (~layerMask);

    }

    public void SetLookAT(ObjectBase target)
    {
        mLookAt = target;
    }

    public void SetCameraInfo(float anglex, float angley, float distance)
    {
        mAnglex = anglex;
        mAngley = angley;
        mDistance = distance;
        mInvalidInfo = true;
    }

    /// <summary>
    /// 摄像机的震动
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="time"></param>
    public void ShakeCamera(float amount = 10, float time = 2f,ShakeType stype = ShakeType.Rotate)
    {
        if (mCameraObj == null)
            return;
        if(stype == ShakeType.Rotate)
            iTween.ShakeRotation(mCameraObj, (new Vector3(0.156f, 0.156f, 0.078f)) * amount, time);
        else if (stype == ShakeType.Position)
        {
            m_shakeTimer = m_shakeDuration = time;
            m_shakeMagnitude = amount;
        }
    }

    private float m_shakeMagnitude = 30;
    private float m_shakeTimer = 0;
    private float m_shakeDuration = 0.6f;
    private Vector3 ShakeCamerPos()
    {
       
        Vector3 offset = Vector3.zero;
        if (m_shakeTimer > 0)
        {
            float max = ((m_shakeMagnitude * (m_shakeTimer / m_shakeDuration)) * Time.deltaTime);
            offset.x = Random.Range(-max, max);
            offset.y = Random.Range(-max, max);
            m_shakeTimer -= Time.deltaTime;
        }
        return offset;

    }

    /// <summary>
    /// 摄像机拉近效果
    /// </summary>
    /// <param name="showpos"></param>
    /// <param name="time"></param>
    public void PlayCameraEffect(float showpos, float endpos, float time)
    {
        mEffect.Initialize(showpos, endpos, time);
    }

    public void Update()
    {
        #region 摄像机的Debug操作
        float step = 20 * Time.fixedDeltaTime;

        if (Input.GetKeyDown(KeyCode.W) && Input.GetKey(KeyCode.LeftControl))
        {
            //SceneManager.Instance.GetCurScene().GetEffectManager().DarkenScene(new uint[] { PlayerController.Instance.GetControl() });
            //PlayCameraAnimation("base_gk03_^_^_path1");

            //(PlayerController.Instance.GetControlObj() as Player).ChangeAttach(AttachMountType.Wing, AssetConfig.WeaponPath + "weapon_wingemo", "chibang");
            //SceneManager.Instance.GetCurScene().StopDynamicToTarget();
            //ShakeCamera(10, 2);

            (PlayerController.Instance.GetControlObj() as VisualObject).AddEffect(516, null, float.NaN, AttachMountType.Wing);
        }
        if (Input.GetKey(KeyCode.J) && Input.GetKey(KeyCode.LeftControl))
        {
            //SceneManager.Instance.GetCurScene().PlayGameObjAnim("realistic_gk03_guanmen");
            //SceneManager.Instance.GetCurScene().PlayDynamicAnimation();
            //SceneManager.Instance.GetCurScene().GetEffectManager().RecoverScene();


        }

        if (allowRotate)
        {
            if (Input.GetKey(KeyCode.H))
            {
                mAngley -= step;
            }
            if (Input.GetKey(KeyCode.K))
            {
                mAngley += step;
            }
            if (Input.GetKey(KeyCode.U))
            {
                mAnglex += step;
            }
            if (Input.GetKey(KeyCode.J))
            {
                mAnglex -= step;
            }
            //if (Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.LeftAlt))
            //{
            //    PlayCameraEffect(5f, 0.8f);
            //}
            float mousestep = 10f;
            if (Input.GetKey(KeyCode.LeftShift))
                mousestep = 50;
            mousestep = (Input.GetAxis("Mouse ScrollWheel") * mousestep) * Time.deltaTime;
            if (mousestep != 0)
            {
                mDistance -= mousestep;
            }
        }

        #endregion


        bool switchFree = Input.GetKeyDown(KeyCode.I) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt);
        if (switchFree)
        {
            Free = !mFree;
        }
        if (mFree)
        {
            UpdateFreeCamera();
        }
        else if (!LockCamera)
        {

            if (mAnimatorPath != null)
            {
                /*
                 * 当有摄像机动画时
                 */
            }
            else
            {
                if (mCamera != null && mLookAt != null)
                {
                    if (mFollowPath != null)
                    {
                        UpdateFollowCamera(mLookAt.GetPosition());
                    }
                    else
                    {

                        float amentDistance = mDistance;
#if UNITY_STANDALONE_WIN || UNITY_EDITER
                        //                         if( ((float)Screen.width / (float)Screen.height) > 1.5f)
                        //                             amentDistance = mDistance + 3.5f;
#endif
                        if (mEffect.InUse)
                        {
                            mEffect.Update(Time.deltaTime);
                            amentDistance = mEffect.Distance;
                        }

                        //计算摄像机的位置和旋转
                        Vector3 oldPos = mCameraTransform.position;

                        Vector3 lookPos = mLookAt.GetPosition() + new Vector3(0, 1.5f, 0);

                        mCameraTransform.position = lookPos + new Vector3(0, amentDistance, 0);
                        mCameraTransform.LookAt(lookPos);
                        mCameraTransform.RotateAround(lookPos, Vector3.right, mAnglex);
                        mCameraTransform.RotateAround(lookPos, Vector3.up, mAngley);

                        Vector3 currentVelocity = Vector3.zero;

                        mCameraTransform.position += ShakeCamerPos();

                        if (!mInvalidInfo)
                            mCameraTransform.position = Vector3.SmoothDamp(oldPos, mCameraTransform.position, ref currentVelocity, 0.05f);
                        else
                            mInvalidInfo = false;
                    }
                }



                //#if UNITY_STANDALONE_WIN || UNITY_EDITER

                if (mCameraTest != null && mLookAt != null)
                {
                    mCameraTest.player_pos = mLookAt.GetPosition();
                    mCameraTest.dir_angle = mLookAt.GetDirection() / Mathf.Deg2Rad;
                }
                //#endif

            }
        }

    }

    /// <summary>
    /// 场景自定义的摄像机跟随路线
    /// </summary>
    /// <param name="lookPos"></param>
    private void UpdateFollowCamera(Vector3 lookPos)
    {
        float nearestPercent = mFollowPath.GetNearestPoint(lookPos);
        float usePercent = Mathf.Lerp(lastPercent, nearestPercent, 0.4f);
        lastPercent = usePercent;
        Vector3 nearestPoint = mFollowPath.GetPathPosition(usePercent, true) + mFollowPath.transform.position;


        mCameraTransform.position = Vector3.Lerp(mCameraTransform.position, nearestPoint, 0.4f);

        Quaternion lookRotation = Quaternion.LookRotation(lookPos - mCameraTransform.position);

        mCameraTransform.rotation = Quaternion.Slerp(mCameraTransform.rotation, lookRotation, 0.4f);
    }

    public void UpdateFreeCamera()
    {
        return;
        if (mCamera == null)
            return;
        float axisangle = 10f;
        float updownspeed = 0.2f;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            updownspeed *= 2;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            CurCamera.transform.Rotate(Vector3.up, axisangle * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            CurCamera.transform.Rotate((Vector3)((Vector3.up * step) * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            CurCamera.transform.Rotate((Vector3)((-Vector3.right * step) * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            CurCamera.transform.Rotate((Vector3)((Vector3.right * step) * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.PageUp))
        {
            CurCamera.transform.Translate(Vector3.up * updownspeed);
        }
        if (Input.GetKey(KeyCode.PageDown))
        {
            CurCamera.transform.Translate(Vector3.up * -updownspeed);
        }

    }
    public Vector3 WorldToScreenPoint(Vector3 worldPos)
    {
        if (this.CurCamera == null)
        {
            return Vector2.zero;
        }
        return CurCamera.WorldToScreenPoint(worldPos);
    }

    /// <summary>
    /// 播放摄像机器动画
    /// </summary>
    /// <param name="name"></param>
    public void PlayCameraAnimation(string name = null)
    {
        if (FollowAnimator != null && (FollowAnimator.name == name || string.IsNullOrEmpty(name)))
        {
            FollowAnimator.Play();
            return;
        }
        mCurnAnimatorame = name;
        AssetParam param = new AssetParam();
        param.listener = ResourceListener; 
        AssetManager.Instance.LoadResource(FantasyEngine.AssetConfig.ScenePath + "animation/" + name + ".xml", param);
    }

    void ResourceListener(AssetPtr ptr)
    {
        if (ptr == null || ptr.Data == null)
            return;

        if (!ptr.Data.url.Contains(mCurnAnimatorame))
            return;

        GameObject cameraanimator = new GameObject(mCurnAnimatorame);
        CameraPath cPath = cameraanimator.AddComponent<CameraPath>();
        CameraPathAnimator animator = cameraanimator.AddComponent<CameraPathAnimator>();
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(ptr.Data.text);
        cPath.FromXML(xml);
        if (Camera.main != null)
            animator.animationObject = Camera.main.transform;


        BaseScene scene = SceneManager.Instance.GetCurScene();
        if (scene != null)
        {
            animator.AnimationCustomEvent += scene.GetCameraPathManager().OnCustomEvent;
            animator.AnimationFinishedEvent += animator_AnimationFinishedEvent;

        }
        FollowAnimator = animator;
        animator.Play();
    }
    void animator_AnimationFinishedEvent()
    {
        if (FollowAnimator != null)
        {
            SceneManager.Instance.GetCurScene().GetCameraPathManager().OnFinish(FollowAnimator.gameObject.name);
        }
    }
    public void PauseCameraAniamtion()
    {
        if (FollowAnimator != null)
        {
            FollowAnimator.Pause();
        }
    }
    public void StopCameraAniamtion()
    {
        if (FollowAnimator != null)
        {
            FollowAnimator.Stop();
        }
        FollowAnimator = null;
    }
    public void Clear()
    {
        mAnimatorPath = null;
        mFollowPath = null;
		LockCamera = false;
        mCurnAnimatorame = null;
    }

    public void Destory()
    {
        mEvents.Clear();
        mCamera = null;
        mCameraObj = null;
        mCameraTransform = null;
    }
}
