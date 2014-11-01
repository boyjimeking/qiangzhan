using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UIAtlasTexture : UITexture
{
	public 	UIAtlas m_atlas;
	public 	string mSprite;
	Rect m_rect;
	bool isGrey;
	
	public bool IsGrey {
		get {
			return this.isGrey;
		}
		set {
			isGrey = value;
			if(value)
			{
				shader = Shader.Find ("Custom/ItmeCantUseShader");
			}else{
				shader  = Shader.Find ("Unlit/Transparent Colored");
			}
		}
	}
	
	public Rect rect {
		get {
			return this.m_rect;
		}
		set {
			m_rect = value;
			uvRect = m_rect;
			
		}
	}
	
	public string spriteName {
		get {
			return this.mSprite;
		}
		set {
			mSprite = value;
			if(m_atlas!=null)
			{
				var v = m_atlas.GetSprite(value);
			if(mainTexture!=null)
				if(v!=null)
				{
					Rect temp = new Rect();
					temp.Set(v.x + v.borderLeft, v.y + v.borderTop,
					             v.width - v.borderLeft - v.borderRight,
					             v.height - v.borderBottom - v.borderTop);
					rect =  NGUIMath.ConvertToTexCoords(temp,mainTexture.width,mainTexture.height);	
				}else{
					rect = new Rect(0f,0f,0f,0f);	
				}
			}
		}
	}
	
	public  UIAtlas atlas {
		get {
			return this.m_atlas;
		}
		set {
			m_atlas = value;
			mainTexture = m_atlas.texture;
		}
	}
	
	override public void MakePixelPerfect ()
	{
		Texture tex = mainTexture;
		Vector3 scale = cachedTransform.localScale;
		if (tex != null)
		{
			
			scale.x = tex.width * uvRect.width;
			scale.y = tex.height * uvRect.height;
			scale.z = 1f;
			cachedTransform.localScale = scale;
		}
		//Vector3 scale = cachedTransform.localScale;

		int width  = Mathf.RoundToInt(scale.x);
		int height = Mathf.RoundToInt(scale.y);

		scale.x = width;
		scale.y = height;
		scale.z = 1f;

		Vector3 pos = cachedTransform.localPosition;
		//pos.z = Mathf.RoundToInt(pos.z);

		if (width % 2 == 1 && (pivot == Pivot.Top || pivot == Pivot.Center || pivot == Pivot.Bottom))
		{
			pos.x = Mathf.Floor(pos.x) + 0.5f;
		}
		else
		{
			pos.x = Mathf.Round(pos.x);
		}

		if (height % 2 == 1 && (pivot == Pivot.Left || pivot == Pivot.Center || pivot == Pivot.Right))
		{
			pos.y = Mathf.Ceil(pos.y) - 0.5f;
		}
		else
		{
			pos.y = Mathf.Round(pos.y);
		}

		cachedTransform.localPosition = pos;
		cachedTransform.localScale = scale;
	}
	
}
