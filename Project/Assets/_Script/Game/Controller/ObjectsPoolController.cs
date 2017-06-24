using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ObjectsPoolController : Controller
{
	private ObjectsPoolModel 			_objectsPoolModel			{ get { return game.model.objectsPoolModel; } }
	private PlatformsFactoryModel 		_platformsFactoryModel		{ get { return game.model.platformsFactoryModel; } }
	private ObjectsPoolView				_objectsPoolView			{ get { return game.view.objectsPoolView;}}

	private List<PoolingObjectView>		_poolObjectsList			{ get { return game.model.objectsPoolModel.poolObjectsList;}}
	private List<PoolingObjectView>		_instantiatedObjectsList	{ get { return game.model.objectsPoolModel.instantiatedObjectsList;}}
	private const string				PlatformHideDelayTween		= "platformHideDelayTween";
	//private List<PoolingObjectView>		_waitPoolingObjectsList		= new List<PoolingObjectView>();

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

			case N.PlayerImpactItem__:
				{
					ItemView itemView = (ItemView)data [0];
					Vector2 contactPoint = (Vector2)data [1];

					switch (itemView.ItemType)
					{
						case ItemType.Crystal:
							{
								if (_instantiatedObjectsList.Contains (itemView))
									_instantiatedObjectsList.Remove (itemView);
								break;
							}
					}
					break;
				}

			case N.OnItemInvisible_:
				{
					ItemView itemView = (ItemView)data [0];

					if(game.model.gameState == GameState.Playing)
						RestoreItem (itemView);
					break;
				}

			case N.OnPlatformInvisible_:
				{
					PlatformView platformView = (PlatformView)data [0];

					//Debug.LogFormat ("Platform is invisible: {0}", platformView.name);

					if (game.model.gameState == GameState.Playing)
					{
						RestorePlatform (platformView);
					}
					break;
				}

			case N.OnRocketInvisible_:
				{
					RocketView rocketView = (RocketView)data [0];

					StoreObjectToPool (PoolingObjectType.ROCKET, rocketView);
					break;
				}

			case N.GameOver_:
				{
					//GameOverData gameOverData = (GameOverData)data[0];
					List<PoolingObjectView> copyInstantiatedObjectsList = new List<PoolingObjectView> (_instantiatedObjectsList);

					if (DOTween.IsTweening (PlatformHideDelayTween))
						DOTween.Kill (PlatformHideDelayTween);

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

	public void PoolObject(PoolingObjectType poolingObjectType, int objectCount, Vector3? objectPosition, System.Enum objectType)
	{
		switch(poolingObjectType)
		{
			case PoolingObjectType.PLATFORM:
				{
					PlatformType platformType = (PlatformType)objectType;

					PoolPlatform (platformType, objectCount, objectPosition);
					break;
				}

			case PoolingObjectType.ITEM:
				{
					ItemType itemType = (ItemType)objectType;

					// If want to pool Magnet at vertical platform - go swap to pool item. :)
					if (itemType == ItemType.Magnet && (_objectsPoolModel.lastPooledPlatform.platformType == PlatformType.Vertical || _objectsPoolModel.lastPooledPlatform.platformType == PlatformType.Vertical_Moving))
					{
						itemType = Random.Range (0, 2) == 1 ? ItemType.Coin : ItemType.Crystal;
					}

					PoolItem (itemType, objectCount, objectPosition);
					break;
				}

			case PoolingObjectType.BONUS:
				{
					break;
				}

			case PoolingObjectType.ROCKET:
				{
					RocketType rocketType = (RocketType)objectType;

					PoolRocket (rocketType, objectPosition);
					break;
				}
		}

	}
	#endregion

	private void StoreObjectToPool(PoolingObjectType poolingObjectType, PoolingObjectView poolingObject)
	{
		switch (poolingObjectType)
		{
			case PoolingObjectType.PLATFORM:
				{
					PlatformView platformView = (PlatformView)poolingObject;

					//Debug.LogFormat ("Add platform to pool. {0}", platformView.name);

					AddObjectToPool (platformView);
					break;
				}

			case PoolingObjectType.ITEM:
				{
					ItemView itemView = (ItemView)poolingObject;

					AddObjectToPool (itemView);

					break;
				}

			case PoolingObjectType.ROCKET:
				{
					RocketView rocketView = (RocketView)poolingObject;

					AddObjectToPool (rocketView);
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

	private void RestoreItem(ItemView itemView)
	{
		if (IsValidPoolingObject (itemView))
			StoreObjectToPool(PoolingObjectType.ITEM, itemView);
		else
		{
			Destroy (itemView.gameObject);
		}
	}


	private void RestorePlatform(PlatformView platformView)
	{
		if (IsValidPoolingObject (platformView))
		{
			DG.Tweening.DOVirtual.DelayedCall (game.model.playerModel.invisibleBeforeDie, () =>
			{
				StoreObjectToPool (PoolingObjectType.PLATFORM, platformView);
			}).SetId(PlatformHideDelayTween);
		}else
		{
			Destroy (platformView.gameObject);
			Debug.LogErrorFormat ("Trying to restore platform which not is pool valid. Strange behaviour!");
		}
	}

	private void AddObjectToPool(PoolingObjectView poolingObjectview)
	{
		if (!_poolObjectsList.Contains (poolingObjectview))
		{
			_poolObjectsList.Add (poolingObjectview);

			if (_instantiatedObjectsList.Contains (poolingObjectview))
				_instantiatedObjectsList.Remove (poolingObjectview);

			poolingObjectview.OnAddToPool ();

			poolingObjectview.gameObject.SetActive (false);
		}
		else
		{
			Debug.LogErrorFormat ("Trying to add pooling object that already in pool! {0}", poolingObjectview.name);
		}
	}

	private void PoolItem(ItemType itemType, int count, Vector3? objectPosition)
	{
		//Debug.LogFormat("Pool item {0}. count = {1}. position = {2}", itemType, count, objectPosition);
		ItemView itemView = (ItemView)GetPoolingObjectByType (PoolingObjectType.ITEM, itemType);

		if (itemView == null)
		{
			itemView = Instantiate (game.model.itemsFactoryModel.itemsPrefabsList.Find (item => item.ItemType == itemType),
									game.view.transform.Find("_Items"));
		}

		if (objectPosition == null)
		{
			itemView.transform.position = GetItemRandomPosition (itemType);
		}
		else
		{
			itemView.transform.position = objectPosition.GetValueOrDefault ();
		}
			
		if (!itemView.gameObject.activeInHierarchy)
			itemView.gameObject.SetActive (true);

		int itemAddCount = 0;

		switch (itemType)
		{
			case ItemType.Coin:
				{
					itemAddCount = core.playerDataModel.isDoubleCoin ? 2 : 1;
					break;
				}

			case ItemType.Crystal:
				{
					itemAddCount = game.model.itemModel.crystalFractureCount;
					break;
				}
		}

		itemView.Init (itemAddCount);

		_instantiatedObjectsList.Add (itemView);
	}



	private Vector3 GetItemRandomPosition(ItemType itemType)
	{
		Vector3 randomPosition = Vector3.zero;
		Vector3 lastPlatformPosition = _objectsPoolModel.lastPooledPlatform.platformPosition;
		float lastPlatformWidth = _objectsPoolModel.lastPooledPlatform.platformWidth;
		float screenHeight = GM.Instance.ScreenSize.y;
		Vector3 itemRendererSize = game.model.itemModel.GetItemRendererSize (itemType);

		switch (_objectsPoolModel.lastPooledPlatform.platformType)
		{
			case PlatformType.Horizontal:
				{
					float horizontalPlatformHeight = game.model.gameTheme.GetPlatformRendererSize (PlatformType.Horizontal).y;

					randomPosition.x = Random.Range (lastPlatformPosition.x - lastPlatformWidth / 2f + itemRendererSize.x, lastPlatformPosition.x + lastPlatformWidth / 2f - itemRendererSize.x);
					randomPosition.y = Random.Range (-screenHeight / 2f + itemRendererSize.y + horizontalPlatformHeight / 2f, screenHeight / 2f - itemRendererSize.y - horizontalPlatformHeight / 2f);
					break;
				}

			case PlatformType.Vertical_Moving:
			case PlatformType.Vertical:
				{
					float platformsGap = game.model.playerModel.playerRendererSize.y + _platformsFactoryModel.verticalPlatformsGap;
					float randomY = Random.Range (lastPlatformPosition.y - platformsGap / 2f + itemRendererSize.y, lastPlatformPosition.y + platformsGap / 2f - itemRendererSize.y);
					float randomX = Random.Range (lastPlatformPosition.x - lastPlatformWidth / 2f, lastPlatformPosition.x + lastPlatformWidth / 2f);

					randomPosition.x = randomX;
					randomPosition.y = randomY;
					break;
				}
		}

		return randomPosition;
	}

	private void PoolPlatform(PlatformType platformType, int count, Vector3? platformPosition)
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

	private void OnPoolingPlatform(PlatformType platformType, Vector3? platformPosition)
	{
		PlatformView platformView = null;
		bool isInPoolList = false;
		Vector3 platformRendererSize = game.model.gameTheme.GetPlatformRendererSize(platformType);

		//If platform already in pooling list and waiting for being pooled

		platformView = (PlatformView)GetPoolingObjectByType(PoolingObjectType.PLATFORM, platformType);

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
		_objectsPoolModel.lastPooledPlatform.prevPlatformType = _objectsPoolModel.lastPooledPlatform.platformType;
		_objectsPoolModel.lastPooledPlatform.platformType = platformType;

		//Debug.LogFormat ("Pooling platform: {0}. Already instantiated: {1}",platformView.name, _instantiatedPlatformsList.Count);

		platformView.Init ();
	}

	private Vector3 GetPlatfromRandomPosition(PlatformType platformType)
	{
		Vector2 screenSize = GM.Instance.ScreenSize;
		Vector3 platformRendererSize = game.model.gameTheme.GetPlatformRendererSize(platformType);
		float randomY = 0f;	//Screen center Y coord
		Vector3 platformRandomPosition = default(Vector3);

		switch (platformType)
		{
			case PlatformType.Horizontal:
				{
					randomY = Random.Range (-screenSize.y / 2f, screenSize.y / 2f);
					break;
				}

			case PlatformType.Vertical:
				{
					float platformsGap = game.model.playerModel.playerRendererSize.y / 2f + game.model.platformsFactoryModel.verticalPlatformsGap * 1.5f;	//Add 10% of player height to gap

					randomY = Random.Range (-screenSize.y / 2f + platformsGap, screenSize.y / 2f - platformsGap);
					break;
				}
		}

		platformRandomPosition.x = _objectsPoolModel.lastPooledPlatform.platformPosition.x + _objectsPoolModel.lastPooledPlatform.platformWidth / 2f + platformRendererSize.x / 2f + _objectsPoolModel.platformsGap;
		platformRandomPosition.y = randomY;
		platformRandomPosition.z = 0f;

		return platformRandomPosition;
	}

	private void PoolRocket(RocketType rocketType, Vector3? rocketPosition)
	{
		RocketView rocketView = null;
		bool isInPoolList = false;
		Vector3 platformRendererSize = game.model.rocketModel.GetItemRendererSize(rocketType);

		//If platform already in pooling list and waiting for being pooled

		rocketView = (RocketView)GetPoolingObjectByType(PoolingObjectType.ROCKET, rocketType);

		if (rocketView == null)
		{
			rocketView = (RocketView)
				Instantiate (
				game.model.rocketsFactoryModel.rocketsPrefabsList.Find (rocket => rocket.RocketType == rocketType),	//Find platformView in current theme list
					game.view.cameraView.transform.FindChild("_Rockets")	//Set parent container 
			);

			rocketView.name = string.Format ("{0}Rocket_{1}", rocketType.ToString ().ToLower (), Random.Range (0, 100));
		}

		//If we got position where platform should be placed
		if (rocketPosition != null)
		{
			rocketView.transform.localPosition = rocketPosition.GetValueOrDefault ();
		}
		else
		{
			rocketView.transform.localPosition = Vector3.zero;
		}

		if (!rocketView.gameObject.activeInHierarchy)
			rocketView.gameObject.SetActive (true);

		_instantiatedObjectsList.Add (rocketView);

		rocketView.Init ();
	}

	private PoolingObjectView GetPoolingObjectByType(PoolingObjectType poolingObjectType, System.Enum objectType)
	{
		PoolingObjectView poolingObject = null;

		switch (poolingObjectType)
		{
			case PoolingObjectType.ITEM:
				{
					poolingObject = _poolObjectsList.Find (obj => obj.PoolingType == poolingObjectType && ((ItemView)obj).ItemType == (ItemType)objectType);
					break;
				}

			case PoolingObjectType.PLATFORM:
				{
					poolingObject = _poolObjectsList.Find (obj => obj.PoolingType == poolingObjectType && ((PlatformView)obj).PlatformType == (PlatformType)objectType);
					break;
				}

			case PoolingObjectType.ROCKET:
				{
					poolingObject = _poolObjectsList.Find (obj => obj.PoolingType == poolingObjectType && ((RocketView)obj).RocketType == (RocketType)objectType);
					break;
				}
		}

		if (poolingObject != null)
		{
			poolingObject.gameObject.SetActive (true);

			_poolObjectsList.Remove (poolingObject);
		}

		return poolingObject;
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

