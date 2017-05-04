using UnityEngine;
using System.Collections;

public class CameraModel : Model
{
	public float	playZoomSize	{ get { return _playZoomSize; } }

	[SerializeField]
	private float	_playZoomSize;

}
