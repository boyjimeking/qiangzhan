
using FantasyEngine;
using UnityEngine;
public class CreateRoleManager : Singleton<CreateRoleManager>
{
    private GameObject mRoleRoot;
    private SimpleModel mActivePlayer;
    private int mPlayerID = -1;
    public static float sScale = 1.5f;
    public static Vector3 sPos = new Vector3(-4.11f, 0.25f, -3.38f);

    private SceneParticleManager mParticleMng;
    public void EnterVirtualScene(string name)
    {
        EngineApplication.LoadLevel(name, OnSceneComplete);

    }

    private GameObject GetRoleOBJ()
    {
        if(mRoleRoot == null)
        {
            mRoleRoot = new GameObject("CreateRoleRoot");
            GameObject.DontDestroyOnLoad(mRoleRoot);
        }
        return mRoleRoot;
    }

    public void Update(uint elapsed)
    {
        if (mActivePlayer != null)
        {
            if(mActivePlayer.GetVisual()!= null && mActivePlayer.GetVisual().Visual != null)
            {
                if (!mActivePlayer.GetVisual().VisualTransform.IsChildOf(GetRoleOBJ().transform))
                {
                    mActivePlayer.GetVisual().VisualTransform.parent = GetRoleOBJ().transform;
                }
            }
            mActivePlayer.Update();
        }
    }
    private void OnSceneComplete()
    {
        mParticleMng = new SceneParticleManager();
        if(Camera.main != null)
        {
            Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer("3D UI");
            Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer("UI");
            Camera.main.GetComponent<AudioListener>().enabled = false;
        }
        WindowManager.Instance.CloseAllUI();
        WindowManager.Instance.OpenUI("createrole");
        RenderSettings.ambientLight = Color.white;

        //场景中加入一个聚光灯照亮玩家
        GameObject lightObj = new GameObject("playerlight");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Spot;
        light.range = 10;
        light.spotAngle = 56.6f;
        light.intensity = 1.69f;

        lightObj.transform.localPosition = new Vector3(-4.65615f, 6.147261f, -3.736148f);
        lightObj.transform.localEulerAngles = new Vector3(78.3259f, 82.48914f, -119.2915f);


        if (mActivePlayer != null)
        {
            mPlayerID = -1;
            mActivePlayer.Destroy();
        }
        ShowPlayer((int)ModuleManager.Instance.FindModule<CreateRoleModule>().GetRoleID());
     

    }

    public void ShowPlayer(int playerid)
    {
        if (mPlayerID == playerid)
            return;
        mPlayerID = playerid;

        if (mActivePlayer != null)
            mActivePlayer.Destroy();

        CreatePlayerItem table = DataManager.CreatePlayerTable[playerid] as CreatePlayerItem;
        if (table == null)
            return;

        ModelTableItem modeltable = DataManager.ModelTable[table.modelid] as ModelTableItem;
        if (modeltable == null)
            return;

        if (mActivePlayer != null)
            mActivePlayer.Destroy();

        mActivePlayer = new SimpleModel();
        mActivePlayer.CreateWithConfig(table.modelid, null, table.wingid);
        mActivePlayer.ChangeWeapon(table.weaponid);

        mActivePlayer.Position = sPos;
        mActivePlayer.Scale = sScale;
        mActivePlayer.RotationY = -120;

        mActivePlayer.ParticleMng = mParticleMng;
    }

    public void Rotate(bool positive)
    {
        if(mActivePlayer != null)
        {
            mActivePlayer.RotationY += positive ? -10 : 10;
        }
    }

    public void Clear()
    {
        if (mActivePlayer != null)
            mActivePlayer.Destroy();
        mActivePlayer = null;

        if(mRoleRoot != null)
        {
            GameObject.Destroy(mRoleRoot);
            mRoleRoot = null;
        }

    }
}

