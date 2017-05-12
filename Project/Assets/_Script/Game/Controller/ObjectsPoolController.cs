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

			case N.PlayerImpactItem__:
				{
					ItemView itemView = (ItemView)data [0];

					switch (itemView.ItemType)
					{
						case ItemTypes.DIMOND:
							{
								if (_instantiatedObjectsList.Contains (itemView))
									_instantiatedObjectsList.Remove (itemView);
								break;
							}
					}
					break;
				}

			case N.GameOver:
				{
					List<PoolingObjectView> copyInstantiatedObjectsList = new List<PoolingObjectView> (_instantiatedObjectsList);

					copyInstantiatedObjectsList.ForEach(obj=>
					{
						StoreObjectToPool(obj.PoolingType, obj);
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
	public void StoreObjectToPool(PoolingObjectType poolingObjectType, PoolingObjectView poolingObject)
	{
		switch (poolingObjectType)
		{
			case PoolingObjectType.PLATFORM:
				{
					PlatformView platformView = (PlatformView)poolingObject;

					//Debug.LogFormat ("Add platform to pool. {0}", platformView.name);

					OnStoringPlatformToPool (platformView);
					break;
				}

			case PoolingObjectType.ITEM:
				{
					ItemView itemView = (ItemView)poolingObject;

					OnStoringItemToPool (itemView);

					break;
				}
		}
	}

	public void PoolObject(PoolingObjectType poolingObjectType, int objectCount, Vector3? objectPosition, System.Enum objectType)
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

	public bool IsValidPoolingObject(PoolingObjectView poolingObjectView)
	{
		bool isValid = false;

		if (_poolObjectsList.Contains (poolingObjectView) || _instantiatedObjectsList.Contains (poolingObjectView))
			isValid = true;
		else
		{
			Debug.LogErrorFormat ("Got invalid pooling object! {0}", poolingObjectView.name);
		}

		return isValid;
	}
	#endregion

	private void OnStoringPlatformToPool(PlatformView platformView)
	{
		if (!_poolObjectsList.Contains (platformView))
		{
			_poolObjectsList.Add (platformView);

			if (_instantiatedObjectsList.Contains (platformView))
				_instantiatedObjectsList.Remove (platformView);

			platformView.OnAddToPool ();

			platformView.gameObject.SetActive (false);
		}
	}

	private void OnStoringItemToPool(ItemView itemView)
	{
		ItemTypes itemType = itemView.ItemType;

		if (!_poolObjectsList.Contains (itemView))
		{
			_poolObjectsList.Add (itemView);

			if (_instantiatedObjectsList.Contains (itemView))
				_instantiatedObjectsList.Remove (itemView);

			itemView.OnAddToPool ();

			itemView.gameObject.SetActive (false);
		}
		else
		{
			Debug.LogErrorFormat ("Trying to add item that already in pool! {0}", itemView.name);
		}
	}

	private void PoolItem(ItemTypes itemType, int count, Vector3? objectPosition)
	{
		//Debug.LogFormat("Pool item {0}. count = {1}. position = {2}", itemType, count, objectPosition);
		ItemView itemView = GetPoolListItem(itemType);

		if (itemView == null)
		{
			itemView = Instantiate (game.model.itemsFactoryModel.itemsPrefabsList.Find (item => item.ItemType == itemType),
									game.view.transform.Find("_Items"));

			if (!gameObject.activeInHierarchy)
				gameObject.SetActive (true);
		}

		if (objectPosition == null)
		{
			itemView.transform.position = GetItemRandomPosition (itemType);
		}
		else
		{
			itemView.transform.position = objectPosition.GetValueOrDefault ();
		}

		itemView.OnInit ();

		_instantiatedObjectsList.Add (itemView);
	}

	private Vector3 GetItemRandomPosition(ItemTypes itemType)
	{
		Vector3 randomPosition = Vector3.zero;
		Vector3 lastPlatformPosition = _objectsPoolModel.lastPooledPlatform.platformPosition;
		float lastPlatformWidth = _objectsPoolModel.lastPooledPlatform.platformWidth;
		float screenHeight = GM.Instance.ScreenSize.y;
		Vector3 itemRendererSize = game.model.itemsFactoryModel.GetItemRendererSize (itemType);

		switch (_objectsPoolModel.lastPooledPlatform.platformType)
		{
			case PlatformTypes.HORIZONTAL:
				{
					float horizontalPlatformHeight = game.model.gameTheme.GetPlatformRendererSize (PlatformTypes.HORIZONTAL).y;

					randomPosition.x = Random.Range (lastPlatformPosition.x - lastPlatformWidth / 2f + itemRendererSize.x, lastPlatformPosition.x + lastPlatformWidth / 2f - itemRendererSize.x);
					randomPosition.y = Random.Range (-screenHeight / 2f + itemRendererSize.y + horizontalPlatformHeight / 2f, screenHeight / 2f - itemRendererSize.y - horizontalPlatformHeight / 2f);
					break;
				}

			case PlatformTypes.VERTICAL:
				{
					randomPosition.x = lastPlatformPosition.x;
					randomPosition.y = lastPlatformPosition.y;
					break;
				}
		}

		return randomPosition;
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

		platformView = GetPoolListPlatform (platformType);

		if(platformView == null)
		{
			platformView = (PlatformView)
					Instantiate (
						game.model.gameTheme.PlatformsViewList.Find(platform=>platform.PlatformType == platformType),	//Find platformView in current theme list
						game.view.transform.Find("_Platforms")	//Set parent container ("Game/View/_Platforms")
					);

			platformView.name = string.Format ("{0}Platform_{1}", platformType.ToString().ToLower(), Random.Range(0,100));

			platformView.gameObject.SetActive (true);
		}

		//If we got position where platform should be placed
		if (platformPosition != null)
		{
			platformView.transform.position = platformPosition.GetValueOrDefault ();
		}
		else
		{
			//Get random position for platform
			platformView.transform.position = GetPlatfromRandomPosition(platformType);
		}

		_instantiatedObjectsList.Add (platformView);

		_objectsPoolModel.lastPooledPlatform.platformPosition = platformView.transform.position;
		_objectsPoolModel.lastPooledPlatform.platformWidth = platformRendererSize.x;
		_objectsPoolModel.lastPooledPlatform.platformType = platformType;

		//Debug.LogFormat ("Pooling platform: {0}. Already instantiated: {1}",platformView.name, _instantiatedPlatformsList.Count);

		platformView.OnInit ();
	}

	private Vector3 GetPlatfromRandomPosition(PlatformTypes platformType)
	{
		Vector2 screenSize = GM.Instance.ScreenSize;
		Vector3 platformRendererSize = game.model.gameTheme.GetPlatformRendererSize(platformType);
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
					float platformsGap = game.model.playerModel.playerRendererSize.y / 2f + game.model.platformsFactoryModel.verticalPlatformsGap;	//Add 10% of player height to gap

					randomY = Random.Range (-screenSize.y / 2f + platformsGap / 2f, screenSize.y / 2f - platformsGap / 2f);
					break;
				}
		}

		platformRandomPosition.x = _objectsPoolModel.lastPooledPlatform.platformPosition.x + _objectsPoolModel.lastPooledPlatform.platformWidth / 2f + platformRendererSize.x / 2f + _objectsPoolModel.platformsGap;
		platformRandomPosition.y = randomY;
		platformRandomPosition.z = 0f;

		return platformRandomPosition;
	}

	private PlatformView GetPoolListPlatform(PlatformTypes platformType)
	{
		PlatformView pooledPlatform = null;

		foreach(var platformObj in _poolObjectsList)
		{
			if(platformObj is PlatformView)
			{
				PlatformView platform = (PlatformView)platformObj;

				if(platform.PlatformType == platformType)
				{
					pooledPlatform = platform;

					pooledPlatform.gameObject.SetActive (true);

					_poolObjectsList.Remove (pooledPlatform);
					break;
				}
			}
		}

		return pooledPlatform;
	}

	private ItemView GetPoolListItem(ItemTypes itemType)
	{
		ItemView pooledItem = null;

		foreach(var itemObj in _poolObjectsList)
		{
			if (itemObj is ItemView)
			{
				ItemView item = (ItemView)itemObj;

				if (item.ItemType == itemType)
				{
					pooledItem = item;

					pooledItem.gameObject.SetActive (true);

					_poolObjectsList.Remove (pooledItem);
					break;
				}
			}
		}

		return pooledItem;
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

