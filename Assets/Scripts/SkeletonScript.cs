using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkeletonScript : MonoBehaviour
{

	public const float Velocity = 0.71f;
	private float _playerVelocity;

	public enum MonsterType
	{
		Ogre = 1,
		Harpy = 2,
		Beast = 3,
		Skeleton = 4,
	}
	
	public MonsterType Monster;
	
	public enum DodgeType
	{
		None = 0,
		Defend= 1,
		Dash = 2,
		Jump = 3,
		Attack = 4
	}

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update () {
		_playerVelocity = GameObject.FindGameObjectWithTag("Player").GetComponent<KnightBehavior>().Speed;
		if(GameObject.FindGameObjectWithTag("Player").GetComponent<KnightBehavior>().Running)
			transform.Translate(Time.deltaTime*Velocity*_playerVelocity*Vector3.left);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if(!other.CompareTag("Player"))
			return;
		var animator = other.gameObject.GetComponent<Animator>();
		var state = animator.GetCurrentAnimatorStateInfo(0);
		var cases = new Dictionary<int, DodgeType>()
		{
			{ Animator.StringToHash("attacking"), DodgeType.Attack },
			{ Animator.StringToHash("blocking"), DodgeType.Defend },
			{ Animator.StringToHash("dashing"), DodgeType.Dash },
			{ Animator.StringToHash("jumping"), DodgeType.Jump },
		};

		var dodge = DodgeType.None;
		cases.TryGetValue(state.shortNameHash, out dodge);
		Debug.Log(state.fullPathHash);
		if ((int) dodge != (int) Monster)
		{
			other.GetComponent<KnightBehavior>().GetDamage(1);
		}
	}
}
