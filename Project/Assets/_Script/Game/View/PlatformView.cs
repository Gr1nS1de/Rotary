using UnityEngine;
using System.Collections;
using DG.Tweening;

//Custom editor: PlatformViewEditor
public class PlatformView : PoolingObjectView, IPoolObject
{
	public PlatformTypes PlatformType;

	#region horizontal platform vars
	[SerializeField]
	private SpriteRenderer HorizontalPlatformRenderer;
	#endregion

	#region vertical platform vars
	[SerializeField]
	private SpriteRenderer[] VerticalPlatformRenderers;
	#endregion

	private bool _isWasVisible = false;
	private bool _isInPool = true;
	private Sequence _platformInitSequence = null;
	private float _verticalPlatformGapHeight;

	private PlatformsFactoryModel platformsFactoryModel { get { return game.model.platformsFactoryModel;}} 

	public void OnInit()
	{
		ObjectVisibleState = PoolingObjectState.WAIT_FOR_VISIBLE;
		_isInPool = false;

		switch (PlatformType)
		{
			case PlatformTypes.Horizontal:
				{
					//Obsolete. Now set position directly for platform view.
					//HorizontalPlatformRenderer.transform.localPosition = Vector3.zero;
					break;
				}

			case PlatformTypes.Vertical_Moving:
			case PlatformTypes.Vertical:
				{
					Vector3 verticalPlatformSize = VerticalPlatformRenderers [0].bounds.size;
					Vector3 topPlatformPosition = VerticalPlatformRenderers [0].transform.localPosition;
					Vector3 bottomPlatformPosition = VerticalPlatformRenderers [1].transform.localPosition;

					_verticalPlatformGapHeight = game.model.playerModel.playerRendererSize.y + platformsFactoryModel.verticalPlatformsGap;

					topPlatformPosition.y = verticalPlatformSize.y / 2f + _verticalPlatformGapHeight / 2f;
					bottomPlatformPosition.y = -verticalPlatformSize.y / 2f - _verticalPlatformGapHeight / 2f;

					VerticalPlatformRenderers [0].transform.localPosition = topPlatformPosition;
					VerticalPlatformRenderers [1].transform.localPosition = bottomPlatformPosition;
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
			case PlatformTypes.Horizontal:
				{
					break;
				}

			case PlatformTypes.Vertical:
				{
					break;
				}

			case PlatformTypes.Vertical_Moving:
				{
					float screenHeight = GM.Instance.ScreenSize.y;

					_platformInitSequence
						.Append(transform.DOMoveY (screenHeight / 2f - _verticalPlatformGapHeight * 0.6f, 1f))
						.Append(transform.DOMoveY (-screenHeight / 2f + _verticalPlatformGapHeight * 0.6f, 1f))
						.SetLoops(-1)
						.SetRecyclable(true)
						.SetAutoKill(false);

					_platformInitSequence.Play ();
					break;
				}

		}
	}

	public bool IsObjectVisible()
	{
		bool isVisible = false;

		switch (PlatformType)
		{
			case PlatformTypes.Horizontal:
				{
					isVisible = HorizontalPlatformRenderer.isVisible;
					break;
				}

			case PlatformTypes.Vertical:
				{
					isVisible = VerticalPlatformRenderers [0].isVisible;
					break;
				}

			case PlatformTypes.Vertical_Moving:
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
			case PlatformTypes.Horizontal:
				{
					rendererSize = HorizontalPlatformRenderer.bounds.size;
					break;
				}

			case PlatformTypes.Vertical:
				{
					rendererSize = VerticalPlatformRenderers [0].bounds.size;
					break;
				}

			case PlatformTypes.Vertical_Moving:
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
		ObjectVisibleState = PoolingObjectState.VISIBLE;
	}

	public virtual void OnInvisible()
	{
		Notify (N.OnPlatformInvisible_, NotifyType.GAME, this);
		ObjectVisibleState = PoolingObjectState.WAS_VISIBLE;
	}

	public virtual void OnAddToPool()
	{
		_isInPool = true;


		if(_platformInitSequence != null)
			_platformInitSequence.Rewind ();
	}
}

