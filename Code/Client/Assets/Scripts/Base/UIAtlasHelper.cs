using UnityEngine;
using System;
using System.Collections;

public class UIAtlasHelper  {

    // 表里填 atlas:image
	public static UIAtlas AnalyzeAtlas(string imgName , ref string spriteName)
    {
        string[] splitStrs = imgName.Split(new char[] {':'} , StringSplitOptions.RemoveEmptyEntries);

        if( splitStrs.Length != 2 )
        {
            return null;
        }

        UIAtlas atlas = LoadAtlas(splitStrs[0]);

        if( atlas == null )
        {
            GameDebug.LogError("Atlas : " + imgName + ":" + spriteName + "  Not Found");
            return null;
        }

        spriteName = splitStrs[1];

		//判断图集中是存在spriteName对应的sprite;
//		if(atlas.spriteList.Exists(x => x.name.Equals(spriteName)) // 谓词定义条件;
        if(atlas.GetSprite(spriteName) == null)
		{
#if UNITY_EDITOR
			GameDebug.LogError("Atlas:" + atlas.name + " not exist sprite named:" + spriteName);
#endif
		}

		return atlas;
    }

    public static void SetSpriteName(UISprite sprite, string spriteName, bool pixelPerfect = false)
    {
        if (sprite == null || string.IsNullOrEmpty(spriteName))
        {
            return;
        }

        sprite.spriteName = spriteName;

        if(pixelPerfect)
            sprite.MakePixelPerfect();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="imgName">格式为“图集:该图集中的图片名”</param>
    /// <param name="pixelPerfect"></param>
    public static void SetSpriteImage(UISprite sprite , string imgName , bool pixelPerfect = false)
    {
		if (sprite == null) return;
		if (string.IsNullOrEmpty(imgName))
		{
			sprite.atlas = null;
			sprite.spriteName = "";
			return;
		}

        string spriteName = "";
        UIAtlas atlas = AnalyzeAtlas(imgName, ref spriteName);
        if (atlas == null)
            return;
        sprite.atlas = atlas;
        sprite.spriteName = spriteName;
		if(pixelPerfect)
			sprite.MakePixelPerfect();
    }

    public static void SetSpriteByMoneyType(UISprite sprite, ProceedsType type, bool pixelPerfect = true)
    {
        string imgName = "";
        switch (type)
        {
            case ProceedsType.Money_Game:
                imgName = "jinbi1";
                break;

            //钻石;
            case ProceedsType.Money_RMB:
                imgName = "zhuanshi1";
                break;

            //声望;
            case ProceedsType.Money_Prestige:
                imgName = "shengwang1";
                break;

			//竞技场货币;
			case ProceedsType.Money_Arena:
				imgName = "hupbi3";
				break;

            default:
                Debug.LogError("没处理的货币类型");
                break;
        }

        SetSpriteImage(sprite,"common:" + imgName, pixelPerfect);
    }

    /// <summary>
    /// 给按钮设置贴图信息;
    /// </summary>
    /// <param name="button"></param>
    /// <param name="normalImg">格式为“图集:该图集中的图片名”</param>
    /// <param name="pixelPerfect"></param>
	public static void SetButtonImage(UIButton button, string normalImg , bool pixelPerfect = false)
    {
        UISprite sprite = button.gameObject.GetComponent<UISprite>();

        if( sprite == null )
        {
            return;
        }

        if (normalImg == null || string.IsNullOrEmpty(normalImg))
        {
            SetSpriteImage(sprite, null , pixelPerfect);
            button.normalSprite = "";
            return;
        }

        string spriteName = "";
        UIAtlas atlas = AnalyzeAtlas(normalImg, ref spriteName);
        if (atlas == null)
            return;

        
        SetSpriteImage(sprite, normalImg , pixelPerfect);
        button.normalSprite = spriteName;
    }

    public static void SetSpriteShaderGrey(UISprite sp, bool isGrey)
    {
        UIButton btn = sp.GetComponent<UIButton>();
        bool isBtn = btn != null;

        Color c = isGrey ? new Color(0,255,255) : new Color(1,255,255);
      
        if(isBtn)
        {
            btn.defaultColor = c;
            btn.hover = c;
            btn.pressed = c;
            btn.disabledColor = c;
        }
       
        sp.color = c;
    }

    public static void SetSpriteGrey(UISprite sp, bool isGrey)
    {
        
        if (isGrey)
        {
            if (!sp.spriteName.EndsWith("_hui"))
            {             
                UIAtlasHelper.SetSpriteImage(sp, sp.atlas.name + ":" + sp.spriteName + "_hui");
            }
        }
        else
        {
            if (sp.spriteName.EndsWith("_hui"))
            {
                int index = sp.spriteName.IndexOf("_hui");
                UIAtlasHelper.SetSpriteImage(sp, sp.atlas.name + ":" + sp.spriteName.Substring(0, index));
            }
        }
    }
    public static UIAtlas GetSpriteAtlas(UISprite sprite)
    {
        if (sprite == null)
            return null;

        return sprite.atlas;
    }

    public static UIAtlas LoadAtlas(string name)
    {
       // string atlasName = "Atlas/" + name;

        UIAtlas atlas = UIResourceManager.Instance.GetAtlas(name); //ResourceManager.Instance.LoadAtlas(atlasName);
        if (atlas == null)
            return null;

        return atlas;
    }
}
