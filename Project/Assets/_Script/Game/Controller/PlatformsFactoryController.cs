using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

			case N.GameStartPlay:
				{
					OnGamePlay ();
					break;
				}

			case N.OnPlatformInvisible_:
				{
					PlatformView platformView = (PlatformView)data [0];

					Debug.LogFormat ("Platform is invisible: {0}", platformView.name);

					if (game.model.gameState == GameState.Playing)
					{
						RestorePlatform (platformView);
						CheckPlatformSpawn (platformView);
					}
					break;
				}

		}
	}

	private void OnStart()
	{
		
	}

	private void OnGamePlay()
	{
		InitHorizontalPlatforms (game.model.objectsPoolModel.platformsMaxCount);
	}

	private void InitHorizontalPlatforms(int count)
	{
		Vector3 platformSize = game.model.gameTheme.GetPlatformRendererSize(PlatformTypes.Horizontal);
		Vector3 screenSize = GM.Instance.ScreenSize;

		Vector3 platformPosition = new Vector3( (-screenSize.x / 2f) + platformSize.x / 2f, -screenSize.y / 2f + (platformSize.y / 2f * 1.5f), 0f );

		game.controller.objectsPoolController.PoolObject(PoolingObjectType.PLATFORM, count, platformPosition, PlatformTypes.Horizontal);
	}

	private void RestorePlatform(PlatformView platformView)
	{
		if (game.controller.objectsPoolController.IsValidPoolingObject (platformView))
		{
			DG.Tweening.DOVirtual.DelayedCall (game.model.playerModel.invisibleBeforeDie, () =>
			{
				game.controller.objectsPoolController.StoreObjectToPool (PoolingObjectType.PLATFORM, platformView);
			});
		}else
		{
			Destroy (platformView.gameObject);
			Debug.LogErrorFormat ("Trying to restore platform which not is pool valid. Strange behaviour!");
		}
	}

	private void CheckPlatformSpawn(PlatformView platformView)
	{
		switch (platformView.PlatformType)
		{
			case PlatformTypes.Horizontal:
				{
					game.controller.objectsPoolController.PoolObject(PoolingObjectType.PLATFORM, 1, null, PlatformTypes.Horizontal);

					if (Random.Range (0, 10) > 8)
					{
						game.controller.objectsPoolController.PoolObject(PoolingObjectType.PLATFORM, 1, null, PlatformTypes.Vertical);
					}
					break;
				}

			case PlatformTypes.Vertical:
				{
					break;
				}
		}
	}
}

