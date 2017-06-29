using UnityEngine;
using System.Collections;

public class ObjectFixerComponent : MonoBehaviour 
{
	public bool IsFixRotation = false;
	public bool IsFixPosition = false;
	public bool UpdateInitRotationFlag = false;
	public bool UpdateInitPositionFlag = false;

	private Vector3 _position;
	private Quaternion _rotation;
	private Vector3	_deltaParentPosition;

	// Use this for initialization
	void Awake () 
	{
		_rotation = transform.rotation;
		_position = transform.position;
		_deltaParentPosition = _position - transform.parent.position;
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		if (IsFixRotation)
		{
			transform.rotation = _rotation;	
		}

		if (IsFixPosition)
		{
			transform.position = transform.parent.position + _deltaParentPosition;
		}

		if (UpdateInitRotationFlag)
		{
			UpdateInitRotationFlag = false;
			_rotation = transform.rotation;
		}


		if (UpdateInitPositionFlag)
		{
			UpdateInitPositionFlag = false;
			_position = transform.position;
			_deltaParentPosition = _position - transform.parent.position;
		}
	}

}

