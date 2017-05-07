using UnityEngine;
using System.Collections;

public class PlatformView : View, IPlatform, IPoolObject
{
	public PlatformTypes PlatformType;
	[SerializeField]
	private SpriteRenderer PlatformRenderer;

	private bool _isWasVisibleOnce = false;

	public void OnInit()
	{
		_isWasVisibleOnce = false;

		PlatformRenderer.transform.localPosition = Vector3.zero;
	}

	public SpriteRenderer GetMainPlatformRenderer()
	{
		return PlatformRenderer;
	}
		
	void Update()
	{
		if (!_isWasVisibleOnce)
		{
			if (PlatformRenderer.isVisible)
			{
				OnVisible ();
			}
		}else
		if (_isWasVisibleOnce)
		{
			if (!PlatformRenderer.isVisible)
			{
				OnInvisible ();
			}
		}
	}

	public virtual void OnVisible()
	{
		_isWasVisibleOnce = true;
	}

	public virtual void OnInvisible()
	{
		Notify (N.OnPlatformInvisible_, NotifyType.GAME, this);
	}

	public virtual void OnAddToPool()
	{

	}
}

