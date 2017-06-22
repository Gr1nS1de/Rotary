using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ItemsController : Controller
{
	public override void OnNotification (string alias, Object target, params object[] data)
	{
		switch (alias)
		{
			case N.OnStart:
				{
					OnStart ();

					break;
				}

			case N.GameAddScore:
				{
					UpdateCrystalFractureCount();
					break;
				}
				/*
			case N.DestructibleBreakEntity___:
				{
					var obstacleDestructible = (D2dDestructible)data [0];
					var fractureCount = (int)data [1];
					var collisionPoint = (Vector2)data [2];

					BreakEntity( obstacleDestructible, fractureCount, collisionPoint);

					break;
				}*/

			case N.PlayerImpactItem__:
				{
					ItemView itemView = (ItemView)data [0];
					Vector2 contactPoint = (Vector2)data [1];

					OnImpactItem (itemView, contactPoint);
					break;
				}

		}
	}

	private void OnStart()
	{

	}

	private void OnImpactItem(ItemView itemView, Vector2 contactPoint)
	{
		ItemType itemType = itemView.ItemType;

		switch (itemType)
		{
			case ItemType.Coin:
				{
					
					break;
				}

			case ItemType.Crystal:
				{
					//Notify (N.DestructibleBreakEntity___, NotifyType.GAME, destructibleItem, itemView.DistructFractureCount, collision.contacts [0].point);
					game.controller.distructibleController.BreakItem(itemType, itemView.CrystalRenderer, itemView.CrystalFractureCount, contactPoint);

					Destroy (itemView.gameObject, itemView.CrystalDestroyTime + 1f);
					break;
				}

			case ItemType.Magnet:
				{
					break;
				}
		}

		itemView.OnPlayerImpact (contactPoint);
	}

	private void UpdateCrystalFractureCount()
	{
		int fractureCount = 0;

		switch (game.model.gameSpeedState)
		{
			default:
				{
					fractureCount = 1;
					break;
				}

			case GameSpeedState.Speed_2:
				{
					fractureCount = 2;
					break;
				}

			case GameSpeedState.Speed_3:
				{
					fractureCount = 3;
					break;
				}

			case GameSpeedState.Speed_4:
				{
					fractureCount = 4;
					break;
				}

			case GameSpeedState.Speed_5:
				{
					fractureCount = 5;
					break;
				}
			case GameSpeedState.Speed_6:
				{	
					fractureCount = 6;
					break;
				}

			case GameSpeedState.Speed_7:
				{	
					fractureCount = 7;
					break;
				}
		}

		game.model.itemModel.crystalFractureCount = fractureCount;
	}

}

