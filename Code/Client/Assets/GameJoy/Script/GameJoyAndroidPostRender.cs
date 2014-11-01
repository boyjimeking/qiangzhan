using UnityEngine;
using System.Collections;

public class GameJoyAndroidPostRender : MonoBehaviour {
	bool hasPrintLog = false;
#if !UNITY_EDITOR && UNITY_ANDROID
	void OnPostRender() {
		int result = GameJoySDK.getGameJoyInstance().EndDraw();
		if(hasPrintLog == false){
			Debug.Log("GameJoy on post render | result=" + result);
			hasPrintLog = true;
		}				
		if(result == 1){
			Debug.Log("GameJoy on post render | result=" + result);
			int cameraCount = 0;
			//Camera.main.Render ();
			Camera[] cameras = Camera.allCameras;
			if(cameras != null){
				foreach(Camera camera in cameras){
					if(camera != null){
						camera.Render();
						cameraCount++;
					}
				}
			}
			Debug.Log("GameJoy on post render camreas = " + cameraCount);
		}		
	}
#endif
}
