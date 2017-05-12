using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformsFactoryModel : Model
{
	public float	verticalPlatformsGap	{ get { return _verticalPlatformsGap; } }

	[SerializeField]
	private float	_verticalPlatformsGap;
}

