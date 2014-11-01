using FantasyEngine;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

public class EngineRun : MonoBehaviour
{
    public string rolename = "weapon";
    public string particlename = "cursor";
    MeshVisual visual;

    ParticleVisual parVisual;

    public UIInput animInput;

    public CameraPathAnimator cameraPath;

    void Start()
    {
        NGUIDebug.Log("设置的显示级别为"+ QualitySettings.GetQualityLevel());
        EngineInitialize.Init();
    }

    public void btnLoadSkeletal()
    {
        visual = new MeshVisual();
        visual.CreateWithConfig(animInput.value,null , null, true);
    }
    public void btnLoadParticle()
    {

        //parVisual = new ParticleVisual();
        //parVisual.CreateWithConfig(animInput.value);

    }

    public void btnLoadscene()
    {
        //string scn = "Scene/pve_xgk01.ab";

        GameObject.DontDestroyOnLoad(GameObject.Find("UI Root"));
        EngineRun runScript = GameObject.FindObjectOfType<EngineRun>();
        GameObject.DontDestroyOnLoad(runScript.gameObject);

        //Resources.UnloadUnusedAssets();

        //ResourceManager.Instance.LoadScene(scn, "pve_xgk01", SceneLoadeComplete);
    }
    private void SceneLoadeComplete()
    {
      GameObject obj =  GameObject.Find("CameraPath");
      if (obj == null)
          return;
      cameraPath = obj.GetComponent<CameraPathAnimator>();

    }

    public void btnPlay()
    {
        if (cameraPath == null)
            return;
        cameraPath.Play();
    }
    public void btnPause()
    {
        if (cameraPath == null)
            return;
        cameraPath.Pause();
    }
    public void playAnimation()
    {
        //GameObject ghost = new GameObject("ghost");
        //parVisual.Visual.transform.parent = ghost.transform;

        //visual.AnimControl.Play(animInput.value);

        //visual.PlayActionIndex("ActionIndex", int.Parse(animInput.value));
    }

    public void btnLoadLight()
    {
        LightShadowManager.Instance.InitRoleLight();
    }

    public void WalkRun()
    {
       //if(visual.AnimControl != null)
       //{
       //    visual.AnimControl.Speed = float.Parse(animInput.value);
       //}
    }

    public void wwwTest()
    {

        //StartCoroutine(loadItem("file://E:/abc/bone.ab"));
        //StartCoroutine(loadItem("file://E:/abc/bone.ab"));

        //StartCoroutine(loadItem("file://E:/8087/mobileGame/Bin/client/StreamingAssets/Model/Role/dazhaungyuancheng/bone_characterbase.ab"));
        //StartCoroutine(loadItem("file://E:/8087/mobileGame/Bin/client/StreamingAssets/Model/Role/male/bone_characterbase.ab"));

        //StartCoroutine(loadItem("file://E:/8087/mobileGame/Bin/client/StreamingAssets/Model/Role/male/skins/gangtiezhanshi_renxingbing.ab"));
        //StartCoroutine(loadItem("file://E:/8087/mobileGame/Bin/client/StreamingAssets/Model/Role/male/skins/hongyan_hongyan1.ab"));
        string path = Application.dataPath;
        GameDebug.Log(path);

        
    }

    private IEnumerator loadItem(string asset)
    {
        WWW www = new WWW(asset);

        yield return www;

        AssetBundle bundle = www.assetBundle;

        Debug.Log(bundle.GetHashCode());

        Object[] objs = bundle.LoadAll();

        foreach (Object obj in objs)
        {
            Debug.Log(obj.GetHashCode());
        }
        //SingleStringHolder holder = bundle.mainAsset as SingleStringHolder;
        Debug.Log(true);
    }
}
