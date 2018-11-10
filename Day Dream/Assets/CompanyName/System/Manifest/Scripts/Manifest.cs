using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Manifest", menuName = "CompanyName/Manifest", order = 1)]
public class Manifest : ScriptableObject
{
	[SerializeField]
	private ManifestType manifestType;
	public ManifestType ManifestType
	{
		get { return manifestType; }
	}

	///<Summary>
	/// Theses are scenes that are actual playable levels in the game.
	///</Summary>
	[Tooltip("Theses are scenes that are actual playable levels in the game.")]
	[SerializeField]
	private List <LevelData> levels = new List<LevelData>();
	public List <LevelData> Levels
	{
		get { return levels; }
	}

	///<Summary>
	/// These are the scenes that the game uses on startup. 
	///</Summary>
	[Tooltip("These are the scenes that the game uses on startup. ")]
	[SerializeField]
	private List <UnityEngine.SceneManagement.Scene> startUpScenes = new List<UnityEngine.SceneManagement.Scene>();
	public List <UnityEngine.SceneManagement.Scene> StartUpScenes
	{
		get { return startUpScenes; }
	}

	///<Summary>
	/// Theses are scenes that the game needs to run the game logic. UI, Entities, ect...
	///</Summary>
	[Tooltip("Theses are scenes that the game needs to run the game logic. UI, Entities, ect...")]
	[SerializeField]
	private List <UnityEngine.SceneManagement.Scene> systemScenes = new List <UnityEngine.SceneManagement.Scene>();
	public List <UnityEngine.SceneManagement.Scene> SystemScenes
	{
		get { return systemScenes; }
	}

	///<Summary>
	/// The Main Menu Scene
	///</Summary>
	[Tooltip("The Main Menu Scene")]
	[SerializeField]
	private UnityEngine.SceneManagement.Scene mainMenu;
	public UnityEngine.SceneManagement.Scene MainMenu
	{
		get { return mainMenu; }
	}

	[ShowInInspector]
	private Manifest currentlyActivatedManifest
	{
		get { return ManifestManager.CurrentManifest; }
	}

	[Button]
	public void Activate()
	{
		ManifestManager.SetManifestType(this);		
	}
}

