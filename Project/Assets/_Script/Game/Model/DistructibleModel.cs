using UnityEngine;
using System.Collections;

public class DistructibleModel : Model
{
	public float			breakForce							{ get { return _breakForce;}}
	public Vector3 			entityBreakPoint					{ get { return _entityBreakPoint;} set { _entityBreakPoint = value; }}
	public int				playerFtactureCount					{ get { return _playerFractureCount;}}
	public int				distructibleObstacleFractureCount	{ get { return _distructibleObstacleFractureCount;}}

	[SerializeField]
	private float 			_breakForce;
	private Vector3 		_entityBreakPoint;
	[SerializeField]
	private int				_playerFractureCount;
	[SerializeField]
	private	int 			_distructibleObstacleFractureCount;
}
