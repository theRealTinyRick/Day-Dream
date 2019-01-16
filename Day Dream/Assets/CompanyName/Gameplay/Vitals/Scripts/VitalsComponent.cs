using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.System;
using AH.Max;

public class VitalsComponent : SerializedMonoBehaviour
{
	[SerializeField]
	[TabGroup(Tabs.Properties)]
	[Tooltip("Will be filled out at run time")]
	public Entity entity;

	[SerializeField]
	[TabGroup(Tabs.Stats)]
	public StatType healthStatType;

	[SerializeField]
	[TabGroup(Tabs.Stats)]
	private Dictionary <StatType, Stat> stats = new Dictionary<StatType, Stat>();
	public Dictionary <StatType, Stat> Stats
	{
		get
		{
			return stats;
		}
		set
		{
			stats = value;
			if(stats[healthStatType].Amount <= stats[healthStatType].MinimumAmount)
			{
				noHealthEvent.Invoke();
			}
		}
	}

	[SerializeField]
	[TabGroup(Tabs.Events)]
	public AddedHealthEvent addedHealthEvent = new AddedHealthEvent();

	[SerializeField]
	[TabGroup(Tabs.Events)]
	public RemovedHealthEvent removedHealthEvent = new RemovedHealthEvent();

	[SerializeField]
	[TabGroup(Tabs.Events)]
	public NoHealthEvent noHealthEvent = new NoHealthEvent();

	private void Start()
	{
		Initialize();
	}

	private void Initialize()
	{
		entity = transform.root.GetComponentInChildren<Entity>();
		stats[healthStatType].Reset();
	}

	public void RemoveHealth(float amount)
	{
		stats[healthStatType].Subtract(amount);
		
		removedHealthEvent.Invoke();

		if(stats[healthStatType].Amount <= stats[healthStatType].MinimumAmount)
		{
			noHealthEvent.Invoke();
		}
	}

	public void AddHealth(float amount)
	{
		stats[healthStatType].Add(amount);

		addedHealthEvent.Invoke();
	}

	public void RemoveAllHealth()
	{
		stats[healthStatType].RemoveAll();
		noHealthEvent.Invoke();
	}
}
