using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PoolingObjectType
{
	OBSTACLE
}

public class PoolingObject
{
	public PoolingObjectType 	poolingType;
	public Object 				poolingObject;
}

public class ObjectsPoolModel : Model
{
	public Queue<PoolingObject>		poolingQueue		{ get { return _poolingList; } }
	public float					gapPercentage		{ get { return _gapPercentage;} set { _gapPercentage = value; }}
	public Vector3					poolerPositionDelta	{ get { return _poolerPositionDelta;} set { _poolerPositionDelta = value;}}

	private Queue<PoolingObject> 	_poolingList 	= new Queue<PoolingObject>();
	private float					_gapPercentage;
	private Vector3					_poolerPositionDelta;
}

