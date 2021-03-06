﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Destructible2D;
using DG.Tweening;
using tk2dRuntime;

//ItemViewEditor
public class ItemView : PoolingObjectView
{
	public ItemType	ItemType;
	public int 			CrystalFractureCount = 5;
	public float 		CrystalDestroyTime = 2f;

	[SerializeField]
	public D2dDestructible CrystalRenderer;
	[SerializeField]
	public SpriteRenderer CoinRenderer;
	public SpriteRenderer DoubleCoinRenderer;
	public SpriteRenderer MagnetRenderer;
	public tk2dTextMesh[] CountRenderers;
	public WeaponTrail MagnetTrail;

	private bool _isInPool = false;
	private bool _isPlayerImpact = false;
	private Tween _magnetTurnTween = null;
	private Sequence _itemInitSequence = null;
	private Sequence _itemImpactSequence = null;

	//public void Start()
	//{
	//	OnInit ();
	//}

	public void Init(int itemCount = 0) //OnInit
	{
		//Debug.LogFormat ("Init item {0}", transform.name);
		GoToVisibleState (PoolingObjectState.WAIT_FOR_VISIBLE);

		_isInPool = false;
		_isPlayerImpact = false;

		ResetCountRenderers ();

		if(CoinRenderer != null)
			CoinRenderer.color = new Color (CoinRenderer.color.r, CoinRenderer.color.g, CoinRenderer.color.b, 1f);

		InitItem (itemCount);

		if (_itemInitSequence == null)
			SetupItemTweening ();
		else
			_itemInitSequence.Restart ();
	}

	private void InitItem(int itemCount = 0)
	{
		switch (ItemType)
		{
			case ItemType.Coin:
				{
					ActivateRendererCount (false, ItemType, itemCount);
					break;
				}

			case ItemType.Crystal:
				{
					CrystalFractureCount = itemCount;
					ActivateRendererCount (false, ItemType, CrystalFractureCount);
					break;
				}

			case ItemType.Magnet:
				{
					MagnetRenderer.transform.eulerAngles = new Vector3 (0f, 0f, Random.Range(0f, 180f));
					break;
				}
		}
	}

	private void ResetCountRenderers()
	{
		if (CountRenderers != null)
		{
			foreach (var renderer in CountRenderers)
			{
				renderer.color = new Color (renderer.color.r, renderer.color.g, renderer.color.b, 0f);
				renderer.transform.localPosition = Vector3.zero;
			}
		}
	}

	private void SetupItemTweening()
	{
		_itemInitSequence = DOTween.Sequence ();

		switch (ItemType)
		{
			case ItemType.Coin:
				{
					_itemInitSequence
						.Append(CoinRenderer.transform.DOPunchScale (new Vector3(0.25f, 0.25f, 0f), 0.6f, 1, 1f).SetEase(Ease.InBounce))
						.SetLoops(-1)
						.SetRecyclable(true)
						.SetAutoKill(false);
					break;
				}

			case ItemType.Crystal:
				{
					_itemInitSequence
						.Append(CrystalRenderer.transform.DORotate (new Vector3(0f, 0f, 360f), 1f, RotateMode.FastBeyond360).SetEase(Ease.Linear))
						.SetLoops(-1)
						.SetRecyclable(true)
						.SetAutoKill(false);
					break;
				}
					
		}

		_itemInitSequence.Play ();
	}
		
	#region public methods
	public Vector3 GetMainRendererSize()
	{
		Vector3 rendererSize = Vector3.zero;

		switch (ItemType)
		{
			case ItemType.Coin:
				{
					rendererSize = CoinRenderer.bounds.size;
					break;
				}

			case ItemType.Crystal:
				{
					rendererSize = CrystalRenderer.GetComponent<MeshRenderer> ().bounds.size;
					break;
				}

			case ItemType.Magnet:
				{
					rendererSize = MagnetRenderer.bounds.size;
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
			case ItemType.Coin:
				{
					isVisible = CoinRenderer.isVisible;
					break;
				}

			case ItemType.Crystal:
				{
					isVisible = CrystalRenderer.GetComponent<MeshRenderer> ().isVisible;
					break;
				}

			case ItemType.Magnet:
				{
					isVisible = MagnetRenderer.isVisible;
					break;
				}
		}

		return isVisible;
	}

	void Update()
	{
		if (_isInPool || _isPlayerImpact)
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
				UpdateItemOnVisible ();

				if (!IsObjectVisible())
				{
					OnInvisible ();
				}
			}
				
	}

	private void UpdateItemOnVisible()
	{
		switch (ItemType)
		{
			case ItemType.Magnet:
				{
					Vector3 currentPlayerPosition = game.view.playerView.PlayerRenderer.transform.position;
					//Debug.LogErrorFormat ("Distance to player: {0}", Vector2.Distance (currentPlayerPosition, transform.position));
					if (Vector2.Distance (currentPlayerPosition, transform.position) < 7.5f)
					{
						//MagnetTrail.Itterate (Time.time);
						//MagnetTrail.UpdateTrail (Time.time, Time.deltaTime);
						if (_magnetTurnTween == null || _magnetTurnTween.IsActive())
						{
							if (_magnetTurnTween == null)
							{
								var dir = currentPlayerPosition - transform.position;
								var angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;

								_magnetTurnTween = MagnetRenderer.transform
									.DORotate (Quaternion.AngleAxis (angle - 90, Vector3.forward).eulerAngles, 0.3f)
									.OnComplete (() =>
									{
									}).SetId (this);
							}
						}
						else
						{
							//MagnetRenderer.transform.DOLookAt (currentPlayerPosition, 0.05f, AxisConstraint.W, new Vector3(0f, 1f, 0f));
							var dir = currentPlayerPosition - transform.position;
							var angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;

							MagnetRenderer.transform.DORotate (Quaternion.AngleAxis (angle - 90, Vector3.forward).eulerAngles, 5f * Time.deltaTime);
							transform.DOShakeRotation (0.5f, new Vector3 (0f, 0f, 30f), 3, 90f, true);
						}
					}
					break;
				}

			default:
				{
					break;
				}
		}

	}

	public void OnVisible()
	{
		GoToVisibleState (PoolingObjectState.VISIBLE);
	}

	public void OnInvisible()
	{
		GoToVisibleState (PoolingObjectState.WAS_VISIBLE);

		Notify (N.OnItemInvisible_, NotifyType.GAME, this);
	}

	public void OnPlayerImpact(Vector2 contactPoint)
	{
		if (_isPlayerImpact)
			return;
		
		if(_itemInitSequence != null)
			_itemInitSequence.Rewind ();

		if (_itemImpactSequence != null)
			_itemImpactSequence.Rewind ();
		
		_isPlayerImpact = true;

		switch (ItemType)
		{
			case ItemType.Coin:
				{
					if (_itemImpactSequence == null)
					{
						_itemImpactSequence = DOTween.Sequence ();

						_itemImpactSequence
							.Append (CoinRenderer.transform.DOPunchScale(new Vector3(1f, 1f, 0f), 0.3f, 1))
							.Join(CoinRenderer.transform.DOPunchPosition(Vector3.up * 2f, 0.4f, 0, 0))
							.Insert(0.2f, CoinRenderer.DOFade(0f, 0.3f))
							.Append(CoinRenderer.transform.DOPunchScale(-Vector3.one * 1.5f, 0.2f, 0, 0f))
							.SetRecyclable(true)
							.SetAutoKill(false);

						foreach (var renderer in CountRenderers)
						{
							_itemImpactSequence
								.Insert(0.05f, renderer.transform.DOLocalMoveY(2f, 0.5f))
								.Insert(0.05f, renderer.DOFade(1f, 0.1f));
						}
					}

					_itemImpactSequence.Play ();
					break;
				}

			case ItemType.Crystal:
				{
					//Debug.LogErrorFormat ("On player impact me. {0}",transform.name);
					if (_itemImpactSequence == null)
					{
						_itemImpactSequence = DOTween.Sequence ();
						List<D2dDestructible> crystalFragmentsList = new List<D2dDestructible> (transform.GetComponentsInChildren<D2dDestructible>());

						_itemImpactSequence.AppendInterval (CrystalDestroyTime / 2f);

						crystalFragmentsList.ForEach (crystalFragment =>
						{
							_itemImpactSequence.Join(DOVirtual.DelayedCall(CrystalDestroyTime / 2f, ()=>
							{

							}));

							crystalFragment.gameObject.layer = LayerMask.NameToLayer("DestroyedItemBack");
						});

						foreach (var renderer in CountRenderers)
						{
							Vector3 newPos = transform.position - ( new Vector3 (contactPoint.x, contactPoint.y, renderer.transform.position.z) - transform.position) * 2f;
							newPos.z = renderer.transform.position.z;
							_itemImpactSequence
								.Insert(0.05f, renderer.transform.DOMove(newPos, 0.5f))
								.Insert(0.05f, renderer.DOFade(1f, 0.1f));
						}

						_itemImpactSequence
							.SetRecyclable(true)
							.SetAutoKill(false);
					}

					ActivateRendererCount (true, ItemType);

					_itemImpactSequence.Play ();

					break;
				}

			case ItemType.Magnet:
				{
					break;
				}
		}
	}


	private void ActivateRendererCount(bool isAcivate, ItemType ItemType, int count = -1)
	{
		foreach (var renderer in CountRenderers)
		{
			if (isAcivate)
			{
				renderer.DOFade (1f, 0.1f);
			}
			else
			{
				if(count > 0)
					renderer.text = string.Format ("+{0}", count);
				
				renderer.transform.position = transform.position;
			}
		}
	}

	public override void OnRendererTriggerEnter (ViewTriggerDetect triggerDetector, Collider2D otherCollider)
	{
		switch (ItemType)
		{
			case ItemType.Crystal:
				{
					break;
				}
		}
	}

	public override void OnAddToPool()
	{
		if(_itemInitSequence != null)
			_itemInitSequence.Rewind ();

		if (_itemImpactSequence != null)
			_itemImpactSequence.Rewind ();

		if (_magnetTurnTween != null)
		{
			if(_magnetTurnTween.IsActive())
				_magnetTurnTween.Kill ();
			
			_magnetTurnTween = null;
		}
		
		_isInPool = true;
	}

	void OnDestroy()
	{
		if (_itemInitSequence != null)
		{
			if(_itemInitSequence.IsActive())
				_itemInitSequence.Kill ();
			
			_itemInitSequence = null;
		}

		if (_itemImpactSequence != null)
		{
			if(_itemImpactSequence.IsActive())
				_itemImpactSequence.Kill ();

			_itemImpactSequence = null;
		}
	}
}

