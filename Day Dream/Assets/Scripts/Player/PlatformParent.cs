using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformParent{

	public static void ParentToPlatform(Transform platform, Transform player){
		if(platform.tag == "Platform"){
			player.SetParent(platform);
		}else{
			player.SetParent(null);
		}
			
	}

	public static void RemoveParent(Transform player){
		player.SetParent(null);
	}
}
