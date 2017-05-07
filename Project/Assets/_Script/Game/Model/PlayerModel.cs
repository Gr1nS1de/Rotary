using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : Model 
{
	public float					angularSpeed		{ get { return _angularSpeed; } 	set { _angularSpeed	= value; } }
	public float					forceOnInit			{ get { return _forceOnInit; } 		set { _forceOnInit 	= value; } }
	public float					invisibleBeforeDie	{ get { return _invisibleBeforeDie;}set { _invisibleBeforeDie 	= value; } }
	public float					offsetBackSpeed		{ get { return _offsetBackSpeed; } 	set { _offsetBackSpeed	= value; } }
	public float					initScreenPosX		{ get { return _initScreenPosX; } 	set { _initScreenPosX 	= value; } }
	public List<PlatformView>		scorePlatformsList	{ get { return _scorePlatformsList; } }
	public float	 				linearSpeed			{ get { return _linearSpeed; } 	set { _linearSpeed = value;}}

	[SerializeField]
	private float					_linearSpeed			= 1f;
	private List<PlatformView> 		_scorePlatformsList = new List<PlatformView>();
	[SerializeField]
	private float					_angularSpeed; // -85.5 = 1 camera speed. -257.5 = 3 camera speed...
	[SerializeField]
	private float 					_forceOnInit;
	[SerializeField]
	private float 					_invisibleBeforeDie;
	[SerializeField]
	private float					_offsetBackSpeed;
	[SerializeField]
	private float 					_initScreenPosX 	= 0.2f; // Init x position on 1/5 of screen from left screen side.


}
