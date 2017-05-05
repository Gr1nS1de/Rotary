using UnityEngine;
using System.Collections;

public class CameraModel : Model
{
	public float	moveSpeed	{ get { return _moveSpeed; } }

	[SerializeField]
	private float	_moveSpeed;

}
