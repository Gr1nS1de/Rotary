﻿using UnityEngine;
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

			case N.GameStart:
				{
					OnGamePlay ();
					break;
				}

			case N.OnPlatformInvisible_:
				{
					PlatformView platformView = (PlatformView)data [0];

					//Debug.LogFormat ("Platform is invisible: {0}", platformView.name);

					if (game.model.gameState == GameState.Playing)
					{
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
		Vector3 platformSize = game.model.gameTheme.GetPlatformRendererSize(PlatformType.Horizontal);
		Vector3 screenSize = GM.Instance.ScreenSize;

		Vector3 platformPosition = new Vector3( screenSize.x / 2f + platformSize.x / 2f, -screenSize.y / 2f + (platformSize.y / 2f * 1.5f), 0f );

		game.controller.objectsPoolController.PoolObject(PoolingObjectType.PLATFORM, count, platformPosition, PlatformType.Horizontal);
	}

	private void CheckPlatformSpawn(PlatformView platformView)
	{
		switch (platformView.PlatformType)
		{
			case PlatformType.Horizontal:
				{
					game.controller.objectsPoolController.PoolObject(PoolingObjectType.PLATFORM, 1, null, PlatformType.Horizontal);

					if (Random.Range (0, 10) > 8)
					{
						game.controller.objectsPoolController.PoolObject(PoolingObjectType.PLATFORM, 1, null, PlatformType.Vertical);
							
					}
					break;
				}
			
			case PlatformType.Vertical:
				{
					game.controller.objectsPoolController.PoolObject(PoolingObjectType.PLATFORM, 1, null, PlatformType.Horizontal);
					game.controller.objectsPoolController.PoolObject(PoolingObjectType.PLATFORM, 1, null, Random.Range(0, 2) == 1 ? PlatformType.Vertical : PlatformType.Vertical_Moving);

					break;
				}

			case PlatformType.Vertical_Moving:
				{
					game.controller.objectsPoolController.PoolObject(PoolingObjectType.PLATFORM, 1, null, PlatformType.Horizontal);
					game.controller.objectsPoolController.PoolObject(PoolingObjectType.PLATFORM, 1, null, Random.Range(0, 3) == 1 ? PlatformType.Vertical : PlatformType.Vertical_Moving);

					break;
				}
		}
	}
}

