using UnityEngine;
using System.Collections;

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
	private PlatformsFactoryModel platformsFactoryModel { get { return game.model.platformsFactoryModel;}} 

	public void OnInit()
	{
		ObjectVisibleState = PoolingObjectState.WAIT_FOR_VISIBLE;
		_isInPool = false;

		switch (PlatformType)
		{
			case PlatformTypes.HORIZONTAL:
				{
					//Obsolete. Now set position directly for platform view.
					//HorizontalPlatformRenderer.transform.localPosition = Vector3.zero;
					break;
				}

			case PlatformTypes.VERTICAL:
				{
					Vector3 verticalPlatformSize = VerticalPlatformRenderers [0].bounds.size;
					Vector3 topPlatformPosition = VerticalPlatformRenderers [0].transform.position;
					Vector3 bottomPlatformPosition = VerticalPlatformRenderers [1].transform.position;

					topPlatformPosition.y = verticalPlatformSize.y / 2f + game.model.playerModel.playerRendererSize.y + platformsFactoryModel.verticalPlatformsGap;
					bottomPlatformPosition.y = -verticalPlatformSize.y / 2f - game.model.playerModel.playerRendererSize.y - platformsFactoryModel.verticalPlatformsGap;

					VerticalPlatformRenderers [0].transform.position = topPlatformPosition;
					VerticalPlatformRenderers [1].transform.position = bottomPlatformPosition;
					break;
				}
		}
	}

	public bool IsObjectVisible()
	{
		bool isVisible = false;

		switch (PlatformType)
		{
			case PlatformTypes.HORIZONTAL:
				{
					isVisible = HorizontalPlatformRenderer.isVisible;
					break;
				}

			case PlatformTypes.VERTICAL:
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
			case PlatformTypes.HORIZONTAL:
				{
					rendererSize = HorizontalPlatformRenderer.bounds.size;
					break;
				}

			case PlatformTypes.VERTICAL:
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
	}
}

