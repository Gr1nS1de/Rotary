using UnityEngine;
using System.Collections;

public class RocketsFactoryController : Controller
{
	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.OnStart:
				{
					OnStart ();

					break;
				}

			case N.GameAddScore:
				{
					CheckRocketSpawn ();
					break;
				}
					
					
		}

	}

	private void OnStart()
	{
		
	}

	private void CheckRocketSpawn()
	{
		int scoreCount = core.playerDataModel.currentScore;

		if (scoreCount % 10f == 0f)
		{
			game.controller.objectsPoolController.PoolObject (PoolingObjectType.ROCKET, 1, null, RocketType.Default);
		}
	}
}

