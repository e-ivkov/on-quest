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

    private float _distance = 0;

    private float _score = 0;

    public float ScoreMultiplier;

	public float SpeedInc;

    public GameObject MainMenu;

    public Text ScoreText;

    public GameObject TutorialCanvas;

    public float Distance { get => _distance; private set => _distance = value; }
    public GameMode GameMode { get; private set; } = GameMode.Game;

    public LevelGenerator LevelGenerator;

    // Use this for initialization
    void Start ()
	{
		_animator = GetComponent<Animator>();
		_rig = GetComponent<Rigidbody2D>();
        int maxScore = PlayerPrefs.GetInt("maxScore", 0);
        ScoreText.text = "Max Score: " + maxScore.ToString();
        LevelGenerator.Generate(GameMode);
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
            Distance += ScoreMultiplier * Speed;
            _score = Distance;
            ScoreText.text = "Score: " + ((int)_score).ToString();
            _animator.SetFloat("speed", 1 + (Speed - StartSpeed)*AnimMultiplier);
        }
        else
            Speed = 0;
	}

    private void StartRunning()
    {
        _animator.enabled = true;
        _animator.SetTrigger("run");
        Speed = StartSpeed;
        Running = true;
    }

    public void StartGame()
    {
        GameMode = GameMode.Game;
        LevelGenerator.Generate(GameMode);
        StartRunning();
        MainMenu.SetActive(false);
    }

    public void StartTutorial()
    {
        GameMode = GameMode.Tutorial;
        LevelGenerator.Generate(GameMode);
        StartRunning();
        MainMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Freeze()
    {
        Running = false;
        _animator.enabled = false;
    }

    private delegate void TutorialActionHandler(DodgeType dodge);
    private TutorialActionHandler _tutorialActionHandler;
    private bool _waitingForTutorialInput = false;

    public void ShowTutorialMessage(SkeletonScript enemy, MissScript missScript)
    {
        TutorialCanvas.SetActive(true);
        TutorialCanvas.GetComponent<TutorialTextScript>().ShowHint(enemy.Monster);
        Freeze();
        _waitingForTutorialInput = true;
        _tutorialActionHandler = (dodge) =>
        {
            if ((int)dodge == (int)enemy.Monster)
            {
                missScript.Miss();
                _waitingForTutorialInput = false;
                TutorialCanvas.SetActive(false);
                StartRunning();
            }
        };
    }

	void UpdateAnimationSates()
	{
        if (!_recieveInput)
            return;
        var action = DodgeType.None;
		if (Input.GetKeyDown("return"))
		{
            StartGame();
		}
		if (Input.GetKeyDown("a"))
		{
			_animator.SetTrigger("attack");
            action = DodgeType.Attack;
		}
		if (Input.GetKeyDown("d"))
		{			
			_animator.SetTrigger("block");
            action = DodgeType.Defend;
        }
		if (Input.GetKeyDown("w") && grounded)
		{
			_animator.ResetTrigger("land");
			_animator.SetTrigger("jump");
			_rig.AddForce(Vector2.up*JumpForce, ForceMode2D.Impulse);
			grounded = false;
            action = DodgeType.Jump;
        }
		if (Input.GetKeyDown("s"))
		{
			_animator.SetTrigger("dash");
            action = DodgeType.Dash;
        }
        if (_waitingForTutorialInput)
        {
            _tutorialActionHandler(action);
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
