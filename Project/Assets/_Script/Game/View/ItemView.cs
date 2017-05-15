﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Destructible2D;
using DG.Tweening;
using tk2dRuntime;

//ItemViewEditor
public class ItemView : PoolingObjectView
{
	public ItemTypes	ItemType;
	public int 			CrystalFractureCount = 5;
	public float 		CrystalDestroyTime = 2f;

	[SerializeField]
	public D2dDestructible DimondRenderer;
	[SerializeField]
	public SpriteRenderer CoinRenderer;
	public tk2dTextMesh CountRenderer;

	private bool _isWasVisible = false;
	private bool _isInPool = true;
	private bool _isPlayerImpact = false;
	private Tweener _itemInitTween = null;
	private Sequence _itemImpactSequence = null;
	private int _crystalFragmetsCollised = 0;

	//public void Start()
	//{
	//	OnInit ();
	//}

	public void OnInit() //OnInit
	{
		//Debug.LogFormat ("Init item {0}", transform.name);
		_isWasVisible = false;
		_isInPool = false;
		_isPlayerImpact = false;

		if (CountRenderer != null)
		{
			CountRenderer.color = new Color (CountRenderer.color.r, CountRenderer.color.g, CountRenderer.color.b, 0f);
			CountRenderer.transform.localPosition = Vector3.zero;
		}

		if(CoinRenderer != null)
			CoinRenderer.color = new Color (CoinRenderer.color.r, CoinRenderer.color.g, CoinRenderer.color.b, 1f);

		if (_itemInitTween == null)
			SetupItemTweening ();
		else
			_itemInitTween.Restart ();
	}

	private void SetupItemTweening()
	{
		switch (ItemType)
		{
			case ItemTypes.Coin:
				{
					_itemInitTween = CoinRenderer.transform.DOPunchScale (new Vector3(0.15f, 0.15f, 0f), 0.6f, 1, 1f)
						.SetEase(Ease.InBounce)
						.SetLoops(-1)
						.SetAutoKill(false);
					break;
				}

			case ItemTypes.Crystal:
				{
					_itemInitTween = DimondRenderer.transform.DORotate (new Vector3(0f, 0f, 360f), 1f, RotateMode.FastBeyond360)
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
		if (_isPlayerImpact)
			return;
		
		if(_itemInitTween != null)
			_itemInitTween.Rewind ();

		if (_itemImpactSequence != null)
			_itemImpactSequence.Rewind ();
		
		_isPlayerImpact = true;

		switch (ItemType)
		{
			case ItemTypes.Coin:
				{
					if (_itemImpactSequence == null)
					{
						_itemImpactSequence = DOTween.Sequence ();

						_itemImpactSequence
							.Append (CoinRenderer.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0f), 0.3f, 1))
							.Join(CoinRenderer.transform.DOPunchPosition(Vector3.up * 2f, 0.5f, 0, 0))
							.Join(CoinRenderer.DOFade(0f, 0.5f))
							.Append(CoinRenderer.transform.DOPunchScale(-Vector3.one * 1.5f, 0.2f, 0, 0f))
							.Insert(0.05f, CountRenderer.DOFade(1f, 0.1f))
							.Insert(0.05f, CountRenderer.transform.DOLocalMoveY(2f, 0.5f))
							.SetRecyclable(true)
							.SetAutoKill(false);
					}

					_itemImpactSequence.Play ();
					break;
				}

			case ItemTypes.Crystal:
				{
					if (_itemImpactSequence == null)
					{
						_itemImpactSequence = DOTween.Sequence ();
						List<D2dDestructible> crystalFragmentsList = new List<D2dDestructible> (transform.GetComponentsInChildren<D2dDestructible>());

						_itemImpactSequence.AppendInterval (CrystalDestroyTime / 2f);

						crystalFragmentsList.ForEach (crystalFragment =>
						{
							_itemImpactSequence.Insert(CrystalDestroyTime / 2f, DOVirtual.DelayedCall(CrystalDestroyTime / 2f, ()=>
							{

							}).OnUpdate(()=>
							{
								crystalFragment.transform.DOMove(game.view.playerView.transform.position, CrystalDestroyTime / 2f);
							}));

							crystalFragment.gameObject.layer = LayerMask.NameToLayer("DestroyedItemBack");
						});

						_itemImpactSequence
							.SetRecyclable(true)
							.SetAutoKill(false);
					}

					_itemImpactSequence.Play ();

					break;
				}

			case ItemTypes.Magnet:
				{
					break;
				}
		}
	}

	public override void OnRendererTriggerEnter (ViewTriggerDetect triggerDetector, Collider2D otherCollider)
	{
		switch (ItemType)
		{
			case ItemTypes.Crystal:
				{
					if (++_crystalFragmetsCollised > CrystalFractureCount)
					{
						CountRenderer.DOFade (1f, 0.1f);
						CountRenderer.text = string.Format ("+{0}", _crystalFragmetsCollised);
						CountRenderer.transform.position = triggerDetector.transform.position;

						Debug.LogFormat ("Impact crystal to player");
					}
					break;
				}
		}
	}

	public void OnAddToPool()
	{
		if(_itemInitTween != null)
			_itemInitTween.Rewind ();

		if (_itemImpactSequence != null)
			_itemImpactSequence.Rewind ();
		
		_isInPool = true;
	}

	void OnDestroy()
	{
		if (_itemInitTween != null)
		{
			if(_itemInitTween.IsActive())
				_itemInitTween.Kill ();
			
			_itemInitTween = null;
		}

		if (_itemImpactSequence != null)
		{
			if(_itemImpactSequence.IsActive())
				_itemImpactSequence.Kill ();

			_itemImpactSequence = null;
		}
	}
}

