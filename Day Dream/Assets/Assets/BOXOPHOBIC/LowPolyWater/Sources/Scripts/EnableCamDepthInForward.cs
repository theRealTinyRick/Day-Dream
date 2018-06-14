using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class EnableCamDepthInForward : MonoBehaviour {

	void Start () {
		if(GetComponent<Camera>().depthTextureMode == DepthTextureMode.None)
			GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
	}

}
