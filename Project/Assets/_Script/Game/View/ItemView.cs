using UnityEngine;
using System.Collections;
using Destructible2D;
using DG.Tweening;

public class ItemView : PoolingObjectView
{
	public ItemTypes	ItemType;
	public int 			DistructFractureCount = 5;

	[SerializeField]
	public D2dDestructible DimondRenderer;
	[SerializeField]
	public SpriteRenderer CoinRenderer;

	private bool _isWasVisible = false;
	private bool _isInPool = true;
	private bool _isPlayerImpact = false;
	private Tweener _itemTween = null;

	public void OnInit()
	{
		//Debug.LogFormat ("Init item {0}", transform.name);
		_isWasVisible = false;
		_isInPool = false;
		_isPlayerImpact = false;

		if (_itemTween == null)
			SetupItemTweening ();
		else
			_itemTween.Restart ();
	}

	private void SetupItemTweening()
	{
		switch (ItemType)
		{
			case ItemTypes.Coin:
				{
					_itemTween = CoinRenderer.transform.DOPunchScale (new Vector3(0.1f, 0.1f, 0f), 0.5f, 1, 1f)
						.SetEase(Ease.InBounce)
						.SetLoops(-1)
						.SetAutoKill(false);
					break;
				}

			case ItemTypes.Crystal:
				{
					_itemTween = DimondRenderer.transform.DORotate (new Vector3(0f, 0f, 360f), 1f, RotateMode.FastBeyond360)
						.SetEase(Ease.Linear)
						.SetLoops(-1)
						.SetAutoKill(false);
					break;
				}
		}
	}
		
	#region public methods
	public Vector3 GetMainRendererSize()
	{
		Vector3 rendererSize = Vector3.zero;

		switch (ItemType)
		{
			case ItemTypes.Coin:
				{
					rendererSize = CoinRenderer.bounds.size;
					break;
				}

			case ItemTypes.Crystal:
				{
					rendererSize = DimondRenderer.GetComponent<MeshRenderer> ().bounds.size;
					break;
				}
		}

		return rendererSize;
	}
	#endregion

	private bool IsObjectVisible()
	{
		bool isVisible = false;

		switch (ItemType)
		{
			case ItemTypes.Coin:
				{
					isVisible = CoinRenderer.isVisible;
					break;
				}

			case ItemTypes.Crystal:
				{
					isVisible = DimondRenderer.GetComponent<MeshRenderer> ().isVisible;
					break;
				}
		}

		return isVisible;
	}

	void Update()
	{
		if (_isInPool || _isPlayerImpact)
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

	public void OnVisible()
	{
		_isWasVisible = true;
	}

	public void OnInvisible()
	{
		Notify (N.OnItemInvisible_, NotifyType.GAME, this);
	}

	public void OnPlayerImpact()
	{
		if(_itemTween != null)
			_itemTween.Rewind ();
		
		_isPlayerImpact = true;
	}

	public void OnAddToPool()
	{
		if(_itemTween != null)
			_itemTween.Rewind ();
		
		_isInPool = true;
	}

	void OnDestroy()
	{
		if (_itemTween != null)
		{
			_itemTween.Kill ();
			_itemTween = null;
		}
	}
}

