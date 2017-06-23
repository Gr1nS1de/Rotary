using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RocketView : PoolingObjectView, IPoolObject
{
	public RocketType			RocketType;
	public SpriteRenderer		RocketRenderer;
	public SpriteRenderer		ExclamationBackground;
	public float				RocketMoveTime				= 2f;
	public float 				RocketStartDelay			= 3f;
	public float				RocketLaunchDelay			= 0.3f;

	private Sequence			_rocketActionSequence;
	private bool 				_isInPool 					= false;
	private bool 				_isPlayerImpact 			= false;

	public void Init()
	{
		GoToVisibleState (PoolingObjectState.WAIT_FOR_VISIBLE);

		_isInPool = false;
		_isPlayerImpact = false;

		ResetElements ();

		StartRocketAction ();
	}

	private void ResetElements()
	{
		RocketRenderer.transform.localPosition = new Vector3 (GM.Instance.ScreenSize.x * 0.5f + GetMainRendererSize().x, 0f, 0f);
		ExclamationBackground.transform.localPosition = new Vector3 (GM.Instance.ScreenSize.x * 0.5f + ExclamationBackground.bounds.size.x, 0f, 0f);
		ExclamationBackground.transform.localScale = Vector3.one;
		ExclamationBackground.DOFade (0f, 0.01f);
	}

	private void StartRocketAction()
	{
		if (_rocketActionSequence != null && _rocketActionSequence.IsActive ())
			_rocketActionSequence.Kill ();

		_rocketActionSequence = DOTween.Sequence ();

		_rocketActionSequence
			//1. Show exclamation mark
			.Append(ExclamationBackground.transform.DOLocalMoveX (GM.Instance.ScreenSize.x * 0.5f - ExclamationBackground.bounds.size.x - 0.05f, 0.3f))
			//2. Move exclamation mark for player
			.Append (DOVirtual.DelayedCall (RocketStartDelay, () =>
			{

			}).OnUpdate (() =>
			{
				ExclamationBackground.transform.DOLocalMoveY (game.view.playerView.PlayerRenderer.transform.position.y, 2f);
			}))
			//3. Start fade in with punch scale background of exclamation mark (begin rocket launch)
			.Append (ExclamationBackground.DOFade (1f, 0.1f))
			.Join (ExclamationBackground.transform.DOPunchScale (new Vector3 (0.2f, 0.2f, 0f), 0.15f, 2))
			//4. Wait for delay before rocket starts moving
			.AppendInterval(RocketLaunchDelay)
			//5. Start move rocket
			.Append (RocketRenderer.transform.DOLocalMoveX (-(GM.Instance.ScreenSize.x * 0.5f) - GetMainRendererSize ().x, RocketMoveTime).SetEase(Ease.Linear))
			//6. Hide exclamation mark
			.Join(ExclamationBackground.transform.DOScale(0f, 0.1f))
			.Join(ExclamationBackground.transform.DOLocalMoveX(GM.Instance.ScreenSize.x, 0.1f));

		_rocketActionSequence.Play ();
	}

	#region public methods
	public Vector3 GetMainRendererSize()
	{
		Vector3 rendererSize = Vector3.zero;

		switch (RocketType)
		{
			case RocketType.Default:
				{
					rendererSize = RocketRenderer.bounds.size;
					break;
				}

		}

		return rendererSize;
	}
	#endregion
		
	private bool IsObjectVisible()
	{
		bool isVisible = false;

		switch (RocketType)
		{
			case RocketType.Default:
				{
					isVisible = RocketRenderer.isVisible;
					break;
				}

		}

		return isVisible;
	}

	void Update()
	{
		if (_isInPool || _isPlayerImpact)
			return;

		if (ObjectVisibleState == PoolingObjectState.WAIT_FOR_VISIBLE)
		{
			if (IsObjectVisible())
			{
				OnVisible ();
			}
		}else
			if (ObjectVisibleState == PoolingObjectState.VISIBLE)
			{
				UpdateItemOnVisible ();

				if (!IsObjectVisible())
				{
					OnInvisible ();
				}
			}

	}

	public void OnPlayerImpact(Vector2 contactPoint)
	{
		if (_isPlayerImpact)
			return;

		_isPlayerImpact = true;


	}

	private void UpdateItemOnVisible()
	{
		switch (RocketType)
		{
			case RocketType.Default:
				{
					break;
				}

			default:
				{
					break;
				}
		}

	}

	public void OnVisible()
	{
		GoToVisibleState (PoolingObjectState.VISIBLE);
	}

	public void OnInvisible()
	{
		GoToVisibleState (PoolingObjectState.WAS_VISIBLE);
		Notify (N.OnRocketInvisible_, NotifyType.GAME, this);
	}

	public override void OnAddToPool ()
	{
		_isInPool = true;

		if (_rocketActionSequence != null)
		{
			if (_rocketActionSequence.IsActive ())
				_rocketActionSequence.Kill ();

			_rocketActionSequence = null;
		}
	}
		
}
