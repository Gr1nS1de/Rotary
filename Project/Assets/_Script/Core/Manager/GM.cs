using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
//using Destructible2D;

#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

public class GM : Controller
{
	public static GM Instance;

	public GameThemeType DefaultGameTheme = GameThemeType.DarkBlueGarage;
	[HideInInspector]
	public Vector2 ScreenSize;

	private const string LeaderBoardPrivate = "http://dreamlo.com/lb/HmyLFso9EUmOvvnmRzgKsw1og-BQzKSU-1t0Vk36HwIg";

	private AnalyticsManager _analyticsManager = null;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;

			InitGameSettings ();
			InitTweening ();
			Localization.InitLanguage ();
		}
		else
		{
			if (Instance != this)
				Destroy (this.gameObject);
		}
			
	}

	private void InitGameSettings()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		float screenHeight = Camera.main.orthographicSize * 2.0f;
		float screenWidth = screenHeight * Camera.main.aspect;

		ScreenSize = new Vector2 (screenWidth, screenHeight);

		if(_analyticsManager == null)
			_analyticsManager = new AnalyticsManager ();

		Notify( N.RCAwakeLoad );
	}

	private void InitTweening()
	{
		//DOTween.KillAll();	

		//if (Time.realtimeSinceStartup < 1)
		DOTween.Init ();
		//DOTween.defaultAutoPlay = AutoPlay.None;
	}

	void Start()
	{		

		SetGameTheme(DefaultGameTheme);
		Notify(N.OnStart);
	}
		
	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.GameOver_:
				{
					GameOverData gameOverData = (GameOverData)data[0];
					break;
				}
		}

	}

	public void SetGameTheme(GameThemeType gameThemeType)
	{
		Notify (N.RCLoadGameTheme_, NotifyType.CORE, gameThemeType);
	}
	/*
	public Gradient		backgroundMenuGradient 	{ get { return _backgroundMenuGradient; }		set { _backgroundMenuGradient = value; } } 
	public Gradient		backgroundGameGradient 	{ get { return _backgroundGameGradient; } }
	public float		menuGradientDuration	{ get { return _menuGradientDuration; } }
	public float		gameGradientDuration	{ get { return _gameGradientDuration; } }
	public Color		currentBackgroundColor 	{ get { return _currentBackgroundColor; }		set { _currentBackgroundColor = value; } } 

	[SerializeField]
	private Gradient	_backgroundMenuGradient;
	[SerializeField]
	private Gradient 	_backgroundGameGradient;
	[SerializeField]
	private Color		_currentBackgroundColor;
	[SerializeField]
	private float		_menuGradientDuration;
	[SerializeField]
	private float		_gameGradientDuration;

	public Sprite[]		PlayerSprites;

	private	GameState	gameState				{ get { return game.model.gameState; } }
	private GameState	_lastGameState;
	private bool 		_fadeColorFlag = false;
	private float 		_fadeColorTimestamp = 0f;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			if (instance != this)
				Destroy (this.gameObject);
		}
	}
	
	void Update()
	{
		if (!game)
			return;
		//cam.backgroundColor = gameManager.m_ThemeDynamicColor;

		switch(gameState)
		{
			case GameState.MAIN_MENU:
				{
					currentBackgroundColor  = EvaluateColorFromGradient(backgroundMenuGradient, menuGradientDuration);
					break;
				}

			case GameState.PLAYING:
				{
					if (_lastGameState != gameState)
						_fadeColorFlag = true;
					
					currentBackgroundColor = EvaluateColorFromGradient(backgroundGameGradient, gameGradientDuration);

					break;
				}
		}

		_lastGameState = gameState;
	}

	private Color EvaluateColorFromGradient(Gradient gradient, float durationTime)
	{
		Color currentColor;

		float t = Mathf.PingPong (Time.time / durationTime, 1f);

		currentColor = gradient.Evaluate (t);

		if (_fadeColorFlag)
		{
			_fadeColorTimestamp = Time.time + 1f;
			_fadeColorFlag = false;
		}

		if(_fadeColorTimestamp > Time.time)
			currentColor = Color.Lerp (currentBackgroundColor, currentColor, Time.deltaTime * 5f);

		return currentColor;
	}*/
}