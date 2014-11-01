using UnityEngine;
using System.Collections;

public class ObjectManager : MonoBehaviour {

    public static readonly string PathURL =
#if UNITY_ANDROID
        "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
        Application.dataPath + "/Raw/";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
 "file://" + Application.dataPath + "/StreamingAssets/";
#else
        string.Empty;
#endif
	// Use this for initialization
	void Start () {
        StartCoroutine(LoadObject(PathURL + "MRole.unity3d"));
	}
    private IEnumerator LoadObject(string path)
    {
        WWW obj = WWW.LoadFromCacheOrDownload(path, 5);

        yield return obj;

        //加载到游戏中
        // yield return Instantiate(bundle.assetBundle.mainAsset);
        AssetBundle bundle = obj.assetBundle;
        GameObject role = Instantiate(bundle.mainAsset, new Vector3(118.2422f, 21.47904f, 84.3544f), Quaternion.identity) as GameObject;

        //string name = role.name;
        // StartCoroutine(loadScene());
        // bundle.assetBundle.Unload(false);
    }
	// Update is called once per frame
	void Update () {
	
	}
}
