using UnityEngine;
using System.Collections;
using Destructible2D;

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

	public void OnInit()
	{
		_isWasVisible = false;
		_isInPool = false;

		switch (ItemType)
		{
			case ItemTypes.COIN:
				{
					break;
				}

			case ItemTypes.DIMOND:
				{

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
		Notify (N.OnItemInvisible_, NotifyType.GAME, this);
	}

	public virtual void OnAddToPool()
	{
		_isInPool = true;
	}
}

