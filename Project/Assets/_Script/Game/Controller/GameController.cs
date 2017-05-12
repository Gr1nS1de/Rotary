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

	//private GearModel 					playerModel	{ get { return game.model.playerModel;}}

	private GameSpeedState _currentGameSpeedState;

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

					game.model.gameState = GameState.MAIN_MENU;
					break;
				}

			case N.GamePlay:
				{
					SetNewGame ();

					game.model.gameState = GameState.PLAYING;
					break;
				}

			case N.PlayerLeftPlatform_:
				{
					PlatformView platformView = (PlatformView)data [0];

					OnAddScore ();
					CheckGameSpeedState ();
					break;
				}

			case N.OnPlayerInvisible:
				{
					GameOver ();

					game.model.gameState = GameState.GAME_OVER;

					Notify (N.GameOver);
					break;
				}

			case N.GamePause:
				{
					game.model.gameState = GameState.PAUSE;
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

	//https://ussrgames.atlassian.net/wiki/pages/viewpage.action?pageId=40027034
	public void SetGameSpeed(GameSpeedState speedState)
	{
		switch (speedState)
		{
			case GameSpeedState.SPEED_1:
				{
					game.model.gameSpeed = 3f;
					game.model.playerModel.angularSpeed = -70f;
					break;
				}

			case GameSpeedState.SPEED_2:
				{
					game.model.gameSpeed = 4f;
					game.model.playerModel.angularSpeed = -65f;
					break;
				}

			case GameSpeedState.SPEED_3:
				{
					game.model.gameSpeed = 6f;
					game.model.playerModel.angularSpeed = -60f;
					break;
				}

			case GameSpeedState.SPEED_4:
				{
					game.model.gameSpeed = 7f;
					game.model.playerModel.angularSpeed = -60f;
					break;
				}

			case GameSpeedState.SPEED_5:
				{
					game.model.gameSpeed = 8f;
					game.model.playerModel.angularSpeed = -50f;
					break;
				}
		}

		_currentGameSpeedState = speedState;
		game.model.gameSpeedState = _currentGameSpeedState;
	}
	#endregion

	private void OnStart()
	{
		SetNewGame ();

		SetGameSpeed( game.model.gameSpeedState);
	}

	void Update()
	{
		//if (_currentGameSpeedState != game.model.gameSpeedState)
		//	SetGameSpeed (game.model.gameSpeedState);
	}

	private void SetNewGame()
	{
		game.model.currentScore = 0;

		//m_PointText.text = _pointScore.ToString();

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

	private void CheckGameSpeedState()
	{
		int currentScore = game.model.currentScore;
		GameSpeedState correctGameSpeedState = GameSpeedState.SPEED_1;

		if (currentScore < 5)
		{
			//
		}
		else if (currentScore >= 5 && currentScore < 30)
		{
			correctGameSpeedState = GameSpeedState.SPEED_2;
		}
		else if (currentScore >= 30 && currentScore < 50)
		{
			correctGameSpeedState = GameSpeedState.SPEED_3;
		}
		else if (currentScore >= 50 && currentScore < 70)
		{
			correctGameSpeedState = GameSpeedState.SPEED_4;
		}
		else
		{
			correctGameSpeedState = GameSpeedState.SPEED_5;
		}

		if (game.model.gameSpeedState != correctGameSpeedState)
		{
			SetGameSpeed (correctGameSpeedState);
		}
	}
	/*
	public void OnImpactObstacleByPlayer(RobotView obstacleView, Vector2 collisionPoint)
	{
		var obstacleModel = game.model.robotsFactoryModel.currentModelsDictionary[obstacleView];

		if (!obstacleModel)
		{
			Debug.LogError ("Cant find model");
			return;
		}
			
		switch (obstacleModel.bodyType)
		{
			case RobotBodyType.HEAD:
				{
					//obstacleRenderObject.GetComponent<Rigidbody2D> ().isKinematic = true;
					Notify(N.GameOver, collisionPoint);

					break;
				}

			case RobotBodyType.BODY:
				{
					/*
					var obstacleDestructible = obstacleView.GetComponent<D2dDestructible> ();

					Add1Score ();

					obstacleView.gameObject.layer = LayerMask.NameToLayer (GM.instance.destructibleObstaclePieceLayerName);

					Notify (N.DestructibleBreakEntity___, obstacleDestructible, game.model.destructibleModel.destructibleObstacleFractureCount, collisionPoint);

					break;
				}
			default:
				break;
		}
	}
*/
	private void GameOver()
	{
		if (game.model.gameState == GameState.GAME_OVER)
			return;

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

	private void ReloadScene()
	{

		#if UNITY_5_3_OR_NEWER
		SceneManager.LoadSceneAsync( 0, LoadSceneMode.Single );
		#else
		Application.LoadLevel(Application.loadedLevel);
		#endif
	}



}
