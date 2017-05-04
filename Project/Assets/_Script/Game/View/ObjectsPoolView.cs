using UnityEngine;
using System.Collections;
//using DG.Tweening;

public class ObjectsPoolView : View
{
	public ObjectsPoolModel objectsPoolModel	{ get { return game.model.objectsPoolModel; } } 

	public void UpdateMovePooler()
	{/*
		//float spriteHeightOffset = obstacleSpriteSize.y * transform.localScale.y;//*2f;
		float playerPathElapsedPercentage = game.model.playerModel.playerPath.ElapsedPercentage(false);
		float forwarpPointPercentage = playerPathElapsedPercentage + objectsPoolModel.gapPercentage;

		if (forwarpPointPercentage > 1.0f)
			forwarpPointPercentage -= 1.0f;

		Vector3 forwardPosition = game.model.playerModel.playerPath.PathGetPoint(forwarpPointPercentage);

		transform.position = forwardPosition;*/
	}
}

