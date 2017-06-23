using UnityEngine;
using System.Collections;

public class RocketModel : Model
{
	public Vector3 GetItemRendererSize(RocketType rocketType)
	{
		return game.model.rocketsFactoryModel.rocketsPrefabsList.Find (rocket => rocket.RocketType == rocketType).GetMainRendererSize();
	}

}

