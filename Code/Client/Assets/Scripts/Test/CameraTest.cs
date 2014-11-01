using UnityEngine;
using System.Collections;

public class CameraTest : MonoBehaviour {

	private GUIText gui;
	
	private float updateInterval = 1.0f;
	private double lastInterval; // Last interval end time
	private int frames = 0; // Frames over current interval

	public Vector3 player_pos = Vector3.zero;
	public float dir_angle = 0.0f;
	
	void Start()
	{
		lastInterval = Time.realtimeSinceStartup;
		frames = 0;
	}
	
	void OnDisable()
	{
		if (gui)
			DestroyImmediate(gui.gameObject);
	}
	
	void Update()
	{
		#if !UNITY_FLASH
		++frames;
		float timeNow = Time.realtimeSinceStartup;
		if (timeNow > lastInterval + updateInterval)
		{
			if (!gui)
			{
				GameObject go = new GameObject("FPS Display", typeof(GUIText));
				go.hideFlags = HideFlags.HideAndDontSave;
				go.transform.position = new Vector3(0,0,0);
				gui = go.guiText;
                gui.pixelOffset = new Vector2(Screen.width/2 + 10, Screen.height * 0.5f);
			}
            //Vector3 pos = CameraController.Instance.CurCamera.transform.localPosition;
            //Vector3 rot = CameraController.Instance.CurCamera.transform.localEulerAngles;

            float angleX = CameraController.Instance.AngleX;
            float angleY = CameraController.Instance.AngleY;
            float distance = CameraController.Instance.Distance;
            float fieldOfView = CameraController.Instance.FieldOfView;

            string cameraOutPut = string.Format("摄像机 anglex={0},angley={1},distance={2},fieldOfView={3}", angleX, angleY, distance, fieldOfView);

			float fps = (float)(frames / (timeNow - lastInterval));
			float ms = 1000.0f / Mathf.Max (fps, 0.00001f);
			gui.text = ms.ToString("f1") + "ms " + fps.ToString("f2") + "FPS";
			gui.text += ("   玩家坐标:" + player_pos.ToString());
			gui.text += ("   玩家朝向:" +dir_angle.ToString());
            gui.text += "\r\n" + cameraOutPut;
			frames = 0;
			lastInterval = timeNow;
		}
		#endif
	}
}
