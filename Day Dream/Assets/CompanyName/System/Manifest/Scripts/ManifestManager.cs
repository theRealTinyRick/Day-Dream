using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AH.Max;


[System.Serializable]
public class ManifestManager : MonoBehaviour
{
	private static ManifestType currentManifestType;
	public static ManifestType CurrentManifestType
	{
		get { return currentManifestType; }
	}

	private static Manifest currentManifest;
	public static Manifest CurrentManifest
	{
		get { return currentManifest; }
	}

	private Manifest testManifest;
	private Manifest FullManifest;
	private Manifest demoManifest;

	public static void SetManifestType( Manifest _manifest )
	{ 
		if( _manifest == null ) return;

		currentManifestType = _manifest.ManifestType;		
		currentManifest = _manifest;
	} 

	public static void LoadManifest()
	{

	}
}



