using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : Model 
{
	public float					angularSpeed		{ get { return _angularSpeed; } 	set { _angularSpeed	= value; } }
	//public float					moveSpeed			{ get { return _moveSpeed; } 		set { _moveSpeed 	= value; } }
	public float					offsetBackSpeed		{ get { return _offsetBackSpeed; } 	set { _offsetBackSpeed	= value; } }
	public float					initScreenPosX		{ get { return _initScreenPosX; } 	set { _initScreenPosX 	= value; } }

	[SerializeField]
	private float					_angularSpeed; // -85.5 = 1 camera speed. -257.5 = 3 camera speed...
	//[SerializeField]
	//private float 				_moveSpeed;
	[SerializeField]
	private float					_offsetBackSpeed;
	[SerializeField]
	private float 					_initScreenPosX 	= 0.2f; // Init x position on 1/5 of screen from left screen side.


}
