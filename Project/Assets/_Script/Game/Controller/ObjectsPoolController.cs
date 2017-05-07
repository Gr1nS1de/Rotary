using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using DG.Tweening;

public class ObjectsPoolController : Controller
{
	private ObjectsPoolModel 		_objectsPoolModel			{ get { return game.model.objectsPoolModel; } }
	private PlatformsFactoryModel 	_platformsFactoryModel		{ get { return game.model.platformsFactoryModel; } }
	private ObjectsPoolView			_objectsPoolView			{ get { return game.view.objectsPoolView;}}
	private List<PlatformView>		_poolPlatformsList			{ get { return game.model.objectsPoolModel.poolPlatformsList;}}
	private List<PlatformView>		_instantiatedPlatformsList	{ get { return game.model.objectsPoolModel.instantiatedPlatforms;}}

	private Vector3					_lastObstaclePoolerViewPosition;

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

			case N.AddObjectToPool__:
				{
					PoolingObjectType poolingObjectType = (PoolingObjectType)data [0];
					Object poolingObject = (Object)data [1];

					AddObjectToPoolList (poolingObjectType, poolingObject);

					break;
				}

			case N.PoolObject____:
				{
					PoolingObjectType poolingObjectType = (PoolingObjectType)data [0];
					int objectCount = (int)data [1];
					Vector3? objectPosition = (Vector3?)data [2];

					if (game.model.gameState == GameState.PLAYING)
					{
						switch(poolingObjectType)
						{
							case PoolingObjectType.PLATFORM:
								{
									PlatformTypes platformType = (PlatformTypes)data [3];

									PoolPlatform (platformType, objectCount, objectPosition);
									break;
								}

							case PoolingObjectType.ITEM:
								{
									ItemTypes itemType = (ItemTypes)data [3];

									PoolItem (itemType, objectCount, objectPosition);
									break;
								}

							case PoolingObjectType.BONUS:
								{
									break;
								}
						}
					}
					
					break;
				}

			case N.GameOver:
				{
					List<PlatformView> copyInstantiatedPlatformsList = new List<PlatformView> (_instantiatedPlatformsList);

					copyInstantiatedPlatformsList.ForEach(platform=>
					{
						AddObjectToPoolList(PoolingObjectType.PLATFORM, platform);
					});

					copyInstantiatedPlatformsList = null;
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

	private void AddObjectToPoolList(PoolingObjectType poolingObjectType, Object poolingObject)
	{
		switch (poolingObjectType)
		{
			case PoolingObjectType.PLATFORM:
				{
					PlatformView platformView = (PlatformView)poolingObject;

					//Debug.LogFormat ("Add platform to pool. {0}", platformView.name);

					if (platformView != null && !_poolPlatformsList.Contains (platformView))
					{
						_poolPlatformsList.Add (platformView);

						if (_instantiatedPlatformsList.Contains (platformView))
							_instantiatedPlatformsList.Remove (platformView);

						platformView.OnAddToPool ();

						platformView.gameObject.SetActive (false);
					}
					break;
				}

			case PoolingObjectType.ITEM:
				{
					
					break;
				}
		}
	}

	private void PoolItem(ItemTypes itemType, int count, Vector3? objectPosition)
	{
		//Debug.LogFormat("Pool item {0}. count = {1}. position = {2}", itemType, count, objectPosition);

		switch (itemType)
		{
			case ItemTypes.COIN:
				{
					break;
				}
		}
	}

	private void PoolPlatform(PlatformTypes platformType, int count, Vector3? platformPosition)
	{
		//Debug.LogFormat("Pool platform {0}. count = {1}. position = {2}", platformType, count, platformPosition);

		for (int i = 0; i < count; i++)
		{
			OnPoolingPlatform (platformType, platformPosition);

			if (platformPosition != null)
			{
				Vector3 platformSize = game.model.gameTheme.GetPlatformRendererSize(platformType);
				Vector3 screenSize = GM.Instance.ScreenSize;

				platformPosition = new Vector3(platformPosition.GetValueOrDefault().x + (platformSize.x + _objectsPoolModel.platformsGap) , platformPosition.GetValueOrDefault().y, 0f ); 
			}
		}
	}

	private void OnPoolingPlatform(PlatformTypes platformType, Vector3? platformPosition)
	{
		PlatformView platformView = null;
		bool isInPoolList = false;
		Vector3 platformRendererSize = game.model.gameTheme.GetPlatformRendererSize(platformType);

		//If platform already in pooling list and waiting for being pooled
		if (_poolPlatformsList.Count > 0)
		{
			platformView = _poolPlatformsList.Find((platform)=>platform.PlatformType == platformType);

			if (platformView != null)
			{
				platformView.gameObject.SetActive (true);

				_poolPlatformsList.Remove (platformView);

				isInPoolList = true;
			}
		}

		if(!isInPoolList)
		{
			platformView = (PlatformView)
					Instantiate (
						game.model.gameTheme.PlatformsViewList.Find(platform=>platform.PlatformType == platformType),	//Find platformView in current theme list
						game.view.transform.Find("_Platforms")	//Set parent container ("Game/View/_Platforms")
					);

			platformView.name = string.Format ("{0}Platform_{1}", platformType.ToString().ToLower(), Random.Range(0,100));
		}

		platformView.gameObject.SetActive (true);

		//If we got position where platform should be placed
		if (platformPosition != null)
		{
			platformView.transform.position = platformPosition.GetValueOrDefault ();
		}
		else //Get random position for platform
		{
			Vector2 screenSize = GM.Instance.ScreenSize;
			float randomY = 0f;	//Screen center Y coord
			Vector3 platformRandomPosition = default(Vector3);

			switch (platformType)
			{
				case PlatformTypes.HORIZONTAL:
					{
						randomY = Random.Range (-screenSize.y / 2f, screenSize.y / 2f);
						break;
					}

				case PlatformTypes.VERTICAL:
					{
						float platformsGap = game.view.playerView.GetComponent<SpriteRenderer> ().bounds.size.y * 1.1f;	//Add 10% of player height to gap

						randomY = Random.Range (-screenSize.y / 2f + platformsGap / 2f, screenSize.y / 2f - platformsGap / 2f);
						break;
					}
			}

			platformRandomPosition.x = _objectsPoolModel.lastPlatformPosition.x + _objectsPoolModel.lastPlatformWidth / 2f + platformRendererSize.x / 2f + _objectsPoolModel.platformsGap;
			platformRandomPosition.y = randomY;
			platformRandomPosition.z = 0f;

			platformView.transform.position = platformRandomPosition;
		}

		_instantiatedPlatformsList.Add (platformView);

		_objectsPoolModel.lastPlatformPosition = platformView.transform.position;
		_objectsPoolModel.lastPlatformWidth = platformRendererSize.x;

		//Debug.LogFormat ("Pooling platform: {0}. Already instantiated: {1}",platformView.name, _instantiatedPlatformsList.Count);

		platformView.OnInit ();
	}

	/*
	private void PoolObstacle(ObstacleView obstacleView)
	{
		var directionPoint = _objectsPoolModel.poolerPositionDelta;
		var angle = Mathf.Atan2(directionPoint.y, directionPoint.x) * Mathf.Rad2Deg;
		bool isDownDirection = false;

		if (Random.Range (0, 2) == 0)
		{
			isDownDirection = true;
			angle += 180;
		}

		Quaternion obstacleRotation = Quaternion.AngleAxis(angle, Vector3.forward);

		obstacleView.OnInit (_objectsPoolView.transform.position, obstacleRotation, isDownDirection );

	}*/
}

