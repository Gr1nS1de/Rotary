using UnityEngine;
using System.Collections;

public class RocketsController : Controller
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

			case N.PlayerImpactRocket__:
				{
					RocketView rocketView = (RocketView)data [0];
					Vector2 contactPoint = (Vector2)data [1];

					rocketView.PlayExplode ((Vector3)contactPoint);
					break;
				}
		}

	}

	private void OnStart()
	{

	}

}

