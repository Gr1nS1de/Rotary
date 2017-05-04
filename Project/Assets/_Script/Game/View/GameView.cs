using UnityEngine;
using System.Collections;

public class GameView : View
{
	public CameraView					cameraView					{ get { return _cameralView 				= SearchLocal<CameraView>(					_cameralView,				typeof(CameraView).Name ); } }
	//public GearLightView				gearLightView				{ get { return _gearLightView 				= SearchLocal<GearLightView>(				_gearLightView,				typeof(GearLightView).Name ); } }
	//public PlayerTraceView			playerTraceView				{ get { return _playerTraceView 			= SearchLocal<PlayerTraceView>(				_playerTraceView,			typeof(PlayerTraceView).Name); } }
	public ObjectsPoolView				objectsPoolView				{ get { return _objectsPoolView				= SearchLocal<ObjectsPoolView>(				_objectsPoolView,			typeof(ObjectsPoolView).Name);}}
	//public RotatableComponent			rotatableComponent			{ get { return _rotatableComponent 			= SearchLocal<RotatableComponent>(			_rotatableComponent,		typeof(RotatableComponent).Name); } }

	private CameraView					_cameralView;
	//private GearView					_playerView;
	//private GearLightView				_gearLightView;
	private ObjectsPoolView				_objectsPoolView;
	//private RotatableComponent        _rotatableComponent;

}
