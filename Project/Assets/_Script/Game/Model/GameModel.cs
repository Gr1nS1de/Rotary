using UnityEngine;
using System.Collections;
//using DG.Tweening;

public enum GameState
{
	MAIN_MENU,
	PLAYING,
	PAUSE,
	GAME_OVER
}

public class GameModel : Model
{

	#region Game model
	public GameState					gameState				{ get { return _gameState; } 		set { _gameState 	= value; } }
	public int							currentScore			{ get { return _currentScore; } 	set { _currentScore = value; } }
	[SerializeField]
	private GameState					_gameState 				= GameState.MAIN_MENU;
	[SerializeField]
	private int 						_currentScore;
	#endregion

	#region Declare models reference
	public CameraModel					cameraModel				{ get { return _cameraModel 				= SearchLocal<CameraModel>(					_cameraModel,				typeof(CameraModel).Name); } }
	public DestructibleModel			destructibleModel		{ get { return _destructibleModel 			= SearchLocal<DestructibleModel>( 			_destructibleModel, 		typeof(DestructibleModel).Name ); } }
	public GameSoundModel				soundModel				{ get { return _soundModel 					= SearchLocal<GameSoundModel>(				_soundModel,				typeof(GameSoundModel).Name ); } }
	public RCModel						RCModel					{ get { return _RCModel 					= SearchLocal<RCModel>(						_RCModel,					typeof(RCModel).Name ); } }
	public ObjectsPoolModel				objectsPoolModel		{ get { return _objectsPoolModel			= SearchLocal<ObjectsPoolModel>(			_objectsPoolModel,			typeof(ObjectsPoolModel).Name );}}

	private CameraModel					_cameraModel;
	private DestructibleModel   		_destructibleModel;
	private GameSoundModel				_soundModel;
	private RCModel						_RCModel;
	private ObjectsPoolModel			_objectsPoolModel;
	#endregion
}
	