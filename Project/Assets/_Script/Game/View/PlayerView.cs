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
	private float _offsetBackspeedRate = 0f;

	void Awake()
	{
		_playerRB = GetComponent<Rigidbody2D> ();
	}

	public void OnInit(Vector3 initPosition)
	{
		_playerInitPosition = initPosition;

		_initCameraDistanceX = Mathf.Abs (initPosition.x - game.view.cameraView.transform.position.x);

		//initPosition.x /= 1.5f;

		transform.position = initPosition;

		if(_playerModel.forceOnInit != 0f)
			transform.GetComponent<Rigidbody2D> ().AddForce (Vector2.right * _playerModel.forceOnInit, ForceMode2D.Impulse);
	}

	void OnBecameInvisible()
	{
		_lastInvisibleTimestamp = Time.time + _playerModel.invisibleBeforeDie;
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

		if (_playerModel.angularSpeed != 0)
		{
			if (Mathf.Abs (currentOffset) > 1f)
			{
				_offsetBackspeedRate += 0.1f;

				if (currentOffset < 0f)
					_playerRB.angularVelocity = _playerModel.angularSpeed * game.model.gameMoveSpeed - Mathf.Lerp(0f, _playerModel.offsetBackSpeed, _offsetBackspeedRate * Time.fixedDeltaTime);
				else
					_playerRB.angularVelocity = _playerModel.angularSpeed * game.model.gameMoveSpeed + Mathf.Lerp(0f, _playerModel.offsetBackSpeed, _offsetBackspeedRate * Time.fixedDeltaTime);
			}
			else
			{
				_offsetBackspeedRate = 0f;
				_playerRB.angularVelocity = _playerModel.angularSpeed * game.model.gameMoveSpeed;
			}
		}

		if (_lastInvisibleTimestamp != null && _lastInvisibleTimestamp < Time.time)
			Notify (N.GameOver);

		//Debug.LogErrorFormat ("Current distance to camera = {0}. Last frame distance = {1}. Offset = {2}", Vector2.Distance(game.view.cameraView.transform.position, transform.position), _lastCameraDistance, Vector2.Distance(game.view.cameraView.transform.position, transform.position) - _lastCameraDistance );
		//Debug.Log("Current camera offset = "+currentOffset +  (Mathf.Abs (currentOffset) > 1f ? " > " + (Mathf.Abs (currentOffset) - 1f) : "") );
		//_lastCameraDistance = Vector2.Distance(game.view.cameraView.transform.position, transform.position);
		//_playerRB.DOMoveX (transform.position.x + _playerModel.moveSpeed, 1f).SetUpdate(UpdateType.Fixed);

		if(_playerModel.linearSpeed != 0)
			transform.position += new Vector3( _playerModel.linearSpeed * game.model.gameMoveSpeed * Time.fixedDeltaTime, 0f, 0f);
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.transform.parent == null)
			return;

		PlatformView platformView = collision.transform.parent.GetComponent<PlatformView> ();
		
		if(platformView != null)
		{
			if (!_playerModel.scorePlatformsList.Contains (platformView))
			{
				Notify (N.GameAddScore, NotifyType.ALL);

				_playerModel.scorePlatformsList.Add (platformView);
			}
		}
	}
}
