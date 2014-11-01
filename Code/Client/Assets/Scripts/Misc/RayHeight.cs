using UnityEngine;
using System.Collections;


//通过射线取 "地面" 高度
public static class RayHeight {

	private static float checkMaxHeight = 200.0f;
	public static float rayHeight(float x , float z)
	{
		Vector3 pos = new Vector3(x , checkMaxHeight , z);
		RaycastHit rh = new RaycastHit();
		if( !Physics.Raycast(pos , Vector3.down ,out rh) )
			return 0.0f;
		return checkMaxHeight - rh.distance;
	}
}
