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

		[SerializeField]
		private WeaponType currentWeaponType;

		public static string UsingSword = "UsingSword";
		public static string UsingAxe = "UsingAxe";

		string[] swordAnimations = new string[] {"Swing1", "Swing2", "Swing3", "Swing4", "Swing5", "Swing6"};
		string[] axeAnimations = new string[] {"Swing1", "Swing2", "Swing3", "Swing4", "Swing5", "Swing6"};

		public string[] currentWeaponAnimations
		{
			get 
			{
				if(currentWeaponType == WeaponType.GreatSword)
				{
					anim.SetBool(UsingSword, true);
					anim.SetBool(UsingAxe, false);
					return swordAnimations;
				}
				else
				{
					anim.SetBool(UsingSword, false);
					anim.SetBool(UsingAxe, true);
					return axeAnimations;
				}
			}
		}

		public int numberOfClicks = 0;
		int maxNumberOfClicks = 0;
		private float timeToAtk = 0.4f;//the amount of time between clicks the player will stop attacking
		private float _time = 0;

		[SerializeField]
		float attackRangeMin  = 5f;

		[SerializeField]
		float attackRangeMax = 9f;

		private PlayerManager pManager;
		private PlayerController pController;
		private Animator anim;

		private void Start() 
		{
			pController = GetComponent<PlayerController>();
			anim = GetComponent<Animator>();

			pManager = PlayerManager.instance;

			maxNumberOfClicks = currentWeaponAnimations.Length;
		}

		private void Update()
		{
			ClickTimer();
		}

		public void ClickTimer()
		{
			if((Time.time - _time) > timeToAtk)
			{
				foreach(string animation in currentWeaponAnimations)
				{
					if(anim.GetCurrentAnimatorStateInfo(0).IsName(animation))
					{
						isAttacking = true;
						return;
					}
				}

				foreach(string a in currentWeaponAnimations)
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
			numberOfClicks++;
			
			if(numberOfClicks > maxNumberOfClicks)
			{
				numberOfClicks = maxNumberOfClicks;
				return;   
			}

			string a = "";
			foreach(string animation in currentWeaponAnimations)
			{
				if(anim.GetCurrentAnimatorStateInfo(0).IsName(animation))
				{
					a = animation;
				}
			}

			int i = Array.IndexOf(currentWeaponAnimations, a);

			if(i > numberOfClicks)
				return;

			anim.SetBool(currentWeaponAnimations[numberOfClicks - 1], true);
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
			{
				collider.enabled = false;
			}

			isAttacking = false;

			foreach(string animation in currentWeaponAnimations)
			{
				if(anim.GetCurrentAnimatorStateInfo(0).IsName(animation))
				{
					int animIndex = Array.IndexOf(currentWeaponAnimations, animation);
					anim.SetBool(currentWeaponAnimations[animIndex], false);
					return;
				}
			}
		}
	}
}