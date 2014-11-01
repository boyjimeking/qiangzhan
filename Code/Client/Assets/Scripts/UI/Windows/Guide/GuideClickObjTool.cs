using UnityEngine;
using System.Collections;

public class GuideClickObjTool : MonoBehaviour {

    private UICamera mCamera = null;

    private GUIText gui;

    UICamera MC
    {
        get
        {
            if (mCamera == null)
            {
                mCamera = gameObject.GetComponent<UICamera>();
            }
            return mCamera;
        }
    }

	// Use this for initialization
	void Start () {
	    
	}

    void OnDestroy()
    {
        if (!gui)
            GameObject.Destroy(gui);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (MC == null)
                return;

//#if UNITY_EDITOR
            GameObject go = UICamera.hoveredObject;
            if (!go)
                return;

            //GameDebug.LogError("控件名：" + go.name);
            string uiname = "";
            //PromptUIManager.Instance.AddNewPrompt("控件名：" + getObjectFullName(go , ref uiname));
            GameDebug.LogError("控件名：" + getObjectFullName(go, ref uiname));
            //PromptUIManager.Instance.AddNewPrompt("界面名：" + uiname);
            GameDebug.LogError("界面名：" + uiname);

            string head = "控件类型：";
            string type = "";
            string content = "";
            if (go.GetComponent<UIButton>() != null)
            {
                //GUILayout.Label(head + "button", GUILayout.Width(400), GUILayout.Height(225));
                GameDebug.LogError(head + "button");
                //PromptUIManager.Instance.AddNewPrompt(head + "button");
            }
            else if (go.GetComponent<UIToggle>() != null)
            {
                GameDebug.LogError(head + "toggle");
            }
            else
            {
                do
                {
                    if (go.GetComponent<UILabel>() != null)
                    {
                        type = "label";
                        break;
                    }
                    if (go.GetComponent<UISprite>() != null)
                    {
                        type = "sprite";
                        break;
                    }
                    if (go.GetComponent<UIWidget>() != null)
                    {
                        type = "sprite";
                        break;
                    }

                } while (false);
                content = "不支持的控件类型:" + type + "，找程序添加";
                //PromptUIManager.Instance.AddNewPrompt(content);
                GameDebug.LogError("不支持的控件类型:" + type + "，找程序添加");
            }
//#endif
        }
	}

    
    
    /// <summary>
    /// 获取对象的完整名称;
    /// </summary>
    /// <param name="go">子控件名字</param>
    /// <param name="uiname">界面名字</param>
    /// <returns></returns>
    string getObjectFullName(GameObject go , ref string uiname)
    {
        Transform temp = null;

        string res = ""; //界面预设名;

        int i = 10;     //防锁死;

        temp = go.transform;
        while (true)
        {
            res = res.Insert(0, "\\" + temp.name);

            if (WindowManager.Instance.IsUIByPrefabName(temp.name, ref uiname))
                break;

            temp = temp.parent;
            --i;

            if (i < 0)
                return "没找到";
        }

        return res.Remove(0,1);
    }
}
