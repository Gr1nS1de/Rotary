using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameController : Controller
{
	#region Declare controllers reference
	public CameraController					cameraController				{ get { return _cameraController 			= SearchLocal<CameraController>(			_cameraController,				typeof(CameraController).Name ); } }
	public GameSoundController				gameSoundController				{ get { return _gameSoundController			= SearchLocal<GameSoundController>(			_gameSoundController,			typeof(GameSoundController).Name ); } }
	public GameResourcesController			resourcesController				{ get { return _resourcesController 		= SearchLocal<GameResourcesController>(			_resourcesController,			typeof(GameResourcesController).Name ); } }
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
	private GameResourcesController			_resourcesController;
	private ObjectsPoolController 			_objectsPoolController;
	#endregion

	private GameModel 				_gameModel	{ get { return game.model;}}

	public override void OnNotification( string alias, Object target, params object[] data )
	{
		switch ( alias )
		{
			case N.RCAwakeLoad:
				{
					OnAwakeInit ();
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
			case N.PlayerLeftPlatform_:
				{
					PlatformView platformView = (PlatformView)data [0];

					OnAddScore ();
					break;
				}

			case N.OnPlayerInvisible:
				{
					GameOver ();
					break;
				}

			case N.OnPurchasedCoinsPack_00:
				{
					ChangePlayerItemCount (ItemTypes.Coin, 3000);
					break;
				}

			case N.OnPurchasedCoinsPack_01:
				{
					ChangePlayerItemCount (ItemTypes.Coin, 15000);
					break;
				}

			case N.OnPurchasedDoubleCoin:
				{
					Prefs.PlayerData.SetDoubleCoin ();
					_gameModel.isDoubleCoin = true;
					break;
				}

			case N.OnPlayerBuySkin_:
				{
					int skinId = (int)data [0];

					OnPlayerBuySkin (skinId);

					break;
				}

			case N.OnEndShowAdVideo_:
				{
					bool isSuccess = (bool)data [0];

					if (isSuccess)
					{
						int rewardCoinsCount = 50;

						ChangePlayerItemCount (ItemTypes.Coin, rewardCoinsCount);
					}
					break;
				}

			case N.GamePause:
				{
					GoGameState(GameState.Pause);
					break;
				}

			case N.GameContinue:
				{
					GoGameState( GameState.Playing);
					break;
				}
		}
	}

	#region public methods
	public void SetGameTheme(GameTheme gameTheme)
	{
		_gameModel.gameTheme = gameTheme;

		if (game.view.backgroundView != null)
			Destroy (game.view.backgroundView.gameObject);

		BackgroundView backgroundView = (BackgroundView)Instantiate (_gameModel.gameTheme.BackgroundView, game.view.transform);//.cameraView.transform);
	
		Notify (N.GameThemeChanged_, NotifyType.GAME, gameTheme);
	}
	#endregion

	private void OnAwakeInit()
	{
		_gameModel.playerRecord = Prefs.PlayerData.GetRecord ();
		_gameModel.coinsCount = Prefs.PlayerData.GetCoinsCount ();
		_gameModel.crystalsCount = Prefs.PlayerData.GetCrystalsCount ();
		_gameModel.isDoubleCoin = Prefs.PlayerData.GetDoubleCoin () == 1;
		_gameModel.playedGamesCount = Prefs.PlayerData.GetPlayedGamesCount();
	}

	private void OnStart()
	{

	}

	private void OnGameStartPlay()
	{
		_gameModel.currentScore = 0;

		_gameModel.gameOverData.CoinsCount = 0;
		_gameModel.gameOverData.CrystalsCount = 0;
		_gameModel.gameOverData.ScoreCount = 0;
		_gameModel.gameOverData.MagnetsCount = 0;
		_gameModel.gameOverData.GameType = _gameModel.gameType;

		//m_PointText.text = _pointScore.ToString();
	}

	private void GoGameState(GameState gameState)
	{
		Debug.LogFormat ("GameState = {0}", gameState); 

		_gameModel.gameState = gameState;
	}

	private void OnPlayerBuySkin(int skinId)
	{
		int skinPrice = ui.view.GetPlayerSkinElement (skinId).SkinPrice;

		ChangePlayerItemCount (ItemTypes.Coin, -skinPrice);
	}

	private void ChangePlayerItemCount(ItemTypes itemType, int count, bool isNotify = true)
	{
		int absCount = Mathf.Abs (count);

		switch(itemType)
		{
			case ItemTypes.Coin:
				{
					if (count > 0)
						Prefs.PlayerData.CreditCoins (absCount);
					else
						Prefs.PlayerData.DebitCoins (absCount);
					
					game.model.coinsCount += count;
					break;
				}

			case ItemTypes.Crystal:
				{
					if (count > 0)
						Prefs.PlayerData.CreditCrystals (absCount);
					else
						Prefs.PlayerData.DebitCrystals (absCount);
					
					game.model.crystalsCount += count;
					break;
				}
		}

		if(isNotify)
			Notify (N.PlayerItemCountChange__, NotifyType.ALL, itemType, count);
	}

	private void OnPlayerImpactItem(ItemView itemView)
	{
		ItemTypes itemType = itemView.ItemType;

		switch (itemType)
		{
			case ItemTypes.Coin:
				{
					int coinsCount = (_gameModel.isDoubleCoin ? 2 : 1);

					ChangePlayerItemCount(ItemTypes.Coin, coinsCount, false);

					_gameModel.gameOverData.CoinsCount += coinsCount;
					break;
				}

			case ItemTypes.Crystal:
				{
					int crystalsCount = itemView.CrystalFractureCount;

					ChangePlayerItemCount (ItemTypes.Crystal, crystalsCount, false);

					_gameModel.gameOverData.CrystalsCount += crystalsCount;
					break;
				}

			case ItemTypes.Magnet:
				{
					_gameModel.gameOverData.MagnetsCount++;

					DOVirtual.DelayedCall (0.3f, () =>
					{
						GameOver ();

					});
					break;
				}
		}
	}

	private void OnAddScore()
	{
		_gameModel.currentScore++;

		if (_gameModel.currentScore > _gameModel.playerRecord)
		{
			OnNewRecord (_gameModel.currentScore);
		}

		Notify (N.GameAddScore);
	}

	private void OnNewRecord(int score)
	{
		Prefs.PlayerData.SetRecord (score);
		_gameModel.playerRecord = score;
		Notify (N.OnPlayerNewRecord_, NotifyType.ALL, score);
	}

	private void GameOver()
	{
		IncreasePlayedGamesCount ();

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

	private void IncreasePlayedGamesCount()
	{
		int currentPlayedGamesCount = 1;

		if (!PlayerPrefs.HasKey (Prefs.PlayerData.GamesPlayedCount))
		{
			PlayerPrefs.SetInt (Prefs.PlayerData.GamesPlayedCount, currentPlayedGamesCount);
		}
		else
		{
			currentPlayedGamesCount = Prefs.PlayerData.GetPlayedGamesCount();

			Prefs.PlayerData.IncreasePlayedGamesCount ();
		}

		_gameModel.playedGamesCount = currentPlayedGamesCount;
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
