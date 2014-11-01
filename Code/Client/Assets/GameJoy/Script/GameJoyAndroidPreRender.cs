using UnityEngine;
using System.Collections;

public class GameJoyAndroidPreRender : MonoBehaviour {
	bool hasPrintLog = false;
#if !UNITY_EDITOR && UNITY_ANDROID
	void OnPreRender() {
		int result = GameJoySDK.getGameJoyInstance().BeginDraw();
		if(hasPrintLog == false){
			Debug.Log("GameJoy on pre render | result = " + result);
			hasPrintLog = true;
		}
	}
#endif
}
