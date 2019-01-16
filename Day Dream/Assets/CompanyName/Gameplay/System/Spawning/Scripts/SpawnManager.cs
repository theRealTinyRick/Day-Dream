using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max;

namespace AH.Max.System
{
	public class SpawnManager : Singleton_MonoBehavior<SpawnManager> 
	{
		[SerializeField]
		private GameObject spawnPoolPrefab;

		public List <SpawnPool> spawnPools = new List <SpawnPool>();

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}

		///<Summary>
		/// Call this method to spawn an identityType
		///</Summary>
		public void Spawn(IdentityType identityType)
		{
			SpawnPool _spawnPool = FindSpawnPool(identityType);

			if(_spawnPool != null)
			{
				if(_spawnPool.pool.Count > 0)
				{
					if(_spawnPool.pool[0] != null)
					{
						Spawn(_spawnPool);
						return;
					}
				}
			}

			Spawn(identityType.prefab);
		}

		///<Summary>
		/// This method is used to spawn prefab from an identity type.
		///</Summary>
		private void Spawn(GameObject prefab)
		{
			// check for spawnable component
			if(prefab == null)
			{
				Debug.LogError("The entity passed into the spawn method is null. You should check on that Boss.");
				return;
			}

			GameObject _entity = Instantiate(prefab, Vector3.zero, Quaternion.identity);

			SpawnableComponent _spawnableComponent = _entity.GetComponentInChildren<SpawnableComponent>();
			if(_spawnableComponent != null)
			{
				_spawnableComponent.Spawned();
			}
		}

		///<Summary>
		/// This will spawn a spawnable from a spawn pool
		///</Summary>
		private void Spawn(SpawnPool spawnPool)
		{
			if(spawnPool == null)
			{
				Debug.LogError("The spawnPool passed into the spawn method is null. You should check on that Boss.");
				return;
			}

			GameObject _entity = spawnPool.pool[0];

			if(_entity != null)
			{
				SpawnableComponent _spawnableComponent = _entity.GetComponentInChildren<SpawnableComponent>();
				if(_spawnableComponent != null)
				{
					_entity.SetActive(true);
					_entity.transform.position = Vector3.zero;
					_entity.transform.rotation = Quaternion.identity;

					spawnPool.Remove(_entity);
					
					_spawnableComponent.Spawned();
				}
			}
		}

		///<Summary>
		/// This will despawn a spawnable and add it to an appropriate spawn pool
		///</Summary>
		public void Despawn(Entity entity)
		{
			SpawnableComponent _spawnableComponent = entity.GetComponentInChildren<SpawnableComponent>();

			if(_spawnableComponent)
			{
				SpawnPool _spawnPool = FindOrCreateSpawnPool(entity.IdentityType);

				if(_spawnPool != null)
				{
					_spawnPool.Add(entity.gameObject);

					_spawnPool.gameObject.name = entity.IdentityType.name + " _spawnPool";

					entity.transform.SetParent(_spawnPool.transform);
					
					entity.gameObject.SetActive(false);

					_spawnableComponent.Despawned();
				}
			}
		}

		///<Summary>
		/// This will find a spawn pool of a given idenetity type. If it cant it will create one.
		///</Summary>
		private SpawnPool FindOrCreateSpawnPool(IdentityType identityType)
		{
			foreach(var _spawnPool in spawnPools)
			{
				if(_spawnPool.spawnPoolType == identityType)
				{
					return _spawnPool;
				}
			}
			
			var _spawnPoolPrefab = Instantiate(spawnPoolPrefab, Vector3.zero, Quaternion.identity);

			var _spawnPoolComponent = _spawnPoolPrefab.GetComponentInChildren<SpawnPool>();

			spawnPools.Add(_spawnPoolComponent);

			_spawnPoolComponent.spawnPoolType = identityType;

			return _spawnPoolComponent;
		}

		///<Summary>
		/// This will find a spawn pool of a given idenetity type.
		///</Summary>
		private SpawnPool FindSpawnPool(IdentityType identityType)
		{
			foreach(var _spawnPool in spawnPools)
			{
				if(_spawnPool.spawnPoolType == identityType)
				{
					return _spawnPool;
				}
			}

			return null;
		}
	}
}
