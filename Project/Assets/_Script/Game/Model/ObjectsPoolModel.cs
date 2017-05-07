using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PoolingObjectType
{
	PLATFORM,
	ITEM,
	BONUS
}

public class PoolingObject
{
	public PoolingObjectType 	poolingType;
	public Object 				poolingObject;
}

public class ObjectsPoolModel : Model
{
	public Queue<PoolingObject>		poolingItemsQueue		{ get { return _poolingItemsQueue; } }
	public List<PlatformView>		poolPlatformsList		{ get { return _poolPlatformsList; } }
	public int						platformsMaxCount		{ get { return _platformsMaxCount;} set { _platformsMaxCount = value; }}
	public float					platformsGap			{ get { return _platformsGap;} set { _platformsGap = value; }}
	public Vector3					poolerPositionDelta		{ get { return _poolerPositionDelta;} set { _poolerPositionDelta = value;}}
	public Vector3					lastPlatformPosition	{ get { return _lastPlatformPosition;} set { _lastPlatformPosition = value;}}
	public float					lastPlatformWidth		{ get { return _lastPlatformWidth;} set { _lastPlatformWidth = value;}}
	public List<PlatformView>		instantiatedPlatforms	{ get { return _instantiatedPlatforms;}}


	private List<PlatformView> 		_instantiatedPlatforms 	= new List<PlatformView>();
	private Queue<PoolingObject> 	_poolingItemsQueue 		= new Queue<PoolingObject>();
	private List<PlatformView> 		_poolPlatformsList 		= new List<PlatformView>();
	[SerializeField]
	private int						_platformsMaxCount;
	[SerializeField]
	private float					_platformsGap;
	private Vector3					_lastPlatformPosition;
	private float					_lastPlatformWidth;
	private Vector3					_poolerPositionDelta;
}

