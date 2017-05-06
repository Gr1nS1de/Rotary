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
	private float 					_desirableGapLength = 1.884f;
	private Vector2 				_screenSize;

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

					AddObjectToPool (poolingObjectType, poolingObject);

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
		float screenHeight = Camera.main.orthographicSize * 2.0f;
		float screenWidth = screenHeight * Camera.main.aspect;
		_screenSize = new Vector2 (screenWidth, screenHeight);
		Vector3 poolerPosition = _objectsPoolView.transform.position;

		poolerPosition.x = screenWidth;

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

	private void AddObjectToPool(PoolingObjectType poolingObjectType, Object poolingObject)
	{
		switch (poolingObjectType)
		{
			case PoolingObjectType.PLATFORM:
				{
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
		switch (poolingObjectType)
		{
			case PoolingObjectType.PLATFORM:
				{
					for (int i = 0; i < count; i++)
						PoolingPlatform (objectPosition);
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
			//Instantiate ();
		}

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

