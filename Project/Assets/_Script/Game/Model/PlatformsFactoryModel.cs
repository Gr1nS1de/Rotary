using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformsFactoryModel : Model
{
	public float	verticalPlatformsGap	{ get { return _verticalPlatformsGap; }	set { _verticalPlatformsGap = value;} }

	[SerializeField]
	private float	_verticalPlatformsGap;
}

