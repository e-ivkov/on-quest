using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KnightBehavior : MonoBehaviour
{
	[HideInInspector]
	public bool Running;

    public float StartSpeed;

    public float AnimMultiplier;

    [HideInInspector]
	public float Speed;

	public float JumpForce;

	private Animator _animator;
	private Rigidbody2D _rig;
	private bool grounded = true;

    private bool _recieveInput = true;

    private float _score = 0;

    public float ScoreMultiplier;

	public float SpeedInc;

    public GameObject MainMenu;

    public Text ScoreText;

	// Use this for initialization
	void Start ()
	{
		_animator = GetComponent<Animator>();
		_rig = GetComponent<Rigidbody2D>();
        int maxScore = PlayerPrefs.GetInt("maxScore", 0);
        ScoreText.text = "Max Score: " + maxScore.ToString();
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
        if (Running)
        {
            Speed += SpeedInc * Time.deltaTime;
            _score += ScoreMultiplier * Speed;
            ScoreText.text = "Score: " + ((int)_score).ToString();
            _animator.SetFloat("speed", 1 + (Speed - StartSpeed)*AnimMultiplier);
        }
        else
            Speed = 0;
	}

    public void StartGame()
    {
        _animator.SetTrigger("run");
        Speed = StartSpeed;
        Running = true;
        MainMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

	void UpdateAnimationSates()
	{
        if (!_recieveInput)
            return;
		if (Input.GetKeyDown("return"))
		{
            StartGame();
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

    public void PrepareToDie()
    {
        Running = false;
        _animator.SetTrigger("prepare_to_die");
        _recieveInput = false;
    }

	public IEnumerator GetDamage(int dmg)
	{
		if (dmg > 0)
		{
            //game over
            _animator.SetTrigger("die");
            while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("dying"))
            {
                yield return null;
            }
            while (_animator.GetCurrentAnimatorStateInfo(0).IsName("dying"))
            {
                yield return null;
            }
            int maxScore = PlayerPrefs.GetInt("maxScore", 0);
            if((int)_score > maxScore)
            {
                PlayerPrefs.SetInt("maxScore", (int)_score);
                PlayerPrefs.Save();
            }
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
