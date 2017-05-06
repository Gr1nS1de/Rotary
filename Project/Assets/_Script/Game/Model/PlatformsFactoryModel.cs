using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformsFactoryModel : Model
{
	public List<PlatformView>		platformsPrefabsList	{ get { return _platformsPrefabsList; } }

	[SerializeField]
	private List<PlatformView>		_platformsPrefabsList	= new List<PlatformView>();
}

