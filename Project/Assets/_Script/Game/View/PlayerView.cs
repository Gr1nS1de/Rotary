using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerView : View
{
	private PlayerModel _playerModel	{ get { return game.model.playerModel; } }
	private Rigidbody2D _playerRB;
	private Vector3 _playerInitPosition;
	private float _initCameraDistanceX = 0f;
	//private float _lastCameraDistance = 0f;
	private float? _lastInvisibleTimestamp = 0f;

	void Awake()
	{
		_playerRB = GetComponent<Rigidbody2D> ();
	}

	public void OnInit(Vector3 initPosition)
	{
		_playerInitPosition = initPosition;

		_initCameraDistanceX = Mathf.Abs (initPosition.x - game.view.cameraView.transform.position.x);

		initPosition.x /= 1.5f;

		transform.position = initPosition;
	}

	void OnBecameInvisible()
	{
		_lastInvisibleTimestamp = Time.time + 1f;
	}

	void OnBecameVisible()
	{
		_lastInvisibleTimestamp = null;
	}
		

	void FixedUpdate()
	{
		if (game.model.gameState != GameState.PLAYING)
			return;

		float currentCameraDistanceX = Mathf.Abs (game.view.cameraView.transform.position.x - transform.position.x);
		float currentOffset = _initCameraDistanceX - currentCameraDistanceX;

		if (Mathf.Abs (currentOffset) > 0.15f)
		{
			if (currentOffset < 0f)
				_playerRB.angularVelocity = _playerModel.angularSpeed - _playerModel.offsetBackSpeed;
			else
				_playerRB.angularVelocity = _playerModel.angularSpeed + _playerModel.offsetBackSpeed;
		}
		else
		{
			_playerRB.angularVelocity = _playerModel.angularSpeed;
		}

		if (_lastInvisibleTimestamp != null && _lastInvisibleTimestamp < Time.time)
			Notify (N.GameOver);

		//Debug.LogErrorFormat ("Current distance to camera = {0}. Last frame distance = {1}. Offset = {2}", Vector2.Distance(game.view.cameraView.transform.position, transform.position), _lastCameraDistance, Vector2.Distance(game.view.cameraView.transform.position, transform.position) - _lastCameraDistance );
		//Debug.LogError("Current camera offset = "+currentOffset +  (Mathf.Abs (currentOffset) > 0.15f ? " > " + (Mathf.Abs (currentOffset) - 0.15f) : "") );
		//_lastCameraDistance = Vector2.Distance(game.view.cameraView.transform.position, transform.position);
		//_playerRB.DOMoveX (transform.position.x + _playerModel.moveSpeed, 1f).SetUpdate(UpdateType.Fixed);
		//transform.position += new Vector3( _playerModel.moveSpeed * Time.fixedDeltaTime, 0f, 0f);
	}



}
