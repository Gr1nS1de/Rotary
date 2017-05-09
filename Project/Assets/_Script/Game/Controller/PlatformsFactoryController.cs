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

					Debug.LogFormat ("Platform is invisible: {0}", platformView.name);

					if(game.model.gameState == GameState.PLAYING)
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
		Vector3 platformSize = game.model.gameTheme.GetPlatformRendererSize(PlatformTypes.HORIZONTAL);
		Vector3 screenSize = GM.Instance.ScreenSize;

		Vector3 platformPosition = new Vector3( (-screenSize.x / 2f) + platformSize.x / 2f, -screenSize.y / 2f + platformSize.y * 1.25f, 0f );

		//Notify(N.PoolObject____, NotifyType.GAME, PoolingObjectType.PLATFORM, count, platformPosition, PlatformTypes.HORIZONTAL);
		game.controller.objectsPoolController.PoolObject(PoolingObjectType.PLATFORM, count, platformPosition, PlatformTypes.HORIZONTAL);
	}

	private void RestorePlatform(PlatformView platformView)
	{
		//Notify (N.AddObjectToPool__, NotifyType.GAME, PoolingObjectType.PLATFORM, platformView);
		game.controller.objectsPoolController.AddObjectToPool(PoolingObjectType.PLATFORM, platformView);

		switch (platformView.PlatformType)
		{
			case PlatformTypes.HORIZONTAL:
				{
					//Notify (N.PoolObject____, NotifyType.GAME, PoolingObjectType.PLATFORM, 1, null, platformView.PlatformType);
					game.controller.objectsPoolController.PoolObject(PoolingObjectType.PLATFORM, 1, null, PlatformTypes.HORIZONTAL);

					if (Random.Range (0, 10) > 8)
					{
						game.controller.objectsPoolController.PoolObject(PoolingObjectType.PLATFORM, 1, null, PlatformTypes.VERTICAL);
					}
					break;
				}

			case PlatformTypes.VERTICAL:
				{
					break;
				}
		}
	}
}

