using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KnightBehavior : MonoBehaviour
{
	[HideInInspector]
	public bool Running;

	public float Speed;

	public float JumpForce;

	private Animator _animator;
	private Rigidbody2D _rig;
	private bool grounded = true;

	public float SpeedInc;

	// Use this for initialization
	void Start ()
	{
		_animator = GetComponent<Animator>();
		_rig = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		UpdateAnimationSates();
		foreach (var o in GameObject.FindGameObjectsWithTag("background"))
		{
			var scroller = o.GetComponent<BackgroundScroller>();
			scroller.enabled = Running;
		}
		if(Running)
			Speed += SpeedInc * Time.deltaTime;
	}

	void UpdateAnimationSates()
	{
		if (Input.GetKeyDown("return"))
		{
			_animator.SetTrigger("run");
			Running = true;
		}
		if (Input.GetKeyDown("a"))
		{
			_animator.SetTrigger("attack");
		}
		if (Input.GetKeyDown("d"))
		{
			
			_animator.SetTrigger("block");
		}
		if (Input.GetKeyDown("w") && grounded)
		{
			_animator.ResetTrigger("land");
			_animator.SetTrigger("jump");
			_rig.AddForce(Vector2.up*JumpForce, ForceMode2D.Impulse);
			grounded = false;
		}
		if (Input.GetKeyDown("s"))
		{
			_animator.SetTrigger("dash");
		}
	}

	public void GetDamage(int dmg)
	{
		if (dmg > 0)
		{
			//game over
			SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex ) ;
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.collider.CompareTag("ground"))
		{
			_animator.SetTrigger("land");
			grounded = true;
		}
	}
}
