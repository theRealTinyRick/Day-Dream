using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.System
{
	public class SpawnableComponent : MonoBehaviour 
	{	
		[SerializeField]
		public Entity entity;

		[SerializeField]
		public SpawnedEvent spawnedEvent = new SpawnedEvent();

		[SerializeField]
		public DespawnedEvent despawnedEvent = new DespawnedEvent();

		private void Start()
		{
			entity = GetComponentInChildren<Entity>();
		}

		public void Spawned()
		{
			if(spawnedEvent != null)
			{
				spawnedEvent.Invoke();
			}
		}

		public void Despawned()
		{
			if(despawnedEvent != null)
			{
				despawnedEvent.Invoke();
			}
		}
	}
}
