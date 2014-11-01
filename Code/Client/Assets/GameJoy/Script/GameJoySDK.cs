using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text.RegularExpressions;
public class GameJoySDK : MonoBehaviour {
	public bool enableOnStart = true;
	public static GameJoySDK gameJoy = null;
	// Use this for initialization

#if !UNITY_EDITOR && UNITY_ANDROID
	private AndroidJavaClass mQMiObj = null;
	private AndroidJavaObject playerActivityContext = null;
	void Start() {
		Debug.Log ("gamejousdk start");
		if (enableOnStart == true) {
			Debug.Log ("gamejousdk start qmi");
			StartQMi ();
			Debug.Log ("end gamejousdk start qmi");
		}

		DontDestroyOnLoad (this.gameObject);
	}
	
	public static GameJoySDK getGameJoyInstance(){
		if(gameJoy == null){
			gameJoy = new GameJoySDK();
		}
		return gameJoy;
	}
	
	// Update is called once per frame
	public void Update () {
		if(mQMiObj == null){
			mQMiObj = mQMiObjJavaClass();
		}
		if (mQMiObj != null) {
			mQMiObj.CallStatic("onUpdateVideoFrame");
		}
		else{
			Debug.Log("Update mQMiObj = null");
		}

	}
	
	public int BeginDraw(){
		int result = 0;
		if(mQMiObj == null){
			mQMiObj = mQMiObjJavaClass();
		}
		if (mQMiObj != null) {
			result = mQMiObj.CallStatic<int>("beginDraw");
		}
		else{
			Debug.Log("BeginDraw mQMiObj = null");
		}
		return result;
	}
	
	public int EndDraw(){
		int result = 0;
		if(mQMiObj == null){
			mQMiObj = mQMiObjJavaClass();
		}
		if (mQMiObj != null) {
			result = mQMiObj.CallStatic<int>("endDraw");
		}
		else{
			Debug.Log("EndDraw mQMiObj = null");
		}
		return result;
	}
	
	void Awake(){
		Debug.Log("GameJoy Awake");
		if(mQMiObj == null){
			mQMiObj = mQMiObjJavaClass();
		}
		if(mQMiObj == null)
		{
			Debug.Log ("GameJoy: Java class not accessible from C#.");
		}
		else
		{
			InitializeRenderCamera("Pre");
			InitializeRenderCamera("Post");
		}
	}
	
	private void InitializeRenderCamera(string type){
		if( type.Equals("Pre") || type.Equals("Post") ){
			// Set up the render camera, if it doesn't already exist.
			if( GameObject.Find("GameJoy"+type+"Camera") == null ){
				GameObject cameraObject = new GameObject();
				Camera camera = (Camera) cameraObject.AddComponent("Camera");
				camera.name = "GameJoy"+type+"Camera";
				camera.clearFlags = CameraClearFlags.Nothing;
				camera.cullingMask = 0;

				if( type.Equals("Pre") ){
					camera.depth = Single.MinValue;
				}
				else{
					camera.depth = Single.MaxValue;
				}
				Debug.Log ("InitializeRenderCamera start add gamejoy render");
				// Attach the render script.
				camera.gameObject.AddComponent("GameJoyAndroid"+type+"Render");
				Debug.Log ("InitializeRenderCamera start SetActive");
				// Keep this fella around.
				cameraObject.SetActive(true);
				UnityEngine.Object.DontDestroyOnLoad(cameraObject);
			}
		}
	}
	
	private AndroidJavaObject getActivityContext(){
		if(playerActivityContext == null){
			AndroidJavaClass actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			if(actClass == null){
				Debug.Log("Get UnityPlayer Class failed");
				return null;
			}
			playerActivityContext = actClass.GetStatic<AndroidJavaObject>("currentActivity");
			if (playerActivityContext == null){
				Debug.Log("get context failed");
				return null;
			}
		}
		return playerActivityContext;
	}
	
	private AndroidJavaClass mQMiObjJavaClass(){
		if ( mQMiObj == null ){
			mQMiObj = new AndroidJavaClass("com.tencent.qqgamemi.QmiSdkApi");
		}
		if ( mQMiObj == null ){
			Debug.Log("GameJoy: Unable to find GameJoy java class.");
		}
		return mQMiObj;
	}

	public void StartQMi(){
		if (mQMiObj == null) {
			mQMiObj = mQMiObjJavaClass();
		}
		if(mQMiObj != null){
			Debug.Log ("gamejousdk StartQMi new qmioubj");
			playerActivityContext = getActivityContext();
			if (playerActivityContext == null){
				Debug.Log("startqmi get context failed");
				return;
			}
			Debug.Log ("gamejousdk StartQMi start call show qim");
			mQMiObj.CallStatic("showQMi", playerActivityContext,"Unity3D_412");
			Debug.Log ("gamejousdk StartQMi end call show qim");
		}
		else{
			Debug.Log ("gamejousdk StartQMi mqmiobj = null");
		}
	}

	public void StopQMi(){
		Debug.Log("gamejoy start hidQMi");
		if(mQMiObj == null){
			Debug.Log("gamejoy hidqmi mqmiobj = null");
			mQMiObj = mQMiObjJavaClass();
		}
		if (mQMiObj != null) {
			playerActivityContext = getActivityContext();
			if(playerActivityContext == null) {
				Debug.Log ("stop qmi get context fail");
				return;
			}
			Debug.Log("gamejoy call hidQMi");
			mQMiObj.CallStatic("hideQMi",playerActivityContext);
		}
		else{
			Debug.Log ("StopQMi get mQMiObj fail");
		}
	}
	
	public void ScollToSide(){
		Debug.Log("gamejoy start ScollToSide");
		if(mQMiObj == null){
			Debug.Log("gamejoy ScollToSide mqmiobj = null");
			mQMiObj = mQMiObjJavaClass();
		}
		if (mQMiObj != null) {
			playerActivityContext = getActivityContext();
			if(playerActivityContext == null) {
				Debug.Log ("ScollToSide get context fail");
				return;
			}
			Debug.Log("gamejoy call ScollToSide");
			mQMiObj.CallStatic("scollToSide",playerActivityContext);
		}
		else{
			Debug.Log ("ScollToSide get mQMiObj fail");
		}
	}

	public void StartRecord(){
		Debug.Log("gamejoy start StartRecord");
		if(mQMiObj == null){
			mQMiObj = mQMiObjJavaClass();
		}
		if (mQMiObj != null) {
			mQMiObj.CallStatic ("onStartRecordVideo");
		}
		else{
			Debug.Log("StartRecord mQMiObj = null");
		}
	}

	public void StopRecord(){
		Debug.Log("gamejoy start StopRecord");
		if(mQMiObj == null){
			mQMiObj = mQMiObjJavaClass();
		}		
		if (mQMiObj != null) {
			mQMiObj.CallStatic("onStopRecordVideo");
		}
		else{
			Debug.Log("StopRecord mQMiObj = null");
		}
	}

	void OnApplicationPause(){
		Debug.Log("gamejoy start OnApplicationPause");
		if(mQMiObj == null){
			mQMiObj = mQMiObjJavaClass();
		}		
		if (mQMiObj != null) {
			mQMiObj.CallStatic("onGameEnterBackground");
		}
		else{
			Debug.Log("OnApplicationPause mQMiObj = null");
		}
	}

	void OnApplicationFocus(){
		Debug.Log("gamejoy start OnApplicationFocus");
		if(mQMiObj == null){
			mQMiObj = mQMiObjJavaClass();
		}		
		if (mQMiObj != null) {
			mQMiObj.CallStatic("onGameEnterForeground");
		}
		else{
			Debug.Log("OnApplicationFocus mQMiObj = null");
		}
	}

	void OnApplicationQuit(){
		StopRecord();
		StopQMi ();
	}
	
	void GetGameEngineType(){
		if(mQMiObj == null){
			mQMiObj = mQMiObjJavaClass();
		}		
		if (mQMiObj != null) {
			String unityVersion = "Unity3D_" + Application.unityVersion;
			mQMiObj.CallStatic("setGameEngineType",unityVersion);
			Debug.Log("GetGameEngineType unityVersion = " + unityVersion);
		}
		else{
			Debug.Log("GetGameEngineType mQMiObj = null");
		}
	}
#else
    public static GameJoySDK getGameJoyInstance()
    {
        if (gameJoy == null)
        {
            gameJoy = new GameJoySDK();
        }
        return gameJoy;
    }
    public void ScollToSide()
   {
    }
#endif
}
