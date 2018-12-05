using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.System;
using AH.Max;

[RequireComponent(typeof(VitalsComponent))]
public class HittableComponet : MonoBehaviour 
{
	private VitalsComponent vitalsComponent;

	[SerializeField]
	[TabGroup(Tabs.Properties)]
	[Tooltip("Will be filled out at run time")]
	public Entity entity;

	[SerializeField]
	[TabGroup(Tabs.Properties)]
	public List <IdentityType> entitiesThatCanDamage;

	[SerializeField]
	[TabGroup(Tabs.Properties)]
	public List <DamageType> vulnerableToTypes;

	[SerializeField]
	[TabGroup(Tabs.Events)]
	public HitEvent hitEvent = new HitEvent();

	void Start () 
	{
		vitalsComponent = transform.parent.GetComponentInChildren<VitalsComponent>();
		entity = transform.root.GetComponentInChildren<Entity>();
	}
	
	public void Hit(DamageData data, IdentityType identityType)
	{
		if(CanDamage(identityType))
		{
			// apply resistance

			vitalsComponent.RemoveHealth(data.amount);
			hitEvent.Invoke();
		}
	}

	public void Hit(float damageAmount, IdentityType identityType)
	{
		if(CanDamage(identityType))
		{
			vitalsComponent.RemoveHealth(damageAmount);
			hitEvent.Invoke();
		}
	}

	///<Summary>
	///
	///</Summary>
	private bool CanDamage(IdentityType identityType)
	{
		foreach(IdentityType _identityTypes in entitiesThatCanDamage)
		{
			if(_identityTypes == identityType)
			{
				return true;
			}
		}
		return false;
	}

	// public void Hit(float damageAmount, )
}
