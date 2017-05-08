using UnityEngine;
using System.Collections;

//Custom editor: PlatformViewEditor
public class PlatformView : PoolingObjectView, IPlatform, IPoolObject
{
	public PlatformTypes PlatformType;

	#region horizontal platform vars
	[SerializeField]
	private SpriteRenderer HorizontalPlatformRenderer;
	#endregion

	#region vertical platform vars
	[SerializeField]
	private SpriteRenderer[] VerticalPlatformRenderers;
	[SerializeField]
	private float _platformsGap = 0f;
	#endregion

	private bool _isWasVisible = false;
	private bool _isInPool = true;

	public void OnInit()
	{
		_isWasVisible = false;
		_isInPool = false;

		switch (PlatformType)
		{
			case PlatformTypes.HORIZONTAL:
				{
					HorizontalPlatformRenderer.transform.localPosition = Vector3.zero;
					break;
				}

			case PlatformTypes.VERTICAL:
				{
					
					break;
				}
		}
	}

	public SpriteRenderer GetMainPlatformRenderer()
	{
		SpriteRenderer platformRenderer = null;

		switch (PlatformType)
		{
			case PlatformTypes.HORIZONTAL:
				{
					platformRenderer = HorizontalPlatformRenderer;
					break;
				}

			case PlatformTypes.VERTICAL:
				{
					platformRenderer = VerticalPlatformRenderers [0];
					break;
				}
		}

		return platformRenderer;
	}
		
	void Update()
	{
		if (_isInPool)
			return;
		
		if (!_isWasVisible)
		{
			if (GetMainPlatformRenderer().isVisible)
			{
				OnVisible ();
			}
		}else
		if (_isWasVisible)
		{
			if (!GetMainPlatformRenderer().isVisible)
			{
				OnInvisible ();
			}
		}
	}

	public virtual void OnVisible()
	{
		_isWasVisible = true;
	}

	public virtual void OnInvisible()
	{
		Notify (N.OnPlatformInvisible_, NotifyType.GAME, this);
	}

	public virtual void OnAddToPool()
	{
		_isInPool = true;
	}
}

