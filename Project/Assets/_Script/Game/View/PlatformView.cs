using UnityEngine;
using System.Collections;

public class PlatformView : View
{
	public SpriteRenderer PlatformRenderer;

	private bool _isWaitForInvisible = false;
	private bool _isWasVisibleOnce = false;

	public void OnInit()
	{
		_isWaitForInvisible = true;
		PlatformRenderer.transform.localPosition = Vector3.zero;
	}
		
	void Update()
	{
		if (!_isWasVisibleOnce)
		{
			if (PlatformRenderer.isVisible)
				_isWasVisibleOnce = true;
		}else
		if (_isWaitForInvisible && _isWasVisibleOnce)
		{
			if (!PlatformRenderer.isVisible)
			{
				_isWaitForInvisible = false;

				Notify (N.OnPlatformInvisible_, NotifyType.GAME, this);

				_isWasVisibleOnce = false;
			}
		}
	}
}

