using UnityEngine;
using System.Collections;

public class PlatformsFactoryController : Controller
{
	public PlatformsFactoryModel _platformsFactoryModel	{ get { return game.model.platformsFactoryModel; } } 

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

			case N.OnPlatformHidden_:
				{
					PlatformView platformView = (PlatformView)data [0];

					HidePlatform (platformView);
					break;
				}
		}
	}

	private void OnStart()
	{
		
	}

	private void OnGamePlay()
	{
		InitFirstPlatforms ();
	}

	private void InitFirstPlatforms()
	{

	}

	private void HidePlatform(PlatformView platformView)
	{
		Notify (N.AddObjectToPoolQueue__, NotifyType.GAME, PoolingObjectType.PLATFORM, platformView);
	}
}

