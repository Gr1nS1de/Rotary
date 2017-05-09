using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using DG.Tweening;

public class ObjectsPoolController : Controller
{
	private ObjectsPoolModel 			_objectsPoolModel			{ get { return game.model.objectsPoolModel; } }
	private PlatformsFactoryModel 		_platformsFactoryModel		{ get { return game.model.platformsFactoryModel; } }
	private ObjectsPoolView				_objectsPoolView			{ get { return game.view.objectsPoolView;}}
	private List<PoolingObjectView>		_poolObjectsList			{ get { return game.model.objectsPoolModel.poolObjectsList;}}
	private List<PoolingObjectView>		_instantiatedObjectsList	{ get { return game.model.objectsPoolModel.instantiatedObjectsList;}}

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

			case N.GameOver:
				{
					List<PoolingObjectView> copyInstantiatedObjectsList = new List<PoolingObjectView> (_instantiatedObjectsList);

					copyInstantiatedObjectsList.ForEach(obj=>
					{
						AddObjectToPool(obj.PoolingType, obj);
					});

					copyInstantiatedObjectsList = null;
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

	#region public methods
	public void AddObjectToPool(PoolingObjectType poolingObjectType, PoolingObjectView poolingObject)
	{
		switch (poolingObjectType)
		{
			case PoolingObjectType.PLATFORM:
				{
					PlatformView platformView = (PlatformView)poolingObject;

					//Debug.LogFormat ("Add platform to pool. {0}", platformView.name);

					if (platformView != null && !_poolObjectsList.Contains (platformView))
					{
						_poolObjectsList.Add (platformView);

						if (_instantiatedObjectsList.Contains (platformView))
							_instantiatedObjectsList.Remove (platformView);

						platformView.OnAddToPool ();

						platformView.gameObject.SetActive (false);
					}
					break;
				}

			case PoolingObjectType.ITEM:
				{
					ItemView itemView = (ItemView)poolingObject;



					break;
				}
		}
	}

	public void PoolObject(PoolingObjectType poolingObjectType, int objectCount, Vector3? objectPosition, System.Enum objectType)
	{

		if (game.model.gameState == GameState.PLAYING)
		{
			switch(poolingObjectType)
			{
				case PoolingObjectType.PLATFORM:
					{
						PlatformTypes platformType = (PlatformTypes)objectType;

						PoolPlatform (platformType, objectCount, objectPosition);
						break;
					}

				case PoolingObjectType.ITEM:
					{
						ItemTypes itemType = (ItemTypes)objectType;

						PoolItem (itemType, objectCount, objectPosition);
						break;
					}

				case PoolingObjectType.BONUS:
					{
						break;
					}
			}
		}

	}
	#endregion

	private void PoolItem(ItemTypes itemType, int count, Vector3? objectPosition)
	{
		//Debug.LogFormat("Pool item {0}. count = {1}. position = {2}", itemType, count, objectPosition);

		switch (itemType)
		{
			case ItemTypes.COIN:
				{

					break;
				}

			case ItemTypes.DIMOND:
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
		if (_poolObjectsList.Count > 0)
		{
			_poolObjectsList.ForEach((platformObj)=>
			{
				if(platformView== null)
				{
					PlatformView platform = (PlatformView)platformObj;

					if(platform.PlatformType == platformType)
					{
						platformView = platform;
					}
				}
			});
				//.Find(platform=>((PlatformView)platform).PlatformType == platformType);

			if (platformView != null)
			{
				platformView.gameObject.SetActive (true);

				_poolObjectsList.Remove (platformView);

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

		_instantiatedObjectsList.Add (platformView);

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

