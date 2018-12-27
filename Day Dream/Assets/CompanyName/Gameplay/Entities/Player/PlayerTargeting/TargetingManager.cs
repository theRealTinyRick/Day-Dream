using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.System;

namespace AH.Max.Gameplay
{
	public class TargetingManager : MonoBehaviour 
	{
		/// <summary>
		/// entities that can be targeted
		/// </summary>
		/// <returns></returns>
		[TabGroup(Tabs.Properties)]
		[SerializeField]
		private List<Entity> entitiesToTarget = new List<Entity>();

		/// <summary>
		/// the identity types that can be targeted
		/// </summary>
		/// <returns></returns>
		[TabGroup(Tabs.Properties)]
		[SerializeField]
		private List <IdentityType> targetableIdentities = new List <IdentityType>();

		/// <summary>
		/// the currently targeted entity
		/// </summary>
		[SerializeField]
		private Entity currentTarget;
		
		/// <summary>
		/// the previously targeted entity
		/// </summary>
		[SerializeField]
		private Entity previousTarget;

		/// <summary>
		/// Is the current manager currently locked on
		/// </summary>
		[ShowInInspector]
		private bool lockedOn;
		public bool LockedOn
		{
			get
			{
				return lockedOn;
			}
		}

		private Transform referenceTransform;

		private void Start()
		{
			if(referenceTransform == null)
			{
				referenceTransform = transform.root;
			}
		}

		/// <summary>
		/// OnTriggerEnter is called when the Collider other enters the trigger.
		/// </summary>
		/// <param name="other">The other Collider involved in this collision.</param>
		private void OnTriggerEnter(Collider other)
		{
			Entity _entity = other.transform.root.GetComponentInChildren<Entity>();
			if(_entity != null)
			{
				if(ValidateEntity(_entity))
				{
					AddEntity(_entity);
				}
			}
		}

		/// <summary>
		/// OnTriggerExit is called when the Collider other has stopped touching the trigger.
		/// </summary>
		/// <param name="other">The other Collider involved in this collision.</param>
		private void OnTriggerExit(Collider other)
		{
			Entity _entity = other.transform.root.GetComponentInChildren<Entity>();
			if(_entity != null)
			{
				if(entitiesToTarget.Contains(_entity))
				{
					RemoveEntity(_entity);
				}
			}			
		}

		private void AddEntity(Entity entity)
		{
			entitiesToTarget.Add(entity);
		}

		private void RemoveEntity(Entity entity)
		{
			if(currentTarget == entity)
			{
				Next();
			}

			entitiesToTarget.Remove(entity);
		}

		private bool ValidateEntity(Entity entity)
		{
			if(targetableIdentities.Contains(entity.IdentityType) && !entitiesToTarget.Contains(entity))
			{
				return true;
			}
			return false;
		}

		public void Next()
		{
			if(lockedOn && currentTarget == null)
			{
				SortEntitiesToTarget();
				
				uint _currentTargetIndex = (uint)entitiesToTarget.IndexOf(currentTarget);
				uint _nextTargetIndex = _currentTargetIndex++;

				if(_nextTargetIndex == entitiesToTarget.Count - 1)
				{
					_nextTargetIndex = 0;
				}

				previousTarget = currentTarget;
				currentTarget  = entitiesToTarget[(int)_nextTargetIndex];
			}
		}

		
		public void Previous()
		{

		}

		public void LockOn()
		{

		}

		private void SortEntitiesToTarget()
		{
			// first sort by angle from left to right
			// then by distance
			Dictionary<Entity, float> entityToAngleMapper = new Dictionary<Entity, float>();

			foreach(var _entity in entitiesToTarget)
			{
				Vector3 _toVector = _entity.transform.position - referenceTransform.position;
				float _angle = Vector3.Angle(referenceTransform.forward, _toVector);

				entityToAngleMapper.Add(_entity, _angle);
			}			

			Dictionary<Entity, float> _sortedMapper = (Dictionary<Entity, float>)entityToAngleMapper.OrderBy(_entity => _entity.Value);
			entitiesToTarget = _sortedMapper.Keys.ToList();
		}

		public void LockOff()
		{

		}
	}
}
