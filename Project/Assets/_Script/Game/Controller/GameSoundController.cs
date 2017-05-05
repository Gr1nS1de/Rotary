using UnityEngine;
using System.Collections;
//using Destructible2D;
//using DarkTonic.MasterAudio;

public class GameSoundController : Controller
{
	public override void OnNotification( string alias, Object target, params object[] data )
	{
		switch ( alias )
		{
			case N.OnStart:
				{
					break;
				}

			case N.DestructibleBreakEntity___:
				{
					//var obstacleDestructible = (D2dDestructible)data [0];
					//var fractureCount = (int)data [1];
					//var collisionPoint = (Vector2)data [2];


					//MasterAudio.PlaySoundAndForget (game.model.currentRoadModel.alias.ToString().ToLower() +"_obstacle_break");

					break;
				}
		}

	}
}
