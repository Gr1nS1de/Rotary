using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using DG.Tweening;
//using Destructible2D;

#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

public enum GameThemeType
{
	DarkBlueGarage
}

[System.Serializable]
public struct GameTheme
{
	public GameThemeType GameThemeType;
	public PlatformView PlatformView;
	public BackgroundView BackgroundView;
}

/// <summary>
/// Class in charge of the logic of the game. This class will restart the level at game over, handle and save the point, and call the Ads if you import the VERY SIMPLE ADS asset available here: http://u3d.as/oWD
/// </summary>
public class GM : Controller
{
	public static GM Instance;

	public GameThemeType DefaultGameTheme = GameThemeType.DarkBlueGarage;
	public GameTheme CurrentGameTheme;
	public Vector2 ScreenSize;
	public Vector3 PlatformRendererSize;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			if (Instance != this)
				Destroy (this.gameObject);
		}

		Localization.InitLanguage ();

		float screenHeight = Camera.main.orthographicSize * 2.0f;
		float screenWidth = screenHeight * Camera.main.aspect;

		ScreenSize = new Vector2 (screenWidth, screenHeight);

		Notify (N.RCLoadGameTheme_, NotifyType.CORE, DefaultGameTheme);

		PlatformRendererSize = CurrentGameTheme.PlatformView.PlatformRenderer.bounds.size;
	}

	void Start()
	{

	}

	public void SetGameTheme(GameTheme gameTheme)
	{
		CurrentGameTheme.GameThemeType = gameTheme.GameThemeType;
		CurrentGameTheme.PlatformView = gameTheme.PlatformView;
		CurrentGameTheme.BackgroundView = gameTheme.BackgroundView;

		if (game.view.backgroundView != null)
			Destroy (game.view.backgroundView.gameObject);

		BackgroundView backgroundView = (BackgroundView)Instantiate (CurrentGameTheme.BackgroundView, game.view.transform);
	}

	public override void OnNotification (string alias, Object target, params object[] data)
	{/*
		switch (alias)
		{
			
		}
*/
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