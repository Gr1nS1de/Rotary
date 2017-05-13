﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : Model 
{
	public float					angularSpeed		{ get { return _angularSpeed; } 		set { _angularSpeed	= value; } }
	public float					forceOnInit			{ get { return _forceOnInit; } 			set { _forceOnInit 	= value; } }
	public float					invisibleBeforeDie	{ get { return _invisibleBeforeDie;}	set { _invisibleBeforeDie 	= value; } }
	public float					offsetBackForceMax	{ get { return _offsetBackForceMax; } 	set { _offsetBackForceMax	= value; } }
	public float					initScreenPosX		{ get { return _initScreenPosX; } 		set { _initScreenPosX 	= value; } }
	public float	 				linearForce			{ get { return _linearForce; } 			set { _linearForce = value;}}
	public Vector3					playerRendererSize	{ get { return game.view.playerView.PlayerRenderer.bounds.size;}}

	[SerializeField]
	private float					_linearForce		= 1f;
	[SerializeField]
	private float					_angularSpeed; // -85.5 = 1 camera speed. -257.5 = 3 camera speed...
	[SerializeField]
	private float 					_forceOnInit;
	[SerializeField]
	private float 					_invisibleBeforeDie;
	[SerializeField]
	private float					_offsetBackForceMax;
	[SerializeField]
	private float 					_initScreenPosX 	= 0.2f; // Init x position on 1/5 of screen from left screen side.


}
