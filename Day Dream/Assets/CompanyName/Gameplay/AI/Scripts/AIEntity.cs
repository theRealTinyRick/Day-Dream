using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AH.Max.Gameplay.AI
{
	public abstract class AIEntity : AH.Max.Entity
	{
		public virtual void Initialize()
		{
			//do stuff here to implement any thing in the inheriting class
		}

		protected void Move(NavMeshAgent _navMeshAgent, Vector3 targetPosition)
		{
			_navMeshAgent.SetDestination(targetPosition);
		}

		//Check whether or not ther player is in front of the enemy
		protected bool CheckFieldOfView(Transform _myTransform, Vector3 _targetPosition, float maxAngle)
		{
			float angle;

			Vector3 _toVector = _targetPosition - _myTransform.position;
			_toVector.y = _myTransform.position.y;

			angle = Vector3.Angle(_myTransform.forward, _toVector);

			if(angle <= maxAngle)
			{
				return false;
			}

			return false;
		}

		// This function will check the squared distance
		//This function will also act as if the positions are on the same plane. (The y values will be zerod out)
		protected bool CheckRangeSquared(float _range, Vector3 _myPosition, Vector3 _targetPosition)
		{
			_range *= _range;

			Vector3 _toVector = _targetPosition - _myPosition;
			_toVector.y = 0;

			float _magnitude = ( _toVector.x * _toVector.x ) + (_toVector.z * _toVector.z);

			if(_magnitude <= _range)
			{
				return true;
			}

			return false;
		}

		protected bool CheckLineOfSight(Vector3 _myPosition, Vector3 _targetPosition, LayerMask playerLayer)
		{
			//move the positions up about 2 units to be head level
			_myPosition.y += 2;
			_targetPosition.y += 2;

			Vector3 _direction = _targetPosition - _myPosition;
			float _distance = Vector3.Distance(_myPosition, _targetPosition);
			RaycastHit _hit;

			if(Physics.Raycast(_myPosition, _direction, out _hit, _distance, playerLayer))
				return false;

			return true;
		}

		protected bool CheckHeightDifference(float _range, Vector3 _myPosition, Vector3 _targetPosition)
		{
			float _topHieght = _myPosition.y > _targetPosition.y ? _myPosition.y :  _targetPosition.y;
			float _bottomPosition = _myPosition.y < _targetPosition.y ? _myPosition.y :  _targetPosition.y;

			float _difference = _topHieght - _bottomPosition;

			if(_difference <= _range)
				return true;

			return false;
		}

		protected virtual bool CheckAggro()
		{
			
			return false;
		}
	}
}

