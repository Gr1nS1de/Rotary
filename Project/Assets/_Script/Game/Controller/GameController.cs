using UnityEngine;
using System.Collections.Generic;
//using DG.Tweening;
using UnityEngine.SceneManagement;
//using Destructible2D;

public class GameController : Controller
{
	#region Declare controllers reference
	public CameraController					cameraController				{ get { return _cameraController 			= SearchLocal<CameraController>(			_cameraController,				typeof(CameraController).Name ); } }
	public GameSoundController				gameSoundController				{ get { return _gameSoundController			= SearchLocal<GameSoundController>(			_gameSoundController,			typeof(GameSoundController).Name ); } }
	public ResourcesController				resourcesController				{ get { return _resourcesController 		= SearchLocal<ResourcesController>(			_resourcesController,			typeof(ResourcesController).Name ); } }
	public ObjectsPoolController			objectsPoolController			{ get { return _objectsPoolController 		= SearchLocal<ObjectsPoolController> (		_objectsPoolController, 		typeof(ObjectsPoolController).Name);}}
	public PlayerController 				playerController				{ get { return _playerController			= SearchLocal<PlayerController>(			_playerController,				typeof(PlayerController).Name );}}

	private PlayerController				_playerController;
	private CameraController				_cameraController;
	private GameSoundController				_gameSoundController;
	private ResourcesController				_resourcesController;
	private ObjectsPoolController 			_objectsPoolController;
	#endregion

	//private GearModel 					playerModel	{ get { return game.model.playerModel;}}

	public override void OnNotification( string alias, Object target, params object[] data )
	{
		switch ( alias )
		{
			case N.OnStart:
				{
					//PlayerPrefs.DeleteAll ();
					game.model.gameState = GameState.MAIN_MENU;
					OnStart();
					break;
				}

			case N.GamePlay:
				{
					game.model.gameState = GameState.PLAYING;
					break;
				}

			case N.GameOver:
				{
					var collisionPoint = (Vector2)data [0];

					GameOver (collisionPoint);

					game.model.gameState = GameState.GAME_OVER;

					break;
				}

			case N.GamePause:
				{
					game.model.gameState = GameState.PAUSE;
					break;
				}
		}
	}

	private void OnStart()
	{
		SetNewGame ();
	}

	void SetNewGame()
	{
		game.model.currentScore = 0;

		//m_PointText.text = _pointScore.ToString();

	}

	public void Add1Score()
	{
		game.model.currentScore++;

		//Notify (N.GameAddScore, 1);

		//m_PointText.text = _pointScore.ToString();

		//_soundManager.PlayTouch();

		//FindObjectOfType<Circle>().DOParticle();
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
	private void GameOver( Vector2 collisionPoint )
	{
		if (game.model.gameState == GameState.GAME_OVER)
			return;

		//ReportScoreToLeaderboard(point);

		//_player.DesactivateTouchControl();

		//DOTween.KillAll();
		StopAllCoroutines();

		//Utils.SetLastScore(game.model.currentScore);

		//playerModel.particleTrace.Stop ();

		//ShowAds();

		//_soundManager.PlayFail();

		ui.controller.OnGameOver(() =>
		{
			ReloadScene();
		});
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
