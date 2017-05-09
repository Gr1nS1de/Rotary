using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using DG.Tweening;

public enum GameState
{
	MAIN_MENU,
	PLAYING,
	PAUSE,
	GAME_OVER
}

public enum GameThemeType
{
	DarkBlueGarage
}

public enum GameSpeedState
{
	SPEED_1,
	SPEED_2,
	SPEED_3,
	SPEED_4,
	SPEED_5
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

public class GameModel : Model
{
	#region Game model
	public GameState					gameState				{ get { return _gameState; } 		set { _gameState 	= value; Debug.LogFormat ("GameState = {0}", value); } }
	public int							currentScore			{ get { return _currentScore; } 	set { _currentScore = value; } }
	public GameTheme 					gameTheme				{ get { return _gameTheme; } 		set { _gameTheme = value;}}
	public float			 			gameSpeed				{ get { return _gameSpeed; } 		set { _gameSpeed = value;}}
	public GameSpeedState	 			gameSpeedState			{ get { return _gameSpeedState; } 	set { _gameSpeedState = value;}}

	[SerializeField]
	private GameSpeedState				_gameSpeedState;
	[SerializeField]
	private float						_gameSpeed				= 1f;
	[SerializeField]
	private GameTheme					_gameTheme;
	[SerializeField]
	private GameState					_gameState 				= GameState.MAIN_MENU;
	[SerializeField]
	private int 						_currentScore;
	#endregion

	#region Declare models reference
	public CameraModel					cameraModel				{ get { return _cameraModel 				= SearchLocal<CameraModel>(					_cameraModel,				typeof(CameraModel).Name); } }
	public DistructibleModel			distructibleModel		{ get { return _distructibleModel 			= SearchLocal<DistructibleModel>( 			_distructibleModel, 		typeof(DistructibleModel).Name ); } }
	public GameSoundModel				soundModel				{ get { return _soundModel 					= SearchLocal<GameSoundModel>(				_soundModel,				typeof(GameSoundModel).Name ); } }
	public RCModel						RCModel					{ get { return _RCModel 					= SearchLocal<RCModel>(						_RCModel,					typeof(RCModel).Name ); } }
	public ObjectsPoolModel				objectsPoolModel		{ get { return _objectsPoolModel			= SearchLocal<ObjectsPoolModel>(			_objectsPoolModel,			typeof(ObjectsPoolModel).Name );}}
	public PlayerModel 					playerModel				{ get { return _playerModel					= SearchLocal<PlayerModel>(					_playerModel,				typeof(PlayerModel).Name );}}
	public PlatformsFactoryModel		platformsFactoryModel	{ get { return _platformsFactoryModel		= SearchLocal<PlatformsFactoryModel>(		_platformsFactoryModel,		typeof(PlatformsFactoryModel).Name );}}
	public ItemsFactoryModel			itemsFactoryModel		{ get { return _itemsFactoryModel			= SearchLocal<ItemsFactoryModel>(			_itemsFactoryModel,			typeof(ItemsFactoryModel).Name );}}
	public BonusesFactoryModel			bonusesFactoryModel		{ get { return _bonusesFactoryModel			= SearchLocal<BonusesFactoryModel>(			_bonusesFactoryModel,		typeof(BonusesFactoryModel).Name );}}

	private BonusesFactoryModel			_bonusesFactoryModel;
	private ItemsFactoryModel			_itemsFactoryModel;
	private PlatformsFactoryModel		_platformsFactoryModel;
	private PlayerModel					_playerModel;
	private CameraModel					_cameraModel;
	private DistructibleModel   		_distructibleModel;
	private GameSoundModel				_soundModel;
	private RCModel						_RCModel;
	private ObjectsPoolModel			_objectsPoolModel;
	#endregion
}
	