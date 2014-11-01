using UnityEngine;
using System.Collections;

public class PlayerHeadUI
{
    protected UILabel nameLb;
    protected UISprite titleSp;
    
    private GameObject mObj;

    public PlayerHeadUI(GameObject go)
    {
        mObj = go;
        if (go == null)
            return;
        
        nameLb = ObjectCommon.GetChild(go , "name").GetComponent<UILabel>();
        titleSp = ObjectCommon.GetChildComponent<UISprite>(go, "titleSp");
    }
	
    public void SetName(string name)
    {
        nameLb.text = name;
    }

    public void SetTitle(string spriteInfo)
    {
        if (string.IsNullOrEmpty(spriteInfo))
        {
            titleSp.gameObject.SetActive(false);
            return;
        }
        UIAtlasHelper.SetSpriteImage(titleSp, spriteInfo, true);
        titleSp.gameObject.SetActive(true);
    }

    public GameObject gameObject
    {
        get { return mObj; }
    }
}
