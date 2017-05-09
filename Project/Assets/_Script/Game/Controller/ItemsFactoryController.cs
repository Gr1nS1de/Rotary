using UnityEngine;
using System.Collections;

public class ItemsFactoryController : Controller
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

			case N.GamePlay:
				{
					OnGamePlay ();
					break;
				}

			case N.OnPlatformInvisible_:
				{
					PlatformView platformView = (PlatformView)data [0];


					break;
				}

		}
	}

	private void OnStart()
	{

	}

	private void OnGamePlay()
	{
	}


}

