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

	public void OnInit()
	{
		_isWasVisible = false;
		_isInPool = false;
		_isPlayerImpact = false;

		switch (ItemType)
		{
			case ItemTypes.COIN:
				{
					break;
				}

			case ItemTypes.DIMOND:
				{
					DimondRenderer.transform.DORotate (new Vector3(0f, 0f, 360f), 1f, RotateMode.Fast).SetLoops(-1).SetId(this);
					break;
				}
		}
	}

	public bool IsObjectVisible()
	{
		bool isVisible = false;

		switch (ItemType)
		{
			case ItemTypes.COIN:
				{
					isVisible = CoinRenderer.isVisible;
					break;
				}

			case ItemTypes.DIMOND:
				{
					isVisible = DimondRenderer.GetComponent<MeshRenderer> ().isVisible;
					break;
				}
		}

		return isVisible;
	}

	public Vector3 GetMainRendererSize()
	{
		Vector3 rendererSize = Vector3.zero;

		switch (ItemType)
		{
			case ItemTypes.COIN:
				{
					rendererSize = CoinRenderer.bounds.size;
					break;
				}

			case ItemTypes.DIMOND:
				{
					rendererSize = DimondRenderer.GetComponent<MeshRenderer> ().bounds.size;
					break;
				}
		}

		return rendererSize;
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
		DOTween.Kill(this);
		_isPlayerImpact = true;
	}

	public void OnAddToPool()
	{
		DOTween.Kill(this);
		_isInPool = true;
	}

	void OnDisable()
	{
	}
}

