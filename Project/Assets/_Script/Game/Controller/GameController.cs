using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameController : Controller
{
	#region Declare controllers reference
	//public CameraController					cameraController				{ get { return _cameraController 			= SearchLocal<CameraController>(			_cameraController,				typeof(CameraController).Name ); } }
	public GameSoundController				gameSoundController				{ get { return _gameSoundController			= SearchLocal<GameSoundController>(			_gameSoundController,			typeof(GameSoundController).Name ); } }
	public GameResourcesController			resourcesController				{ get { return _resourcesController 		= SearchLocal<GameResourcesController>(		_resourcesController,			typeof(GameResourcesController).Name ); } }
	public ObjectsPoolController			objectsPoolController			{ get { return _objectsPoolController 		= SearchLocal<ObjectsPoolController> (		_objectsPoolController, 		typeof(ObjectsPoolController).Name);}}
	public PlayerController 				playerController				{ get { return _playerController			= SearchLocal<PlayerController>(			_playerController,				typeof(PlayerController).Name );}}
	public ItemsFactoryController			itemsFactoryController			{ get { return _itemsFactoryController		= SearchLocal<ItemsFactoryController>(		_itemsFactoryController,		typeof(ItemsFactoryController).Name );}}
	public BonusesFactoryController			bonusesFactoryController		{ get { return _bonusesFactoryController	= SearchLocal<BonusesFactoryController>(	_bonusesFactoryController,		typeof(BonusesFactoryController).Name );}}
	public PlatformsFactoryController		platformsFactoryController		{ get { return _platformsFactoryController	= SearchLocal<PlatformsFactoryController>(	_platformsFactoryController,	typeof(PlatformsFactoryController).Name );}}
	public DistructibleController			distructibleController			{ get { return _distructibleController		= SearchLocal<DistructibleController>(		_distructibleController,		typeof(DistructibleController).Name );}}
	public GameSpeedController				gameSpeedController				{ get { return _gameSpeedController			= SearchLocal<GameSpeedController>(			_gameSpeedController,			typeof(GameSpeedController).Name );}}
	public RocketsFactoryController			rocketsFactoryController		{ get { return _rocketsFactoryController	= SearchLocal<RocketsFactoryController>(	_rocketsFactoryController,		typeof(RocketsFactoryController).Name );}}
	public RocketsController				rocketsController				{ get { return _rocketsController			= SearchLocal<RocketsController>(			_rocketsController,				typeof(RocketsController).Name );}}

	private RocketsController				_rocketsController;
	private RocketsFactoryController		_rocketsFactoryController;
	private GameSpeedController				_gameSpeedController;
	private DistructibleController			_distructibleController;
	private PlatformsFactoryController		_platformsFactoryController;
	private BonusesFactoryController		_bonusesFactoryController;
	private ItemsFactoryController			_itemsFactoryController;
	private PlayerController				_playerController;
	//private CameraController				_cameraController;
	private GameSoundController				_gameSoundController;
	private GameResourcesController			_resourcesController;
	private ObjectsPoolController 			_objectsPoolController;
	#endregion

	private GameModel 				_gameModel	{ get { return game.model;}}
	private PlayerDataModel 		_playerDataModel	{ get { return core.playerDataModel;}}

	public override void OnNotification( string alias, Object target, params object[] data )
	{
		switch ( alias )
		{
			case N.RCAwakeLoad:
				{
					break;
				}

			case N.OnStart:
				{
					OnStart();

					GoGameState( GameState.MainMenu );
					break;
				}

			case N.GameStart:
				{
					OnGameStartPlay ();

					GoGameState (GameState.Playing);
					break;
				}

			case N.PlayerImpactItem__:
				{
					ItemView itemView = (ItemView)data [0];
					Vector2 contactPoint = (Vector2)data [1];

					OnPlayerImpactItem (itemView);
					break;
				}

			case N.PlayerImpactRocket__:
				{
					RocketView rocketView = (RocketView)data [0];
					Vector2 contactPoint = (Vector2)data [1];

					OnPlayerImpactRocket (rocketView);
					break;
				}

			case N.OnPlayerInvisible:
				{
					GameOver ();
					break;
				}

			case N.GamePause:
				{
					GoGameState(GameState.Pause);
					break;
				}

			case N.GameAddScore:
				{
					_gameModel.gameOverData.ScoreCount++;
					break;
				}

			case N.GameContinue:
				{
					GoGameState( GameState.Playing);
					break;
				}

			case N.OnPlayerNewRecord_:
				{
					int score = (int)data[0];

					if (_gameModel.gameState == GameState.Playing)
						_gameModel.gameOverData.IsNewRecord = true;
					break;
				}
		}
	}

	#region public methods
	public void SetGameTheme(GameTheme gameTheme)
	{
		_gameModel.gameTheme = gameTheme;

		Notify (N.GameThemeChanged_, NotifyType.GAME, gameTheme);
	}
	#endregion

	private void OnStart()
	{

	}

	private void OnGameStartPlay()
	{
		_gameModel.gameOverData.CoinsCount = 0;
		_gameModel.gameOverData.CrystalsCount = 0;
		_gameModel.gameOverData.ScoreCount = 0;
		_gameModel.gameOverData.MagnetsCount = 0;
		_gameModel.gameOverData.GameType = _gameModel.gameType;
		_gameModel.gameOverData.IsNewRecord = false;

		//m_PointText.text = _pointScore.ToString();
	}

	private void GoGameState(GameState gameState)
	{
		Debug.LogFormat ("GameState = {0}", gameState); 

		_gameModel.gameState = gameState;
	}

	private void OnPlayerImpactItem(ItemView itemView)
	{
		ItemType itemType = itemView.ItemType;

		switch (itemType)
		{
			case ItemType.Coin:
				{
					int coinsCount = (core.playerDataModel.isDoubleCoin ? 2 : 1);

					_gameModel.gameOverData.CoinsCount += coinsCount;
					break;
				}

			case ItemType.Crystal:
				{
					int crystalsCount = itemView.CrystalFractureCount;

					_gameModel.gameOverData.CrystalsCount += crystalsCount;
					break;
				}

			case ItemType.Magnet:
				{
					_gameModel.gameOverData.MagnetsCount++;

					DOVirtual.DelayedCall (0.1f, () =>
					{
						GameOver ();

					});
					break;
				}
		}
	}

	private void OnPlayerImpactRocket(RocketView rocketView)
	{
		RocketType rocketType = rocketView.RocketType;

		switch (rocketType)
		{
			case RocketType.Default:
				{

					DOVirtual.DelayedCall (1f, () =>
					{
						GameOver ();

					});
					break;
				}
		}
	}
		

	private void GameOver()
	{

		GoGameState( GameState.GameOver);

		Notify (N.GameOver_, NotifyType.ALL, new GameOverData (_gameModel.gameOverData));

		//ReportScoreToLeaderboard(point);

		//_player.DesactivateTouchControl();

		//DOTween.KillAll();
		//StopAllCoroutines();

		//Utils.SetLastScore(_gameModel.currentScore);

		//playerModel.particleTrace.Stop ();

		//ShowAds();

		//_soundManager.PlayFail();

		//ui.controller.OnGameOver(() =>
		//{
			//ReloadScene();
		//});
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
