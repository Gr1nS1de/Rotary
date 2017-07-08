using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RocketView : PoolingObjectView, IPoolObject
{
	public RocketType			RocketType;
	public SpriteRenderer		RocketRenderer;
	public SpriteRenderer		ExclamationBackground;
	public SpriteRenderer		ExclamationMark;
	public SpriteRenderer 		AttentionArrow;
	public Color				ExclamationMarkBlinkColor;
	public float				RocketMoveTime				= 2f;
	public float 				RocketStartDelay			= 3f;
	public float				RocketLaunchDelay			= 0.3f;

	private Sequence			_rocketActionSequence;
	private bool 				_isInPool 					= false;
	private bool 				_isPlayerImpact 			= false;
	private Color?				_exclamationMarkInitColor	= null;

	public void Init()
	{
		if (_exclamationMarkInitColor == null)
			_exclamationMarkInitColor = ExclamationMark.color;
		
		GoToVisibleState (PoolingObjectState.WAIT_FOR_VISIBLE);

		_isInPool = false;
		_isPlayerImpact = false;

		ResetElements ();

		StartRocketAction ();
	}

	private void ResetElements()
	{
		RocketRenderer.transform.localPosition = new Vector3 (GM.Instance.ScreenSize.x * 0.5f + GetMainRendererSize().x * 2f, 0f, 0f);
		ExclamationBackground.transform.localPosition = new Vector3 (GM.Instance.ScreenSize.x * 0.5f + ExclamationBackground.bounds.size.x, 0f, 0f);
		ExclamationBackground.transform.localScale = Vector3.one;
		ExclamationBackground.DOFade (0f, 0.01f);
	}

	private void StartRocketAction()
	{
		if (_rocketActionSequence != null && _rocketActionSequence.IsActive ())
			_rocketActionSequence.Kill ();

		_rocketActionSequence = DOTween.Sequence ();

		RocketRenderer.gameObject.SetActive (false);

		_rocketActionSequence
			//1. Show exclamation mark and arrow - move them from right
			.Append(ExclamationBackground.transform.DOLocalMoveX (GM.Instance.ScreenSize.x * 0.5f - ExclamationBackground.bounds.size.x - 0.01f, 0.5f))
			.Join(AttentionArrow.DOFade(1f, 0.1f))
			//2. Move exclamation mark and arrow to player during rocket start delay
			.Append (DOVirtual.DelayedCall (RocketStartDelay, () =>
			{

			})
				.OnStart(()=>
				{
					float blinkStepTime = 0.3f;
					int loopSteps = (int)(RocketStartDelay / blinkStepTime);

					ExclamationMark
						.DOColor(ExclamationMarkBlinkColor, blinkStepTime)
						.OnComplete(()=>
						{
							ExclamationMark.DOColor(_exclamationMarkInitColor.GetValueOrDefault(), blinkStepTime);	
						}).SetLoops(loopSteps);

					AttentionArrow.transform.localPosition = new Vector3(0.85f, 0f, 0f);

					Sequence arrowMoveSequence = DOTween.Sequence ();

					arrowMoveSequence
						.Append(AttentionArrow.transform.DOLocalMoveX(1.1f, blinkStepTime ))
						.Append(AttentionArrow.transform.DOLocalMoveX(0.85f, blinkStepTime))
						.SetLoops(loopSteps);
				})
				.OnUpdate (() =>
				{
					ExclamationBackground.transform.DOLocalMoveY (game.view.playerView.PlayerRenderer.transform.position.y, 2f);
				}))
			//3. Start fade in with punch scale background of exclamation mark (begin rocket launch)
			.Append (ExclamationBackground.DOFade (1f, 0.1f))
			.Append(AttentionArrow.DOFade(0f, 0.1f))
			.Join (ExclamationBackground.transform.DOPunchScale (new Vector3 (0.2f, 0.2f, 0f), 0.15f, 2))
			//4. Wait for delay before rocket starts moving
			.AppendInterval(RocketLaunchDelay)
			//5. Start move rocket
			.AppendCallback(()=>
			{
				RocketRenderer.transform.position = new Vector3(RocketRenderer.transform.position.x, ExclamationBackground.transform.position.y, RocketRenderer.transform.position.z);
				RocketRenderer.gameObject.SetActive(true);

				Sequence rocketShakeSequence = DOTween.Sequence ();

				RocketRenderer.transform.localEulerAngles = new Vector3 (0f, 0f, 90f);

				rocketShakeSequence
					.Append (RocketRenderer.transform.DOLocalRotate (new Vector3 (0f, 0f, 85f), 0.05f, RotateMode.FastBeyond360).SetEase(Ease.Linear))
					.Append(RocketRenderer.transform.DOLocalRotate (new Vector3 (0f, 0f, 90f), 0.05f, RotateMode.FastBeyond360).SetEase(Ease.Linear))
					.Append(RocketRenderer.transform.DOLocalRotate (new Vector3 (0f, 0f, 95f), 0.05f, RotateMode.FastBeyond360).SetEase(Ease.Linear))
					.Append(RocketRenderer.transform.DOLocalRotate (new Vector3 (0f, 0f, 90f), 0.05f, RotateMode.FastBeyond360).SetEase(Ease.Linear))
					.SetLoops ((int)(RocketMoveTime / (0.1f * 2)) + 1);
			})
			.Append (RocketRenderer.transform.DOLocalMoveX (-(GM.Instance.ScreenSize.x * 0.5f) - GetMainRendererSize ().x * 3f, RocketMoveTime).SetEase(Ease.Linear))
			//.Join(RocketRenderer.transform.DOShakeRotation(RocketMoveTime, new Vector3(0f, 0f, 90f), 50, 10).SetEase(Ease.Linear))
			//6. Hide exclamation mark
			.Join(ExclamationBackground.transform.DOScale(0f, 0.1f))
			.Join(ExclamationBackground.transform.DOLocalMoveX(GM.Instance.ScreenSize.x, 0.1f))
			.Append(ExclamationBackground.DOFade (0f, 0.01f));

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
