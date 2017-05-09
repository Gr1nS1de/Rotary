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
		
		if (!_isWasVisible)
		{
			if (IsObjectVisible())
			{
				OnVisible ();
			}
		}else
		if (_isWasVisible)
		{
				if (!IsObjectVisible())
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

