using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using DG.Tweening;

public enum GameState
{
	MainMenu,
	Playing,
	Pause,
	GameOver
}

public enum GameThemeType
{
	DarkBlueGarage	= 0
}

public enum GameSpeedState
{
	NotDefined,
	Speed_1,
	Speed_2,
	Speed_3,
	Speed_4,
	Speed_5,
	Speed_6,
	Speed_7
}

public enum GameType
{
	Classic
}

[System.Serializable]
public struct GameTheme
{
	public GameThemeType GameThemeType;
	public List<PlatformView> PlatformsViewList;
	public BackgroundView BackgroundView;

	public Vector3 GetPlatformRendererSize(PlatformTypes platformType)
	{
		return PlatformsViewList.Find (platform => platform.PlatformType == platformType).GetMainRendererSize();
	}
}

public class GameOverData
{
	public GameOverData(){}
	public GameOverData(GameOverData gameOverData)
	{
		GameType = gameOverData.GameType;
		CoinsCount = gameOverData.CoinsCount;
		CrystalsCount = gameOverData.CrystalsCount;
		MagnetsCount = gameOverData.MagnetsCount;
		ScoreCount = gameOverData.ScoreCount;
	}

	public GameType GameType;
	public int CoinsCount;
	public int CrystalsCount;
	public int MagnetsCount;
	public int ScoreCount;
}

public class GameModel : Model
{
	#region Game model
	public GameState					gameState				{ get { return _gameState; } 		set { _gameState 	= value; } }
	public int							currentScore			{ get { return _currentScore; } 	set { _currentScore = value; } }
	public GameTheme 					gameTheme				{ get { return _gameTheme; } 		set { _gameTheme = value;}}
	public float			 			gameSpeed				{ get { return _gameSpeed; } 		set { _gameSpeed = value;}}
	public GameSpeedState	 			gameSpeedState			{ get { return _gameSpeedState; } 	set { _gameSpeedState = value;}}
	public int	 						playedGamesCount		{ get { return _playedGamesCount; } set { _playedGamesCount = value;}}
	public GameType						gameType				{ get { return _gameType; } 		set { _gameType = value;}}
	public GameOverData					gameOverData			{ get { return _gameOverData; } 	set { _gameOverData = value;}}
	public bool							isDoubleCoin			{ get { return _isDoubleCoin; } 	set { _isDoubleCoin = value;}}
	public int							playerRecord			{ get { return _playerRecord; } 	set { _playerRecord = value;}}
	public int							coinsCount				{ get { return _coinsCount; } 		set { _coinsCount = value;}}
	public int							crystalsCount			{ get { return _crystalsCount; } 	set { _crystalsCount = value;}}

	private int							_crystalsCount;
	private int							_coinsCount;
	private int							_playerRecord;
	private bool						_isDoubleCoin;
	private GameOverData				_gameOverData			 = new GameOverData ();
	[SerializeField]
	private GameType					_gameType;
	[SerializeField]
	private int							_playedGamesCount;
	[SerializeField]			
	private GameSpeedState				_gameSpeedState;
	[SerializeField]
	private float						_gameSpeed				= 1f;
	[SerializeField]
	private GameTheme					_gameTheme;
	[SerializeField]
	private GameState					_gameState 				= GameState.MainMenu;
	[SerializeField]
	private int 						_currentScore;
	#endregion

	#region Declare models reference
	public CameraModel					cameraModel				{ get { return _cameraModel 				= SearchLocal<CameraModel>(					_cameraModel,				typeof(CameraModel).Name); } }
	public DistructibleModel			distructibleModel		{ get { return _distructibleModel 			= SearchLocal<DistructibleModel>( 			_distructibleModel, 		typeof(DistructibleModel).Name ); } }
	public GameSoundModel				soundModel				{ get { return _soundModel 					= SearchLocal<GameSoundModel>(				_soundModel,				typeof(GameSoundModel).Name ); } }
	//public GameRCModel				GameRCModel				{ get { return _GameRCModel 				= SearchLocal<GameRCModel>(						_GameRCModel,					typeof(GameRCModel).Name ); } }
	public ObjectsPoolModel				objectsPoolModel		{ get { return _objectsPoolModel			= SearchLocal<ObjectsPoolModel>(			_objectsPoolModel,			typeof(ObjectsPoolModel).Name );}}
	public PlayerModel 					playerModel				{ get { return _playerModel					= SearchLocal<PlayerModel>(					_playerModel,				typeof(PlayerModel).Name );}}
	public PlatformsFactoryModel		platformsFactoryModel	{ get { return _platformsFactoryModel		= SearchLocal<PlatformsFactoryModel>(		_platformsFactoryModel,		typeof(PlatformsFactoryModel).Name );}}
	public ItemsFactoryModel			itemsFactoryModel		{ get { return _itemsFactoryModel			= SearchLocal<ItemsFactoryModel>(			_itemsFactoryModel,			typeof(ItemsFactoryModel).Name );}}
	public BonusesFactoryModel			bonusesFactoryModel		{ get { return _bonusesFactoryModel			= SearchLocal<BonusesFactoryModel>(			_bonusesFactoryModel,		typeof(BonusesFactoryModel).Name );}}
	public ItemModel					itemModel				{ get { return _itemModel					= SearchLocal<ItemModel>(					_itemModel,					typeof(ItemModel).Name );}}
	public PlatformModel				platformModel			{ get { return _platformModel				= SearchLocal<PlatformModel>(				_platformModel,				typeof(PlatformModel).Name );}}

	private PlatformModel				_platformModel;
	private ItemModel					_itemModel;
	private BonusesFactoryModel			_bonusesFactoryModel;
	private ItemsFactoryModel			_itemsFactoryModel;
	private PlatformsFactoryModel		_platformsFactoryModel;
	private PlayerModel					_playerModel;
	private CameraModel					_cameraModel;
	private DistructibleModel   		_distructibleModel;
	private GameSoundModel				_soundModel;
	//private GameRCModel				_GameRCModel;
	private ObjectsPoolModel			_objectsPoolModel;
	#endregion

}
	