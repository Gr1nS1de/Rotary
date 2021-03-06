﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PoolingObjectType
{
	PLATFORM,
	ITEM,
	BONUS,
	ROCKET
}

public class PoolingObject
{
	public PoolingObjectType 	poolingType;
	public Object 				poolingObject;
}

public class LastPooledPlatform
{
	public Vector3 platformPosition;
	public float platformWidth;
	public PlatformType platformType;
	public PlatformType prevPlatformType;

}

public enum PoolingObjectState
{
	WAIT_FOR_VISIBLE,
	VISIBLE,
	WAS_VISIBLE,

}

public class ObjectsPoolModel : Model
{
	public int						platformsMaxCount			{ get { return _platformsMaxCount; } 		set { _platformsMaxCount = value; }}
	public float					platformsGap				{ get { return _platformsGap; } 			set { _platformsGap = value; }}
	//public Vector3				lastPlatformPosition		{ get { return _lastPlatformPosition; } 	set { _lastPlatformPosition = value;}}
	//public float					lastPlatformWidth			{ get { return _lastPlatformWidth; } 		set { _lastPlatformWidth = value;}}
	public LastPooledPlatform		lastPooledPlatform			{ get { return _lastPooledPlatform; } 		set { _lastPooledPlatform = value;}}
	public List<PoolingObjectView>	instantiatedObjectsList		{ get { return _instantiatedObjectsList; }}
	public List<PoolingObjectView>	poolObjectsList				{ get { return _poolObjectsList; } }

	[SerializeField]
	private int						_platformsMaxCount;
	[SerializeField]
	private float					_platformsGap;
	//private Vector3				_lastPlatformPosition;
	//private float					_lastPlatformWidth;
	private LastPooledPlatform		_lastPooledPlatform			= new LastPooledPlatform();
	private List<PoolingObjectView>	_instantiatedObjectsList	= new List<PoolingObjectView>();
	private List<PoolingObjectView> _poolObjectsList 			= new List<PoolingObjectView>();
}

