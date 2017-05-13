using UnityEngine;
using System.Collections.Generic;
//using DG.Tweening;
using UnityEngine.SceneManagement;
//using Destructible2D;
using DG.Tweening;

public class GameController : Controller
{
	#region Declare controllers reference
	public CameraController					cameraController				{ get { return _cameraController 			= SearchLocal<CameraController>(			_cameraController,				typeof(CameraController).Name ); } }
	public GameSoundController				gameSoundController				{ get { return _gameSoundController			= SearchLocal<GameSoundController>(			_gameSoundController,			typeof(GameSoundController).Name ); } }
	public ResourcesController				resourcesController				{ get { return _resourcesController 		= SearchLocal<ResourcesController>(			_resourcesController,			typeof(ResourcesController).Name ); } }
	public ObjectsPoolController			objectsPoolController			{ get { return _objectsPoolController 		= SearchLocal<ObjectsPoolController> (		_objectsPoolController, 		typeof(ObjectsPoolController).Name);}}
	public PlayerController 				playerController				{ get { return _playerController			= SearchLocal<PlayerController>(			_playerController,				typeof(PlayerController).Name );}}
	public ItemsFactoryController			itemsFactoryController			{ get { return _itemsFactoryController		= SearchLocal<ItemsFactoryController>(		_itemsFactoryController,		typeof(ItemsFactoryController).Name );}}
	public BonusesFactoryController			bonusesFactoryController		{ get { return _bonusesFactoryController	= SearchLocal<BonusesFactoryController>(	_bonusesFactoryController,		typeof(BonusesFactoryController).Name );}}
	public PlatformsFactoryController		platformsFactoryController		{ get { return _platformsFactoryController	= SearchLocal<PlatformsFactoryController>(	_platformsFactoryController,	typeof(PlatformsFactoryController).Name );}}
	public DistructibleController			distructibleController			{ get { return _distructibleController		= SearchLocal<DistructibleController>(		_distructibleController,		typeof(DistructibleController).Name );}}
	public GameSpeedController				gameSpeedController				{ get { return _gameSpeedController			= SearchLocal<GameSpeedController>(			_gameSpeedController,			typeof(GameSpeedController).Name );}}

	private GameSpeedController				_gameSpeedController;
	private DistructibleController			_distructibleController;
	private PlatformsFactoryController		_platformsFactoryController;
	private BonusesFactoryController		_bonusesFactoryController;
	private ItemsFactoryController			_itemsFactoryController;
	private PlayerController				_playerController;
	private CameraController				_cameraController;
	private GameSoundController				_gameSoundController;
	private ResourcesController				_resourcesController;
	private ObjectsPoolController 			_objectsPoolController;
	#endregion

	//private GameModel 				playerModel	{ get { return game.model.playerModel;}}

	public override void OnNotification( string alias, Object target, params object[] data )
	{
		switch ( alias )
		{
			case N.RCAwakeLoad:
				{
					Notify (N.RCLoadGameTheme_, NotifyType.CORE, GM.Instance.DefaultGameTheme);
					break;
				}

			case N.OnStart:
				{
					OnStart();

					game.model.gameState = GameState.MainMenu;
					break;
				}

			case N.GameStartPlay:
				{
					OnGameStartPlay ();

					game.model.gameState = GameState.Playing;
					break;
				}

			case N.PlayerImpactItem__:
				{
					ItemView itemView = (ItemView)data [0];
					Vector2 contactPoint = (Vector2)data [1];

					OnPlayerImpactItem (itemView);
					break;
				}

			case N.PlayerLeftPlatform_:
				{
					PlatformView platformView = (PlatformView)data [0];

					OnAddScore ();
					break;
				}

			case N.OnPlayerInvisible:
				{
					GameOver ();

					game.model.gameState = GameState.GameOver;

					Notify (N.GameOver_, NotifyType.ALL, new GameOverData (game.model.gameOverData));
					break;
				}

			case N.GamePause:
				{
					game.model.gameState = GameState.Pause;
					break;
				}

			case N.GameContinue:
				{
					game.model.gameState = GameState.Playing;
					break;
				}
		}
	}

	#region public methods
	public void SetGameTheme(GameTheme gameTheme)
	{
		game.model.gameTheme = gameTheme;

		if (game.view.backgroundView != null)
			Destroy (game.view.backgroundView.gameObject);

		BackgroundView backgroundView = (BackgroundView)Instantiate (game.model.gameTheme.BackgroundView, game.view.transform);//.cameraView.transform);
	}
	#endregion

	private void OnStart()
	{
		game.model.playedGamesCount = PlayerPrefs.GetInt (Prefs.PlayerData.GamesPlayedCount);
	}

	void Update()
	{
		//if (_currentGameSpeedState != game.model.gameSpeedState)
		//	SetGameSpeed (game.model.gameSpeedState);
	}

	private void OnGameStartPlay()
	{
		game.model.currentScore = 0;

		game.model.gameOverData.CoinsCount = 0;
		game.model.gameOverData.CrystalsCount = 0;
		game.model.gameOverData.ScoreCount = 0;
		game.model.gameOverData.GameType = game.model.gameType;

		//m_PointText.text = _pointScore.ToString();

	}

	private void OnPlayerImpactItem(ItemView itemView)
	{
		ItemTypes itemType = itemView.ItemType;

		switch (itemType)
		{
			case ItemTypes.Coin:
				{
					game.model.gameOverData.CoinsCount++;
					break;
				}

			case ItemTypes.Crystal:
				{
					game.model.gameOverData.CrystalsCount += itemView.DistructFractureCount;
					break;
				}

			case ItemTypes.Magnet:
				{
					game.model.gameOverData.MagnetsCount++;
					break;
				}
		}
	}

	private void OnAddScore()
	{
		game.model.currentScore++;

		Notify (N.GameAddScore);

		//Notify (N.GameAddScore, 1);

		//m_PointText.text = _pointScore.ToString();

		//_soundManager.PlayTouch();

		//FindObjectOfType<Circle>().DOParticle();
	}

	private void GameOver()
	{
		IncreasePlayedGamesCount ();

		//ReportScoreToLeaderboard(point);

		//_player.DesactivateTouchControl();

		//DOTween.KillAll();
		//StopAllCoroutines();

		//Utils.SetLastScore(game.model.currentScore);

		//playerModel.particleTrace.Stop ();

		//ShowAds();

		//_soundManager.PlayFail();

		//ui.controller.OnGameOver(() =>
		//{
			//ReloadScene();
		//});
	}

	private void IncreasePlayedGamesCount()
	{
		int currentPlayedGamesCount = 1;

		if (!PlayerPrefs.HasKey (Prefs.PlayerData.GamesPlayedCount))
		{
			PlayerPrefs.SetInt (Prefs.PlayerData.GamesPlayedCount, currentPlayedGamesCount);
		}
		else
		{
			currentPlayedGamesCount = PlayerPrefs.GetInt (Prefs.PlayerData.GamesPlayedCount);

			PlayerPrefs.SetInt (Prefs.PlayerData.GamesPlayedCount, ++currentPlayedGamesCount);
		}

		game.model.playedGamesCount = currentPlayedGamesCount;
	}

	private void ReloadScene()
	{

		#if UNITY_5_3_OR_NEWER
		SceneManager.LoadSceneAsync( 0, LoadSceneMode.Single );
		#else
		Application.LoadLevel(Application.loadedLevel);
		#endif
	}
}
