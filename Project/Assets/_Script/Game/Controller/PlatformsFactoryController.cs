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

			case N.OnPlatformInvisible_:
				{
					PlatformView platformView = (PlatformView)data [0];

					RestorePlatform (platformView);
					break;
				}
		}
	}

	private void OnStart()
	{
		
	}

	private void OnGamePlay()
	{
		InitPlatforms (game.model.objectsPoolModel.platformsMaxCount);
	}

	private void InitPlatforms(int count)
	{
		Vector3 platformSize = GM.Instance.PlatformRendererSize;
		Vector3 screenSize = GM.Instance.ScreenSize;

		Vector3 platformPosition = new Vector3( (-screenSize.x / 2f) + platformSize.x / 2f, -screenSize.y / 2f + platformSize.y * 1.25f, 0f );

		Notify(N.PoolObject___, NotifyType.GAME, PoolingObjectType.PLATFORM, count, platformPosition);
	}

	private void RestorePlatform(PlatformView platformView)
	{
		Notify (N.AddObjectToPoolQueue__, NotifyType.GAME, PoolingObjectType.PLATFORM, platformView);
		Notify (N.PoolObject___, NotifyType.GAME, PoolingObjectType.PLATFORM, 1, null);
	}
}

