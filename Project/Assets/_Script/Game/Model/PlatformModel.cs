using UnityEngine;
using System.Collections;

public enum PlatformType
{
	Horizontal,
	Vertical,
	Vertical_Moving
}

public class PlatformModel : Model
{
	public float	horizontalPlatformInputSpeed	{ get { return _horizontalPlatformInputSpeed; }	set { _horizontalPlatformInputSpeed = value;} }

	[SerializeField]
	private float	_horizontalPlatformInputSpeed	 = 0.02f;
}

