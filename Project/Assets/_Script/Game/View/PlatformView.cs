using UnityEngine;
using System.Collections;
using DG.Tweening;

//Custom editor: PlatformViewEditor
public class PlatformView : PoolingObjectView, IPoolObject
{
	public PlatformType PlatformType;

	#region horizontal platform vars
	[SerializeField]
	private SpriteRenderer HorizontalPlatformRenderer;
	#endregion

	#region vertical platform vars
	[SerializeField]
	private SpriteRenderer[] VerticalPlatformRenderers;
	#endregion

	private bool _isInPool = true;
	private Sequence _platformInitSequence = null;
	private float _verticalPlatformGapHeight;

	private PlatformsFactoryModel platformsFactoryModel { get { return game.model.platformsFactoryModel;}} 

	public void Init()
	{
		GoToVisibleState (PoolingObjectState.WAIT_FOR_VISIBLE);
		_isInPool = false;

		switch (PlatformType)
		{
			case PlatformType.Horizontal:
				{
					//Obsolete. Now set position directly for platform view.
					//HorizontalPlatformRenderer.transform.localPosition = Vector3.zero;
					break;
				}

			case PlatformType.Vertical_Moving:
				{
					InitVerticalPlatform (game.model.playerModel.playerRendererSize.y);
					Vector3 bottomPoint = new Vector3 (transform.position.x, (-GM.Instance.ScreenSize.y / 2f + _verticalPlatformGapHeight * 0.6f), transform.position.z);

					transform.position = bottomPoint;
					break;
				}
			case PlatformType.Vertical:
				{
					InitVerticalPlatform ();
					break;
				}
		}

		if (_platformInitSequence == null)
			SetupPlatformTweening ();
		else
			_platformInitSequence.Restart ();
	}

	private void SetupPlatformTweening()
	{
		_platformInitSequence = DOTween.Sequence ();

		switch (PlatformType)
		{
			case PlatformType.Horizontal:
				{
					break;
				}

			case PlatformType.Vertical:
				{
					break;
				}

			case PlatformType.Vertical_Moving:
				{
					float screenHeight = GM.Instance.ScreenSize.y;

					Vector3 upperPoint = new Vector3 (transform.position.x, (screenHeight / 2f - _verticalPlatformGapHeight * 0.6f), transform.position.z);
					Vector3 bottomPoint = new Vector3 (transform.position.x, (-screenHeight / 2f + _verticalPlatformGapHeight * 0.6f), transform.position.z);

					_platformInitSequence
						.Append(transform.DOLocalMoveY(upperPoint.y, 2f).SetEase(Ease.Linear))
						.Append(transform.DOLocalMoveY(bottomPoint.y, 2f).SetEase(Ease.Linear))
						.SetLoops(-1)
						.SetRecyclable(true)
						.SetAutoKill(false);

					_platformInitSequence.Play ();
					break;
				}

		}
	}

	private void InitVerticalPlatform(float gapHeight = -1f)
	{
		Vector3 verticalPlatformSize = VerticalPlatformRenderers [0].bounds.size;
		Vector3 topPlatformPosition = VerticalPlatformRenderers [0].transform.localPosition;
		Vector3 bottomPlatformPosition = VerticalPlatformRenderers [1].transform.localPosition;

		_verticalPlatformGapHeight = game.model.playerModel.playerRendererSize.y + (gapHeight == -1f ? platformsFactoryModel.verticalPlatformsGap : gapHeight);

		topPlatformPosition.y = verticalPlatformSize.y / 2f + _verticalPlatformGapHeight / 2f;
		bottomPlatformPosition.y = -verticalPlatformSize.y / 2f - _verticalPlatformGapHeight / 2f;

		VerticalPlatformRenderers [0].transform.localPosition = topPlatformPosition;
		VerticalPlatformRenderers [1].transform.localPosition = bottomPlatformPosition;
	}

	public bool IsObjectVisible()
	{
		bool isVisible = false;

		switch (PlatformType)
		{
			case PlatformType.Horizontal:
				{
					isVisible = HorizontalPlatformRenderer.isVisible;
					break;
				}

			case PlatformType.Vertical:
				{
					isVisible = VerticalPlatformRenderers [0].isVisible;
					break;
				}

			case PlatformType.Vertical_Moving:
				{
					isVisible = VerticalPlatformRenderers [0].isVisible;
					break;
				}
		}

		return isVisible;
	}

	public Vector3 GetMainRendererSize()
	{
		Vector3 rendererSize = Vector3.zero;

		switch (PlatformType)
		{
			case PlatformType.Horizontal:
				{
					rendererSize = HorizontalPlatformRenderer.bounds.size;
					break;
				}

			case PlatformType.Vertical:
				{
					rendererSize = VerticalPlatformRenderers [0].bounds.size;
					break;
				}

			case PlatformType.Vertical_Moving:
				{
					rendererSize = VerticalPlatformRenderers [0].bounds.size;
					break;
				}
		}

		return rendererSize;
	}
		
	void Update()
	{
		if (_isInPool)
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
			if (!IsObjectVisible())
			{
				OnInvisible ();
			}
		}
	}

	public virtual void OnVisible()
	{
		GoToVisibleState (PoolingObjectState.VISIBLE);
	}

	public virtual void OnInvisible()
	{
		GoToVisibleState (PoolingObjectState.WAS_VISIBLE);
		Notify (N.OnPlatformInvisible_, NotifyType.GAME, this);
	}

	public override void OnAddToPool()
	{
		_isInPool = true;


		if (_platformInitSequence != null)
		{
			if (_platformInitSequence.IsActive ())
				_platformInitSequence.Kill ();

			_platformInitSequence = null;
		}
	}
}

