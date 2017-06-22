using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum RocketType
{
	Default
}

public class RocketsFactoryModel : MonoBehaviour
{
	public List<RocketView>		rocketsPrefabsList	{ get { return _rocketsPrefabsList; } }

	[SerializeField]
	private List<RocketView>	_rocketsPrefabsList	= new List<RocketView>();

}

