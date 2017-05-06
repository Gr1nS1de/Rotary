using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PoolingObjectType
{
	PLATFORM,
	COIN
}

public class PoolingObject
{
	public PoolingObjectType 	poolingType;
	public Object 				poolingObject;
}

public class ObjectsPoolModel : Model
{
	public Queue<PoolingObject>		poolingItemsQueue		{ get { return _poolingItemsQueue; } }
	public Queue<PlatformView>		poolingPlatformsQueue	{ get { return _poolingPlatformsQueue; } }
	public int						platformsMaxCount		{ get { return _platformsMaxCount;} set { _platformsMaxCount = value; }}
	public float					platformsGap			{ get { return _platformsGap;} set { _platformsGap = value; }}
	public Vector3					poolerPositionDelta		{ get { return _poolerPositionDelta;} set { _poolerPositionDelta = value;}}
	public Vector3					lastPlatformPosition	{ get { return _lastPlatformPosition;} set { _lastPlatformPosition = value;}}

	private Queue<PoolingObject> 	_poolingItemsQueue 		= new Queue<PoolingObject>();
	private Queue<PlatformView> 	_poolingPlatformsQueue 	= new Queue<PlatformView>();
	[SerializeField]
	private int						_platformsMaxCount;
	[SerializeField]
	private float					_platformsGap;
	private Vector3					_lastPlatformPosition;
	private Vector3					_poolerPositionDelta;
}

