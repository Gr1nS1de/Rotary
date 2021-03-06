﻿using UnityEngine;
using System.Collections;

public class GameView : View
{
	public CameraView					cameraView					{ get { return _cameralView 		= SearchLocal<CameraView>(			_cameralView,			typeof(CameraView).Name ); } }
	public PlayerView					playerView					{ get { return _playerView 			= SearchLocal<PlayerView>(			_playerView,			typeof(PlayerView).Name ); } }
	public ObjectsPoolView				objectsPoolView				{ get { return _objectsPoolView		= SearchLocal<ObjectsPoolView>(		_objectsPoolView,		typeof(ObjectsPoolView).Name);}}
	public BackgroundView				backgroundView				{ get { return _backgroundView		= SearchLocal<BackgroundView>(		_backgroundView,		typeof(BackgroundView).Name);}}

	private BackgroundView				_backgroundView;
	private CameraView					_cameralView;
	private PlayerView					_playerView;
	private ObjectsPoolView				_objectsPoolView;


}
