using UnityEngine;
using System.Collections;

public class UIAnimation : MonoBehaviour
{
    private UISprite mSprite = null;
    void Start()
    {
        mSprite = this.GetComponent<UISprite>();
        if( mSprite == null )
        {
            GameDebug.Log("UIAnimation 必须挂在UISprite上");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
