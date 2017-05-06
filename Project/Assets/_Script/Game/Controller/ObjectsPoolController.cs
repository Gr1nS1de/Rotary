using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using DG.Tweening;

public class ObjectsPoolController : Controller
{
	private ObjectsPoolModel 		_objectsPoolModel		{ get { return game.model.objectsPoolModel; } }
	private PlatformsFactoryModel 	_platformsFactoryModel	{ get { return game.model.platformsFactoryModel; } }
	private ObjectsPoolView			_objectsPoolView		{ get { return game.view.objectsPoolView;}}
	private Queue<PlatformView>		_platformsQueue			{ get { return game.model.objectsPoolModel.poolingPlatformsQueue;}}

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

			case N.AddObjectToPoolQueue__:
				{
					PoolingObjectType poolingObjectType = (PoolingObjectType)data [0];
					Object poolingObject = (Object)data [1];

					AddObjectToPoolQueue (poolingObjectType, poolingObject);

					break;
				}

			case N.PoolObject___:
				{
					PoolingObjectType poolingObjectType = (PoolingObjectType)data [0];
					int objectCount = (int)data [1];
					Vector3? objectPosition = (Vector3?)data [2];

					PoolObject (poolingObjectType, objectCount, objectPosition);
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
				case PoolingObjectType.COIN:
					{
						//ObstacleView obstacleView = (ObstacleView)poolingObject.poolingObject;


						//PoolObstacle (obstacleView);

						yield return new WaitForSeconds( Random.Range( 0.20f, 0.5f ) );

						break;
					}
			}
					
		}
	}

	private void AddObjectToPoolQueue(PoolingObjectType poolingObjectType, Object poolingObject)
	{
		switch (poolingObjectType)
		{
			case PoolingObjectType.PLATFORM:
				{
					_platformsQueue.Enqueue ((PlatformView)poolingObject);
					break;
				}

			case PoolingObjectType.COIN:
				{
					
					break;
				}
		}
	}

	private void PoolObject(PoolingObjectType poolingObjectType, int count, Vector3? objectPosition)
	{
		Debug.LogFormat("PoolObject {0}. count = {1}. position = {2}", poolingObjectType, count, objectPosition);

		switch (poolingObjectType)
		{
			case PoolingObjectType.PLATFORM:
				{
					for (int i = 0; i < count; i++)
					{
						PoolingPlatform (objectPosition);

						if (objectPosition != null)
						{
							Vector3 platformSize = GM.Instance.PlatformRendererSize;
							Vector3 screenSize = GM.Instance.ScreenSize;

							objectPosition = new Vector3(objectPosition.GetValueOrDefault().x + (platformSize.x + _objectsPoolModel.platformsGap) , objectPosition.GetValueOrDefault().y, 0f ); 
						}
					}
					break;
				}
		}
	}

	private void PoolingPlatform(Vector3? platformPosition)
	{
		PlatformView platformView = null;

		if (_platformsQueue.Count > 0)
			platformView = _platformsQueue.Dequeue ();
		else
		{
			platformView = (PlatformView)Instantiate (GM.Instance.CurrentGameTheme.PlatformView, game.view.transform.Find("_Platforms"));
		}

		if (platformPosition != null)
		{
			platformView.transform.position = platformPosition.GetValueOrDefault ();

			_objectsPoolModel.lastPlatformPosition = platformView.transform.position;
		}
		else
		{
			Vector3 platformSize = GM.Instance.PlatformRendererSize;
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

