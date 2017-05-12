using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Destructible2D;

public class PlayerView : View
{
	[HideInInspector]
	public List<PlatformView>	ScorePlatformsList	= new List<PlatformView>();
	public bool IsShowDebugCameraDistance = false;

	private PlayerModel _playerModel	{ get { return game.model.playerModel; } }
	private Rigidbody2D _playerRB;
	private ConstantForce2D _playerConstantForce;
	private Vector3 _playerInitPosition;
	private float _initCameraDistanceX = 0f;
	//private float _lastCameraDistance = 0f;
	private float? _lastInvisibleTimestamp = 0f;
	private float _offsetBackspeedRate = 0f;
	private float _backOffsetSpeed = 0f;

	void Awake()
	{
		_playerRB = GetComponent<Rigidbody2D> ();
		_playerConstantForce = GetComponent<ConstantForce2D> ();
	}

	public void OnInit(Vector3 initPosition)
	{
		_playerInitPosition = initPosition;

		_initCameraDistanceX = Mathf.Abs (- GM.Instance.ScreenSize.x / 2f + (GM.Instance.ScreenSize.x * _playerModel.initScreenPosX)  - game.view.cameraView.transform.position.x);

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

	void Update()
	{
		if (game.model.gameState != GameState.PLAYING)
			return;
		
		Vector3 playerPosition = transform.transform.position;

		playerPosition.x += _playerModel.linearForce * game.model.gameSpeed + _backOffsetSpeed;

		transform.position = Vector3.Lerp(transform.position, playerPosition, Time.deltaTime);

		float currentCameraDistanceX = Mathf.Abs (game.view.cameraView.transform.position.x - transform.position.x);
		float currentOffset = _initCameraDistanceX - currentCameraDistanceX;
		float currentOffsetAbs = Mathf.Abs (currentOffset);


		if (currentOffsetAbs > 0.1f)
		{
			
			//_offsetBackspeedRate += 0.1f;
			if (currentOffset > 0f)
			{
				_backOffsetSpeed = - currentOffsetAbs ;
				//Mathf.Lerp (0f, _playerModel.offsetBackForce, _offsetBackspeedRate);
			}
			else
			{
				_backOffsetSpeed = currentOffsetAbs;
			}

			_backOffsetSpeed = Mathf.Clamp (_backOffsetSpeed, -_playerModel.offsetBackForceMax, _playerModel.offsetBackForceMax);
		}else
		{
			_backOffsetSpeed = 0f;
		}

		if (_playerModel.angularSpeed != 0f)
		{
			transform.Rotate (0f, 0f, _playerModel.angularSpeed * game.model.gameSpeed * Time.fixedDeltaTime);
		}

		if (_lastInvisibleTimestamp != null && _lastInvisibleTimestamp < Time.time)
			Notify (N.OnPlayerInvisible);

		if(IsShowDebugCameraDistance)
			Debug.LogFormat("Current camera offset = {0}. back offset speed = {1}",currentOffset, _backOffsetSpeed );
		
	}

		/*
	void FixedUpdate()
	{
		if (game.model.gameState != GameState.PLAYING)
			return;

		if (_playerModel.angularSpeed != 0f)
		{
			transform.Rotate (0f, 0f, _playerModel.angularSpeed * game.model.gameSpeed * Time.fixedDeltaTime);
		}

			if (Mathf.Abs (currentOffset) > 1f)
			{
				_offsetBackspeedRate += 0.1f;

				if (currentOffset < 0f)
					_playerRB.angularVelocity = _playerModel.angularSpeed * game.model.gameSpeed - Mathf.Lerp(0f, _playerModel.offsetBackForce, _offsetBackspeedRate * Time.fixedDeltaTime);
				else
					_playerRB.angularVelocity = _playerModel.angularSpeed * game.model.gameSpeed + Mathf.Lerp(0f, _playerModel.offsetBackForce, _offsetBackspeedRate * Time.fixedDeltaTime);
			}
			else
			{
				_offsetBackspeedRate = 0f;
				_playerRB.angularVelocity = _playerModel.angularSpeed *  game.model.gameSpeed;
			}
		}


		if (_playerModel.linearForce != 0)
		{
			//transform.position += new Vector3 (_playerModel.linearSpeed * game.model.gameMoveSpeed * Time.fixedDeltaTime, 0f, 0f);
			_playerConstantForce.force = ( new Vector2 (_playerModel.linearForce * game.model.gameSpeed, 0f));
		}


		//Debug.LogErrorFormat ("Current distance to camera = {0}. Last frame distance = {1}. Offset = {2}", Vector2.Distance(game.view.cameraView.transform.position, transform.position), _lastCameraDistance, Vector2.Distance(game.view.cameraView.transform.position, transform.position) - _lastCameraDistance );
		//_lastCameraDistance = Vector2.Distance(game.view.cameraView.transform.position, transform.position);
		//_playerRB.DOMoveX (transform.position.x + _playerModel.moveSpeed, 1f).SetUpdate(UpdateType.Fixed);
	}
*/
	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.transform.parent == null)
			return;
		
		PoolingObjectView poolingObject = collision.transform.parent.GetComponent<PoolingObjectView> ();

		if (poolingObject == null)
			return;
		
		switch(poolingObject.PoolingType)
		{
			case PoolingObjectType.PLATFORM:
				{
					PlatformView platformView = (PlatformView)poolingObject;

					break;
				}

			case PoolingObjectType.ITEM:
				{
					ItemView itemView = (ItemView)poolingObject;
					D2dDestructible destructibleItem = itemView.GetComponentInChildren<D2dDestructible>();

					Notify (N.PlayerImpactItem__, NotifyType.GAME, itemView, collision.contacts [0].point);
					break;
				}

			default:
				{
					break;
				}
		}
	}

	void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.transform.parent == null)
			return;

		PoolingObjectView poolingObject = collision.transform.parent.GetComponent<PoolingObjectView> ();

		if (poolingObject == null)
			return;

		switch (poolingObject.PoolingType)
		{
			case PoolingObjectType.PLATFORM:
				{
					PlatformView platformView = (PlatformView)poolingObject;

					if (transform.position.x > platformView.transform.position.x)
					{
						if (!ScorePlatformsList.Contains (platformView))
						{
							Notify (N.PlayerLeftPlatform_, NotifyType.GAME, platformView);

							ScorePlatformsList.Add (platformView);
						}
					}
					break;
				}

			default:
				{
					break;
				}
		}
	}
}
