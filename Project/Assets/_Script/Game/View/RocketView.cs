using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RocketView : PoolingObjectView, IPoolObject
{
	public RocketType			RocketType;
	public SpriteRenderer		RocketRenderer;
	public SpriteRenderer		ExclamationBackground;

	private Sequence			_rocketActionSequence;
	private bool 				_isInPool 					= false;
	private bool 				_isPlayerImpact 			= false;

	public void Init()
	{
		GoToVisibleState (PoolingObjectState.WAIT_FOR_VISIBLE);

		_isInPool = false;
		_isPlayerImpact = false;

		SetupRocketTweening ();
	}

	private void SetupRocketTweening()
	{
		if (_rocketActionSequence != null && _rocketActionSequence.IsActive ())
			_rocketActionSequence.Kill ();

		_rocketActionSequence = DOTween.Sequence ();

	}
		
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
