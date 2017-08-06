using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Destructible2D;

public class PlayerView : View
{
	[HideInInspector]
	public List<PlatformView>	ScorePlatformsList	= new List<PlatformView>();
	public SpriteRenderer		PlayerRenderer;
	public Transform 			PlayerTriggerDetector;
	public LayerMask 			JumpRayMask;

	private PlayerModel 		_playerModel	{ get { return game.model.playerModel; } }
	private Rigidbody2D 		_playerRB;
	private float 				_initCameraDistanceX = 0f;
	//private float 			_lastCameraDistance = 0f;
	private float? 				_lastInvisibleTimestamp = null;
	private float 				_offsetBackspeedRate = 0f;
	private float 				_backOffsetSpeed = 0f;
	private RaycastHit2D		_jumpRaycast;
	private float				_lastJumpTimestamp;
	private Animator			_playerAnimator;
	private bool				_isRotatable;

	//initialize values 
	void Start() 
	{
		_playerRB = PlayerRenderer.GetComponent<Rigidbody2D>();
		_playerAnimator = PlayerRenderer.GetComponent<Animator> ();
	} 

	public void OnInit()
	{

		_initCameraDistanceX = Mathf.Abs (- GM.Instance.ScreenSize.x / 2f + (GM.Instance.ScreenSize.x * _playerModel.initScreenPosX)  - game.view.cameraView.transform.position.x);

	}

	public void SetupSkin(PlayerSkinView playerSkinView)
	{
		if (playerSkinView.SkinAnimationController != null)
		{

			_playerAnimator.enabled = true;
			_playerAnimator.runtimeAnimatorController = playerSkinView.SkinAnimationController;
			_isRotatable = false;
		}
		else
		{
			_playerAnimator.enabled = false;
			_playerAnimator.runtimeAnimatorController = null;
			PlayerRenderer.sprite = playerSkinView.SkinSprite;
			_isRotatable = true;
		}
	}

	public override void OnInvisible(ViewVisibleDetect visibleDetector)
	{
		_lastInvisibleTimestamp = Time.time + _playerModel.invisibleBeforeDie;
	}

	public override void OnVisible(ViewVisibleDetect visibleDetector)
	{
		_lastInvisibleTimestamp = null;
	}
		
	void Update()
	{
		if (_playerModel.playerState == PlayerState.GamePlay)
		{
			MovePlayer ();
			if (_isRotatable)
			{
				RotatePlayer ();
			}
			CheckJumpRay ();

			if (_lastInvisibleTimestamp != null && _lastInvisibleTimestamp < Time.time)
			{

				_lastInvisibleTimestamp = null;

				Notify (N.OnPlayerInvisible);
			}
		}

		//Debug.LogFormat("Current camera offset = {0}. back offset speed = {1}",currentOffset, _backOffsetSpeed );
		
	}

	private void CheckJumpRay()
	{
		_jumpRaycast = Physics2D.Raycast(new Vector2(PlayerRenderer.transform.position.x, PlayerRenderer.transform.position.y - PlayerRenderer.size.y / 4f), (Vector2)PlayerRenderer.transform.position + Vector2.right, PlayerRenderer.size.x, JumpRayMask);

		if (_jumpRaycast.collider != null)
		{
			if (_lastJumpTimestamp < Time.time)
			{
				_lastJumpTimestamp = Time.time + 1f;
				_playerRB.AddForce (Vector2.up * 4f, ForceMode2D.Impulse);
			}
		}
	}

	private void MovePlayer()
	{
		Vector3 currentPosition = PlayerRenderer.transform.position;
		Vector3 playerNextPosition = currentPosition;

		playerNextPosition.x += _playerModel.linearForce * game.model.gameSpeed + _backOffsetSpeed;

		PlayerRenderer.transform.position = PlayerTriggerDetector.position = Vector3.Lerp(currentPosition, playerNextPosition, Time.deltaTime);

		currentPosition = PlayerRenderer.transform.position;

		float currentCameraDistanceX = Mathf.Abs (game.view.cameraView.transform.position.x - currentPosition.x);
		float currentOffset = _initCameraDistanceX - currentCameraDistanceX;
		float currentOffsetAbs = Mathf.Abs (currentOffset);

		if (currentOffsetAbs > 0.1f)
		{
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
	}

	private void RotatePlayer()
	{
		if (_playerModel.angularSpeed != 0f)
		{
			PlayerRenderer.transform.Rotate (0f, 0f, _playerModel.angularSpeed * game.model.gameSpeed * Time.deltaTime);
		}
	}

	public override void OnRendererCollisionEnter(ViewCollisionDetect collisionDetector, Collision2D collision)
	{
		if (collision.transform.parent == null)
			return;
		
		PoolingObjectView poolingObject = collision.transform.GetComponentInParent<PoolingObjectView> ();

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

					Notify (N.PlayerImpactItem__, NotifyType.GAME, itemView, collision.contacts [0].point);
					break;
				}

			case PoolingObjectType.ROCKET:
				{
					RocketView rocketView = (RocketView)poolingObject;
					Vector2 contactPoint = collision.contacts [0].point;

					//Debug.LogErrorFormat ("Contant point y: {0}. rocketRendererPositionY: {1}. rocket renderer size: {2}", contactPoint.y, rocketView.RocketRenderer.transform.position.y, rocketView.GetMainRendererSize ().y);

					//If player is higher than rocket - just roll on it. :)
					if (contactPoint.y < rocketView.RocketRenderer.transform.position.y + rocketView.GetMainRendererSize ().y / 3f)
					{
						Notify (N.PlayerImpactRocket__, NotifyType.GAME, rocketView, contactPoint);
					}
					break;
				}

			default:
				{
					break;
				}
		}
	}

	public override void OnRendererTriggerEnter (ViewTriggerDetect triggerDetector, Collider2D otherCollider)
	{
		if (game.model.gameState != GameState.Playing)
			return;
		
		PoolingObjectView poolingObject = otherCollider.transform.GetComponentInParent<PoolingObjectView> ();

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

					Notify (N.PlayerImpactItem__, NotifyType.GAME, itemView, Vector2.zero);

					break;
				}

			default:
				{
					break;
				}
		}
	}

	public override void OnRendererTriggerExit (ViewTriggerDetect triggerDetector, Collider2D otherCollider)
	{
		if (game.model.gameState != GameState.Playing)
			return;
		
		PoolingObjectView poolingObject = otherCollider.transform.GetComponentInParent<PoolingObjectView> ();

		if (poolingObject == null)
			return;

		switch (poolingObject.PoolingType)
		{
			case PoolingObjectType.PLATFORM:
				{
					PlatformView platformView = (PlatformView)poolingObject;

					if (PlayerTriggerDetector.position.x > platformView.transform.position.x)
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

	public void SetStaticPlayer(bool isStatic)
	{
		if (isStatic)
		{
			if(_playerRB.bodyType != RigidbodyType2D.Static)
				_playerRB.bodyType = RigidbodyType2D.Static;
		}
		else
		{
			if(_playerRB.bodyType != RigidbodyType2D.Dynamic)
				_playerRB.bodyType = RigidbodyType2D.Dynamic;
		}
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
}
