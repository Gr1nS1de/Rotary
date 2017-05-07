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
						}
					}
					
					break;
				}

			case N.GameOver:
				{
					_instantiatedPlatformsList.ForEach ((platform ) =>
					{
						if(!_poolPlatformsList.Contains(platform))
							_poolPlatformsList.Add(platform);

						_instantiatedPlatformsList.Remove(platform);
					});
					break;
				}
		}
	}

	private void OnStart()
	{
		Vector3 poolerPosition = _objectsPoolView.transform.position;

		poolerPosition.x = GM.Instance.ScreenSize.x;

		_objectsPoolView.transform.position = poolerPosition;
	}

	private void OnGamePlay()
	{
		//Start moving pooler object
		StartCoroutine (MovePoolerViewRoutine());
		//Start pooling
		StartCoroutine( ObjectsPoolingRoutine() );
	}

	public IEnumerator MovePoolerViewRoutine()
	{
		//objectsPoolModel.gapPercentage = _desirableGapLength / game.model.currentRoadModel.roadTweenPath.GetTween ().PathLength ();

		while (game.model.gameState == GameState.PLAYING)
		{
			Vector3 poolerPosition = _objectsPoolView.transform.position;

			_objectsPoolModel.poolerPositionDelta = poolerPosition - _lastObstaclePoolerViewPosition;

			_lastObstaclePoolerViewPosition = poolerPosition;

			_objectsPoolView.UpdateMovePooler ();

			yield return null;
		}
	}

	private IEnumerator ObjectsPoolingRoutine()
	{
		Queue<PoolingObject> poolingQueue = _objectsPoolModel.poolingItemsQueue;

		//wait before move`1asqzx
		yield return null;
		yield return null;

		while ( game.model.gameState == GameState.PLAYING )
		{
			if (poolingQueue.Count <= 0)
			{
				yield return null;
				continue;
			}

			PoolingObject poolingObject = poolingQueue.Dequeue ();

			switch (poolingObject.poolingType)
			{
				case PoolingObjectType.ITEM:
					{
						//ObstacleView obstacleView = (ObstacleView)poolingObject.poolingObject;


						//PoolObstacle (obstacleView);

						yield return new WaitForSeconds( Random.Range( 0.20f, 0.5f ) );

						break;
					}
			}
					
		}
	}

	private void AddObjectToPoolList(PoolingObjectType poolingObjectType, Object poolingObject)
	{
		switch (poolingObjectType)
		{
			case PoolingObjectType.PLATFORM:
				{
					PlatformView platformView = (PlatformView)poolingObject;

					if (platformView != null && !_poolPlatformsList.Contains (platformView))
					{
						_poolPlatformsList.Add (platformView);

						if (_instantiatedPlatformsList.Contains (platformView))
							_instantiatedPlatformsList.Remove (platformView);

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
		Debug.LogFormat("Pool item {0}. count = {1}. position = {2}", itemType, count, objectPosition);

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
		for (int i = 0; i < count; i++)
		{
			StartPoolPlatform (platformType, platformPosition);

			if (platformPosition != null)
			{
				Vector3 platformSize = game.model.gameTheme.GetPlatformRendererSize(platformType);
				Vector3 screenSize = GM.Instance.ScreenSize;

				platformPosition = new Vector3(platformPosition.GetValueOrDefault().x + (platformSize.x + _objectsPoolModel.platformsGap) , platformPosition.GetValueOrDefault().y, 0f ); 
			}
		}
	}

	private void StartPoolPlatform(PlatformTypes platformType, Vector3? platformPosition)
	{
		PlatformView platformView = null;
		bool isInPoolPlatformList = false;

		if (_poolPlatformsList.Count > 0)
		{
			platformView = _poolPlatformsList.Find((platform)=>platform.PlatformType == platformType);

			if (platformView != null)
			{
				platformView.gameObject.SetActive (true);

				_objectsPoolModel.instantiatedPlatforms.Add (platformView);
				_poolPlatformsList.Remove (platformView);

				isInPoolPlatformList = true;
			}
		}

		if(!isInPoolPlatformList)
		{
			platformView = (PlatformView)Instantiate (game.model.gameTheme.PlatformsViewList.Find(platform=>platform.PlatformType == platformType), game.view.transform.Find("_Platforms"));

			_objectsPoolModel.instantiatedPlatforms.Add (platformView);
		}

		if (platformPosition != null)
		{
			platformView.transform.position = platformPosition.GetValueOrDefault ();

			_objectsPoolModel.lastPlatformPosition = platformView.transform.position;
		}
		else
		{
			Vector3 platformSize = game.model.gameTheme.GetPlatformRendererSize(platformType);
			Vector2 screenSize = GM.Instance.ScreenSize;

			float randomY = Random.Range (-screenSize.y / 2f, screenSize.y / 2f);
			Vector3 platformRandomPosition = new Vector3(_objectsPoolModel.lastPlatformPosition.x + platformSize.x + _objectsPoolModel.platformsGap, randomY, 0f);

			platformView.transform.position = platformRandomPosition;

			_objectsPoolModel.lastPlatformPosition = platformRandomPosition;
		}

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

