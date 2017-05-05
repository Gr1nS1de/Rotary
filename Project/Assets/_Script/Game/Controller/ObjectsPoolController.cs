using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using DG.Tweening;

public class ObjectsPoolController : Controller
{
	public ObjectsPoolModel objectsPoolModel	{ get { return game.model.objectsPoolModel; } } 
	public ObjectsPoolView	objectsPoolView		{ get { return game.view.objectsPoolView;}}

	private Vector3			_lastObstaclePoolerViewPosition;
	private float 			_desirableGapLength = 1.884f;

	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.GamePlay:
				{
					OnGamePlay ();

					break;
				}
		}
	}

	private void OnGamePlay()
	{
	}
	/*
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

		while (true)
		{
			Vector3 poolerPosition = objectsPoolView.transform.position;

			objectsPoolModel.poolerPositionDelta = poolerPosition - _lastObstaclePoolerViewPosition;

			_lastObstaclePoolerViewPosition = poolerPosition;

			objectsPoolView.UpdateMovePooler ();

			yield return null;
		}
	}

	private IEnumerator ObjectsPoolingRoutine()
	{
		Queue<PoolingObject> poolingQueue = objectsPoolModel.poolingQueue;

		//wait before move`1asqzx
		yield return null;
		yield return null;

		while ( true )
		{
			if (poolingQueue.Count <= 0)
			{
				yield return null;
				continue;
			}

			PoolingObject poolingObject = poolingQueue.Dequeue ();

			switch (poolingObject.poolingType)
			{
				case PoolingObjectType.OBSTACLE:
					{
						ObstacleView obstacleView = (ObstacleView)poolingObject.poolingObject;


						PoolObstacle (obstacleView);

						yield return new WaitForSeconds( Random.Range( 0.20f, 0.5f ) );

						break;
					}
			}
					
		}
	}

	private void PoolObstacle(ObstacleView obstacleView)
	{
		var directionPoint = objectsPoolModel.poolerPositionDelta;
		var angle = Mathf.Atan2(directionPoint.y, directionPoint.x) * Mathf.Rad2Deg;
		bool isDownDirection = false;

		if (Random.Range (0, 2) == 0)
		{
			isDownDirection = true;
			angle += 180;
		}

		Quaternion obstacleRotation = Quaternion.AngleAxis(angle, Vector3.forward);

		obstacleView.OnInit (objectsPoolView.transform.position, obstacleRotation, isDownDirection );

	}*/
}

