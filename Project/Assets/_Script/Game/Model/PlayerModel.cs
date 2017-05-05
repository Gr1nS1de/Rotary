using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : Model 
{
	public float					angularSpeed		{ get { return _angularSpeed; } set { _angularSpeed	= value; } }
	public float					moveSpeed			{ get { return _moveSpeed; } 	set { _moveSpeed 	= value; } }

	[SerializeField]
	private float					_angularSpeed;
	[SerializeField]
	private float 					_moveSpeed;

}
