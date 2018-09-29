using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
	public class PlayerAttack : MonoBehaviour 
	{
		private bool isAttacking = false;
		public bool IsAttacking{ get{ return isAttacking; } }

		string[] attackAnimations = new string[] {"Swing1", "Swing2", "Swing3", "Swing4", "Swing5", "Swing6"};
		public int numberOfClicks = 0;
		int maxNumberOfClicks = 0;
		private float timeToAtk = .4f;//the amount of time between clicks the player will stop attacking
		private float _time = 0;

		[SerializeField]
		float attackRangeMin  = 5f;

		[SerializeField]
		float attackRangeMax = 9f;

		private PlayerManager pManager;
		private PlayerController pController;
		// private PlayerInventory pInventory;
		private Animator anim;

		private void Start() 
		{
			pController = GetComponent<PlayerController>();
			// pInventory = GetComponent<PlayerInventory>();
			anim = GetComponent<Animator>();

			pManager = PlayerManager.instance;

			maxNumberOfClicks = attackAnimations.Length;
		}

		private void Update()
		{
			ClickTimer();
		}

		public void ClickTimer()
		{
			if((Time.time - _time) > timeToAtk)
			{
				foreach(string animation in attackAnimations)
				{
					if(anim.GetCurrentAnimatorStateInfo(0).IsName(animation))
					{
						isAttacking = true;
						return;
					}
				}

				foreach(string a in attackAnimations)
				{
					anim.SetBool(a, false);
				}

				isAttacking = false;
				numberOfClicks = 0;
				
			}
			else
			{
				isAttacking = true;
			}
		}

		public void Attack()
		{
			// if(!pInventory.Equipped)
			// {
			// 	pInventory.EquipWeapons();
			// }

			numberOfClicks++;
			if(numberOfClicks > maxNumberOfClicks)
			{
				numberOfClicks = maxNumberOfClicks;
				return;   
			}

			string a = "";
			foreach(string animation in attackAnimations)
			{
				if(anim.GetCurrentAnimatorStateInfo(0).IsName(animation))
				{
					a = animation;
				}
			}

			int i = Array.IndexOf(attackAnimations, a);

			if(i > numberOfClicks)
				return;

			anim.SetBool(attackAnimations[numberOfClicks - 1], true);
			_time = Time.time;
		}

		public void AttackStart()
		{
			BoxCollider collider = GetComponentInChildren<BoxCollider>();
			if(collider)
				collider.enabled = true;
		}

		public void EndAttack()
		{
			BoxCollider collider = GetComponentInChildren<BoxCollider>();
			if(collider)
				collider.enabled = false;

			isAttacking = false;

			foreach(string animation in attackAnimations)
			{
				if(anim.GetCurrentAnimatorStateInfo(0).IsName(animation))
				{
					int animIndex = Array.IndexOf(attackAnimations, animation);
					anim.SetBool(attackAnimations[animIndex], false);
					return;
				}
			}
		}
	}
}

