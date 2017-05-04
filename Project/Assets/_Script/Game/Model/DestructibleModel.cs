using UnityEngine;
using System.Collections;

public class DestructibleModel : Model
{
	public float			breakForce							{ get { return _breakForce;}}
	public Vector3 			entityBreakPoint					{ get { return _entityBreakPoint;} set { _entityBreakPoint = value; }}
	public int				playerFtactureCount					{ get { return _playerFractureCount;}}
	public int				destructibleObstacleFractureCount	{ get { return _destructibleObstacleFractureCount;}}

	[SerializeField]
	private float 			_breakForce;
	private Vector3 		_entityBreakPoint;
	[SerializeField]
	private int				_playerFractureCount;
	[SerializeField]
	private	int 			_destructibleObstacleFractureCount;
}
